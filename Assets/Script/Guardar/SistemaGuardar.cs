using UnityEngine;
using System.IO;
using System.Text;
//using UnityEditor.Rendering;

public class SistemaGuardar : MonoBehaviour
{
    public static void  Guardar(MovimientoPersonaje Player, GameObject Enemigo, bool TieneLinterna)
    {
        string Ubicacion = Application.persistentDataPath + "ArchivoGuardado";
        var archivo = File.Open(Ubicacion, FileMode.Create);
        var escribir = new BinaryWriter(archivo, Encoding.Default, false);

        //Posicion
        escribir.Write(Player.transform.position.x);
        escribir.Write(Player.transform.position.y);
        escribir.Write(Player.transform.position.z);

        //Enemigo Activo
        escribir.Write(Enemigo.activeSelf);

        //Estado Linterna
        escribir.Write(TieneLinterna);

        archivo.Close();
    }
    public static void Cargar(MovimientoPersonaje Player, GameObject enemigo, ref bool tieneLinterna)
    {
        string Ubicacion = Application.persistentDataPath + "ArchivoGuardado";
        if (File.Exists(Ubicacion))
        {
            var archivo = File.Open(Ubicacion, FileMode.Open);
            var leer = new BinaryReader(archivo, Encoding.Default, false);

            Vector3 pos;
            pos.x = leer.ReadSingle();
            pos.y = leer.ReadSingle();
            pos.z = leer.ReadSingle();
            // Estado del enemigo

            bool enemigoActivo = leer.ReadBoolean();
            enemigo.SetActive(enemigoActivo); 
            // Estado de la linterna

            tieneLinterna = leer.ReadBoolean();

            Player.transform.position = pos;
            archivo.Close();
        }
    }
}
