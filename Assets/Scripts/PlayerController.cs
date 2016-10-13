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

    [Header("Miscellaneous")]
    public Camera playerCamera;
    #endregion

    public int hp { get; private set; }
    public int mp { get; private set; }

    public float lastTimeOfMeleeAttack { get; private set; }
    public float lastTimeOfBasicAttack { get; private set; }
    public float lastTimeOfSpecialAttack { get; private set; }
    public float lastTimeOfUltimateAttack { get; private set; }

    private new Rigidbody rigidbody;
    private int playerId;

    void Start()
    {
      rigidbody = GetComponent<Rigidbody>();
      StartCoroutine(RecoverHp());
      StartCoroutine(RecoverMp());
      playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void FixedUpdate()
    {
      if (isLocalPlayer)
      {
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
      }
    }

    void Update()
    {
    if (!isLocalPlayer)
      return;

      Vector3 aimDirection = Vector3.ProjectOnPlane((playerCamera.ScreenToWorldPoint(Input.mousePosition) - rigidbody.position), playerCamera.transform.forward).normalized;
      Quaternion aimRotation = Quaternion.FromToRotation(playerCamera.transform.right, aimDirection);

      if (Input.GetKey(KeySettings.MELEE_ATTACK) &&
          (Time.time - lastTimeOfMeleeAttack) > meleeAttackCooldown)
      {
        lastTimeOfMeleeAttack = Time.time;
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
    }

    [Command]
    void CmdBasicAttack(Quaternion aim)
    {
      var attack = (GameObject)Instantiate(basicAttack, rigidbody.position, aim);
      NetworkServer.Spawn(attack);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<BulletId>().playerId == playerId) return; // 
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
