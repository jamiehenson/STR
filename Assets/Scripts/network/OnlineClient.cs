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
        Vector3 camPos = new Vector3(origin.x - (float)4, origin.y, origin.z + 0.1f);
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

    public void moveUniverse(int universeNum, int character)
    {
        Log.Note("Move Universe");
        Vector3 origin = Universe.PositionOfCamera(universeNum);

        // Set camPos to bgPos + 1000 to z
        Vector3 camPos = new Vector3(origin.x - (float)4, origin.y, origin.z + 0.1f);
        Camera.GetComponent<Transform>().position = camPos;

        // Move Spaceship
        Debug.Log("Move Character" + character);
        GameObject.Find("Character" + character).GetComponent<Transform>().position = new Vector3(origin.x - 8, origin.y, origin.z + 15);
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
}