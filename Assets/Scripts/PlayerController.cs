using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    #region Serialized Fields
    [Header("Player Stats")]
    public int maxHp;
    public float hpRecoverTime;
    public int maxMp;
    public float mpRecoverTime;
    public float speed;

    [Header("Player Attacks")]
    public GameObject meleeAttack;
    public float meleeAttackCooldown;

    public GameObject basicAttack;
    public float basicAttackCooldown;

    public GameObject specialAttack;
    public float specialAttackCooldown;
    public int specialAttackMpCost;

    public GameObject ultimateAttack;
    public float ultimateAttackCooldown;
    public int ultimateAttackMpCost;
    #endregion

    #region SyncVars
    [SyncVar]
    private int _playerId = -1;

    [SyncVar]
    private int _hp;

    [SyncVar]
    private int _mp;

    [SyncVar]
    private float _lastTimeOfMeleeAttack;

    [SyncVar]
    private float _lastTimeOfBasicAttack;

    [SyncVar]
    private float _lastTimeOfSpecialAttack;

    [SyncVar]
    private float _lastTimeOfUltimateAttack;
    #endregion

    #region Properties and Fields

    public int playerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    }

    public int hp
    {
        get { return _hp; }
        private set { _hp = value; }
    }
    public int mp
    {
        get { return _mp; }
        private set { _mp = value; }
    }

    public float lastTimeOfMeleeAttack
    {
        get { return _lastTimeOfMeleeAttack; }
        private set { _lastTimeOfMeleeAttack = value; }
    }
    public float lastTimeOfBasicAttack
    {
        get { return _lastTimeOfBasicAttack; }
        private set { _lastTimeOfBasicAttack = value; }
    }
    public float lastTimeOfSpecialAttack
    {
        get { return _lastTimeOfSpecialAttack; }
        private set { _lastTimeOfSpecialAttack = value; }
    }
    public float lastTimeOfUltimateAttack
    {
        get { return _lastTimeOfUltimateAttack; }
        private set { _lastTimeOfUltimateAttack = value; }
    }
    #endregion

    private new Rigidbody rigidbody;
    private Camera playerCamera;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(RecoverHp());
        StartCoroutine(RecoverMp());
        playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
        if (playerId == -1)
        {
            CmdSetPlayerNumber();
        }

        if (playerId == 0)
        {
            transform.position = new Vector3(-7, 0, 0);
        }
        else if (playerId == 1)
        {
            transform.position = new Vector3(7, 0, 0);
        }
        else
        {
            GetComponent<NetworkIdentity>().connectionToServer.Disconnect();
        }
    }

    [Command]
    void CmdSetPlayerNumber()
    {
        Debug.LogError("Player ID set to " + GameState.PlayerCounter);
        playerId = GameState.PlayerCounter++;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        #region Movement

        Vector3 velocity = Vector3.zero;
        if (Input.GetKey(KeySettings.MOVE_UP))
        {
            velocity += Vector3.up;
        }
        if (Input.GetKey(KeySettings.MOVE_DOWN))
        {
            velocity += Vector3.down;
        }
        if (Input.GetKey(KeySettings.MOVE_LEFT))
        {
            velocity += Vector3.left;
        }
        if (Input.GetKey(KeySettings.MOVE_RIGHT))
        {
            velocity += Vector3.right;
        }

        Vector3 newPosition = transform.position + velocity.normalized * speed * Time.deltaTime;
        if (playerId == 0)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, -9, -3);
        }
        else
        {
            newPosition.x = Mathf.Clamp(newPosition.x, 3, 9);
        }
        newPosition.y = Mathf.Clamp(newPosition.y, -5, 5);

        transform.position = newPosition;
        #endregion

        #region Attacks
        Vector3 aimDirection = Vector3.ProjectOnPlane((playerCamera.ScreenToWorldPoint(Input.mousePosition) - rigidbody.position), playerCamera.transform.forward).normalized;
        Quaternion aimRotation = Quaternion.FromToRotation(playerCamera.transform.right, aimDirection);

        if (Input.GetKey(KeySettings.MELEE_ATTACK) &&
            (Time.time - lastTimeOfMeleeAttack) > meleeAttackCooldown)
        {
            lastTimeOfMeleeAttack = Time.time;

            bool up = Vector3.Dot(aimDirection, Vector3.up) > 0;

            CmdMeleeAttack(aimRotation, up ^ playerId == 1);
        }
        if (Input.GetKey(KeySettings.BASIC_ATTACK) &&
            (Time.time - lastTimeOfBasicAttack) > basicAttackCooldown)
        {
            lastTimeOfBasicAttack = Time.time;
            CmdBasicAttack(aimRotation);
        }
        if (Input.GetKey(KeySettings.BASIC_ATTACK) &&
            (Time.time - lastTimeOfSpecialAttack) > specialAttackCooldown &&
            mp > specialAttackMpCost)
        {
            lastTimeOfSpecialAttack = Time.time;
        }
        if (Input.GetKey(KeySettings.BASIC_ATTACK) &&
            (Time.time - lastTimeOfUltimateAttack) > specialAttackCooldown &&
            mp > ultimateAttackMpCost)
        {
            lastTimeOfUltimateAttack = Time.time;
        }
        #endregion Attacks
    }

    [Command]
    void CmdBasicAttack(Quaternion aim)
    {
        RpcBasicAttack(aim);
    }

    [ClientRpc]
    void RpcBasicAttack(Quaternion aim)
    {
        GameObject attack = (GameObject)Instantiate(basicAttack, rigidbody.position, aim);
        attack.GetComponent<_BulletSpawner>().playerId = playerId;
    }

    [Command]
    void CmdMeleeAttack(Quaternion aim, bool clockWise)
    {
        RpcMeleeAttack(aim, clockWise);
    }

    [ClientRpc]
    void RpcMeleeAttack(Quaternion aim, bool clockWise)
    {
        MeleeAttack attack = ((GameObject)Instantiate(meleeAttack, rigidbody.position, aim)).GetComponent<MeleeAttack>();
        attack.target = transform;
        attack.clockWise = clockWise;
        
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<BulletId>().playerId == playerId) return;
        Destroy(c.gameObject);
    }

    private IEnumerator RecoverHp()
    {
        while (true)
        {
            hp = Mathf.Min(hp + 1, maxHp);
            yield return new WaitForSeconds(hpRecoverTime);
        }
    }

    private IEnumerator RecoverMp()
    {
        while (true)
        {
            mp = Mathf.Min(mp + 1, maxMp);
            yield return new WaitForSeconds(mpRecoverTime);
        }
    }
}
