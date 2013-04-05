using UnityEngine;
using System.Collections;

public class SimpleTranslate : MonoBehaviour 
{
	public float speed = 1;
	private float simSpeed;
	
	void Start ()
	{
		simSpeed = -(Random.Range (speed-(speed*0.5f),speed+(speed*0.5f)))/5;
	}
	
	void Update () 
	{
		gameObject.transform.Translate(new Vector3(simSpeed,0,0));
	}
}
