using UnityEngine;
using System.IO;
using System.Text;

public static class SistemaGuardar
{
    private static string Ubicacion = Application.persistentDataPath + "/ArchivoGuardado";
    public static void Guardar(MovimientoPersonaje player, GameManager gm)
    {
        var archivo = File.Open(Ubicacion, FileMode.Create);
        var escribir = new BinaryWriter(archivo, Encoding.Default, false);

        // Posición del jugador
        escribir.Write(player.transform.position.x);
        escribir.Write(player.transform.position.y);
        escribir.Write(player.transform.position.z);

        // Estado del juego desde GameManager
        escribir.Write(gm.enemigo.activeSelf);
        escribir.Write(gm.tieneLinterna);
        escribir.Write(gm.linternaPickup.activeSelf);
        escribir.Write(gm.muertes);
        escribir.Write(gm.piezasRecogidas);
        escribir.Write(gm.puzzle1Completado);
        escribir.Write(gm.puzzle2Completado);
        escribir.Write(gm.cristalMetaActivo);

        // Guardar estado del muro
        MuroBloqueo muro = Object.FindObjectOfType<MuroBloqueo>();
        escribir.Write(muro != null && muro.gameObject.activeSelf);

        // Guardar estado de las puertas tutorial
        PuertaTutorial[] puertas = Object.FindObjectsOfType<PuertaTutorial>();
        escribir.Write(puertas.Length);
        foreach (var p in puertas)
            escribir.Write(p.abierta);

        archivo.Close();
    }
    public static void Cargar(MovimientoPersonaje player, GameManager gm)
    {
        if (!File.Exists(Ubicacion)) return;

        var archivo = File.Open(Ubicacion, FileMode.Open);
        var leer = new BinaryReader(archivo, Encoding.Default, false);

        // Posición del jugador
        Vector3 pos;
        pos.x = leer.ReadSingle();
        pos.y = leer.ReadSingle();
        pos.z = leer.ReadSingle();
        player.transform.position = pos;

        // Estado del juego desde GameManager
        gm.enemigo.SetActive(leer.ReadBoolean());

        gm.tieneLinterna = leer.ReadBoolean();
        bool pickupActivo = leer.ReadBoolean();
        if (gm.linternaPickup != null) gm.linternaPickup.SetActive(pickupActivo);

        gm.muertes = leer.ReadInt32();
        gm.piezasRecogidas = leer.ReadInt32();
        gm.puzzle1Completado = leer.ReadBoolean();
        gm.puzzle2Completado = leer.ReadBoolean();
        gm.cristalMetaActivo = leer.ReadBoolean();

        // 🔧 Cargar estado del muro
        bool muroActivo = leer.ReadBoolean();
        MuroBloqueo muro = Object.FindObjectOfType<MuroBloqueo>();
        if (muro != null) muro.gameObject.SetActive(muroActivo);

        // 🔧 Cargar estado de las puertas tutorial
        int cantidadPuertas = leer.ReadInt32();
        PuertaTutorial[] puertas = Object.FindObjectsOfType<PuertaTutorial>();
        for (int i = 0; i < cantidadPuertas && i < puertas.Length; i++)
        {
            bool abierta = leer.ReadBoolean();
            if (abierta)
                puertas[i].AbrirPuertaVinculada();
            else
                puertas[i].ResetPuerta();
        }

        archivo.Close();

        // 🔧 Sincronizar JugadorLinterna con GameManager
        JugadorLinterna jugadorLinterna = Object.FindObjectOfType<JugadorLinterna>();
        if (jugadorLinterna != null)
        {
            if (gm.tieneLinterna)
            {
                jugadorLinterna.DarLinterna(); // o DarLinternaEncendida() si quieres que aparezca encendida
            }
            else
            {
                jugadorLinterna.ResetLinterna();
            }
        }
    }
}
