using Unity.Mathematics;
using UnityEngine;

public class Camara : MonoBehaviour
{
    [SerializeField] float Sensibilidad = 100f;
    [SerializeField] Transform Player;
    [SerializeField] float rotacionHorizontal = 0f;
    [SerializeField] float rotacionVertical = 0f;
    void Start()
    {
        OcultarMouse();
    }

    void Update()
    {
        float valorX = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float valorY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;

        Debug.Log($"MouseEnX = {valorX:F1}, MouseEnY = {valorY:F1}");

        rotacionHorizontal += valorX;
        rotacionVertical -= valorY;

        rotacionHorizontal = math.clamp(rotacionVertical, -80f, 80f);

        transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
         
        Player.Rotate(Vector3.up * valorX);
    }

    public void OcultarMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
