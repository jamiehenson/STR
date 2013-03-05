using UnityEngine;
using System.Collections;

public class HowTo : MonoBehaviour {
	private bool menuHow = false;
	
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
		menuHow = true;
	}
	
	void Update()
	{
		if (menuHow) 
		{
			iTween.MoveTo(Camera.main.gameObject,new Vector3(0,-75,0), 4);	
			menuHow = false;
		}
	}
}
