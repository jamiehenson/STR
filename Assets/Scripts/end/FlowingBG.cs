using UnityEngine;
using System.Collections;

public class FlowingBG : MonoBehaviour {
	
	public Transform BG;
	private bool notcopied = true;
	
	void Update () {
		transform.Translate(Vector3.left * Time.deltaTime * 4);
		if (transform.position.x > 0 && notcopied) {
			Transform newBG = (Transform) Instantiate (BG, new Vector3(-80,0,50), new Quaternion(0,0,0,0));
			newBG.name = "BG slice";
			newBG.transform.Rotate(new Vector3(90, 180, 0));
			notcopied = false;
		}
		if (transform.position.x > 300) Destroy(gameObject);
	}
}
