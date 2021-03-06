using UnityEngine;
using System.Collections;

public class PilotName : MonoBehaviour {
	public static bool showBox, showBox2, showLimitBox;
	
	void Start () {
		showBox = false;
		showBox2 = false;
		showLimitBox = false;
		iTween.FadeTo(gameObject, 0.6f, 0.1f);	
	}

	void OnMouseEnter() {
		iTween.FadeTo(gameObject, 1.0f, 0.5f);
	}
	
	void OnMouseExit() {
		iTween.FadeTo(gameObject, 0.6f, 0.5f);
	}
}
