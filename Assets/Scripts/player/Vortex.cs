using UnityEngine;
using System.Collections;

public class Vortex : MonoBehaviour {
    private bool scaleswitch;
    private int i;
    private float growth = 0.015f;

    IEnumerator grow()
    {
        float x = 0;
        transform.localScale = new Vector3(0, 0, 0);
        while (x <= 4)
        {
            x += 0.05f;
            transform.localScale = new Vector3(x, 0, x);
            yield return new WaitForSeconds(0.01f);
        }
    }

    void Start()
    {
        StartCoroutine("grow");
    }

	void Update () {
	    transform.Rotate(Vector3.up * Time.deltaTime * 40f);
        transform.Rotate(Vector3.forward * Time.deltaTime * 10f);

        if (i == 0) scaleswitch = true;
        else if (i == 150) scaleswitch = false;

        if (scaleswitch)
        {
            transform.localScale += new Vector3(growth, 0, growth);
            i++;
        }
        else
        {
            transform.localScale += new Vector3(-growth, 0, -growth);
            i--;
        }
	}
}
