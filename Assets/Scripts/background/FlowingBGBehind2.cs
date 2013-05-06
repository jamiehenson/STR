using UnityEngine;
using System.Collections;

public class FlowingBGBehind2 : MonoBehaviour {

	public Transform BG;
	private bool notcopied = true;
	
	private Texture2D spacePix;
	private Vector3 startPos;
	
	void Start () 
	{
		startPos = gameObject.transform.position;
		spacePix = (Texture2D) Resources.Load("bg/space/spacebg1");
		gameObject.renderer.material.mainTexture = spacePix;
	}

	void Update () {
		transform.Translate(Vector3.right * Time.deltaTime * 2);
		if (transform.position.x < 0 && notcopied) {
			Transform newBG = (Transform) Instantiate (BG, new Vector3(startPos.x,0,200), new Quaternion(0,0,0,0));
			newBG.name = "BG slice";
			newBG.transform.Rotate(new Vector3(90, 180, 0));
			notcopied = false;
		}
		if (transform.position.x < startPos.x-2000) Destroy(gameObject);
	}
}
