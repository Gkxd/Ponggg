using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
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

    public int hp { get; private set; }
    public int mp { get; private set; }

    void Start()
    {

    }

    void Update()
    {

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
