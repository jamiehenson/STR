using UnityEngine;
using System.Collections;

public class ExhaustColour : MonoBehaviour
{
	void Start ()
	{
		Color[] colours = {Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.white};
		gameObject.particleSystem.startColor = colours[Random.Range (0,colours.Length)];
	}
}
