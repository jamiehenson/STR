using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    private bool myCharacter;
    private float vertDist;
    private float horDist;
	private bool rotation, rottoggle;
    private int universeNum = 1;
    public Universe positions;
    private int characterNum;
	public static bool charRotate;

    public void activateCharacter(int charNum, int univNum)
    {
        myCharacter = true;
        characterNum = charNum;
        universeNum = univNum;
        networkView.RPC("updateUniverse", RPCMode.Server, universeNum);
    }

	// Update is called once per frame
	void Update () {
      if (Network.isClient && myCharacter) {
            float vertDist = PlayerManager.speed * Input.GetAxis("Vertical") * Time.deltaTime;
            float horDist = PlayerManager.speed * Input.GetAxis("Horizontal") * Time.deltaTime;
			
			// If R is pressed, rotate the character, toggling 90 degrees
			if (Input.GetButtonDown("Rotate Character")) charRotate = true;
			else charRotate = false;
			
			if (charRotate) {
				rotation = true;
				rottoggle = !rottoggle;
			} else {
				rotation = false;
			}
            networkView.RPC("moveCharacter", RPCMode.Server, vertDist, horDist, rotation, rottoggle);

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
            gameObject.transform.Translate(horDist, vertDist, 0);
            gameObject.transform.position = new Vector3(Mathf.Clamp(transform.position.x, positions.leftBorder, positions.rightMovementLimit), Mathf.Clamp(transform.position.y, positions.bottomBorder, positions.topBorder), transform.position.z);
        }
		if (rotation)
		{
			if (rottoggle) gameObject.transform.LookAt(Vector3.up);
			else gameObject.transform.Rotate(0,-90,0);
		}	
      }
	}

    [RPC]
    public void moveCharacter(float vertdist, float hordist, bool rotate, bool rottog)
    {
        vertDist = vertdist;
        horDist = hordist;
		rotation = rotate;
		rottoggle = rottog;
    }

    [RPC]
    public void updateUniverse( int univNum)
    {
       // characterNum = charNum;
        universeNum = univNum;
    }
}
