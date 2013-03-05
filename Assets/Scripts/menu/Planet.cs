using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {

	void Update () {
		transform.Rotate(Vector3.up * Time.deltaTime * 1);
	}
	
}
