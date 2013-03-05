using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {
	private bool menuSettings = false;
	
	void Start () {
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
		menuSettings = true;
	}
	
	void Update()
	{
		if (menuSettings) 
		{
			iTween.MoveTo(Camera.main.gameObject,new Vector3(75,-75,0), 4);	
			menuSettings = false;
		}
	}
}
