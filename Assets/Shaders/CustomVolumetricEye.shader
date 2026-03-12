Shader "Custom/VolumetricEye"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,0,0,1)
        _Center ("Center (0-1)", Vector) = (0.5,0.5,0,0)
        _Radius ("Radius", Float) = 0.35
        _Softness ("Softness", Float) = 0.45
        _Intensity ("Intensity", Float) = 1.0
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 3.0
        _NoiseSpeed ("Noise Speed", Float) = 0.6
        _VignettePower ("Vignette Power", Float) = 1.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_TexelSize;
            float4 _Color;
            float4 _Center;
            float _Radius;
            float _Softness;
            float _Intensity;
            float _NoiseScale;
            float _NoiseSpeed;
            float _VignettePower;
            float _TimeY;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // smoothstep with softness control
            float smoothRing(float dist, float radius, float softness)
            {
                float inner = radius * (1.0 - softness);
                float t = saturate((dist - inner) / (radius - inner));
                return 1.0 - t;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // base scene
                fixed4 scene = tex2D(_MainTex, uv);

                // compute distance from center (center in 0..1)
                float2 center = _Center.xy;
                float2 d = uv - center;
                float dist = length(d);

                // radial mask (1 inside, 0 outside) with softness
                float mask = smoothRing(dist, _Radius, _Softness);

                // noise for volumetric flicker / veins
                float2 noiseUV = uv * _NoiseScale + float2(_TimeY * _NoiseSpeed, -_TimeY * _NoiseSpeed * 0.5);
                float noise = tex2D(_NoiseTex, noiseUV).r;
                // bias noise to create veins and flicker
                float veins = smoothstep(0.45, 0.9, noise) * 0.6 + (noise * 0.4);

                // radial falloff to darken edges inside the eye
                float vignette = pow(1.0 - saturate(dist / _Radius), _VignettePower);

                // final overlay color
                float overlayAlpha = mask * _Intensity * (0.4 + 0.6 * veins) * vignette;
                fixed4 overlay = _Color * overlayAlpha;

                // blend overlay over scene using additive + lerp for intensity
                fixed4 outCol = lerp(scene, scene + overlay.rgb, overlay.a);

                // optional subtle desaturation of scene under overlay
                float desat = overlayAlpha * 0.25;
                float gray = dot(outCol.rgb, float3(0.299,0.587,0.114));
                outCol.rgb = lerp(outCol.rgb, gray.xxx, desat);

                outCol.a = 1.0;
                return outCol;
            }
            ENDCG
        }
    }
    FallBack Off
}
