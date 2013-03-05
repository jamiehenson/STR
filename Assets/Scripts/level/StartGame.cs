using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

    public Transform gamePrefab;

	// Use this for initialization
	void Start () {
        // Camera is instantiated at the 'reverse' of the given vector, so (-2.349982, -3.500479, 8.962741) in the example below 
        //Instantiate(gamePrefab, new Vector3(2.349982f, 3.500479f, -8.962741f), Quaternion.identity);
        Instantiate(gamePrefab, new Vector3(8, -2, 84), Quaternion.identity);       
	}
}
