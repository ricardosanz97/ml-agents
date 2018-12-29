using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Run2Agent : Agent {

    public Vector3 initialPosition;
    public GameObject objective;
    public GameObject spikes;
    public GameObject pit;

    public float jumpForce = 20f;
    public bool grounded = false;
    public bool crouched = false;

    public UnityEvent goodJob;
    public UnityEvent badJob;

    private Rigidbody rb;
    //private BoxCollider bc;
    public float speed = 6;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //bc = GetComponent<BoxCollider>();
    }

    public override void AgentReset()
    {
        //reset agent position
        transform.localPosition = initialPosition;
        Vector3 spikesPos = new Vector3(0f, 2.68f, -1000f);
        Vector3 pitPos = new Vector3(0f, -0.99f, -1000f);
        Vector3 objPos = new Vector3(0f, -0.99f, -1000f);

        int side = Random.value > 0.5f ? 1 : -1; //left or right?

        if (Run2Academy.spikesOffset < 0 && Run2Academy.pitOffset < 0)
        {
            objPos = GetObjectivePosition(Run2Academy.objectiveOffset, side);
        }
        else if (Run2Academy.spikesOffset < 0)
        {
            do
            {
                pitPos = GetPitPosition(Run2Academy.pitOffset, side);
                objPos = GetObjectivePosition(Run2Academy.objectiveOffset, side);
            }
            while (Mathf.Abs(pitPos.z - objPos.z) < 3.5f
            || side * objPos.z < pitPos.z * side);
        }
        else if (Run2Academy.pitOffset < 0)
        {
            do
            {
                spikesPos = GetSpikesPosition(Run2Academy.spikesOffset, side);
                objPos = GetObjectivePosition(Run2Academy.objectiveOffset, side);
            }
            while (Mathf.Abs(spikesPos.z - objPos.z) < 5f
            || side * objPos.z < spikesPos.z * side);
        }
        else
        {
            do
            {
                pitPos = GetPitPosition(Run2Academy.pitOffset, side);
                spikesPos = GetSpikesPosition(Run2Academy.spikesOffset, side);
                objPos = GetObjectivePosition(Run2Academy.objectiveOffset, side);
            }
            while (Mathf.Abs(pitPos.z - objPos.z) < 3.5f 
            || Mathf.Abs(spikesPos.z - objPos.z) < 5f 
            || Mathf.Abs(pitPos.z - spikesPos.z) < 3.5f + 3f
            || side * objPos.z < spikesPos.z * side
            || side * objPos.z < pitPos.z * side);

        }
        objective.transform.localPosition = objPos;
        spikes.transform.localPosition = spikesPos;
        pit.transform.localPosition = pitPos;

        rb.velocity = Vector3.zero;

    }

    private Vector3 GetSpikesPosition(float offset, int side)
    {
        return new Vector3(spikes.transform.localPosition.x, spikes.transform.localPosition.y, Random.Range(side * (4f + spikes.GetComponent<BoxCollider>().size.z/2), side * offset));
    }

    private Vector3 GetPitPosition(float offset, int side)
    {
        return new Vector3(pit.transform.localPosition.x, pit.transform.localPosition.y, Random.Range(side * (4f + pit.GetComponent<BoxCollider>().size.z/2), offset * side));
    }

    private Vector3 GetObjectivePosition(float offset, int side)
    {
        return new Vector3(0f, -0.99f, Random.Range(side * (2f + objective.GetComponent<BoxCollider>().size.z / 2), offset * side));
    }

    public override void CollectObservations()
    {
        float distObjective = objective.transform.localPosition.z - transform.localPosition.z;
        float distSpikes = spikes.transform.localPosition.z - transform.localPosition.z;
        float distPit = pit.transform.localPosition.z - transform.localPosition.z;
        float distObjPit = pit.transform.localPosition.z - objective.transform.localPosition.z;
        float distObjSpikes = spikes.transform.localPosition.z - objective.transform.localPosition.z;

        AddVectorObs(distObjective);  //1
        if (Run2Academy.spikesOffset > 0)
        {
            AddVectorObs(distSpikes); //2
        }
        else
        {
            AddVectorObs(0);
        }
        if (Run2Academy.pitOffset > 0)
        {
            AddVectorObs(distPit); //3
        }
        else
        {
            AddVectorObs(0);
        }
        if (Run2Academy.pitOffset > 0)
        {
            AddVectorObs(distObjPit); //4
        }
        else
        {
            AddVectorObs(0);
        }
        if (Run2Academy.spikesOffset > 0)
        {
            AddVectorObs(distObjSpikes); //5
        }
        else
        {
            AddVectorObs(0);
        }

        AddVectorObs(transform.position.y); //6
        AddVectorObs(grounded ? 1f : 0f); //7
        AddVectorObs(crouched ? 1f : 0f); //8
        AddVectorObs(transform.localPosition.z - objective.transform.localPosition.z < 0 ? 1 : -1); //9
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //UpdateGrounded();
        if (transform.localPosition.y <= -3f)
        {
            badJob.Invoke();
            AddReward(-200f);
            Done();
        }

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            if ((int)vectorAction[0] == 0)
            {
                Move(Vector3.forward);
            }
            if ((int)vectorAction[0] == 1)
            {
                Move(Vector3.back);
            }
            if ((int)vectorAction[0] == 2)
            {
                if (grounded) Jump();
            }
            if ((int)vectorAction[0] == 3)
            {
                if (!crouched) Crouch();
            }
            if ((int)vectorAction[0] == 4)
            {
                if (crouched) LiftUp();
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
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void Crouch()
    {
        Debug.Log("crouching!");
        crouched = true;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - transform.localScale.y / 2, transform.localPosition.z);
        transform.localScale = new Vector3(transform.localScale.x, Mathf.Clamp(transform.localScale.y / 2, transform.localScale.y / 2, transform.localScale.y), transform.localScale.z);
    }

    private void LiftUp()
    {
        Debug.Log("lifting up!");
        crouched = false;
        transform.localScale = new Vector3(transform.localScale.x, Mathf.Clamp(transform.localScale.y * 2, transform.localScale.y / 2, transform.localScale.y *2), transform.localScale.z);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + transform.localScale.y / 2, transform.localPosition.z);
    }

    /*
    public void UpdateGrounded()
    {
        grounded = false;
        Collider[] colliders = Physics.OverlapBox(this.transform.localPosition + Vector3.down * 0.05f, bc.size / 2, transform.rotation);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null && colliders[i].CompareTag("ground"))
            {
                grounded = true;
                break;
            }
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("spikes"))
        {
            badJob.Invoke();
            AddReward(-1f);
            Done();
        }

        if (other.CompareTag("pit"))
        {
            badJob.Invoke();
            AddReward(-1f);
            Done();
        }

        if (other.CompareTag("Objective"))
        {
            if (crouched)
            {
                badJob.Invoke();
                AddReward(-1f);
                Done();
            }
            else
            {
                goodJob.Invoke();
                AddReward(120f);
                Done();
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            grounded = false;
        }
    }

}
