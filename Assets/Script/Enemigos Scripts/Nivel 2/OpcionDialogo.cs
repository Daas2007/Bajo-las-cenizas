using UnityEngine;

[System.Serializable]   // 👈 Esto es lo que faltaba
public class OpcionDialogo
{
    public string textoOpcion;
    public int siguienteDialogoID; // ID del siguiente diálogo
}

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Dialogo")]
public class NuevoDialogo : ScriptableObject
{
    public int dialogoID;
    public string textoPersonaje;
    public OpcionDialogo[] opciones; // Ahora sí aparecerá en el Inspector
}


