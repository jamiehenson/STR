using UnityEngine;
using System.Collections;

public class EditHostDetails : MonoBehaviour
{
	void Start ()
	{
		iTween.FadeTo(gameObject, 0.6f, 0.1f);
	}
	void OnMouseEnter()
	{
		iTween.FadeTo(gameObject, 1.0f, 0.5f);
	}
	void OnMouseExit()
	{
		iTween.FadeTo(gameObject, 0.6f, 0.5f);
	}
	void OnMouseUp()
    {
		MP.openBox = true;
    }
}
