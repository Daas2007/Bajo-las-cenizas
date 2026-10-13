using UnityEngine;

public class MiniPersonaje3 : MonoBehaviour
{
    Rigidbody2D peronajeRB;
    [SerializeField] float fuerzaArriba;

    [SerializeField] Transform detector;
    [SerializeField] LayerMask piso;
    [SerializeField] float radioDetector;

    private void Start()
    {
        peronajeRB = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool enPiso = Physics2D.OverlapCircle(detector.position, radioDetector,piso);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (enPiso)
            {
                peronajeRB.AddForce(Vector2.up * fuerzaArriba);

            }
        }            
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(detector.position, radioDetector);
    }
}
