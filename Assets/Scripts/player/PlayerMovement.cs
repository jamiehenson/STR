using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    public bool myCharacter;
    private float vertDist;
    private float horDist;
	private bool rotation, rottoggle=true, camtoggle=false, rotexception;
    private Vector3 startingPos;
    private int universeNum = 1;
    public Universe positions;
    private int characterNum;
	public bool charRotate;
    public bool isRotating = false;
	private Vector3 camStartingPos;
	public PlayerManager playerManager;
	public OnlineClient onlineClient;
	public Server server;
    public static bool startGame= false;
	
	public void Start()
	{
		if (Application.loadedLevelName == "OnlineClient")
			onlineClient = GameObject.Find("Client Scripts").GetComponent<OnlineClient>();
		else if (Application.loadedLevelName == "server")
			server = GameObject.Find("Network").GetComponent<Server>();

		playerManager = gameObject.GetComponent<PlayerManager>();
        startingPos = gameObject.transform.position;
		camtoggle = false;
		rottoggle = true;
	}

    public void updateStartGame()
    {
        Debug.Log("Start moving");
        startGame = true;
    }
    public void SetCamForBoss() {
        camtoggle = true;
        rottoggle = true;
    }

    public void SetCamAfterBoss() {
        camtoggle = false;
        rottoggle = true;
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
        isRotating = true;
		int direction = (cameraBehind) ? 1 : -1;
        Vector3 origin = Universe.PositionOfOrigin(universe);
        if (cameraBehind) iTween.MoveTo(Camera.main.gameObject, new Vector3(origin.x, origin.y, origin.z + 0.1f), 2);
        else iTween.MoveTo(Camera.main.gameObject, new Vector3(origin.x - 20, origin.y, origin.z + 15), 2);
        gameObject.transform.position = new Vector3(origin.x, gameObject.transform.position.y, gameObject.transform.position.z);
        iTween.RotateBy(Camera.main.gameObject, new Vector3(0, direction * -0.25f, 0), 2);
        iTween.MoveTo(gameObject, new Vector3(startingPos.x, gameObject.transform.position.y, gameObject.transform.position.z), 1);
		yield return new WaitForSeconds(2);
        isRotating = false;
	}

	// Used for universe change
	public void jumpCharacter(Vector3 position) {
		networkView.RPC("jumpCharacterRPC", RPCMode.Server, position);
	}

	[RPC]
	public void jumpCharacterRPC(Vector3 position) {
		gameObject.transform.position = position;
	}

    [RPC]
    public void rotateArm(float angle)
    {
        Transform arm = transform.Find("rightArm");
        if (angle >= -45 && angle <= 50) arm.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
    }

	// Update is called once per frame
	void Update () 
	{
	    if (Network.isClient && myCharacter)  
		{
	        float vertDist = PlayerManager.speed * Input.GetAxis("Vertical") * Time.deltaTime;
	        float horDist = PlayerManager.speed * Input.GetAxis("Horizontal") * Time.deltaTime;

            /* Rotate arm */
            Transform arm = transform.Find("rightArm");
            Vector3 mouse_pos = Input.mousePosition;
            mouse_pos.z = 15;
            Vector3 object_pos = Camera.main.WorldToScreenPoint(arm.transform.position);
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
            // Calculate rotation angle
            float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
            networkView.RPC("rotateArm", RPCMode.All, angle);
            /*End arm rotation*/

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
          //Debug.Log("Start Game Update" + startGame);
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
        if (startGame)
        {
            vertDist = vertdist;
            horDist = hordist;
            rotation = rotate;
            rottoggle = rottog;
            camtoggle = cam;
        }
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

    [RPC]
    public void AnimateWarp(int chNum) {
        if (characterNum == chNum) {
            GameObject warpAni = (GameObject) Resources.Load("bg/trans");
            if (camtoggle) Instantiate(warpAni, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 180, 0)));
            else Instantiate(warpAni, gameObject.transform.position, Quaternion.Euler(new Vector3(0, 90, 0)));
        }
    }

	public void changeUniverse(int universeNum) {
        Debug.Log("Change universe");
		networkView.RPC("changeUniverseRPC", RPCMode.Server, universeNum);
	}

    IEnumerator ChangeUniverseIE(object[] pars) {
        while (isRotating) yield return new WaitForSeconds(0.5f);
        UniverseMove(pars);
    }

    void UniverseMove(object[] pars) {
        int newUniverseNum = (int) pars[0];
        NetworkMessageInfo info = (NetworkMessageInfo) pars[1];
        Log.Note("Move Universe");
        Vector3 curOrigin = Universe.PositionOfOrigin(playerManager.universeNumber);
        Vector3 newOrigin = Universe.PositionOfOrigin(newUniverseNum);

        // Move Spaceship
        Debug.Log("Move Character" + characterNum);
        Vector3 characterPosition = transform.position;
        Vector3 diffFromOrigin = characterPosition - curOrigin;

        Vector3 newPosition = newOrigin + diffFromOrigin;
        //character.transform.position = newPosition;
        transform.position = newPosition;

        server.moveCamera(newUniverseNum, info.sender);
        // Update positions var
        positions = GameObject.Find("Universe" + newUniverseNum + "/Managers/OriginManager").GetComponent<Universe>();
        universeNum = newUniverseNum;
        playerManager.universeNumber = newUniverseNum;
    }

	[RPC]
	public void changeUniverseRPC(int newUniverseNum, NetworkMessageInfo info) {
        object[] pars = new object[2] { newUniverseNum, info };
        StartCoroutine("ChangeUniverseIE", pars);
	}
}
