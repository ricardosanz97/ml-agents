using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunAgent : Agent {

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
    public float speed = 3;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //bc = GetComponent<BoxCollider>();
    }

    public override void AgentReset()
    {
        Vector3 spikesPos = new Vector3(0f, 2.68f, -1000f);
        Vector3 pitPos = new Vector3(0f, -0.99f, -1000f);
        Vector3 objPos = new Vector3(0f, -0.99f, -1000f);

        //reset agent position
        transform.localPosition = initialPosition;

        if (RunAcademy.spikesOffset < 0 && RunAcademy.pitOffset < 0)
        {
            objPos = GetObjectivePosition(RunAcademy.objectiveOffset);
        }
        else if (RunAcademy.spikesOffset < 0)
        {
            do
            {
                pitPos = GetPitPosition(RunAcademy.pitOffset);
                objPos = GetObjectivePosition(RunAcademy.objectiveOffset);
            }
            while (Mathf.Abs(pitPos.z - objPos.z) < 3.5f
            || objPos.z < pitPos.z);

        }
        else if (RunAcademy.pitOffset < 0)
        {
            do
            {
                spikesPos = GetSpikesPosition(RunAcademy.spikesOffset);
                objPos = GetObjectivePosition(RunAcademy.objectiveOffset);
            }
            while (Mathf.Abs(spikesPos.z - objPos.z) < 5f
            || objPos.z < spikesPos.z);
        }
        else
        {
            do
            {
                pitPos = GetPitPosition(RunAcademy.pitOffset);
                spikesPos = GetSpikesPosition(RunAcademy.spikesOffset);
                objPos = GetObjectivePosition(RunAcademy.objectiveOffset);
            }
            while (Mathf.Abs(pitPos.z - objPos.z) < 3.5f
            || Mathf.Abs(spikesPos.z - objPos.z) < 5f
            || Mathf.Abs(pitPos.z - spikesPos.z) < 3.5f + 3f
            || objPos.z < spikesPos.z
            || objPos.z < pitPos.z);
        }

        //reset objective position, spikes position and pit position
        objective.transform.localPosition = objPos;
        spikes.transform.localPosition = spikesPos;
        pit.transform.localPosition = pitPos;

        rb.velocity = Vector3.zero;
    }

    private Vector3 GetSpikesPosition(float offset)
    {
        return new Vector3(spikes.transform.localPosition.x, spikes.transform.localPosition.y, Random.Range((4f + spikes.GetComponent<BoxCollider>().size.z / 2), offset));
    }

    private Vector3 GetPitPosition(float offset)
    {
        return new Vector3(pit.transform.localPosition.x, pit.transform.localPosition.y, Random.Range((4f + pit.GetComponent<BoxCollider>().size.z / 2), offset));
    }

    private Vector3 GetObjectivePosition(float offset)
    {
        return new Vector3(0f, -0.99f, Random.Range((4f + objective.GetComponent<BoxCollider>().size.z / 2), offset));
    }

    /*
    private Vector3 GetSpikesPosition(float offset)
    {
        return new Vector3(spikes.transform.localPosition.x, spikes.transform.localPosition.y, Random.Range(offset - 4f, offset));
    }

    private Vector3 GetPitPosition(float offset)
    {
        return new Vector3(pit.transform.localPosition.x, pit.transform.localPosition.y, Random.Range(offset - 4f, offset));
    }

    private Vector3 GetObjectivePosition(float offset)
    {
        return new Vector3(0f, -0.99f, Random.Range(offset - 4f, offset));
    }
    */
    public override void CollectObservations()
    {
        Vector3 pitPos = pit.transform.localPosition;
        Vector3 spikesPos = spikes.transform.localPosition;
        Vector3 objectivePos = objective.transform.localPosition;

        float distObjective = objective.transform.localPosition.z - transform.localPosition.z;
        float distSpikes = spikes.transform.localPosition.z - transform.localPosition.z;
        float distPit = pit.transform.localPosition.z - transform.localPosition.z;

        AddVectorObs(distObjective);
        AddVectorObs(objectivePos);
          
        if (RunAcademy.spikesOffset > 0) //si hay algun pincho
        {
            AddVectorObs(distSpikes);
            AddVectorObs(spikesPos);
        }
        else
        {
            AddVectorObs(0);
            AddVectorObs(Vector3.zero);
        }
        if (RunAcademy.pitOffset > 0) //si hay algun foso
        {
            AddVectorObs(distPit);
            AddVectorObs(pitPos);
        }
        else
        {
            AddVectorObs(0);
            AddVectorObs(Vector3.zero);
        }
        AddVectorObs(transform.localPosition.y);
        AddVectorObs(grounded ? 1f : 0f);
        AddVectorObs(crouched ? 1f : 0f);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.0005f);

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            float firstDistance = Mathf.Abs(transform.localPosition.z - objective.transform.localPosition.z);
            if ((int)vectorAction[0] == 0)
            {
                Move();
            }
            if ((int)vectorAction[0] == 1)
            {
                if (grounded && !crouched) Jump();
            }
            if ((int)vectorAction[0] == 2)
            {
                if (!crouched && !grounded) Crouch();
            }
            if ((int)vectorAction[0] == 3)
            {
                if (crouched && grounded) LiftUp();
            }
            float secondDistance = Mathf.Abs(transform.localPosition.z - objective.transform.localPosition.z);

            if (Mathf.RoundToInt(secondDistance) < Mathf.RoundToInt(firstDistance))
            {
                AddReward(0.001f);
            }
        }
    }

    private void Move()
    {
        rb.MovePosition(rb.position + (Vector3.forward * RunAcademy.speed * Time.fixedDeltaTime));
    }

    private void Jump()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void Crouch()
    {
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
                AddReward(-0.1f);
                Done();
            }
            else
            {
                goodJob.Invoke();
                AddReward(60f);
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
