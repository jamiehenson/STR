using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {
	public static bool goingOut;
	private Font deco;
    //private Texture2D bg;
	
	void Start () {
		goingOut = false;
		
		GameObject PopExit = GameObject.Find ("PopExit");
		GameObject Exit = GameObject.Find ("Exit");
		iTween.FadeTo(Exit, 0.3f, 0.1f);
		iTween.FadeTo(PopExit,0.5f,0.1f);
	}

	void OnMouseEnter() {
		GameObject Exit = GameObject.Find ("Exit");
		GameObject PopExit = GameObject.Find ("PopExit");
		iTween.FadeTo(Exit, 1.0f, 0.5f);
		iTween.FadeTo(PopExit,1.0f,0.5f);
		iTween.MoveTo(PopExit,new Vector3(PopExit.transform.position.x,PopExit.transform.position.y,28),0.5f);
	}
	
	void OnMouseExit() {
		GameObject Exit = GameObject.Find ("Exit");
		GameObject PopExit = GameObject.Find ("PopExit");
		iTween.FadeTo(Exit, 0.3f, 0.5f);
		iTween.FadeTo(PopExit,0.5f,0.5f);
		iTween.MoveTo(PopExit,new Vector3(PopExit.transform.position.x,PopExit.transform.position.y,30),0.5f);
	}
	
	void OnMouseUp () {
		goingOut = true;
	}
	
	void Update() {
		if (goingOut) {
            Application.Quit();
		}
	}
}
