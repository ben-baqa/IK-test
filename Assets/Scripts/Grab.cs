using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grab : MonoBehaviour
{
    public Transform holdTarget;
    public Transform handR, handL;

    public float grabDistance, grabForce, damp;


    private Grabbable box;
    private Vector3 restPositionR, restPositionL;

    private bool grabbing;

    // Start is called before the first frame update
    void Start()
    {
        restPositionR = handR.localPosition;
        restPositionL = handL.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!grabbing)
                box = FindBox();
            else
                grabbing = false;
        }
    }

    private void FixedUpdate()
    {
        if (grabbing)
        {
            if (!box)
                return;

            box.rb.AddForce((holdTarget.position - box.rb.position) * grabForce);
            box.rb.velocity *= damp;

            handR.position = box.collider.ClosestPoint(
                box.rb.position + transform.right);
            handL.position = box.collider.ClosestPoint(
                box.rb.position - transform.right);
        }
        else
        {
            handR.localPosition = Vector3.Lerp(handR.localPosition, restPositionR, .1f);
            handL.localPosition = Vector3.Lerp(handL.localPosition, restPositionL, .1f);
        }
    }

    private Grabbable FindBox()
    {
        Vector3 grabPos = holdTarget.position;
        Grabbable closest = FindObjectOfType<Grabbable>();
        float dist = (closest.rb.position - holdTarget.position).magnitude;

        foreach (Grabbable g in FindObjectsOfType<Grabbable>())
        {
            float newDist = (g.rb.position - holdTarget.position).magnitude;
            if (newDist < dist)
            {
                closest = g;
                dist = newDist;
            }
        }

        if (dist < grabDistance)
        {
            print("Found box to grab");
            grabbing = true;
            return closest;
        }
        else
            return null;
    }
}
