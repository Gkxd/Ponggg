using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[System.Serializable]
public class MoveSet
{
    public float hpRecoverTime;
    public float mpRecoverTime;
    public float speed;
    
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
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    #region Serialized Fields
    [Header("Player Stats")]
    public int maxHp;
    public int maxMp;

    public MoveSet[] moveSets;
    #endregion

    #region SyncVars
    [SyncVar]
    private int _playerId;

    [SyncVar]
    private int _playerMoveSet;

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

    public int playerMoveSet
    {
        get { return _playerMoveSet; }
        set { _playerMoveSet = (value + moveSets.Length) % moveSets.Length;}
    }

    public int hp
    {
        get { return _hp; }
        private set { _hp = Mathf.Clamp(value, 0, maxHp); }
    }
    public int mp
    {
        get { return _mp; }
        private set { _mp = Mathf.Clamp(value, 0, maxMp); }
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
    private NetworkIdentity networkIdentity;
    private RectTransform hpBar;
    private RectTransform mpBar;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        networkIdentity = GetComponent<NetworkIdentity>();
        playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        hp = maxHp;

        StartCoroutine(RecoverHp());
        StartCoroutine(RecoverMp());

        if (networkIdentity.isServer)
        {
            CmdSetPlayerNumber();
        }

        if (playerId == 0)
        {
            transform.position = new Vector3(-7, 0, 0);
            GameState.Player0 = this;
            hpBar = GameObject.Find("HP1_Fill").GetComponent<RectTransform>();
            mpBar = GameObject.Find("MP1_Fill").GetComponent<RectTransform>();
        }
        else if (playerId == 1)
        {
            transform.position = new Vector3(7, 0, 0);
            GameState.Player1 = this;
            hpBar = GameObject.Find("HP2_Fill").GetComponent<RectTransform>();
            mpBar = GameObject.Find("MP2_Fill").GetComponent<RectTransform>();
        }
        else
        {
            GetComponent<NetworkIdentity>().connectionToServer.Disconnect();
        }
    }

    [Command]
    void CmdSetPlayerNumber()
    {
        playerId = GameState.PlayerCounter++;
    }

