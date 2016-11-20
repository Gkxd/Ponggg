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
        Debug.LogError("Player ID set to " + GameState.PlayerCounter);
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
        newPosition.y = Mathf.Clamp(newPosition.y, -5, 4);

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
            yield return new WaitForSeconds(hpRecoverTime);
        }
    }

    private IEnumerator RecoverMp()
    {
        while (!GameState.IsPlaying) yield return null;

        while (true)
        {
            mp = Mathf.Min(mp + 1, maxMp);
            yield return new WaitForSeconds(mpRecoverTime);
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
