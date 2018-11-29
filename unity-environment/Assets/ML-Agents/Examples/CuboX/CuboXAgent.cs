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
        float initialDistanceToObjective = Vector3.Distance(objective.transform.localPosition, this.transform.localPosition);
        float horizontal = 0, vertical = 0;

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {

            horizontal = Mathf.RoundToInt(Mathf.Clamp(vectorAction[0], -1, 1));
            vertical = Mathf.RoundToInt(Mathf.Clamp(vectorAction[1], -1, 1));

        }
        else if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete){
            switch ((int)vectorAction[0]){
                case 0:
                    horizontal = 1;
                    break;
                case 1:
                    horizontal = -1;
                    break;
                case 2:
                    vertical = 1;
                    break;
                case 3:
                    vertical = -1;
                    break;
            }
        }

        float newX = transform.localPosition.x + (horizontal * speed * Time.deltaTime);
        newX = Mathf.Clamp(newX, -7.3f, 7.3f);
        //transform.localPosition = new Vector3(newX, 0, 0);

        float newZ = transform.localPosition.z + (vertical * speed * Time.deltaTime);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Objective")){
            AddReward(30f);
            Done();
        }
    }
}