    void Update()
    {
        #region UI Update

        hpBar.localScale = new Vector3(Mathf.Lerp(hpBar.localScale.x, hp / (float)maxHp, Time.deltaTime * 20), 1, 1);
        mpBar.localScale = new Vector3(Mathf.Lerp(mpBar.localScale.x, mp / (float)maxMp, Time.deltaTime * 20), 1, 1);

        if (hp == 0)
        {

        }

        #endregion

        // Below code should only be executed by local player
        if (!isLocalPlayer) return;

        #region Attacks
        Vector3 aimDirection = Vector3.ProjectOnPlane((playerCamera.ScreenToWorldPoint(Input.mousePosition) - rigidbody.position), playerCamera.transform.forward).normalized;
        Quaternion aimRotation = Quaternion.FromToRotation(playerCamera.transform.right, aimDirection);

        if (Input.GetKey(KeySettings.MELEE_ATTACK) &&
            (Time.time - lastTimeOfMeleeAttack) > moveSets[playerMoveSet].meleeAttackCooldown)
        {
            lastTimeOfMeleeAttack = Time.time;

            bool up = Vector3.Dot(aimDirection, Vector3.up) > 0;

            CmdMeleeAttack(aimRotation, up ^ playerId == 1);
        }
        if (Input.GetKey(KeySettings.BASIC_ATTACK) &&
            (Time.time - lastTimeOfBasicAttack) > moveSets[playerMoveSet].basicAttackCooldown)
        {
            lastTimeOfBasicAttack = Time.time;
            CmdBasicAttack(aimRotation);
        }
        if (Input.GetKey(KeySettings.SPECIAL_ATTACK) &&
            (Time.time - lastTimeOfSpecialAttack) > moveSets[playerMoveSet].specialAttackCooldown &&
            mp >= moveSets[playerMoveSet].specialAttackMpCost)
        {
            lastTimeOfSpecialAttack = Time.time;
            CmdSpecialAttack(aimRotation);
            mp -= moveSets[playerMoveSet].specialAttackMpCost;
        }
        if (Input.GetKey(KeySettings.ULTIMATE_ATTACK) &&
            (Time.time - lastTimeOfUltimateAttack) > moveSets[playerMoveSet].specialAttackCooldown &&
            mp >= moveSets[playerMoveSet].ultimateAttackMpCost)
        {
            lastTimeOfUltimateAttack = Time.time;
            CmdUltimateAttack(aimRotation);
            mp -= moveSets[playerMoveSet].ultimateAttackMpCost;
        }
        #endregion Attacks

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

        float moveSpeed = moveSets[playerMoveSet].speed;
        if (Input.GetKey(KeySettings.MELEE_ATTACK) || Input.GetKey(KeySettings.BASIC_ATTACK))
        {
            moveSpeed *= 0.5f;
        }

        Vector3 newPosition = transform.position + velocity.normalized * moveSpeed * Time.deltaTime;
        if (playerId == 0)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, -9, -3);
        }
        else
        {
            newPosition.x = Mathf.Clamp(newPosition.x, 3, 9);
        }
        newPosition.y = Mathf.Clamp(newPosition.y, -5, 4);

        transform.position = newPosition;
        #endregion

        // Switch move sets
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerMoveSet++;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerMoveSet--;
        }
    }

    [Command]
    void CmdBasicAttack(Quaternion aim)
    {
        RpcBasicAttack(aim);
    }

    [ClientRpc]
    void RpcBasicAttack(Quaternion aim)
    {
        GameObject attack = (GameObject)Instantiate(moveSets[playerMoveSet].basicAttack, rigidbody.position, aim);
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
        MeleeAttack attack = ((GameObject)Instantiate(moveSets[playerMoveSet].meleeAttack, rigidbody.position, aim)).GetComponent<MeleeAttack>();
        attack.target = transform;
        attack.clockWise = clockWise;
    }

    [Command]
    void CmdSpecialAttack(Quaternion aim)
    {
        RpcSpecialAttack(aim);
    }

    [ClientRpc]
    void RpcSpecialAttack(Quaternion aim)
    {
        GameObject attack = (GameObject)Instantiate(moveSets[playerMoveSet].specialAttack, rigidbody.position, aim);
        attack.GetComponent<_BulletSpawner>().playerId = playerId;
    }

    [Command]
    void CmdUltimateAttack(Quaternion aim)
    {
        RpcUltimateAttack(aim);
    }

    [ClientRpc]
    void RpcUltimateAttack(Quaternion aim)
    {
        GameObject attack = (GameObject)Instantiate(moveSets[playerMoveSet].ultimateAttack, rigidbody.position, aim);
        attack.GetComponent<_BulletSpawner>().playerId = playerId;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<BulletId>().playerId == playerId) return;
        Destroy(c.gameObject);

        if (networkIdentity.isServer)
        {
            hp--;
        }
    }

    private IEnumerator RecoverHp()
    {
        while (!GameState.IsPlaying) yield return null;

        while (true)
        {
            hp = Mathf.Min(hp + 1, maxHp);
            yield return new WaitForSeconds(moveSets[playerMoveSet].hpRecoverTime);
        }
    }

    private IEnumerator RecoverMp()
    {
        while (!GameState.IsPlaying) yield return null;

        while (true)
        {
            mp = Mathf.Min(mp + 1, maxMp);
            yield return new WaitForSeconds(moveSets[playerMoveSet].mpRecoverTime);
        }
    }

    public void MissedPongBall()
    {
        if (networkIdentity.isServer)
        {
            hp -= Mathf.Max(hp / 5, 10);
        }
    }
}
