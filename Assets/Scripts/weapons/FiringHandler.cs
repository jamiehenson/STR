using UnityEngine;
using System.Collections;

public class FiringHandler : MonoBehaviour {
	
	//private Transform bulletPrefab;
    //private string bulletName;
	//private float bulletSpeed, bulletRate;
	//private int bulletType;
    public bool rotated = false;
    private Vector3 gunPosition;
    public float fireDepth = 15;

    PlayerManager manager;
	private WeaponHandler weaponHandler;
	private float timer = 0;
    private bool instantiated;
    private bool myCharacter;
    private int player;
    private int characterNum;

    public void activateCharacter(int num) {
        myCharacter = true;
        characterNum = num;
        Debug.Log("Activate");
    }

	void Start() {
        weaponHandler = gameObject.GetComponent<WeaponHandler>();
	}

    private int universeN() {
        int length = transform.name.Length;
        string num = transform.name.Substring(length - 1, 1);
        if ("0123456789".Contains(num)) return (int.Parse(num));
        else return -1;
    }
	
	void Update () {
        if (universeN() != -1) {
            manager = GameObject.Find("Character" + universeN()).GetComponent<PlayerManager>();
            timer += Time.deltaTime;

            gunPosition = new Vector3(transform.position.x + 3, transform.position.y + 2, transform.position.z);
            Vector3 mousePos = Input.mousePosition;
            if (rotated) mousePos.z = gunPosition.z + fireDepth;
            else mousePos.z = fireDepth;

            // Cast a ray from the cursor into the screen
            Vector3 lookAt; // = Camera.main.ScreenToWorldPoint(mousePos) - gunPosition;
            Ray ray = new Ray(Camera.main.ScreenToWorldPoint(mousePos), Vector3.right);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20)) {
                lookAt = hit.point;
            }
            else lookAt = ray.origin;

            LineRenderer beam = GetComponent<LineRenderer>();
            beam.SetPosition(0, gunPosition);
            beam.SetPosition(1, lookAt);
            Vector3 dir = lookAt - gunPosition;

            // Is player firing?
            if (Network.isClient && myCharacter && Input.GetButton("Primary Fire") && timer >= manager.wepStats.wepRate && manager.getEnergyLevel() != 0){
                // Can I fire?
                if (manager.getEnergyLevel() - manager.getSelectedWepDrain() >= 0) {
                    //Vector3 dir = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;

                    //if (Physics.Raycast(transform.position, dir, out hit, 1000.0f))
                    //    beam.SetPosition(1, hit.point);
                    //else beam.SetPosition(1, Camera.main.ScreenToWorldPoint(mousePos));
                    
                    //Vector3 fireDirection = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;

                    // Send message to fire
                    networkView.RPC("fireWeapon", RPCMode.Server, lookAt, dir, manager.wepStats.wepType);
                    // Update fire stats
                    
                    timer = 0;
                }
            }
        }
	}

    [RPC]
    void fireAnimation(int n) {
        GameObject.Find("Character" + n).animation.Play("RightHandMove");
    }
	
	[RPC]
	void fireWeapon(Vector3 lookAt, Vector3 fireDirection, int bulletType) {
        manager.updateEnergyLevel(-manager.getSelectedWepDrain());
		// Update the WeaponHandler about the type (not the best way to do it)
		//weaponHandler.wepType = bulletType;
		//weaponHandler.Update();
		
		//Transform bullet = (Transform)Network.Instantiate(weaponHandler.wepPrefab, startPos, transform.rotation,200);
       
		Transform bullet = (Transform)Network.Instantiate(manager.wepStats.wepPrefab, gunPosition, transform.rotation, 200);
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
