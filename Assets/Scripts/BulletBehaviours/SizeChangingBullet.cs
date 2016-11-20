using UnityEngine;
using System.Collections;

using UsefulThings;

public class SizeChangingBullet : MonoBehaviour
{
    public Curve sizeChangeCurve;

    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        transform.localScale = Vector3.one * sizeChangeCurve.Evaluate(Time.time - startTime);
    }
}
