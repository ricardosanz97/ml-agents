using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboXAgent : Agent {

    public GameObject objective;
    public float speed = 3;

    public override void AgentReset()
    {
        //reset agent position
        gameObject.transform.localPosition = Vector3.zero;

        //ramdomly set objective position between a range
        objective.transform.localPosition = new Vector3(Random.Range(-7f, 7f)
                                                        * (Random.value <= .5f ? 1 : -1), 0, Random.Range(-7f, 7f));

    }

    public override void CollectObservations()
    {
        Vector3 relativePosition = objective.transform.localPosition - transform.localPosition;

        AddVectorObs(relativePosition.x / 15f);
        AddVectorObs(relativePosition.z / 15f);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous){

            float initialDistanceToObjective = Vector3.Distance(objective.transform.localPosition, this.transform.localPosition);

            float newX = transform.localPosition.x + (vectorAction[0] * speed * Time.deltaTime);
            newX = Mathf.Clamp(newX, -7.3f, 7.3f);
            //transform.localPosition = new Vector3(newX, 0, 0);

            float newZ = transform.localPosition.z + (vectorAction[1] * speed * Time.deltaTime);
            newZ = Mathf.Clamp(newZ, -7.3f, 7.3f);
            transform.localPosition = new Vector3(newX, 0, newZ);

            float finalDistanceToObjective = Vector3.Distance(objective.transform.localPosition, this.transform.localPosition);

            if (finalDistanceToObjective < initialDistanceToObjective){
                //going to the player, good job.
                AddReward(0.1f);
            }
            else{
                //not going to the player, bad job. 
                AddReward(-0.3f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Objective")){
            AddReward(30f);
            Done();
        }
    }
}
