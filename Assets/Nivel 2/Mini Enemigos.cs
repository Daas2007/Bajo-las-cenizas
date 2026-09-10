using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniEnemigos : MonoBehaviour
{
    [Header("Lista de Mini Enemigos (Asignar los 4 en el Inspector)")]
    [SerializeField] private GameObject[] miniEnemigos = new GameObject[4];

    [Header("Configuración de Tiempos")]
    [SerializeField] private float tiempoEntreApariciones = 45f; // Tiempo inicial de aparición
    [SerializeField] private float tiempoMinimoAparicion = 5f;   // Límite mínimo
    [SerializeField] private float reduccionTiempo = 5f;        // Cuánto se reduce
    [SerializeField] private float intervaloAumentoDificultad = 150f; // Cada 2:30 min (150s)

    private float cronometroGeneral = 0f;
    private float cronometroDificultad = 0f;
    private float cronometroAparicion = 0f;

    private void OnEnable()
    {
        // Reseteamos los cronómetros cada vez que el GameObject/Script se activa
        cronometroGeneral = 0f;
        cronometroDificultad = 0f;
        cronometroAparicion = 0f;
    }
    private void Awake()
    {
        for (int i = 0; i < miniEnemigos.Length; i ++)
        {
            miniEnemigos[i].SetActive(false);
        }
    }
    private void Update()
    {
        float delta = Time.deltaTime;

        cronometroGeneral += delta;
        cronometroDificultad += delta;
        cronometroAparicion += delta;

        // 1. Aumentar la dificultad cada 2:30 minutos (150 segundos)
        if (cronometroDificultad >= intervaloAumentoDificultad)
        {
            cronometroDificultad = 0f;
            AumentarDificultad();
        }

        // 2. Controlar la aparición de oleadas de mini enemigos
        if (cronometroAparicion >= tiempoEntreApariciones)
        {
            cronometroAparicion = 0f;
            AparecerMiniEnemigos();
        }
    }

    private void AumentarDificultad()
    {
        if (tiempoEntreApariciones > tiempoMinimoAparicion)
        {
            tiempoEntreApariciones -= reduccionTiempo;

            // Asegurarnos de que no baje del mínimo permitido (5 segundos)
            if (tiempoEntreApariciones < tiempoMinimoAparicion)
            {
                tiempoEntreApariciones = tiempoMinimoAparicion;
            }

            Debug.Log($"[MiniEnemigos] ¡Dificultad aumentada! Nuevo tiempo entre apariciones: {tiempoEntreApariciones}s");
        }
    }

    private void AparecerMiniEnemigos()
    {
        // Calcular cuántos enemigos deben aparecer según los nuevos porcentajes
        int cantidadAInvocador = CalcularCantidadAparicion();

        Debug.Log($"[MiniEnemigos] Intentando aparecer {cantidadAInvocador} mini enemigo(s).");

        int activados = 0;

        // Buscar entre los 4 mini enemigos asignados los que estén desactivados para activarlos
        for (int i = 0; i < miniEnemigos.Length; i++)
        {
            if (activados >= cantidadAInvocador) break;

            if (miniEnemigos[i] != null && !miniEnemigos[i].activeSelf)
            {
                miniEnemigos[i].SetActive(true);
                activados++;
            }
        }

        if (activados < cantidadAInvocador)
        {
            Debug.LogWarning($"[MiniEnemigos] Se querían activar {cantidadAInvocador} enemigos, pero solo había {activados} disponibles desactivados.");
        }
    }

    private int CalcularCantidadAparicion()
    {
        // Generar un número aleatorio entre 0 y 100
        float numeroRnd = Random.Range(0f, 100f);

        // Nueva Tabla de Probabilidades acumulada:
        // 5%   -> 4 Enemigos (Rango 0 a 5)
        // 10%  -> 3 Enemigos (Rango 5 a 15)
        // 20%  -> 2 Enemigos (Rango 15 a 35)
        // 65%  -> 1 Enemigo  (Rango 35 a 100)

        if (numeroRnd <= 5f)
        {
            return 4; // 5% probabilidad
        }
        else if (numeroRnd <= 15f)
        {
            return 3; // 10% probabilidad (5 + 10)
        }
        else if (numeroRnd <= 35f)
        {
            return 2; // 20% probabilidad (15 + 20)
        }
        else
        {
            return 1; // 65% probabilidad
        }
    }
    //------------------EVENTO AL ROMPER LA TELEVISION-------------

    public void ApagarMiniEnemigos()
    {
        for (int i = 0; i < miniEnemigos.Length; i++)
        {
            if (miniEnemigos[i] != null && miniEnemigos[i].activeSelf)
            {
                miniEnemigos[i].SetActive(false);
            }
        }
        this.enabled = false; 
    }
}