using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instace;
    public Transform target;

    private float startFOV, targetFOV;

    public float zoomSpeed = 1f;

    public Camera theCam;

    private void Awake()
    {
        instace = this;
    }
    void Start()
    {
        startFOV = theCam.fieldOfView;
        targetFOV = startFOV;
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;

        theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }

    public void ZoomIn(float newZoom)
    {
        targetFOV = newZoom;
    }

    public void ZoomOut()
    {
        targetFOV = startFOV;
    }

}
