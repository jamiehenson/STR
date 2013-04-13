using UnityEngine;
using System.Collections;

public class ShowCredits : MonoBehaviour {
	public static bool credOn;
	
	void Start () {
		// Set static var
		credOn = false;
		
		iTween.FadeTo(gameObject, 0.3f, 0.1f);	
	}

	void OnMouseEnter() {
		iTween.FadeTo(gameObject, 1.0f, 0.5f);
	}
	
	void OnMouseExit() {
		iTween.FadeTo(gameObject, 0.3f, 0.5f);
	}
	
	void OnMouseUp ()
	{
		credOn = true;
	}
}
