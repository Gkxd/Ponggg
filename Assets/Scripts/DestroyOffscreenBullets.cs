using UnityEngine;
using System.Collections;

public class DestroyOffscreenBullets : MonoBehaviour
{
    void Update()
    {
        if (transform.position.x > 20 || transform.position.x < -20 || transform.position.y > 15 || transform.position.y < -15)
        {
            Destroy(gameObject);
        }
    }
}
