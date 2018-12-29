using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run2Ground : MonoBehaviour {

    public Material neutral;
    public Material correct;
    public Material incorrect;

    public float timeColor = 1f;
    private WaitForSeconds waiter;

    private MeshRenderer mr;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        waiter = new WaitForSeconds(timeColor);
    }

    IEnumerator ChangeColor(Material material)
    {
        mr.material = material;
        yield return waiter;
        mr.material = neutral;
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
}
