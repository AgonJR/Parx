using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target1 ; // Top Left Corner of Grid
    public Transform target2 ; // Bottom Right Corner of Grid
    public float padding = 3f; // Additional padding around the objects

    private Camera _mainCamera;

    public static CameraController instance;

    void Start()
    {
        instance = this;
        _mainCamera = Camera.main;
    }

    void Update()
    {
        FrameGridToCamera();
    }

    private void FrameGridToCamera()
    {
        if (target1 != null && target2 != null)
        {
            Bounds bounds = new Bounds(target1.position, Vector3.zero);
            bounds.Encapsulate(target2.position);
            bounds.Expand(padding);

            float   distance    = Mathf.Max(bounds.size.x, bounds.size.y) / (2f * Mathf.Tan(_mainCamera.fieldOfView * Mathf.Deg2Rad / 2f));
            Vector3 newPosition = bounds.center - _mainCamera.transform.forward * distance;

            _mainCamera.transform.position = newPosition;
        }
    }
}