using UnityEngine;
using System.Collections;

public class CameraRotate : MonoBehaviour {

    public bool behind = false;
	public bool cameraBehind = false;
    private int direction = 1;
	
	private bool allowGeneralRotation = true;
	
	/*
	//private bool allowCharRotation = true;
	//private Vector3 startingPos, startingRot;
	
	void Start() {
		startingPos = gameObject.transform.position;
		startingRot = new Vector3(0,90,0);
	}
	
	//// Below function is obsolete. ////
	
	public IEnumerator rotateCharacter () {
		if (allowCharRotation) {
			string activeChar = "Character" + OnlineClient.characterNum;
			GameObject character = GameObject.Find (activeChar);
			print (activeChar);
			character.transform.Rotate(new Vector3(0,90,0));
			allowGeneralRotation = false;
			direction = (behind) ? 1 : -1;
			if (behind)
        	{
				iTween.MoveTo(character, new Vector3(startingPos.x,character.transform.position.y,Positions.baseZ), 1);
				iTween.RotateTo(character, startingRot, 1);
			}
			else
			{
				iTween.MoveTo(character, new Vector3(startingPos.x + 8, character.transform.position.y, Positions.baseZ - 5), 1);
				iTween.RotateBy(character, new Vector3(0, direction * 0.25f, 0), 1);
			}

	        behind = !behind;
			yield return new WaitForSeconds(2);
			allowGeneralRotation = true;
		}
	}*/
	
	public IEnumerator rotateCamera() 
	{
		direction = (cameraBehind) ? 1 : -1;
        iTween.MoveBy(Camera.main.gameObject, new Vector3(direction * 30f, 0, direction * 6f), 2f);
        iTween.RotateBy(Camera.main.gameObject, new Vector3(0, direction * -0.25f, 0), 2f);
        
		/*
        if (cameraBehind)
        {
            iTween.MoveTo(gameObject, new Vector3(startingPos.x,gameObject.transform.position.y,Positions.baseZ), 0.5f);
			iTween.RotateTo(gameObject, startingRot, 0.5f);
			allowCharRotation = true;
        }
        else
        {
            iTween.MoveTo(gameObject, new Vector3(-15, startingPos.y, Positions.baseZ), 0.5f);
			iTween.RotateTo(gameObject, startingRot, 0.5f);
			allowCharRotation = false;
        }
        */
		yield return new WaitForSeconds(2);
        cameraBehind = !cameraBehind;
	}
	
	void Update () 
    {
		if (Input.GetKeyDown("t") && allowGeneralRotation) 
		{
			StartCoroutine("rotateCamera");
		}
	}
}
