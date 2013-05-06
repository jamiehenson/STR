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
		Color[] colours = {Color.red, Color.blue, Color.green, Color.magenta, Color.yellow, Color.cyan};
		Color chosenOne = colours[Random.Range(0,colours.Length)];
		RenderSettings.fog = true;
		RenderSettings.fogDensity = Random.Range(0.0005f,0.001f);
		RenderSettings.fogColor = chosenOne;
		RenderSettings.ambientLight = chosenOne;
	}
}