using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    public Transform target;
    public Vector3 force;
    public float friction, maxDistance;

    private Rigidbody rb;

    private Vector3 debug = Vector3.zero;


    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(debug, .25f);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        debug = rb.position;
        Vector3 dir = target.position - rb.position;
        bool oob = dir.magnitude > maxDistance;

        if (oob)
            rb.position = target.position - dir.normalized * maxDistance;

        dir.x *= force.x;
        dir.y *= force.y;
        dir.z *= force.z;

        if (oob)
            rb.velocity = dir;
        else
        {
            rb.AddForce(dir);
            rb.velocity *= 1 - friction;
        }
    }
}
