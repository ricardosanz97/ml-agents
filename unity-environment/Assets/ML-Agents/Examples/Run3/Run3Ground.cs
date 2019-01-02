using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run3Ground : MonoBehaviour {

    public Material neutral;
    public Material correct;
    public Material incorrect;

    public float timeColor = 1f;
    private WaitForSeconds waiter;

    private MeshRenderer mr;

    public List<GameObject> grounds = new List<GameObject>();

    public GameObject groundHasPlayer;
    public Vector3 ground1Pos;
    public Vector3 ground2Pos;
    public Vector3 ground3Pos;

    public GameObject ground1;
    public GameObject ground2;
    public GameObject ground3;

    public GameObject groundToTranslate;

    private void Awake()
    {
        waiter = new WaitForSeconds(timeColor);
        ground1Pos = ground1.transform.localPosition;
        ground2Pos = ground2.transform.localPosition;
        ground3Pos = ground3.transform.localPosition;
        //ResetGrounds();
    }

    public void ResetGrounds()
    {
        this.transform.localPosition = Vector3.zero;
        transform.GetChild(0).localPosition = ground1Pos;
        transform.GetChild(1).localPosition = ground2Pos;
        transform.GetChild(2).localPosition = ground3Pos;

        grounds.Clear();
        grounds.Add(ground1);
        grounds.Add(ground2);
        grounds.Add(ground3);

        groundHasPlayer = null;
    }

    IEnumerator ChangeColor(Material material)
    {
        for (int i = 0; i<this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).GetComponent<ObstacleBehaviour>() == null)
            {
                this.transform.GetChild(i).GetComponent<MeshRenderer>().material = material;
            }
        }
        yield return waiter;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).GetComponent<ObstacleBehaviour>() == null)
            {
                this.transform.GetChild(i).GetComponent<MeshRenderer>().material = neutral;
            }
        }
    }

    public void GoodJob()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeColor(correct));
    }

    public void BadJob()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeColor(incorrect));
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.back * Run3Academy.agentSpeed * Time.fixedDeltaTime);
    }

    internal void ChangeGroundsOrder()
    {
        Debug.Log("change grounds order.");
        //vector0 ahora es vector2
        //vector1 ahora es vector0
        //vector2 ahora es vector1
        grounds[0].transform.Translate(Vector3.forward * grounds[0].transform.localScale.z * 3);
        GameObject aux = grounds[0];
        grounds[0] = grounds[1];
        grounds[1] = grounds[2];
        grounds[2] = aux;


    }
}
