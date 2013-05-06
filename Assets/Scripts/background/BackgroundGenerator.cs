using UnityEngine;
using System.Collections;

public class BackgroundGenerator : MonoBehaviour 
{
    private int spawnProbability = 30;
	private Object[] objects;
	
	void Start () 
	{
		objects = Resources.LoadAll("bg/randomObjects", typeof(Transform));
		if (Network.isServer && !transform.parent.parent.name.Equals("Universe1")) this.enabled = false;
		StartCoroutine("genBGElement");
	}
	
	IEnumerator genBGElement()
	{
		while(true)
		{
			int diceRoller = Random.Range(0,spawnProbability);
			int objX = 10140;
			int objY = Random.Range(-80,80);
			int objZ = Random.Range(70,110);
					
			if (diceRoller == 4) 
			{
				int objChoice = Random.Range (0,objects.Length);
				Transform spawnObj = (Transform) objects[objChoice];

				float prob = 0;

				switch(spawnObj.name)
				{
					case "moon1": prob = 0.2f; break;
					case "sun1": prob = 0.1f; break;
					case "debris1": prob = 0.75f; break;
					case "debris2": prob = 0.7f; break;
					case "yellow1": prob = 0.2f; break;
					case "morticles": prob = 0.05f; break;
					case "gasPocket1": prob = 0.7f; break;
					case "gasPocket2": prob = 0.7f; break;
					default: prob = 1; break;
				}

				if (prob >= Random.Range (0f,1f))
				{
					Transform obj = (Transform) Instantiate(spawnObj, new Vector3(objX,objY,objZ), new Quaternion(0,0,0,0));
					obj.name = spawnObj.name;
					if (obj.name == "gasPocket1") obj.particleSystem.startColor = new Color(Random.Range (0.5f,1), Random.Range (0.5f,1), Random.Range (0.5f,1), 1.0f);
					if (obj.name == "gasPocket2") obj.particleSystem.startColor = new Color(Random.Range (0.5f,1), Random.Range (0.5f,1), Random.Range (0.5f,1), 1.0f);
				}
			}
			yield return new WaitForSeconds(1);
		}
	}
}
