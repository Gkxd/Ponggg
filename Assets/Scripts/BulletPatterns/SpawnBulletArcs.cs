using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpawnBulletArcs : _BulletSpawner
{
    [Tooltip("The bullet that you want to spawn. Must have a rigidbody or this will crash.")]
    public GameObject bullet;

    [Tooltip("The number of bullets in each arc")]
    public int arcAmount;
    [Tooltip("The angle of the arc of bullets")]
    public float arcRange;


    [Tooltip("The number of arcs that you will shoot")]
    public int arcLayers;
    [Tooltip("The speed of the slowest layer")]
    public float minSpeed;
    [Tooltip("The speed of the fastest layer")]
    public float maxSpeed;

    void Start()
    {
        Vector3 startDirection = Quaternion.Euler(new Vector3(0, 0, -arcRange * 0.5f)) * transform.right;
        Vector3 endDirection = Quaternion.Euler(new Vector3(0, 0, arcRange * 0.5f)) * transform.right;

        // Debug visuals
        // Debug.DrawRay(transform.position, startDirection, Color.red, 100);
        // Debug.DrawRay(transform.position, endDirection, Color.blue, 100);

        // Spawn the bullets
        for (int i = 0; i < arcLayers; i++)
        {
            float layerSpeed = Mathf.Lerp(minSpeed, maxSpeed, i / (arcLayers - 1f));
            for (int j = 0; j < arcAmount; j++)
            {
                Vector3 direction = Vector3.Slerp(startDirection, endDirection, j / (arcAmount - 1f));
                GameObject b = (GameObject)Instantiate(bullet, transform.position, Quaternion.identity);
                b.GetComponent<Rigidbody>().velocity = layerSpeed * direction;
                b.GetComponent<BulletId>().playerId = playerId;
            }
        }

        Destroy(gameObject); // Destroy the gameObject that created the bullets
    }
}
