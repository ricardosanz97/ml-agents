using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushAgent : Agent {

    public GameObject objective;
    public GameObject cube;
    public float speed = 3f;
    public float jumpForce = 14f;
    public bool grounded = false;
    public float minimumDistanceFinish = 0.2f;
    public UnityEvent goodJob;
    public UnityEvent badJob;

    private Rigidbody rb;
    private BoxCollider bc;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    public override void AgentReset()
    {
        Vector3 agentPos, cubePos, objectivePos;
        do
        {
            agentPos = new Vector3(Random.Range(-PushAcademy.limit, PushAcademy.limit), 0f, 0f);
            objectivePos = new Vector3(Random.Range(-PushAcademy.cubeLimit - minimumDistanceFinish, PushAcademy.cubeLimit + minimumDistanceFinish), objective.transform.localPosition.y, 0f);

            float offsetX = Random.Range(minimumDistanceFinish, PushAcademy.max_offset) * (Random.value > .5f ? 1f : -1f);
            cubePos = new Vector3(Mathf.Clamp(objectivePos.x + offsetX, -PushAcademy.cubeLimit, PushAcademy.cubeLimit), 0, 0);

        } while (Mathf.Abs(objectivePos.x - cubePos.x) <= minimumDistanceFinish ||
          Vector3.Distance(agentPos, cubePos) < 1f);



        rb.velocity = Vector3.zero;
        transform.localPosition = agentPos;
        objective.transform.localPosition = objectivePos;
        cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
        cube.transform.localPosition = cubePos;
    }

    public override void CollectObservations()
    {
        AddVectorObs(cube.transform.localPosition.x - transform.localPosition.x);
        AddVectorObs(transform.localPosition.y);
        AddVectorObs(objective.transform.localPosition.x > cube.transform.localPosition.x ? 1f : 0f);
        AddVectorObs(grounded ? 1f : 0f);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        UpdateGrounded();
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            switch ((int)vectorAction[0])
            {
                case 0:
                    Move(Vector3.right);
                    break;
                case 1:
                    Move(Vector3.left);
                    break;
                case 2:
                    if (grounded) Jump();
                    break;

            }

            if (Mathf.Abs(transform.localPosition.x) > PushAcademy.limit || Mathf.Abs(cube.transform.localPosition.x) > PushAcademy.cubeLimit)
            {
                badJob.Invoke();
                AddReward(-1f);
                Done();
            }

            else if (Mathf.Abs(cube.transform.localPosition.x - objective.transform.localPosition.x) < minimumDistanceFinish)
            {
                goodJob.Invoke();
                AddReward(30f);
                Done();
            }
        }
    }

    public void UpdateGrounded()
    {
        grounded = false;
        Collider[] colliders = Physics.OverlapBox(this.transform.localPosition + Vector3.down * 0.05f, bc.size / 2, transform.rotation);
        for (int i = 0; i<colliders.Length; i++)
        {
            if (colliders[i] != null && colliders[i].CompareTag("ground"))
            {
                grounded = true;
                break;
            }

        }
    }

    private void Move(Vector3 direction)
    {
        rb.MovePosition(rb.position + (direction * speed * Time.fixedDeltaTime));
    }

    private void Jump()
    {
        Debug.Log("jumping!");
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}
