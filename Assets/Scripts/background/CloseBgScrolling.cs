using UnityEngine;
using System.Collections;

public class CloseBgScrolling : MonoBehaviour {

    public Transform Planet;
    public int numberOfPlanets = 1;

    private Transform planet;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < numberOfPlanets; i++) {
            Vector3 pos = new Vector3(Mathf.Abs(transform.localPosition.x / 2), Mathf.Abs(transform.localPosition.y / 2), 0);

            planet = (Transform)Instantiate(Planet, transform.position + pos, Quaternion.identity);
            planet.transform.Translate(pos);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
