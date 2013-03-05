using UnityEngine;
using System.Collections;

public class Refresh : MonoBehaviour {
	
	private float moveConst = 0.1f;

	void Start () {
		iTween.FadeTo(gameObject, 0.6f, 0.1f);	
	}

	void OnMouseEnter() {
		iTween.FadeTo(gameObject, 1.0f, 0.3f);
	}
	
	void OnMouseExit() {
		iTween.FadeTo(gameObject, 0.6f, 0.3f);
	}
	
	void OnMouseUp () {
		iTween.MoveBy(gameObject,new Vector3(moveConst,moveConst,0),0.2f);
		MP.refresh = true;
	}

	void OnMouseDown () {
		iTween.MoveBy(gameObject,new Vector3(-moveConst,-moveConst,0),0.2f);
	}
}
