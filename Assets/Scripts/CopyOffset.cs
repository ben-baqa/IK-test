using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyOffset : MonoBehaviour
{
    // TODO: fix this garbage
    public Transform target, reference;

    private Vector3 initialPosition, initialDelta;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.localPosition;
        initialDelta = target.position - reference.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = target.position - reference.position;
        transform.localPosition = initialPosition + delta - initialDelta;
    }
}
