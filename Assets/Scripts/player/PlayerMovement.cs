using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    private bool myCharacter;
    private float vertDist;
    private float horDist;
	private bool rotation, rottoggle = true, camtoggle;
    private int universeNum = 1;
    public Universe positions;
    private int characterNum;
	public static bool charRotate;
	private Vector3 startingRot, startingPos;
	
	void Start ()
	{
		startingPos = gameObject.transform.position;
		startingRot = new Vector3(0,0,0);	
	}

    public void activateCharacter(int charNum, int univNum)
    {
        myCharacter = true;
        characterNum = charNum;
        universeNum = univNum;
        networkView.RPC("updateUniverse", RPCMode.Server, universeNum);
    }
	
	// Rotation procedures. Made into a coroutine, so that it is uninterruptable
	public IEnumerator rotateChar(bool toggle)
	{
		int direction = (toggle) ? 1 : -1;
		
		if (toggle || camtoggle)
    	{
			iTween.MoveTo(gameObject, new Vector3(startingPos.x,gameObject.transform.position.y,Positions.baseZ), 1);
			iTween.RotateTo(gameObject, startingRot, 1);
		}
		else
		{
			iTween.MoveTo(gameObject, new Vector3(startingPos.x + 4, gameObject.transform.position.y, Positions.baseZ - 5), 1);
			iTween.RotateBy(gameObject, new Vector3(0, direction * 0.25f, 0), 1);
		}

        toggle = !toggle;
		yield return new WaitForSeconds(1.5f);	
	}
	
	public IEnumerator rotateCamera(bool cameraBehind) 
	{
		int direction = (cameraBehind) ? 1 : -1;
        iTween.MoveBy(Camera.main.gameObject, new Vector3(direction * 30f, 0, direction * 6f), 2f);
        iTween.RotateBy(Camera.main.gameObject, new Vector3(0, direction * -0.25f, 0), 2f);
		iTween.MoveBy(gameObject, new Vector3(gameObject.transform.position.x-10, gameObject.transform.position.y, gameObject.transform.position.z), 1f);
		
		if (cameraBehind) StartCoroutine("rotateChar",true);
		
        cameraBehind = !cameraBehind;
		yield return new WaitForSeconds(2);
	}

	// Update is called once per frame
	void Update () {
      if (Network.isClient && myCharacter) {
            float vertDist = PlayerManager.speed * Input.GetAxis("Vertical") * Time.deltaTime;
            float horDist = PlayerManager.speed * Input.GetAxis("Horizontal") * Time.deltaTime;
			
			// If R is pressed, rotate the character, toggling 90 degrees
			if (Input.GetButtonDown("Rotate Character")) charRotate = true;
			else charRotate = false;
			
			if (Input.GetKeyDown("t")) 
			{
				StartCoroutine("rotateCamera",camtoggle);
				camtoggle = !camtoggle;
			}
			
			if (charRotate) {
				rotation = true;
				rottoggle = !rottoggle;
			} else {
				rotation = false;
			}
						
            networkView.RPC("moveCharacter", RPCMode.Server, vertDist, horDist, rotation, rottoggle, camtoggle);

            // Warp between universes
            string x = Input.inputString;
            if (x.Equals("4") || x.Equals("5") || x.Equals("6") || x.Equals("7"))
            {
                int num = int.Parse(x);
                OnlineClient.moveUniverse(num , characterNum);
                networkView.RPC("updateUniverse", RPCMode.Server, num-3);
            }

        }
      else if (Network.isServer)
      {  
        if (vertDist != 0 || horDist != 0)
        {
            positions = GameObject.Find("Universe"+universeNum+"/Managers/OriginManager").GetComponent<Universe>();
            
			// Standard orientation
			if (rottoggle && !camtoggle) 
			{
				gameObject.transform.Translate(horDist, vertDist, 0);
				gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightMovementLimit), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), transform.position.z);
			}
				
			// When rotated to face into the screen
			else if (!rottoggle && !camtoggle)
			{
				gameObject.transform.Translate(0,vertDist,-horDist);
				gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightBorder), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), transform.position.z);
			}
				
			else if (rottoggle && camtoggle)
			{
				gameObject.transform.Translate(0,vertDist,-horDist);
				gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightBorder), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), transform.position.z);
			}			
			
			else
			{
				gameObject.transform.Translate(0,vertDist,-horDist);
				gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightBorder), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), transform.position.z);
			}	
					
			// Stop the lad from going out of bounds	
            
        }
		if (rotation)
		{
			StartCoroutine("rotateChar",rottoggle);
		}	
      }
		
	}

    [RPC]
    public void moveCharacter(float vertdist, float hordist, bool rotate, bool rottog, bool cam)
    {
        vertDist = vertdist;
        horDist = hordist;
		rotation = rotate;
		rottoggle = rottog;
		camtoggle = cam;
    }

    [RPC]
    public void updateUniverse(int univNum)
    {
       // characterNum = charNum;
        universeNum = univNum;
    }
}
