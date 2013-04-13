using UnityEngine;
using System.Collections;

public class NewTitle : MonoBehaviour 
{
	void OnMouseUp () 
	{
		print ("GAY");
		GameObject nameTitle = GameObject.Find("STRName");
        nameTitle.GetComponent<TextMesh>().text = Names.FetchSTRName();
	}
}
