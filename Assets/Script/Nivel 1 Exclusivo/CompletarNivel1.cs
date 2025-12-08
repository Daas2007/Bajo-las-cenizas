using UnityEngine;

public class CompletarNivel1 : MonoBehaviour
{
    [SerializeField] CristalMetaSpawner cristalSpawner;
    [SerializeField] GameManagerNivel1 gameManager;

    void CompletarNivel()
    {
        cristalSpawner.SpawnCristal(); //aquí se decide normal o dañado
    }
}
