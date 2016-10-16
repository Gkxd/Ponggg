using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameState : NetworkBehaviour
{
    public GameObject ballPrefab;

    private static GameState instance;

    public static PongBall Ball { get; set; }
    
    void Start()
    {
      instance = this;

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
    
    void Update()
    {

    }
}
