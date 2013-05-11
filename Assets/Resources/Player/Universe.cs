using UnityEngine;
using System.Collections;

public class Universe : MonoBehaviour {
	
	public Vector3 origin;
    public float leftBorder;
    public float rightBorder;
    public float topBorder;
    public float bottomBorder;
    public float rightMovementLimit;
    public float width = 10.0f;
    public float height = 8.0f;
    public float baseZ = 15;
    public float generationOffset = 5.0f; 

	// Use this for initialization
	void Start () {
        leftBorder = origin.x - width;
        rightBorder = origin.x + width - 1;
        rightMovementLimit = origin.x - 5.0f;
        bottomBorder = origin.y - height;
        topBorder = origin.y + height;
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		stream.Serialize(ref origin);
	}
	
	public static Vector3 PositionOfOrigin(int universeNum) {
		GameObject universe = GameObject.Find("Universe" + universeNum + "/Managers/OriginManager");
		
		if (universe == null)
			Log.Error ("Could not find universe's origin", "In Universe.PositionOfCamera, " +
				"while looking for GameObject called `Universe" + universeNum + "/Managers/OriginManager`, " +
				"only found null :(.");
		
		return universe.GetComponent<Universe>().origin;
	}
}
