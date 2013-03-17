using UnityEngine;
using System.Collections;

public class FiringHandler : MonoBehaviour {
	
	//private Transform bulletPrefab;
    //private string bulletName;
	//private float bulletSpeed, bulletRate;
	//private int bulletType;
	private WeaponHandler weaponHandler;
	private float timer = 0;
    private bool instantiated;
    private int player;
    private bool myCharacter;
    private int characterNum;
	// Incorporate some way of setting a nice name for bullets?

    public void activateCharacter(int num)
    {
        myCharacter = true;
        characterNum = num;
        Debug.Log("Activate");
    }

	void Start()
    {
        weaponHandler = gameObject.GetComponent<WeaponHandler>(); 
	}
	
	void Update () {
		//bulletType = WeaponHandler.wepType;
		//bulletPrefab = WeaponHandler.wepPrefab;
		//bulletSpeed = WeaponHandler.wepSpeed;
		//bulletRate = WeaponHandler.wepRate;
        //bulletName = WeaponHandler.wepName;



        timer += Time.deltaTime;

		// Is player firing?
		if (Network.isClient && myCharacter && Input.GetButton("Fire1") && timer >= weaponHandler.wepRate && PlayerManager.energyLevel != 0)
		{   
            // Can I fire?
            Debug.Log("Rate " + weaponHandler.wepRate + " " + weaponHandler.wepName + " " + weaponHandler.wepPrefab);
			if (PlayerManager.energyLevel - PlayerManager.selectedWepDrain >= 0)
			{
				// Calculate the position to fire at
				float camDist = (transform.position - Camera.main.transform.position).z;
				Vector3 mousePos = Input.mousePosition;
	            mousePos.z = camDist;
				Vector3 fireDirection = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;
				
				// Send message to fire
                networkView.RPC("fireWeapon", RPCMode.Server, Camera.main.ScreenToWorldPoint(mousePos), fireDirection, weaponHandler.wepType);
				// Update fire stats
				timer = 0;
				PlayerManager.energyLevel -= PlayerManager.selectedWepDrain;
			}
		}
	}
	
	[RPC]
	void fireWeapon(Vector3 lookAt, Vector3 fireDirection, int bulletType)
	{
		// Update the WeaponHandler about the type (not the best way to do it)
		weaponHandler.wepType = bulletType;
		weaponHandler.Update();
		
		// Place Weapon
		Vector3 startPos = new Vector3(transform.position.x + 3, transform.position.y, transform.position.z);
		Transform bullet = (Transform)Network.Instantiate(weaponHandler.wepPrefab, startPos, transform.rotation,200);
		Physics.IgnoreCollision(bullet.collider, transform.collider);
		
		// Tell everyone to set up its movement
		NetworkViewID id = bullet.networkView.viewID;
		Vector3 forceToApply = fireDirection.normalized * weaponHandler.wepSpeed;
		networkView.RPC("setupWeapon", RPCMode.All, id, lookAt, forceToApply, bulletType);
	}
	
	[RPC]
	void setupWeapon(NetworkViewID id, Vector3 lookAt, Vector3 forceToApply, int bulletType)
	{
		NetworkView bulletNV = NetworkView.Find (id);
		if (bulletNV == null) {
			Log.Warning("During setupWeapon, unable to find player from their ID");
			return;
		}
		
		GameObject bullet = bulletNV.gameObject;
		bullet.transform.LookAt(lookAt, Vector3.forward);
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
