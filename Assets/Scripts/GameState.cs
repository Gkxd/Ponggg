using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour
{
    public GameObject ballPrefab;

    private static GameState instance;

    public static PongBall Ball { get; set; }
    
    void Start()
    {
        instance = this;
        Ball = ((GameObject)Instantiate(ballPrefab, Vector3.zero, Quaternion.identity)).GetComponent<PongBall>();
    }
    
    void Update()
    {

    }
}
