using UnityEngine;
using System.Collections;

using UsefulThings;

public class SizeChangingBullet : MonoBehaviour
{
    public Curve sizeChangeCurve;

    private float startTime;
    private Vector3 startSize;

    void Start()
    {
        startTime = Time.time;
        startSize = transform.localScale;
        transform.localScale = startSize * sizeChangeCurve.Evaluate(0);
    }

    void Update()
    {
        transform.localScale = startSize * sizeChangeCurve.Evaluate(Time.time - startTime);
    }
}
