using UnityEngine;
using System.Collections;

public class Back : MonoBehaviour {
	private bool menuBack = false;
	
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
		menuBack = true;
	}
	
	void Update()
	{
		if (menuBack) 
		{
            GameObject browserbg = GameObject.Find("SBrowserBG");
            iTween.FadeTo(browserbg, 0, 1f);
			iTween.MoveTo(Camera.main.gameObject,new Vector3(0,0,0), 6);	
			menuBack = false;
            MP.joinScreen = false;
            MP.hostScreen = false;
			MP.openBox = false;
		}
	}
}
