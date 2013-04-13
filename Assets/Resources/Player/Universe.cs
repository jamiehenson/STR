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
	
	void OnSerializeNetworkView( BitStream stream, NetworkMessageInfo info) {

			stream.Serialize(ref origin);
	}
	
	public static Vector3 PositionOfCamera(int universeNum) {
		return GameObject.Find("Universe" + universeNum + "/Managers/OriginManager").GetComponent<Universe>().origin;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
