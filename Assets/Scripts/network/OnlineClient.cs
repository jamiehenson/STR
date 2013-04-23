using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnlineClient : MonoBehaviour
{
    public Transform characterPrefab;
    public Transform camPrefab;
    public int universeNum;
    private Transform playerOrigin;
    private Transform[] universe;
    private PlayerMovement playerMovement;
	private PlayerManager playerManager;
    private static PlayerMovement playerMovementMove;
    public bool joinedUniverse;
    private static Transform Camera;
    public static int characterNum;

    // Use this for initialization
    void Start()
    {
        // Connect to Server
        Network.Connect(MP.hostData[MP.hostnb]);

		// This lets setUnivese know that its its first call. There might be a better way to do this.
        characterNum = 99;
    }

    // Called in Bridge.cs
    public void setUniverse(int num)
    {
        Log.Note("Universe Sent");
        universeNum = num;

        // Get the origin of the set Universe
        Vector3 origin = GameObject.Find("Universe" + num + "/Managers/OriginManager").GetComponent<Universe>().origin;
        Debug.Log(num + "Universe " + origin);

        // Set camPos to bgPos + 1000 to z
        Vector3 camPos = new Vector3(origin.x, origin.y, origin.z + 0.1f);
        Camera = (Transform)Instantiate(camPrefab, camPos, new Quaternion(0, 0, 0, 0));
        Camera.name = "Camera " + num;
		
		// It is the first time this function has been called
        if (characterNum == 99)
        {
            characterNum = universeNum;
            Log.Note("Activate initial");
            playerMovement = GameObject.Find("Character" + num).GetComponent<PlayerMovement>();
            playerMovement.activateCharacter(num, num);
			playerManager = GameObject.Find("Character" + num).GetComponent<PlayerManager>();
            playerManager.activateCharacter(num);
            FiringHandler fireHandler = GameObject.Find("Character" + num).GetComponent<FiringHandler>();
            fireHandler.activateCharacter(num);
        }
		
		playerManager.universeNumber = num;
    }
	
	// When disconnected from server, go back to menu
    void OnDisconnectedFromServer()
    {
        Application.LoadLevel("menu");
    }
	
	// Update universe name
    public void updateUniverseNames(NetworkViewID ID, string name)
    {
        Log.Note("Client Update" + ID + name);
		
		// Look GameObject with that NetworkViewID and rename it
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Universe"))
        {
            NetworkViewID id = obj.networkView.viewID;
            if (ID == id) obj.name = name;
        }

    }
	
	// Update character name
    public void updateCharacterNames(NetworkViewID ID, string name)
    {
        Log.Note("Client Update" + ID + name);
		
		// Look GameObject with that NetworkViewID and rename it
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            NetworkViewID id = obj.networkView.viewID;
            if (ID == id) obj.name = name;

        }
    }

	public void moveCamera(int newUniverseNum){
		print ("In moveCameraRPC");
		Vector3 newOrigin = Universe.PositionOfOrigin(newUniverseNum);
        Vector3 camPos;
        Vector3 rotation;

		// MAKE SHIT GO DOWN
		GameObject transition = (GameObject) Resources.Load ("bg/trans");
		GameObject character = GameObject.Find("Character" + characterNum);
		Instantiate(transition,character.transform.position,Quaternion.Euler(new Vector3(0,90,0)));

		// Rotation for BossUniverse
        if (newUniverseNum == 0) {
            camPos = new Vector3(newOrigin.x - 20, newOrigin.y, newOrigin.z + 15);
            rotation = new Vector3(0, 90, 0);
        }
        // Rotation for all other universes
        else {
            camPos = new Vector3(newOrigin.x, newOrigin.y, newOrigin.z + 0.1f);
            rotation = new Vector3(0, 0, 0);
        }
        Camera.GetComponent<Transform>().position = camPos;
        Camera.GetComponent<Transform>().localEulerAngles = rotation;
		playerManager.universeNumber = newUniverseNum;
	}
}