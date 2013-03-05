using UnityEngine;
using System.Collections;

public class NewGame : MonoBehaviour {
	private bool menuNew = false;
	
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
		menuNew = true;
	}
	
	void Update()
	{
		if (menuNew) 
		{
			iTween.MoveTo(Camera.main.gameObject,new Vector3(75,0,0), 4);	
			menuNew = false;
		}
	}
}
