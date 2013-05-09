using UnityEngine;
using System.Collections;

public class FiringHandler : MonoBehaviour {
	
    public bool rotated = false;
    public float fireDepth = 15;

    PlayerManager manager;
    LineRenderer beam;
    Transform arm;
    private Vector3 gunPosition;
	private WeaponHandler weaponHandler;

	private float timer = 0;
    private bool instantiated;
    private bool myCharacter;
    public string model = "";

    public void playerModel(string s)
    {
        model = s;
    }


    private int player;
    private int characterNum;

    public void activateCharacter(int num) {
        myCharacter = true;
    }

	void Start() {
        weaponHandler = GetComponent<WeaponHandler>();
        beam = GetComponent<LineRenderer>();
        arm = transform.Find(model + "/rightArm");
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
            // Calculate position just in front of the player's arm
            float angle = arm.transform.rotation.z * 100;
            if (angle < 0) angle = 360 + angle;
            angle = Mathf.Floor(Mathf.Abs(360 - angle)) * Mathf.PI / 180f;
            float valX = Mathf.Cos(angle) + transform.position.x + 2.2f;
            if (angle > 4.71) valX = valX - 1 / Mathf.Cos(angle);
            float valY = Mathf.Sin(angle) * 3.2f + transform.position.y + 1.8f;
            gunPosition = new Vector3(Mathf.Abs(valX), valY, arm.transform.position.z);

            Vector3 mousePos = Input.mousePosition;
            if (rotated) mousePos.z = gunPosition.z;
            else mousePos.z = gunPosition.z;

            // Cast a ray from the cursor into the screen
            Vector3 lookAt; // = Camera.main.ScreenToWorldPoint(mousePos) - gunPosition;
            Ray ray = new Ray(Camera.main.ScreenToWorldPoint(mousePos), Vector3.right);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                lookAt = hit.point;
            }
            else lookAt = ray.origin;
            Vector3 dir = lookAt - gunPosition;

            
            if (Network.isClient && myCharacter) {
                beam.SetPosition(1, lookAt); 
                beam.SetPosition(0, gunPosition);


                if (Input.GetButton("Primary Fire") && timer >= manager.wepStats.wepRate && manager.getEnergyLevel() != 0) {
                    // Can I fire?
                    if (manager.getEnergyLevel() - manager.getSelectedWepDrain() >= 0) {
                        // Send message to fire
                        networkView.RPC("fireWeapon", RPCMode.Server, lookAt, dir, manager.wepStats.wepType, model);
                        // Update fire stats

                        timer = 0;

                        // Tell AchievementSystem
                        AchievementSystem.playerFired();
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
	void fireWeapon(Vector3 lookAt, Vector3 fireDirection, int bulletType, string m)
	{
        manager.updateEnergyLevel(-manager.getSelectedWepDrain());
		// Update the WeaponHandler about the type (not the best way to do it)
		//weaponHandler.wepType = bulletType;
		//weaponHandler.Update();
		
		// Place Weapon
		//Vector3 startPos = new Vector3(transform.position.x+3, transform.position.y, transform.position.z);
		//Transform bullet = (Transform)Network.Instantiate(weaponHandler.wepPrefab, startPos, transform.rotation,200);
/*
        Transform arm = transform.Find("rightArm");
        float angle = arm.transform.rotation.z * 100;
        if (angle < 0) angle = 360 + angle;
        angle = Mathf.Floor(Mathf.Abs(360 - angle)) * Mathf.PI / 180f;
        float valX = Mathf.Cos(angle) + transform.position.x + 2.2f;
        if (angle > 4.71) valX = valX - 1 / Mathf.Cos(angle);
        float valY = Mathf.Sin(angle) * 3.2f +transform.position.y + 1.8f;
        Vector3 startP = new Vector3(Mathf.Abs(valX), valY, arm.transform.position.z);
*/
        Transform arm = transform.Find(m+"/rightArm");
        Vector3 startP;
        if (m == "usa")
        {
            float angle = arm.transform.rotation.z * 100;
            if (angle < 0) angle = 360 + angle;
            angle = Mathf.Floor(Mathf.Abs(360 - angle)) * Mathf.PI / 180f;
            float valX = Mathf.Cos(angle) + transform.position.x + 2.2f;
            if (angle > 4.71) valX = valX - 1 / Mathf.Cos(angle);
            float valY = Mathf.Sin(angle) * 3.2f + transform.position.y + 1.8f;
            startP = new Vector3(Mathf.Abs(valX), valY, arm.transform.position.z);
        }
        else
        {
            float angle = arm.transform.rotation.z * 100;
            if (angle < 0) angle = 360 + angle;
            angle = Mathf.Floor(Mathf.Abs(360 - angle)) * Mathf.PI / 180f;
            float valX = Mathf.Cos(angle) + transform.position.x + 3.2f;
            if (angle > 4.91)
            {
                valX = valX - 1 / Mathf.Cos(angle);
                Debug.Log(angle);
            }
            else
            {
                valX = valX - 2 / Mathf.Cos(angle);
                Debug.Log(angle);
            }
            float valY = Mathf.Sin(angle) * -5.2f + transform.Find("russia").position.y + 2.5f;
            Debug.Log("bULLET pOSITION " + valX + "  " +valY);
            startP = new Vector3(Mathf.Abs(valX), valY, arm.transform.position.z);
 
        }
        //Vector3 startP = new Vector3(Mathf.Abs(valX), valY, 15);
        //Debug.Log("Bullet position " + valX + ", " + valY + " Angle " + " , " + Mathf.Cos(angle));
        Debug.Log("Fire");
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
