using UnityEngine;
using System.Collections;

public class FlowingBGBehind : MonoBehaviour {

	public Transform BG;
	private bool notcopied = true;
	
	private Object[] spacePix, objects;
	
	void Start () 
	{
		spacePix = Resources.LoadAll("bg/space", typeof(Texture2D));
		gameObject.renderer.material.mainTexture = (Texture2D) spacePix[Random.Range (0,spacePix.Length)];
		GameObject otherBG = gameObject.transform.parent.Find("Corner Plane").gameObject;
		otherBG.renderer.material.mainTexture = gameObject.renderer.material.mainTexture;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.right * Time.deltaTime * 3f);
		if (transform.position.x < 0 && notcopied) {
			Transform newBG = (Transform) Instantiate (BG, new Vector3(940,0,200), new Quaternion(0,0,0,0));
			newBG.renderer.material.mainTexture = (Texture2D) spacePix[Random.Range (0,spacePix.Length)];
			newBG.name = "BG slice";
			newBG.transform.Rotate(new Vector3(90, 180, 0));
			notcopied = false;
		}
		if (transform.position.x < -400) Destroy(gameObject);
	}
}
