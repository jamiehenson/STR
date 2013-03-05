using UnityEngine;
using System.Collections;

public class Civilian : MonoBehaviour {
	void Update () {
		if (gameObject.transform.position.x >= 200 || gameObject.transform.position.x <= -200) Destroy(gameObject);
		if (gameObject.transform.position.y >= 200 || gameObject.transform.position.y <= -200) Destroy(gameObject);
	}
}
