using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Vector2 mouseSensitivity, gamepadSensitivity;
    public Transform target;
    public float followLerp;

    private Transform horRot, vertRot;
    private Camera cam;
    private Vector2 rot;

    // Start is called before the first frame update
    void Start()
    {
        horRot = transform.GetChild(0);
        vertRot = horRot.GetChild(0);
        cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        horRot.localEulerAngles = rot.x * Vector3.up;
        vertRot.localEulerAngles = rot.y * Vector3.right;

        transform.position = Vector3.Lerp(
            transform.position, target.position,
            Time.deltaTime * followLerp);

        cam.transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 delta = Mouse.current.delta.ReadValue();
        delta.x *= mouseSensitivity.x;
        delta.y *= -mouseSensitivity.y;

        rot += delta;
        rot.y = Mathf.Clamp(rot.y, -20, 70);
        rot.x = LoopAngle(rot.x);
    }

    private float LoopAngle(float angle, float centre = 180)
    {
        if (angle > centre + 180)
            return LoopAngle(angle - 360);
        else if (angle < centre - 180)
            return LoopAngle(angle + 360);
        return angle;
    }
}
