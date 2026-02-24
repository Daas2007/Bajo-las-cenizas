using UnityEngine;

[System.Serializable]
public class CheckpointData
{
    public Vector3 posicionJugador;
    public int muertes;
    public int piezasOso;
    public bool puzzle1Completado;
    public bool puzzle2Completado;
    public bool cristalMetaActivo;
    public bool muroActivo; // 🔧 nuevo campo
}

