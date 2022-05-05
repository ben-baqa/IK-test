using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : MonoBehaviour
{
    [HideInInspector]
    public Vector3 forward;

    [Header("Movement")]
    public float moveSpeed;
    public float friction, turnLerp;

    [Header("Steps")]
    public Transform footL;
    public Transform footR;
    public StepCurves stepCurves;
    public float stepDist, footOffset,
        stepTime, settleTime,
        sideStepMod, stepHeightMod;

    private Transform steppingFoot;
    private Vector3 stepTarget, stepRef, stepAnchor;
    private float stepProgress = 0, settleTimer;
    private bool settled = false;

    [Header("Turning steps")]
    public float turnStepDegs;

    private float rot, turnAnchor;

    [Header("Juice")]
    public Transform core;
    public Transform head;

    private Vector3 headPos;

    [Header("Nose stuff")]
    public Rigidbody nose;
    public float nosePush;

    private Rigidbody rb;
    private ControlKey input;
    private Vector3 dir;
    private Vector3 vPrev = Vector3.zero;
    private Vector2 inputDir = Vector2.zero;

    private bool rightStep;


    private Vector3 debug = Vector3.zero, debug2 = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(footL.position, 0.1f);
        Gizmos.DrawSphere(footR.position, 0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<ControlKey>();
        rb = GetComponentInChildren<Rigidbody>();
        stepAnchor = rb.position;
        headPos = head.localPosition;
    }

    private void Step()
    {
        // select which foot to place/check
        Transform foot = rightStep ? footR : footL;

        // set pos to "hip" position
        Vector3 pos = rb.position;
        pos += rb.transform.forward / 4;
        //pos += core.right * footOffset * (rightStep ? 1 : -1);

        // determine planar distance from core to last step location
        Vector3 dist = pos - stepAnchor;
        dist = Vector3.ProjectOnPlane(dist, Vector3.up);

        // distance has been travelled, step must be taken
        if (dist.magnitude > stepDist ||
            (settled && dist.magnitude > stepDist / 8))
        {
            // reset reference position
            stepAnchor = pos;

            // locate point to step to
            // apply hip offset
            pos += rb.transform.right * footOffset *
                (rightStep ? 1 : -1);
            // apply forward offset
            pos += dir * stepDist * .999f;

            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 1000))
            {
                steppingFoot = rightStep ? footR : footL;
                stepRef = steppingFoot.position;
                stepTarget = hit.point;
                stepProgress = settleTimer = 0;
                turnAnchor = rot;
                settled = false;
                rightStep = !rightStep;
            }
        }

        settleTimer += Time.deltaTime;
        // standing still, move feet to rest position
        if (!settled && stepProgress == 1 && settleTimer > settleTime)
        {
            // reset reference position
            stepAnchor = pos;

            // locate point to step to
            // apply hip offset
            pos += rb.transform.right * footOffset *
                (rightStep ? 1 : -1);
            // remove forward offset
            pos -= rb.transform.forward / 4;

            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 1000))
            {
                steppingFoot = rightStep ? footR : footL;
                stepRef = steppingFoot.position;
                stepTarget = hit.point;
                stepProgress = 0;
                rightStep = !rightStep;
                if (settleTimer >= settleTime + stepTime)
                    settled = true;
            }
        }
    }

    private void MoveFoot()
    {
        if (!steppingFoot)
            return;

        // number representing progress of step [0, 1]
        stepProgress = Mathf.Clamp01(
            stepProgress + Time.deltaTime / stepTime);

        // get height of foot
        float h = (stepProgress == 1) ? 0 :
            -Mathf.Sin(1 - stepProgress)
            * Mathf.Log(1 - stepProgress);

        // get adjecent offset
        Vector3 adj = Vector3.Cross(Vector3.up,
            stepRef - stepTarget)
            * (rightStep ? 1 : -1);
        adj *= -Mathf.Sin(stepProgress)
            * Mathf.Log(stepProgress);

        // place foot
        steppingFoot.position =
            Vector3.Lerp(stepRef, stepTarget, stepProgress)
            + Vector3.up * h * stepHeightMod
            + adj * sideStepMod;

        core.localPosition = Vector3.up * h / 3;
    }

    void FixedUpdate()
    {
        // Get input direction
        Vector3 dir = Vector3.zero;
        Vector3 right = -Vector3.Cross(forward, Vector3.up);

        // Get input Vector
        Vector3 inputDir = Vector2.zero;
        if (input["up"])
            inputDir.y += 1;
        if (input["down"])
            inputDir.y -= 1;
        if (input["right"])
            inputDir.x += 1;
        if (input["left"])
            inputDir.x -= 1;

        // store input direction if it is meaningful
        if (inputDir.magnitude > 0)
            this.inputDir = inputDir.normalized;

        dir = inputDir.y * forward + inputDir.x * right;
        dir = dir.normalized;
        // store movement direction if it is meaningful
        if (dir.magnitude > 0)
            this.dir = dir;

        // move body * apply universal friction
        rb.AddForce(dir * moveSpeed);
        rb.velocity *= 1 - friction;

        // calculate acceleration and use it to offset nose
        Vector3 diff = vPrev - rb.velocity;
        vPrev = rb.velocity;

        // run step logic
        Step();
        MoveFoot();
    }

    public void Rotate(Vector3 forward, float camRot)
    {
        this.forward = forward;
        rot = Mathf.LerpAngle(rot, camRot + Mathf.Rad2Deg *
            Mathf.Atan2(inputDir.x, inputDir.y),
            turnLerp * rb.velocity.magnitude);
        rb.rotation = Quaternion.Euler(Vector3.up * rot);
    }
}
