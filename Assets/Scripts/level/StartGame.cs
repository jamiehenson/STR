using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

    public Transform gamePrefab;

	// Use this for initialization
	void Start () {
        Instantiate(gamePrefab, new Vector3(8, -2, 84), Quaternion.identity);       
	}
}
