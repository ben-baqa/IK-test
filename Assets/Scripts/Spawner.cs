using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;

    public float spawnTime, spawnWidth;

    private float spawnTimer;

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnTime)
        {
            spawnTimer -= spawnTime;
            Vector3 spawnLocation = Vector3.zero;
            spawnLocation.x = Random.Range(-1, 1f) * spawnWidth;
            spawnLocation.z = Random.Range(-1, 1f) * spawnWidth;

            Transform instance = Instantiate(prefab).transform;
            instance.parent = transform;
            instance.localPosition = spawnLocation;
            instance.rotation = Quaternion.Euler(Random.insideUnitSphere * 360);
        }
    }
}
