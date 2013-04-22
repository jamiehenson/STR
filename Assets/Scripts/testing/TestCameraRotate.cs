using UnityEngine;
using System.Collections;

public class TestCameraRotate : MonoBehaviour
{

	// Don't use this class in the main game! Used for background development.

	private bool cameraBehind;
	void Update ()
	{
		if (Input.GetKeyDown("r"))
		{
			int direction = (cameraBehind) ? 1 : -1;
			iTween.MoveBy(gameObject, new Vector3(direction * 20, 0, direction * 4), 2);
        	iTween.RotateBy(gameObject, new Vector3(0, direction * -0.25f, 0), 2);
			cameraBehind = !cameraBehind;
		}
	}
}
