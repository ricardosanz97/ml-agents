using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Run3Agent : Agent {

    public float jumpForce = 20f;
    public bool grounded = false;
    public bool crouched = false;

    public UnityEvent goodJob;
    public UnityEvent badJob;

    private Rigidbody rb;
    public Vector3 initialPosition;

    public GameObject nextObstacle;
    public GameObject spawner;
    public GameObject grounds;

    private void Awake()
    {
        initialPosition = this.transform.localPosition;
        rb = GetComponent<Rigidbody>();
    }

    public override void AgentReset()
    {
        grounds.GetComponent<Run3Ground>().ResetGrounds();
        transform.localPosition = initialPosition;
        nextObstacle.transform.localPosition = new Vector3(nextObstacle.transform.localPosition.x, nextObstacle.transform.localPosition.y,
                                                                spawner.transform.localPosition.z);
        rb.velocity = Vector3.zero;
    }

    public override void CollectObservations()
    {
        AddVectorObs(nextObstacle.transform.localPosition);
        AddVectorObs(nextObstacle.transform.localPosition.z - transform.localPosition.z);
        AddVectorObs(grounded ? 1 : 0);
        AddVectorObs(crouched ? 1 : 0);
        AddVectorObs(transform.localPosition.y);
        AddVectorObs(nextObstacle == spawner.GetComponent<SpawnerController>().pit ? 1 : 0);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            if ((int)vectorAction[0] == 0)
            {
                if (grounded && !crouched) Jump();
            }
            if ((int)vectorAction[0] == 1)
            {
                if (!crouched && grounded) Crouch();
            }

            if ((int)vectorAction[0] == 2)
            {
                if (crouched && grounded) LiftUp();
            }

        }
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
        crouched = false;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + transform.localScale.y / 2, transform.localPosition.z);
        transform.localScale = new Vector3(transform.localScale.x, Mathf.Clamp(transform.localScale.y * 2, transform.localScale.y / 2, transform.localScale.y * 2), transform.localScale.z);
        grounded = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("spikes"))
        {
            Debug.Log("tocamos pinchos");
            badJob.Invoke();
            if (!grounded)
            {
                AddReward(-15f);
            }
            else
            {
                AddReward(-1f);
            }
            Destroy(nextObstacle);
            spawner.GetComponent<SpawnerController>().SpawnObstacle();
            Done();
        }

        if (other.CompareTag("pit"))
        {
            Debug.Log("tocamos foso");
            badJob.Invoke();
            if (crouched)
            {
                AddReward(-5f);
            }
            else
            {
                AddReward(-1f);
            }
            Destroy(nextObstacle);
            spawner.GetComponent<SpawnerController>().SpawnObstacle();
            Done();
        }

        if (other.CompareTag("ground"))
        {
            if (grounds.GetComponent<Run3Ground>().groundHasPlayer == grounds.GetComponent<Run3Ground>().grounds[1])
            {
                grounds.GetComponent<Run3Ground>().ChangeGroundsOrder();
            }
        }

        if (other.CompareTag("goodJob"))
        {
            goodJob.Invoke();
            AddReward(60f);
            Destroy(nextObstacle);
            spawner.GetComponent<SpawnerController>().SpawnObstacle();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            grounded = true;
            grounds.GetComponent<Run3Ground>().groundHasPlayer = collision.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            grounded = false;
            grounds.GetComponent<Run3Ground>().groundHasPlayer = collision.gameObject;
        }
    }
}
