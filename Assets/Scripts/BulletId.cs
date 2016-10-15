using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BulletId : NetworkBehaviour
{
    [SyncVar]
    private uint _playerId;

    public uint playerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    }
}
