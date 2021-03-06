using UnityEngine;
using System.Collections;

public class BackgroundGenerator : MonoBehaviour 
{
    public int spawnProbability = 5;
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
				bool solid = false;

				switch(spawnObj.name)
				{
					case "moon1": prob = 0.4f; solid = true; break;
					case "sun1": prob = 0.3f; solid = true; break;
					case "debris1": prob = 0.75f; break;
					case "debris2": prob = 0.7f; break;
					case "grey1": prob = 0.6f; solid = true; break;
					case "yellow1": prob = 0.4f; solid = true; break;
					case "morticles": prob = 0.3f; break;
					case "gasPocket1": prob = 0.7f; break;
					case "gasPocket2": prob = 0.7f; break;
					case "rock1": prob = 0.9f; solid = true; break;
					case "rock2": prob = 0.9f; solid = true; break;
					case "rock3": prob = 0.9f; solid = true; break;
					case "rock4": prob = 0.9f; solid = true; break;
					case "rock5": prob = 0.9f; solid = true; break;
					case "rock6": prob = 0.9f; solid = true; break;
					case "corner1": prob = 0.7f; solid = true; break;
					case "cylinder1": prob = 0.7f; solid = true; break;
					case "cylinder2": prob = 0.7f; solid = true; break;
					case "cylinder3": prob = 0.75f; solid = true; break;
					case "cylinder4": prob = 0.7f; solid = true; break;
					case "wing1": prob = 0.7f; solid = true; break;
					case "grid1": prob = 0.8f; solid = true; break;
					case "grid2": prob = 0.85f; solid = true; break;
					default: prob = 1; break;
				}

				if (prob >= Random.Range (0.0f,1.0f))
				{
					Transform obj = (Transform) Instantiate(spawnObj, new Vector3(objX,objY,objZ), new Quaternion(0,0,0,0));
					obj.name = spawnObj.name;
					if (obj.name == "gasPocket1") obj.particleSystem.startColor = new Color(Random.Range (0.5f,1), Random.Range (0.5f,1), Random.Range (0.5f,1), 1.0f);
					if (obj.name == "gasPocket2") obj.particleSystem.startColor = new Color(Random.Range (0.5f,1), Random.Range (0.5f,1), Random.Range (0.5f,1), 1.0f);
					if (solid) obj.localScale = new Vector3(Random.Range (0.5f,1.5f),Random.Range (0.5f,1.5f),Random.Range (0.5f,1.5f));
				}
			}
			yield return new WaitForSeconds(0.4f);
		}
	}
}
