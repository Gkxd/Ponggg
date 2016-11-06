using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameState : NetworkBehaviour
{
    public static PongBall Ball { get; set; }
    public static int PlayerCounter
    {
        get { if (instance) return instance.playerCounter; return -1; }
        set { if (instance) instance.playerCounter = value; }
    }

    public GameObject ballPrefab;

    private static GameState instance;
    
    private int playerCounter;

    void Start()
    {
        if (!instance) instance = this;
        else Destroy(gameObject);

        if (!GameObject.FindObjectOfType<PongBall>())
        {
            GameObject ballObject = (GameObject)Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            Ball = ballObject.GetComponent<PongBall>();
            NetworkServer.Spawn(ballObject);
        }
        else
        {
            Ball = GameObject.FindObjectOfType<PongBall>();
        }
    }
}
