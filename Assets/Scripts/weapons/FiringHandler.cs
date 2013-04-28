using UnityEngine;
using System.Collections;

public class FiringHandler : MonoBehaviour {
	
	//private Transform bulletPrefab;
    //private string bulletName;
	//private float bulletSpeed, bulletRate;
	//private int bulletType;
	private float timer = 0;
    private bool instantiated;
    private int player;
    private bool myCharacter;
    PlayerManager manager;

    public void activateCharacter(int num)
    {
        myCharacter = true;
        Debug.Log("Activate");
    }

    private int universeN()
    {
        int length = transform.name.Length;
        string num = transform.name.Substring(length - 1, 1);
        if ("0123456789".Contains(num)) return (int.Parse(num));
        else return -1;
    }

	
	void Update () {
		//bulletType = WeaponHandler.wepType;
		//bulletPrefab = WeaponHandler.wepPrefab;
		//bulletSpeed = WeaponHandler.wepSpeed;
		//bulletRate = WeaponHandler.wepRate;
        //bulletName = WeaponHandler.wepName;
        
        if (universeN() != -1)
        {
            manager = GameObject.Find("Character" + universeN()).GetComponent<PlayerManager>();
            timer += Time.deltaTime;

            // Is player firing?
            if (Network.isClient && myCharacter && Input.GetButton("Primary Fire") && timer >= manager.wepStats.wepRate && manager.getEnergyLevel() != 0)
            {
                // Can I fire?
                if (manager.getEnergyLevel() - manager.getSelectedWepDrain() >= 0)
                {
                    // Calculate the position to fire at
                    float camDist = (transform.position - Camera.main.transform.position).z;
                    camDist = 15;
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = camDist;
                    Vector3 fireDirection = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;

                    // Send message to fire
                    networkView.RPC("fireWeapon", RPCMode.Server, Camera.main.ScreenToWorldPoint(mousePos), fireDirection, manager.wepStats.wepType);
                    // Update fire stats
                    
                    timer = 0;

                }
            }
        }
	}

    [RPC]
    void fireAnimation(int n)
    {
      //  GameObject.Find("Character" + n).animation.Play("RightHandMove");
    }
	
	[RPC]
	void fireWeapon(Vector3 lookAt, Vector3 fireDirection, int bulletType)
	{
        manager.updateEnergyLevel(-manager.getSelectedWepDrain());
		// Update the WeaponHandler about the type (not the best way to do it)
		//weaponHandler.wepType = bulletType;
		//weaponHandler.Update();
		
		// Place Weapon
		Vector3 startPos = new Vector3(transform.position.x+3, transform.position.y, transform.position.z);
		//Transform bullet = (Transform)Network.Instantiate(weaponHandler.wepPrefab, startPos, transform.rotation,200);
        Transform arm = transform.Find("rightArm");
        float angle = arm.transform.rotation.z * 100;
        if (angle < 0) angle = 360 + angle;
        angle = Mathf.Floor(Mathf.Abs(360 - angle)) * Mathf.PI / 180f;
        float valX = Mathf.Cos(angle) + transform.position.x + 2.2f;
        if (angle > 4.71) valX = valX - 1 / Mathf.Cos(angle);
        float valY = Mathf.Sin(angle) * 3.2f +transform.position.y + 1.8f;
        Vector3 startP = new Vector3(Mathf.Abs(valX), valY, arm.transform.position.z);
        Debug.Log("Bullet position " + valX + ", " + valY + " Angle " + " , " + Mathf.Cos(angle));
		Transform bullet = (Transform)Network.Instantiate(manager.wepStats.wepPrefab, startP, transform.rotation,200);
        bullet.name = bullet.name + universeN();
        networkView.RPC("fireAnimation", RPCMode.All, universeN());
		Physics.IgnoreCollision(bullet.collider, transform.collider);
		
		// Tell everyone to set up its movement
		NetworkViewID id = bullet.networkView.viewID;
		Vector3 forceToApply = fireDirection.normalized * manager.wepStats.wepSpeed;
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
