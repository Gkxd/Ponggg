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
    public static PlayerController Player0
    {
        get { if (instance) return instance.player0; return null; }
        set { if (instance) instance.player0 = value; }
    }
    public static PlayerController Player1
    {
        get { if (instance) return instance.player1; return null; }
        set { if (instance) { instance.player1 = value; SpawnBall(); } }
    }

    public GameObject ballPrefab;

    private static GameState instance;

    private int playerCounter;
    private PlayerController player0;
    private PlayerController player1;

    void Start()
    {
        if (!instance) instance = this;
        else Destroy(gameObject);
    }

    public static void SpawnBall()
    {
        Debug.LogError("Spawning Ball... " + Time.time);
        if (instance)
        {
            instance.StartCoroutine(instance.SpawnBallAfterDelay());
        }
    }

    private IEnumerator SpawnBallAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (!GameObject.FindObjectOfType<PongBall>())
        {
            GameObject ballObject = (GameObject)Instantiate(ballPrefab);
            Ball = ballObject.GetComponent<PongBall>();
            NetworkServer.Spawn(ballObject);
        }
        else
        {
            Ball = GameObject.FindObjectOfType<PongBall>();
        }
    }
}
