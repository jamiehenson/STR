using UnityEngine;
using System.Collections;

public class NewTitle : MonoBehaviour 
{
	void OnMouseUp () 
	{
		GameObject nameTitle = GameObject.Find("STRName");
        nameTitle.GetComponent<TextMesh>().text = Names.FetchSTRName();
	}
}
