using UnityEngine;
using System.Collections;

public class Universe : MonoBehaviour {
	
	public Vector3 origin;
    public float leftXBorder;
    public float rightXBorder;
    public float leftZBorder;
    public float rightZBorder;
    public float topBorder;
    public float bottomBorder;
    public float rightMovementLimit;
    public float bottomRotatedBorder;
    public float topRotatedBorder;
    public float width = 10.0f;
    public float height = 8.0f;
    public float baseZ = 15;
    public float generationOffset = 5.0f;

	// Use this for initialization
	void Start () {
        leftXBorder         = origin.x - 13.2f;
        rightXBorder        = origin.x + 14.9f;
        leftZBorder         = 25.0f;
        rightZBorder        = 5.0f;
        rightMovementLimit  = origin.x - 5.0f;
        bottomBorder        = origin.y - 7.0f;
        topBorder           = origin.y + 4.7f;
        bottomRotatedBorder = origin.y - 5.0f;
        topRotatedBorder    = origin.y + 3.1f;
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
