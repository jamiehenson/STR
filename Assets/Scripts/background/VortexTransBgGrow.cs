// Vortex transition animation script
// Written in D flat
// J. Henson

using UnityEngine;
using System.Collections;

public class VortexTransBgGrow : MonoBehaviour
{
	void Start ()
	{
		//float lower = 0.8f, upper = 1f;
		//gameObject.particleSystem.startColor = new Color(Random.Range (lower,upper), Random.Range (lower,upper), Random.Range (lower,upper), 1.0f);
		Color[] colourRange = {Color.red,Color.blue,Color.green,Color.yellow,Color.white};
		gameObject.particleSystem.startColor = colourRange[Random.Range (0,5)];
	}

	void Update ()
	{
		//gameObject.particleSystem.startSize += 0.05f;
		//gameObject.particleSystem.startSpeed += 0.05f;
		var a = GetComponent<ParticleSystemRenderer>();
		if (a.lengthScale < 10) a.lengthScale += 0.3f;
		if (a.velocityScale < 0.3f) a.velocityScale += 0.003f;
	}
}
