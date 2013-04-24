// Vortex transition animation script (inner part)
// Written in D flat
// J. Henson

using UnityEngine;
using System.Collections;

public class VortexTransInner : MonoBehaviour
{
	void Start ()
	{
		Color[] colourRange = {Color.red,Color.blue,Color.green,Color.yellow,Color.white};
		gameObject.particleSystem.startColor = colourRange[Random.Range (0,5)];
	}

	void Update ()
	{
		gameObject.particleSystem.startSize += 0.001f;
	}
}
