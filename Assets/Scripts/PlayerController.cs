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

    #region Properties
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

        rigidbody.velocity = velocity.normalized * speed;
        #endregion

        #region Attacks
        Vector3 aimDirection = Vector3.ProjectOnPlane((playerCamera.ScreenToWorldPoint(Input.mousePosition) - rigidbody.position), playerCamera.transform.forward).normalized;
        Quaternion aimRotation = Quaternion.FromToRotation(playerCamera.transform.right, aimDirection);

        if (Input.GetKey(KeySettings.MELEE_ATTACK) &&
            (Time.time - lastTimeOfMeleeAttack) > meleeAttackCooldown)
        {
            lastTimeOfMeleeAttack = Time.time;
            CmdMeleeAttack(aimRotation);
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
        //GameObject attack = (GameObject)Instantiate(basicAttack, rigidbody.position, aim);
        //attack.GetComponent<_BulletSpawner>().playerId = netId.Value;
        //NetworkServer.Spawn(attack);
        Debug.LogError("Basic Attack Server " + Time.time);
        RpcBasicAttack(aim);
    }

    [ClientRpc]
    void RpcBasicAttack(Quaternion aim)
    {
        Debug.LogError("Basic Attack Client " + Time.time);
        GameObject attack = (GameObject)Instantiate(basicAttack, rigidbody.position, aim);
        attack.GetComponent<_BulletSpawner>().playerId = netId.Value;
    }

    [Command]
    void CmdMeleeAttack(Quaternion aim)
    {
        //GameObject attack = (GameObject)Instantiate(meleeAttack, rigidbody.position, aim);
        //attack.GetComponent<MeleeAttack>().targetObject = this.gameObject;
        //NetworkServer.Spawn(attack);
        Debug.LogError("Melee Attack Server " + Time.time);
        RpcMeleeAttack(aim);
    }

    [ClientRpc]
    void RpcMeleeAttack(Quaternion aim)
    {
        Debug.LogError("Melee Attack Client " + Time.time);
        GameObject attack = (GameObject)Instantiate(meleeAttack, rigidbody.position, aim);
        attack.GetComponent<MeleeAttack>().targetObject = this.gameObject;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<BulletId>().playerId == netId.Value) return;
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
