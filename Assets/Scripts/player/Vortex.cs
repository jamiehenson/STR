using UnityEngine;
using System.Collections;

public class Vortex : MonoBehaviour {
    private bool scaleswitch;
    private int i;
    private float growth = 0.015f;

    public static IEnumerator grow(GameObject vortex)
    {
        float x = 0;
        vortex.transform.localScale = new Vector3(0, 0, 0);
        while (x <= 4)
        {
            x += 0.05f;
            vortex.transform.localScale = new Vector3(x, 0, x);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public static IEnumerator shrink(GameObject vortex)
    {
        float x = 1;
        while (x > 0)
        {
            x -= 0.01f;
            vortex.transform.localScale = new Vector3(x, x, x);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(vortex);
    }

    void Start()
    {
        StartCoroutine(grow(gameObject));
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

    void OnCollisionEnter(Collision collision)
    {
		HudOn.Instance.enteredVortex();
    }
	
	void OnCollisionExit(Collision collision)
    {
		HudOn.Instance.leftVortex();
    }
}
