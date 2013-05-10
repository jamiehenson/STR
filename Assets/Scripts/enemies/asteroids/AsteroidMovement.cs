using UnityEngine;
using System.Collections;

public class AsteroidMovement : MonoBehaviour {

    private float leftLimit;
    private float astBaseForce = 200f;
    private float forceOffset = 25f;
    public Universe Positions;
    string asteroidName;

	// Use this for initialization
	void Start ()
    {
        if (Network.isServer)
        {
            if (!gameObject.name.StartsWith("rock"))
            {
                networkView.RPC("modifyName", RPCMode.All, gameObject.name);
                Debug.Log("Problem" + gameObject.name);

                Positions = transform.parent.parent.FindChild("Managers/OriginManager").GetComponent<Universe>();
            }
            gameObject.rigidbody.AddForce(Vector3.left * (astBaseForce + Random.Range(-forceOffset, forceOffset)));
        }
	}

    [RPC]
    void modifyName(string name)
    {
        gameObject.name = name;
        int universeNb = int.Parse(name.Substring(name.Length-1, 1));
        gameObject.transform.parent = GameObject.Find("Universe"+universeNb+"/Enemies").transform;
        Positions = transform.parent.parent.FindChild("Managers/OriginManager").GetComponent<Universe>();
        gameObject.rigidbody.AddForce(Vector3.left * (astBaseForce + Random.Range(-forceOffset, forceOffset)));

    }
}
