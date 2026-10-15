using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ObstaculeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstacle;
    [SerializeField] GameObject JefeFinal;

    Coroutine spawnObstacleCoroutine;
    private void Start()
    {
        spawnObstacleCoroutine = StartCoroutine(SpawnObstacle());
        StartCoroutine(SpawnFinal());
        JefeFinal.SetActive(false);
    }

    private IEnumerator SpawnObstacle()
    {
        while (true)
        {
             int randomIndex = Random.Range(0, obstacle.Length);

            float minTime = 0.6f;
            float maxTime = 1.8f;
            float randomTime = Random.Range(minTime, maxTime);
            Instantiate(obstacle[randomIndex], transform.position, Quaternion.identity);
            yield return new WaitForSeconds(randomTime);
        }       
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
    }

   private IEnumerator SpawnFinal()
    {
        yield return new WaitForSeconds(45f);
        Debug.Log("Ya pasaron 45 Segundos");
        if (spawnObstacleCoroutine != null)
        {
            StopCoroutine(spawnObstacleCoroutine);
            Debug.Log("Se paro el Spawn De Obstaculos");
        }
        JefeFinal.SetActive(true);
        Debug.Log("El enemgo salio");
    }
}
