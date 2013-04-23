using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    // Object reference vars
    public Universe positions;
    public PlayerManager playerManager;
    public OnlineClient onlineClient;
    public Server server;
    GameObject character;

    // Helper vars
    public bool myCharacter;
    public static bool charRotate;
    private bool rotation, rottoggle = true, camtoggle = false, rotexception;
    private int characterNum, universeNum = 1;
    private float vertDist, horDist;
    private Vector3 startingRot, startingPos, camStartingPos;

    // For the laser pointer
    Object material;

	public void Start()
	{
		if (Application.loadedLevelName == "OnlineClient")
			onlineClient = GameObject.Find("Network").GetComponent<OnlineClient>();
		else if (Application.loadedLevelName == "server")
			server = GameObject.Find("Network").GetComponent<Server>();

		playerManager = gameObject.GetComponent<PlayerManager>();
		
        startingPos = gameObject.transform.position;
		startingRot = new Vector3(0,0,0);	
		camtoggle = false;
		rottoggle = true;
	}

    public void SetCamForBoss() {
        camtoggle = true;
        rottoggle = true;
        gameObject.GetComponent<FiringHandler>().fireDepth = 30;
    }

    public void SetCamAfterBoss() {
        camtoggle = false;
        rottoggle = true;
        gameObject.GetComponent<FiringHandler>().fireDepth = 15;
    }

    public void activateCharacter(int charNum, int univNum)
    {
        myCharacter = true;
        characterNum = charNum;
        universeNum = univNum;
        networkView.RPC("updateUniverse", RPCMode.Server, universeNum, characterNum);
    }
	
	// Rotation procedures. Made into a coroutine, so that it is uninterruptable
	public IEnumerator rotateChar(bool toggle)
	{
		/*int direction = (toggle) ? 1 : -1;
		
		if (toggle)
    	{
			iTween.MoveTo(gameObject, new Vector3(startingPos.x, startingPos.y, positions.baseZ), 1);
			iTween.RotateTo(gameObject, startingRot, 1);
		}
		else
		{
			iTween.MoveTo(gameObject, new Vector3(startingPos.x + 4, startingPos.y - 2, positions.baseZ - 5), 1);
			iTween.RotateBy(gameObject, new Vector3(0, direction * 0.25f, 0), 1);
		}*/
		yield return new WaitForSeconds(1f);	
	}
	
    public void PubRotateCam(int universe) {
        if (Network.isClient && myCharacter) {
            StartCoroutine(rotateCamera(camtoggle, universe));
            //gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightMovementLimit), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), positions.baseZ);
            //rotexception = true;
            camtoggle = !camtoggle;
            networkView.RPC("moveCharacter", RPCMode.Server, 1 + vertDist, horDist, rotation, rottoggle, camtoggle);
        }
    }

	public IEnumerator rotateCamera(bool cameraBehind, int universe) 
	{
        /*rotexception = true;
        camtoggle = !camtoggle;*/
		int direction = (cameraBehind) ? 1 : -1;
        Vector3 origin = Universe.PositionOfOrigin(universe);
        //iTween.MoveBy(Camera.main.gameObject, new Vector3(direction * 20, 0, direction * 4), 2);
        //iTween.MoveBy(Camera.main.gameObject, new Vector3(direction * 20, 0, direction * -15), 2);
        if (cameraBehind) {
            iTween.MoveTo(Camera.main.gameObject, new Vector3(origin.x, origin.y, origin.z + 0.1f), 2);
            gameObject.GetComponent<FiringHandler>().fireDepth = 15;
        }
        else {
            iTween.MoveTo(Camera.main.gameObject, new Vector3(origin.x - 20, origin.y, origin.z + 15), 2);
            gameObject.GetComponent<FiringHandler>().fireDepth = 30;
        }
        iTween.RotateBy(Camera.main.gameObject, new Vector3(0, direction * -0.25f, 0), 2);
        //iTween.MoveTo(gameObject, new Vector3(startingPos.x, startingPos.y, positions.baseZ), 1);
        //iTween.MoveBy(gameObject, new Vector3(-10, 0, 0), 1f);
		yield return new WaitForSeconds(2);
	}

	// Used for universe change
	public void jumpCharacter(Vector3 position) {
		networkView.RPC("jumpCharacterRPC", RPCMode.Server, position);
	}

	[RPC]
	public void jumpCharacterRPC(Vector3 position) {
		gameObject.transform.position = position;
	}

	// Update is called once per frame
	void Update () 
	{
	    if (Network.isClient && myCharacter)  
		{
	        float vertDist = PlayerManager.speed * Input.GetAxis("Vertical") * Time.deltaTime;
	        float horDist = PlayerManager.speed * Input.GetAxis("Horizontal") * Time.deltaTime;
			
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 15;
            LineRenderer beam = GetComponent<LineRenderer>();
            beam.SetPosition(0, gameObject.transform.position);
            beam.SetPosition(1, Camera.main.ScreenToWorldPoint(mousePos));
            
			// If R is pressed, rotate the character, toggling 90 degrees
			//if (Input.GetButtonDown("Rotate Character")) charRotate = true;
			
			/*if (Input.GetKeyDown("t")) 
			{
				StartCoroutine("rotateCamera",camtoggle);
				rotexception = true;
				camtoggle = !camtoggle;
                PubRotateCam(universeNum);
			}*/
			
			/*if (rotexception) 
			{
				if (camtoggle == true) 
				{
					rotation = true; 
					rottoggle = true;
				}
				else if (camtoggle == false) rottoggle = true;
				rotexception = false;
			}
			else if (charRotate && camtoggle == false) 
			{
				rotation = true;
				rottoggle = !rottoggle;
			} 
			else rotation = false;
			
			charRotate = false;*/
						
	        networkView.RPC("moveCharacter", RPCMode.Server, vertDist, horDist, rotation, rottoggle, camtoggle);

			/*
	        // Warp between universes
	        string x = Input.inputString;
	        if (x.Equals("4") || x.Equals("5") || x.Equals("6") || x.Equals("7"))
	        {
	            int num = int.Parse(x);
	            OnlineClient.moveUniverse(num-3 , characterNum);
	            networkView.RPC("updateUniverse", RPCMode.Server, num-3, characterNum);
	        }*/
		}
      else if (Network.isServer)
      {  
        if (vertDist != 0 || horDist != 0)
        {
            positions = GameObject.Find("Universe"+universeNum+"/Managers/OriginManager").GetComponent<Universe>();
            
			// Stop the lad from going out of bounds - INCOMPLETE
			// Standard orientation
			if (rottoggle && !camtoggle) 
			{
				gameObject.transform.Translate(horDist, vertDist, 0);
				gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightMovementLimit), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), positions.baseZ);
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
    public void updateUniverse(int univNum, int character)
    {
        if (universeNum != 0 && GameObject.Find("Universe" + universeNum + "/Managers/EnemyManager").GetComponent<Commander>().enabled == true)
        {
            GameObject.Find("Universe" + universeNum + "/Managers/EnemyManager").GetComponent<Commander>().updateActiveChar(character, false);
            universeNum = univNum;
            GameObject.Find("Universe" + universeNum + "/Managers/EnemyManager").GetComponent<Commander>().updateActiveChar(character, true);
        }
    }

    [RPC]
    public void RotateCamera(bool toBehind, int chNum, int uNum) {
        if (toBehind != camtoggle && characterNum == chNum) {
            PubRotateCam(uNum);
        }
    }

	public void changeUniverse(int universeNum) {
		networkView.RPC("changeUniverseRPC", RPCMode.Server, universeNum);
	}

	[RPC]
	public void changeUniverseRPC(int newUniverseNum, NetworkMessageInfo info) {

		Log.Note("Move Universe");
        Vector3 curOrigin = Universe.PositionOfOrigin(playerManager.universeNumber);
		Vector3 newOrigin = Universe.PositionOfOrigin(newUniverseNum);

        // Move Spaceship
        Debug.Log("Move Character" + characterNum);
		Vector3 characterPosition = transform.position;
		Vector3 diffFromOrigin =  characterPosition - curOrigin;

		Vector3 newPosition = newOrigin + diffFromOrigin;
		//character.transform.position = newPosition;
		transform.position = newPosition;

		server.moveCamera(newUniverseNum, info.sender);
		// Update positions var
		positions = GameObject.Find("Universe" + newUniverseNum + "/Managers/OriginManager").GetComponent<Universe>();
		universeNum = newUniverseNum;
		playerManager.universeNumber = newUniverseNum;
	}
}
