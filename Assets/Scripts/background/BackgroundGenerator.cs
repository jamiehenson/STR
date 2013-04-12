using UnityEngine;
using System.Collections;

public class BackgroundGenerator : MonoBehaviour 
{
	private int spawnProbability = 100, count=0;	
	private Object[] objects;
	
	void Start () 
	{
		objects = Resources.LoadAll("bg/randomObjects", typeof(Transform));

		if (Network.isServer && !transform.parent.parent.name.Equals("Universe1"))
			this.enabled = false;
	}
	
	void Update () 
	{
		int diceRoller = Random.Range(0,spawnProbability);
		int objX = 10140;
		int objY = Random.Range(-80,80);
		int objZ = Random.Range (70,110);
				
		if (diceRoller == 47) 
		{
			int objChoice = Random.Range (0,objects.Length);
			Transform obj = (Transform) Instantiate((Transform) objects[objChoice], new Vector3(objX,objY,objZ), new Quaternion(0,0,0,0));
			//obj.name = objects[objChoice].name;
			//if (obj.name == "gasPocket2" || obj.name == "gasPocket1")
			//{
		//		obj.particleSystem.startColor = new Color(Random.Range (0.5f,1), Random.Range (0.5f,1), Random.Range (0.5f,1), 1.0f);
			//}
			count++;
			//print (count);
		}
	}
}
