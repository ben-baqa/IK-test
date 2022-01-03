using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpringJoint : MonoBehaviour
{
    [Header("Points")]
    public Transform anchor;
    public Transform tip;

    [Header("Settings")]
    public float spring;
    public float damp;
    public float rotSpring;
    public float rotDamp;

    public float linearStrength;
    public float rotationStrength;


    private Transform parent;

    private Vector3 prevAnchor, prevTip;
    private Vector3 prevRot, diff;

    private Vector3 rotation = Vector3.zero, rotationalVelocity = Vector3.zero;
    private float distance, linearVelocity;
    private float magnitude;


    private void OnDrawGizmos()
    {
        if (!parent)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(parent.position, 0.1f);
        Gizmos.DrawSphere(transform.position, 0.05f);
        Gizmos.DrawLine(parent.position, transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        parent = new GameObject().transform;
        parent.name = "Joint Anchor";
        parent.parent = transform.parent;
        transform.parent = parent;
        parent.localEulerAngles = Vector3.zero;
        parent.position = anchor.position;

        transform.position = tip.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyInput();
        Move();
        ApplyOutput();
    }

    private void ApplyInput()
    {
        Vector3 tipDelta = tip.position - prevTip;

        diff = tip.position - anchor.position;
        Vector3 prevDiff = prevTip - prevAnchor;
        Quaternion rotationDelta = Quaternion.FromToRotation(
            prevDiff, diff);

        rotationalVelocity += rotationDelta.eulerAngles *
            rotationStrength / 360;

        float linearDelta = Vector3.Dot(
            prevTip - tip.position, diff);

        linearVelocity += linearDelta * linearStrength;

        prevAnchor = anchor.position;
        prevRot = anchor.eulerAngles;
        prevTip = tip.position;
    }

    private void Move()
    {
        rotationalVelocity.x -= rotation.x * rotSpring;
        rotationalVelocity.y -= rotation.y * rotSpring;
        rotationalVelocity.z -= rotation.z * rotSpring;
        linearVelocity -= distance * spring;

        rotationalVelocity.x *= rotDamp;
        rotationalVelocity.y *= rotDamp;
        rotationalVelocity.z *= rotDamp;
        linearVelocity *= damp;

        rotation += rotationalVelocity;
        distance += linearVelocity;
    }

    private void ApplyOutput()
    {
        parent.position = anchor.position;
        //parent.eulerAngles = anchor.eulerAngles + rotation;
        //parent.localEulerAngles = rotation;
        parent.eulerAngles = parent.parent.eulerAngles + rotation;
        transform.position = tip.position + diff.normalized * distance;
    }
}
