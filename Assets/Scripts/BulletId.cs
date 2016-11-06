using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BulletId : MonoBehaviour
{
    //[SyncVar]
    private int _playerId;

    public int playerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    }
}
