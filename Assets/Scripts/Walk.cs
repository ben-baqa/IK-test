using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour
{
    public Transform footL, footR, core;
    public float moveSpeed, stepDist, footOffset;

    public Rigidbody nose;
    public float nosePush;

    private ControlKey input;

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
        pos += core.right * offsetMod * footOffset;
        // determine horizontal distance from foot to hip
        Vector3 dist = pos - foot.position;
        dist = Vector3.ProjectOnPlane(dist, Vector3.up);

        if (dist.magnitude > stepDist)
        {
            // locate point to step to
            pos += dist.normalized * stepDist * .99f;
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

    void FixedUpdate()
    {
        Vector3 dir = Vector3.zero;
        if (input["up"])
            dir += Vector3.forward;
        if (input["down"])
            dir += Vector3.back;
        if (input["right"])
            dir += Vector3.right;
        if (input["left"])
            dir += Vector3.left;

        core.position += (dir.normalized * moveSpeed);
        nose.AddForce(-dir.normalized * moveSpeed * nosePush);

        Step(footR);
        Step(footL, -1);
    }
}
