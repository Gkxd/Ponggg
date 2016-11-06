using UnityEngine;
using System.Collections;

/// <summary>
/// Forces a camera to be a specific aspecct ratio.
/// Adapted from http://wiki.unity3d.com/index.php?title=AspectRatioEnforcer
/// </summary>
[RequireComponent(typeof(Camera))]
public class ForceCameraAspectRatio : MonoBehaviour
{

    public float aspectRatio;

    private Camera camera;

    // Use this for initialization
    void Start()
    {
        camera = GetComponent<Camera>();
        SetAspectRatio();
    }

    // Update is called once per frame
    void Update()
    {
        SetAspectRatio();
    }

    private void SetAspectRatio()
    {
        float currentAspectRatio = (float)Screen.width / Screen.height;
        if (Mathf.Abs(currentAspectRatio / aspectRatio - 1) < 0.01f)
        {
            camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        }
        else if (currentAspectRatio > aspectRatio)
        {
            float inset = 1.0f - aspectRatio / currentAspectRatio;
            camera.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);
        }
        else
        {
            float inset = 1.0f - currentAspectRatio / aspectRatio;
            camera.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);
        }
    }
}
