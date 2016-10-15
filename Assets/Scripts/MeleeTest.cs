using UnityEngine;
using System.Collections;

public class MeleeTest : MonoBehaviour
{


    public GameObject attack;

    private new Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 aimDirection = Vector3.ProjectOnPlane((camera.ScreenToWorldPoint(Input.mousePosition)), camera.transform.forward).normalized;
            Quaternion aimRotation = Quaternion.FromToRotation(camera.transform.right, aimDirection);
            MeleeAttack meleeAttack = ((GameObject)Instantiate(attack, Vector3.zero, aimRotation)).GetComponent<MeleeAttack>();
            meleeAttack.clockWise = Vector3.Dot(aimDirection, Vector3.up) > 0;
        }
    }
}
