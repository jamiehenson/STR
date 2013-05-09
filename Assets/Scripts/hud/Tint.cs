using UnityEngine;
using System.Collections;

public class Tint : MonoBehaviour
{
	void Start ()
	{
		SetRandomColour();
	}

	public void SetColour(Color colour)
	{
		RenderSettings.fog = true;
		RenderSettings.fogDensity = Random.Range(0.0005f,0.001f);
		RenderSettings.fogColor = colour;
		RenderSettings.ambientLight = colour;
	}

	public void SetRandomColour()
	{
		float vibrance = 0.75f;
		Color[] colours = {new Color(vibrance,0,0), new Color(0,vibrance,0), new Color(0,0,vibrance), new Color(vibrance, vibrance, 0), new Color(0, vibrance, vibrance), new Color(vibrance, 0, vibrance)};

		Color chosenOne = colours[Random.Range(0,colours.Length)];
		RenderSettings.fog = true;
		RenderSettings.fogDensity = Random.Range(0.0005f,0.001f);
		RenderSettings.fogColor = chosenOne;
		RenderSettings.ambientLight = chosenOne;
	}
}