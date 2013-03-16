using UnityEngine;
using System.Collections;

public class HowTo : MonoBehaviour {
	private bool menuHow = false;
	
	void Start () {
		GameObject PopHow = GameObject.Find ("PopHow");
		GameObject How = GameObject.Find ("How To Play button");
		iTween.FadeTo(How, 0.3f, 0.1f);	
		iTween.FadeTo(PopHow,0.5f,0.1f);
	}

	void OnMouseEnter() {
		GameObject PopHow = GameObject.Find ("PopHow");
		GameObject How = GameObject.Find ("How To Play button");
		iTween.FadeTo(How, 1.0f, 0.5f);
		iTween.FadeTo(PopHow,1.0f,0.5f);
		iTween.MoveTo(PopHow,new Vector3(PopHow.transform.position.x,PopHow.transform.position.y,28),0.5f);
	}
	
	void OnMouseExit() {
		GameObject PopHow = GameObject.Find ("PopHow");
		GameObject How = GameObject.Find ("How To Play button");
		iTween.FadeTo(How, 0.3f, 0.5f);
		iTween.FadeTo(PopHow,0.5f,0.5f);
		iTween.MoveTo(PopHow,new Vector3(PopHow.transform.position.x,PopHow.transform.position.y,30),0.5f);
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
