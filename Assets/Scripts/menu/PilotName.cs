using UnityEngine;
using System.Collections;

public class PilotName : MonoBehaviour {
	public static bool showBox = false, showBox2 = false, showLimitBox = false;
	
	void Start () {
		iTween.FadeTo(gameObject, 0.6f, 0.1f);	
	}

	void OnMouseEnter() {
		iTween.FadeTo(gameObject, 1.0f, 0.5f);
	}
	
	void OnMouseExit() {
		iTween.FadeTo(gameObject, 0.6f, 0.5f);
	}
	
	void OnMouseUp ()
	{
		if(gameObject.name.Equals("SDetailsNameBox")) showBox = true;
		if(gameObject.name.Equals("SDetailsNameBox2")) showBox2 = true;
		if(gameObject.name.Equals("SDetailsLimitBox")) showLimitBox = true;
	}
}
