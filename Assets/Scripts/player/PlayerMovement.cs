using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    // Object reference vars
    public Universe positions;
    public PlayerManager playerManager;
    public OnlineClient onlineClient;
    public Server server;
    private FiringHandler firingHandler;
    GameObject character;

    // Helper vars
    public bool myCharacter;
    public bool charRotate;
    public bool isRotating = false;

	private Vector3 camStartingPos;
    public static bool startGame= false;
    public string model = "";

    // Indicator for how far from the universe position the player needs to be when rotated
    private int baseRotDepth = -8;
    
    private bool rotation, rottoggle = true, camtoggle = false, rotexception;
    private int characterNum, universeNum = 1;
    private float vertDist, horDist;
    private Vector3 startingRot;


	public void Start()
	{
		if (Application.loadedLevelName == "OnlineClient")
			onlineClient = GameObject.Find("Client Scripts").GetComponent<OnlineClient>();
		else if (Application.loadedLevelName == "server")
			server = GameObject.Find("Network").GetComponent<Server>();

		playerManager = gameObject.GetComponent<PlayerManager>();
        firingHandler = GetComponent<FiringHandler>();
		camtoggle = false;
		rottoggle = true;
	}

    public void updateStartGame()
    {
        Debug.Log("Start moving");
        startGame = true;
    }

    public void SetCamBehind() {
        camtoggle = true;
        rottoggle = true;
        firingHandler.rotated = true;
        gameObject.networkView.RPC("FixCharDepth", RPCMode.Server);
    }

    public void SetCamSide() {
        camtoggle = false;
        rottoggle = true;
        firingHandler.rotated = false;
    }

    public void activateCharacter(int charNum, int univNum)
    {
        myCharacter = true;
        characterNum = charNum;
        universeNum = univNum;
        networkView.RPC("updateUniverse", RPCMode.Server, universeNum, characterNum);
    }
	
	
    public void PubRotateCam(int universe) {
        if (Network.isClient && myCharacter) {
            StartCoroutine(rotateCamera(camtoggle, universe));
            camtoggle = !camtoggle;
            networkView.RPC("moveCharacter", RPCMode.Server, 1 + vertDist, horDist, rotation, rottoggle, camtoggle);
        }
    }

    [RPC]
    private void FixCharDepth() {
        if (Network.isServer) {
            Vector3 uniPos = Universe.PositionOfOrigin(gameObject.GetComponent<PlayerManager>().universeNumber);
            gameObject.transform.position = new Vector3(uniPos.x + baseRotDepth, gameObject.transform.position.y, gameObject.transform.position.z);
        }
    }

	public IEnumerator rotateCamera(bool cameraBehind, int universe) 
	{
        isRotating = true;
		int direction = (cameraBehind) ? 1 : -1;
        Vector3 origin = Universe.PositionOfOrigin(universe);

        if (cameraBehind) {
            iTween.MoveTo(Camera.main.gameObject, new Vector3(origin.x, origin.y, origin.z + 0.1f), 2);
            firingHandler.rotated = false;
        }
        else {
            iTween.MoveTo(Camera.main.gameObject, new Vector3(origin.x - 20, origin.y, origin.z + 15), 2);
            firingHandler.rotated = true;
        }
        iTween.RotateBy(Camera.main.gameObject, new Vector3(0, direction * -0.25f, 0), 2);
        gameObject.networkView.RPC("FixCharDepth", RPCMode.Server);
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
    public void rotateArm(float angle, string m)
    {
        Transform arm = transform.Find(m +"/rightArm");
        if(m =="usa")
        {
            if (angle >= -45 && angle <= 50) arm.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
        }
        else
            if (angle >= -45 && angle <= 50) arm.transform.rotation = Quaternion.Euler(new Vector3(-angle, 90, 0));
    }

    public void playerModel(string s)
    {
        model = s;
    }

	// Update is called once per frame
	void Update () 
	{
	    if (Network.isClient && myCharacter)  
		{
	        float vertDist = PlayerManager.speed * Input.GetAxis("Vertical") * Time.deltaTime;
	        float horDist = PlayerManager.speed * Input.GetAxis("Horizontal") * Time.deltaTime;
            /* Rotate arm */
            Transform arm = transform.Find(model +"/rightArm");
            Vector3 mouse_pos = Input.mousePosition;
            mouse_pos.z = 15;
            Vector3 object_pos = Camera.main.WorldToScreenPoint(arm.transform.position);
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
            // Calculate rotation angle
            float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
            networkView.RPC("rotateArm", RPCMode.All, angle, model);
            /*End arm rotation*/
						
	        networkView.RPC("moveCharacter", RPCMode.Server, vertDist, horDist, rotation, rottoggle, camtoggle);
		}
        else if (Network.isServer) {
            if (vertDist != 0 || horDist != 0) {
                positions = GameObject.Find("Universe" + universeNum + "/Managers/OriginManager").GetComponent<Universe>();

                // Stop the lad from going out of bounds - INCOMPLETE
                // Standard orientation
                if (rottoggle && !camtoggle) {
                    gameObject.transform.Translate(horDist, vertDist, 0);
                    gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightMovementLimit), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), positions.baseZ);
                }

                // When rotated to face into the screen
                else if (!rottoggle && !camtoggle) {
                    gameObject.transform.Translate(0, vertDist, -horDist);
                    gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightBorder), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), transform.position.z);
                }

                else if (rottoggle && camtoggle) {
                    gameObject.transform.Translate(0, vertDist, -horDist);
                    gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightBorder), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), transform.position.z);
                }

                else {
                    gameObject.transform.Translate(0, vertDist, -horDist);
                    gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightBorder), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), transform.position.z);
                }
            }
            if (rotation) {
                StartCoroutine("rotateChar", rottoggle);
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
    [RPC]
    public void SetRotationScheme(bool rotated) {
        if (Network.isClient && myCharacter) {
            if (rotated) {
                camtoggle = true;
                rottoggle = true;
                firingHandler.rotated = true;
            }
            else {
                camtoggle = false;
                rottoggle = true;
                firingHandler.rotated = false;
            }
            networkView.RPC("moveCharacter", RPCMode.Server, 1 + vertDist, horDist, rotation, rottoggle, camtoggle);
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
        Vector3 characterPosition = transform.position;
        Vector3 diffFromOrigin = characterPosition - curOrigin;

        Vector3 newPosition = newOrigin + diffFromOrigin;
        transform.position = newPosition;

        bool inEnemies = false;

        if (newUniverseNum != 0) inEnemies = GameObject.Find("Universe" + newUniverseNum + "/Managers/EnemyManager").GetComponent<Commander>().inEnemies;

        server.moveCamera(newUniverseNum, info.sender, !inEnemies);

        // Work out and set desired rotation - automatically warps in side-on (enemy wave) view
        /* SET THESE ON THE CLIENT AS WELL */
        
        /* ******************************* */
        /*bool enWaveCurr = true;
        if (newUniverseNum != 0) enWaveCurr = GameObject.Find("Universe" + newUniverseNum + "/Managers/EnemyManager").GetComponent<Commander>().inEnemies;
        Debug.Log("CURR WAVE ENEMIES? " + enWaveCurr);
        if (!enWaveCurr) {
            Debug.Log("HITTING HERE");
            networkView.RPC("RotateCamera", RPCMode.Others, true, characterNum, newUniverseNum);
        }*/
       

        // Update positions var
        positions = GameObject.Find("Universe" + newUniverseNum + "/Managers/OriginManager").GetComponent<Universe>();
        universeNum = newUniverseNum;
        playerManager.universeNumber = newUniverseNum;

        networkView.RPC("SetCamVars", RPCMode.Others, inEnemies);
    }

    [RPC]
    private void SetCamVars(bool toSide) {
        if (Network.isClient && myCharacter) {
            if (toSide) {
                SetCamSide();
            }
            else {
                SetCamBehind();
            }
        }
    }

	[RPC]
	public void changeUniverseRPC(int newUniverseNum, NetworkMessageInfo info) {
        object[] pars = new object[2] { newUniverseNum, info };
        StartCoroutine("ChangeUniverseIE", pars);
	}
}
