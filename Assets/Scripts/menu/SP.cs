using UnityEngine;
using System.Collections;

public class SP : MonoBehaviour {
	private bool menuSP = false;
	public static bool singleplayerStart;
	
	void Start () {
		GameObject PopSP = GameObject.Find ("PopSP");
		GameObject SP = GameObject.Find ("SP Button");
		iTween.FadeTo(SP, 0.3f, 0.1f);	
		iTween.FadeTo(PopSP,0.5f,0.1f);
		singleplayerStart = false;
	}

	void OnMouseEnter() {
		GameObject PopSP = GameObject.Find ("PopSP");
		GameObject SP = GameObject.Find ("SP Button");
		iTween.FadeTo(SP, 1.0f, 0.5f);
		iTween.FadeTo(PopSP,1.0f,0.5f);
		iTween.MoveTo(PopSP,new Vector3(PopSP.transform.position.x,PopSP.transform.position.y,28),0.5f);
	}
	
	void OnMouseExit() {
		GameObject PopSP = GameObject.Find ("PopSP");
		GameObject SP = GameObject.Find ("SP Button");
		iTween.FadeTo(SP, 0.3f, 0.5f);
		iTween.FadeTo(PopSP,0.5f,0.5f);
		iTween.MoveTo(PopSP,new Vector3(PopSP.transform.position.x,PopSP.transform.position.y,30),0.5f);
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
			menuSP = false;
		}
	}
}
