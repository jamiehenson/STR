using UnityEngine;
using System.Collections;

public class SP : MonoBehaviour {
	private bool menuSP = false;
    public static bool singleplayerStart = false;
	
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
        menuSP = true;
	}
	
	void Update()
	{
        if (menuSP) 
		{
			iTween.MoveTo(Camera.main.gameObject,new Vector3(75,-75,0), 4);
            singleplayerStart = true;
            menuSP = false;
		}
	}
}
