using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyOffset : MonoBehaviour
{
    // TODO: fix this garbage
    public Transform target;

    private Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = new GameObject().transform;
        parent.name = name + "Offset parent";
        parent.parent = transform.parent;
        parent.position = target.position;

        transform.parent = parent;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    Vector3 delta = target.position - reference.position;
    //    transform.localPosition = initialPosition + delta - initialDelta;
    //}

    private void FixedUpdate()
    {
        parent.position = target.position;
    }
}
