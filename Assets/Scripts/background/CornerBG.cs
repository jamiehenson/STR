using UnityEngine;
using System.Collections;

public class CornerBG : MonoBehaviour
{
	public Transform BG;
	public bool grow;
	private Vector3 origSize;

	void Start () 
	{
		gameObject.renderer.material.shader = Shader.Find ("Transparent/Diffuse");
		origSize = gameObject.transform.localScale;
	}

	public void BigLad()
	{
		grow = true;
	}

	public void Midget()
	{
		grow = false;
		gameObject.transform.localScale = origSize;
	}

	void Update ()
	{
		// Weetabix
		if (grow)
		{
			gameObject.transform.localScale *= 1.0001f;
		}

	}
}
