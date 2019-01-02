using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour {

    public GameObject Agent;
    public GameObject pit;
    public GameObject spikes;
    private GameObject lastObstacle;
    public GameObject grounds;

    public int spawnTime;

    private void Awake()
    {
        SpawnObstacle();
    }


    public void SpawnObstacle()
    {
        float rand = Random.value;
        lastObstacle = Instantiate(rand > 0.5f ? pit : spikes, 
                                    new Vector3(transform.position.x, 
                                    rand > 0.5f ? pit.transform.localPosition.y : spikes.transform.localPosition.y, 
                                    transform.localPosition.z), 
                                    transform.localRotation);
                                    
        lastObstacle.transform.parent = grounds.transform;
        Agent.GetComponent<Run3Agent>().nextObstacle = lastObstacle;
    }
}
