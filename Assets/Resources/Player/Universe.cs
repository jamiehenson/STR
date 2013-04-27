using UnityEngine;
using System.Collections;

public class Universe : MonoBehaviour {
	
	public Vector3 origin;
    public float leftBorder;
    public float rightBorder;
    public float topBorder;
    public float bottomBorder;
    public float rightMovementLimit;
    public float width = (float)10;
    public float height = (float)8.0;
    public float baseZ = 15;
    public float generationOffset = 5;
    

	// Use this for initialization
	void Start () {

        leftBorder = origin.x-width;
        rightBorder = origin.x + width - 1;
        rightMovementLimit = origin.x - (float)5;
        bottomBorder = origin.y - height;
        topBorder = origin.y + height;
	}

   /*public void activateUniverse(bool act)
    {
        if(Network.isClient) active = act;
        networkView.RPC("activateUniverse", RPCMode.Server, act);
    }

    [RPC]
    void activateUniverse(bool act)
    {
        if (Network.isServer) active = act;
    }*/
	
	[RPC]
	public void SetOrigin(Vector3 o) {
		if (Network.isServer)
			networkView.RPC ("SetOrigin", RPCMode.OthersBuffered, o);

		origin = o;
		Start ();
	}
	
	public static Vector3 PositionOfOrigin(int universeNum) {
		print ("numNum = "+universeNum);
		GameObject universe = GameObject.Find("Universe" + universeNum + "/Managers/OriginManager");
		
		if (universe == null)
			Log.Error ("Could not find universe's origin", "In Universe.PositionOfCamera, " +
				"while looking for GameObject called `Universe" + universeNum + "/Managers/OriginManager`, " +
				"only found null :(.");
		
		return universe.GetComponent<Universe>().origin;
	}
}
