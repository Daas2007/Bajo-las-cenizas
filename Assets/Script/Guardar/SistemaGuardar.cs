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
        escribir.Write(gm.muertes);
        escribir.Write(gm.piezasRecogidas);
        escribir.Write(gm.puzzle1Completado);
        escribir.Write(gm.puzzle2Completado);
        escribir.Write(gm.cristalMetaActivo);

        archivo.Close();
    }

    public static void Cargar(MovimientoPersonaje player, GameManager gm)
    {
        if (!File.Exists(Ubicacion)) return;

        var archivo = File.Open(Ubicacion, FileMode.Open);
        var leer = new BinaryReader(archivo, Encoding.Default, false);

        Vector3 pos;
        pos.x = leer.ReadSingle();
        pos.y = leer.ReadSingle();
        pos.z = leer.ReadSingle();
        player.transform.position = pos;

        gm.enemigo.SetActive(leer.ReadBoolean());
        gm.tieneLinterna = leer.ReadBoolean();
        gm.muertes = leer.ReadInt32();
        gm.piezasRecogidas = leer.ReadInt32();
        gm.puzzle1Completado = leer.ReadBoolean();
        gm.puzzle2Completado = leer.ReadBoolean();
        gm.cristalMetaActivo = leer.ReadBoolean();

        archivo.Close();
    }
}
