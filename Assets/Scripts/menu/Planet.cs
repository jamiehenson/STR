using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {
	
	public float rotSpeed;
	private float rotSimSpeed;
	
	void Start () {
		rotSimSpeed = (Random.Range (0,0.8f));
	}
	
	void Update () {
		transform.Rotate(Vector3.up * Time.deltaTime * rotSimSpeed);
	}
	
}
