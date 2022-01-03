using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour
{
    [HideInInspector]
    public Vector3 forward;

    [Header("Movement")]
    public float moveSpeed;
    public float friction, turnLerp;

    [Header("Steps")]
    public Transform footL;
    public Transform footR;
    public float stepDist, footOffset;

    [Header("Nose stuff")]
    public Rigidbody nose;
    public float nosePush;

    private Rigidbody core;
    private ControlKey input;
    private Vector3 stepTarget, lastStep, dir;
    private Vector3 vPrev = Vector3.zero;

    private bool rightStep;


    private Vector3 debug = Vector3.zero, debug2 = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(footL.position, 0.1f);
        Gizmos.DrawSphere(footR.position, 0.1f);
        //Gizmos.DrawSphere(debug, 0.2f);
        //Gizmos.DrawSphere(debug2, 0.2f);
    }

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<ControlKey>();
        core = GetComponentInChildren<Rigidbody>();
        lastStep = core.position;
    }

    // Update is called once per frame
    private void Update()
    {
        //Vector3 dist, pos = core.position;
        //dist = pos - footL.position;
        //if (dist.magnitude > stepDist)
        //    footL.Translate(dist.normalized * stepDist);
        //dist = pos - footR.position;
        //if (dist.magnitude > stepDist)
        //    footR.Translate(dist.normalized * stepDist);

        Debug.DrawRay(footL.position, Vector3.down);
        Debug.DrawRay(footR.position, Vector3.down);
    }

    private void Step(Transform foot, float offsetMod = 1)
    {
        // locate hip position
        Vector3 pos = core.position;
        pos += core.transform.right * offsetMod * footOffset;
        // determine horizontal distance from foot to hip
        Vector3 dist = pos - foot.position;
        dist = Vector3.ProjectOnPlane(dist, Vector3.up);

        // distance has been travelled, step is taken
        if (dist.magnitude > stepDist)
        {
            // reset reference position
            lastStep = core.position;

            // locate point to step to
            pos += dir * stepDist * .99f;
            if (offsetMod > 0)
                debug = pos;
            else
                debug2 = pos;

            RaycastHit hit;
            if (Physics.Raycast(pos,
                Vector3.down, out hit, 1000)) {
                foot.position = hit.point;
                print("Step Taken");
            }
        }
    }

    private void Step()
    {
        // select which foot to place/check
        Transform foot = rightStep ? footR : footL;

        // set pos to "hip" position
        Vector3 pos = core.position;
        //pos += core.right * footOffset * (rightStep ? 1 : -1);

        // determine planar distance from core to last step location
        Vector3 dist = pos - lastStep;
        dist = Vector3.ProjectOnPlane(dist, Vector3.up);

        // distance has been travelled, step must be taken
        if (dist.magnitude > stepDist)
        {
            // reset reference position
            lastStep = core.position;

            // locate point to step to
                // apply hip offset
            pos += core.transform.right * footOffset *
                (rightStep ? 1 : -1);
                // apply forward offset
            pos += dir * stepDist * .999f;

            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 1000))
            {
                (rightStep ? footR : footL).position = hit.point;
                rightStep = !rightStep;
            }
        }
    }

    void FixedUpdate()
    {
        // Get input direction
        Vector3 dir = Vector3.zero;
        Vector3 right = -Vector3.Cross(forward, Vector3.up);
        if (input["up"])
            dir += forward;
        if (input["down"])
            dir -= forward;
        if (input["right"])
            dir += right;
        if (input["left"])
            dir -= right;
        dir = dir.normalized;
        // store input direction if it is meaningful
        if (dir.magnitude > 0)
            this.dir = dir;

        // move body * apply universal friction
        core.AddForce(dir * moveSpeed);
        core.velocity *= 1 - friction;

        // calculate acceleration and use it to offset nose
        Vector3 diff = vPrev - core.velocity;
        vPrev = core.velocity;
        print(diff.magnitude);
        nose.AddForce(-diff * nosePush);

        // run step logic
        Step();
    }

    public void Rotate(Vector3 forward, float rot)
    {
        this.forward = forward;
        float r = core.rotation.eulerAngles.y;
        r = Mathf.LerpAngle(r, rot,
            turnLerp * core.velocity.magnitude);
        core.rotation = Quaternion.Euler(Vector3.up * r);
    }
}
