using UnityEngine;
using System.Collections;

public class FiringHandler : MonoBehaviour {
	
    public bool rotated = false;
    public float fireDepth = 15;

    PlayerManager manager;
    LineRenderer beam;
    Transform arm, spawn, lSpawn, rSpawn;
    private Vector3 spawnPosition;

	private float timer = 0;
    private bool instantiated;
    private bool myCharacter;
    public string model = "";

    public void playerModel(string s)
    {
        model = s;
        arm = transform.Find(model + "/rightArm");
        if      (arm != null && model == "usa")    spawn = arm.Find("gunR/LeftCannonSpawn");
        else if (arm != null && model == "russia") spawn = arm.Find("right_hand_gun/RightCannonSpawn");
        else if (arm != null && model == "china") {
            lSpawn = arm.Find("pCylinder27/LeftCannonSpawn");
            rSpawn = arm.Find("pCylinder12/RightCannonSpawn");
        }
    }

    private int player;
    private int characterNum;

    public void activateCharacter(int num) {
        myCharacter = true;
    }

	void Start() {
        beam = GetComponent<LineRenderer>();
	}

    private int universeN() {
        int length = transform.name.Length;
        string num = transform.name.Substring(length - 1, 1);
        if ("0123456789".Contains(num)) return (int.Parse(num));
        else return -1;
    }

	[RPC]
	private void PlayNetworkShot(string wDir, int univ) {
		if (Network.isClient) {
			if (manager.universeNumber == univ) {
				GameObject.Find("Client Scripts").GetComponent<BGMusic>().PlayShot(wDir);
			}
		}
	}
	
	void Update () {
        if (universeN() != -1) {
            manager = GameObject.Find("Character" + universeN()).GetComponent<PlayerManager>();
            timer += Time.deltaTime;

            if (Network.isClient && myCharacter) {

                if (model == "china") spawnPosition = Random.value > 0.5 ? lSpawn.position : rSpawn.position;
                else spawnPosition = spawn.position;
                
                Camera cam = GameObject.Find("Camera " + universeN()).camera;
                Vector3 lookAt, dir;
                RaycastHit hit;
                Ray ray;
                if (rotated) {
                    ray = cam.ScreenPointToRay(Input.mousePosition);
                    ray.origin = spawnPosition;
                    if (Physics.Raycast(ray, out hit, 1000)) lookAt = hit.point;
                    else lookAt = ray.GetPoint(100);
                    dir = ray.direction;
                }
                else {
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = spawnPosition.z;
                    ray = new Ray(cam.ScreenToWorldPoint(mousePos), Vector3.right);
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                    if (Physics.Raycast(ray, out hit, 100)) lookAt = hit.point;
                    else lookAt = ray.origin;
                    dir = lookAt - spawnPosition;
                }

                beam.SetPosition(1, lookAt);
                beam.SetPosition(0, spawnPosition);

                if (Input.GetButton("Primary Fire") && timer >= manager.wepStats.wepRate && manager.getEnergyLevel() != 0) {
                    // Can I fire?
                    if (manager.getEnergyLevel() - manager.getSelectedWepDrain() >= 0) {
						int wType = manager.wepType;

						string wDir = "";

						switch(wType)
						{
							case 1: wDir = "beam/beam2";
								break;
							case 2: wDir = "cannon/cannon";
								break;
							case 3: wDir = "mine/mine";
								break;
							default: wDir = "beam/beam2";
								break;
						}
						networkView.RPC("PlayNetworkShot", RPCMode.All, wDir, manager.universeNumber);

                        // Send message to fire
                        if (dir.x > 0) networkView.RPC("fireWeapon", RPCMode.Server, lookAt, dir, spawnPosition, manager.wepStats.wepType);

                        timer = 0;

                        // Tell AchievementSystem
                        //AchievementSystem.playerFired();
                    }
                }
            }
        }
	}

    [RPC]
    void fireAnimation(int n) {
        //GameObject.Find("Character" + n).animation.Play("RightHandMove");
    }
	
	[RPC]
	void fireWeapon(Vector3 lookAt, Vector3 fireDirection, Vector3 spawnPosition, int bulletType)
	{
        manager.updateEnergyLevel(-manager.getSelectedWepDrain());

        Transform bullet = (Transform)Network.Instantiate(manager.wepStats.wepPrefab, spawnPosition, transform.rotation, 200);
        bullet.name = bullet.name + universeN();
        networkView.RPC("fireAnimation", RPCMode.All, universeN());
		Physics.IgnoreCollision(bullet.collider, transform.collider);
		
		// Tell everyone to set up its movement
		NetworkViewID id = bullet.networkView.viewID;
        Vector3 forceToApply = fireDirection.normalized * manager.wepStats.wepSpeed;
		networkView.RPC("setupWeapon", RPCMode.All, id, lookAt, forceToApply, bulletType);
	}

	[RPC]
	void setupWeapon(NetworkViewID id, Vector3 lookAt, Vector3 forceToApply, int bulletType) {
		NetworkView bulletNV = NetworkView.Find (id);
		if (bulletNV == null) {
			Log.Warning("During setupWeapon, unable to find player from their ID");
			return;
		}
		
		GameObject bullet = bulletNV.gameObject;
        bullet.transform.LookAt(lookAt); //, Vector3.forward
	    bullet.transform.Rotate(new Vector3(90, 0, 90));
		if (bulletType == 3)
	    	bullet.transform.Rotate(new Vector3(0, 0, 90));
		
		Physics.IgnoreCollision(gameObject.collider, bullet.collider);
		// Add ignores for all characters if we want frendly fire off
		
		// Set up movement
		bullet.rigidbody.AddForce(forceToApply);
	    bullet.rigidbody.freezeRotation = true;
	}
}
