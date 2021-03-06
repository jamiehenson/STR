using UnityEngine;
using System.Collections;

public class FlowingBGMain : MonoBehaviour {
	
	public int spawnProbability = 200;
	public Transform BG;
	private bool notcopied = true;
	private Vector3 startPos;
	
	private Object[] spacePix, objects;
	
	void Start () 
	{
		startPos = gameObject.transform.position;
		spacePix = Resources.LoadAll("bg/space", typeof(Texture2D));
		objects = Resources.LoadAll("bg/randomObjects", typeof(Transform));
		gameObject.renderer.material.mainTexture = (Texture2D) spacePix[Random.Range (0,spacePix.Length)];
	}

	void Update () 
	{
		if (Network.isClient)
		{
			transform.Translate(Vector3.right * Time.deltaTime * 2);
			if (transform.position.x < 0 && notcopied) {
				Transform newBG = (Transform) Network.Instantiate (BG, new Vector3(startPos.x+100,0,120), new Quaternion(0,0,0,0),200);
				newBG.renderer.material.mainTexture = (Texture2D) spacePix[Random.Range (0,spacePix.Length)];
				newBG.name = "BG slice";
				newBG.transform.Rotate(new Vector3(90, 180, 0));
				notcopied = false;
			}
			if (transform.position.x < startPos.x-2000) Destroy(gameObject);

			networkView.RPC("makeBGelement", RPCMode.Server);
		}
	}
	
	[RPC]
	void makeBGelement()
	{
		int diceRoller = Random.Range(0,spawnProbability);
		int objX = Random.Range(130,170);
		int objY = Random.Range(-60,60);
		int objZ = Random.Range(10,30);
				
		if (diceRoller == 47) 
		{
			int objChoice = Random.Range (0,objects.Length);
			Transform obj = (Transform) Network.Instantiate((Transform) objects[objChoice], new Vector3(objX,objY,objZ), new Quaternion(0,0,0,0),200);
			obj.name = objects[objChoice].name;
			if (obj.name == "gasPocket2" || obj.name == "gasPocket1")
			{
				obj.particleSystem.startColor = new Color(Random.Range (0.5f,1), Random.Range (0.5f,1), Random.Range (0.5f,1), 1.0f);
			}
			Vector3 forceToApply = new Vector3(5,0,0);
			obj.rigidbody.AddForce(forceToApply);
		    obj.rigidbody.freezeRotation = true;
		}
	}
}
