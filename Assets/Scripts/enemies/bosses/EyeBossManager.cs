using UnityEngine;
using System.Collections;

public class EyeBossManager : BossManager
{
    public ParticleSystem chargingBeamPrefab, chargedBeamPrefab;
    public GameObject beamColliderPrefab;
    public Transform bulletPrefab;
    public bool inPlane = false;
    public bool irisOpen = false;
    private BossMovement bossMovement;
    private GameObject character;
    private GameObject[] characters;
    private GameObject[] cannons;
    private GameObject beamCollider;
    private GameObject beamSpawn;
    private ParticleSystem chargingBeam;
    private ParticleSystem chargedBeam;

    private float beamPower, cannonPower;

    new public void Start() {
        if (Network.isServer) {
            base.Start();
            bossMovement = GetComponent<BossMovement>();
            characters   = GameObject.FindGameObjectsWithTag("Player");
            beamSpawn    = GameObject.Find("BeamSpawn");
            cannons      = GameObject.FindGameObjectsWithTag("BossCannonSpawn");
            initStats();

            StartCoroutine(Shoot());
            StartCoroutine(Beam());
        }
    }

    void initStats() {
        if (Network.isServer) {
            gameObject.name     = "Boss0";
            health              = characters.Length*300;
            beamPower           = 0.01f;
            cannonPower         = 15f;
            killPoints          = 5000;
            speed               = 10.0f;
            firingDelay         = 1.4f;
            moveDelay           = 5f;
            forceMultiplier     = 5000;
            rotation            = 0.0f;
            typeForceMultiplier = 1f;
        }
    }

    void Update() {
        if (Network.isServer) {
            if (chargingBeam != null) {
                iTween.MoveUpdate(chargingBeam.gameObject, beamSpawn.transform.position, 0.1f);
            }
            if (chargedBeam != null) {
                iTween.MoveUpdate(chargedBeam.gameObject, beamSpawn.transform.position, 0.1f);
            }
            if (beamCollider != null) {
                iTween.MoveUpdate(beamCollider, beamSpawn.transform.position + chargedBeam.transform.forward * 50, 0.1f);
            }
        }
    }

    public override IEnumerator Shoot() {
        while (true) {
            if (inPlane) {
                for (int i = 0; i < characters.Length; i++) fireTurret();
                yield return new WaitForSeconds(firingDelay);
            }
            else yield return new WaitForSeconds(1f);
        }
    }

	[RPC]
	private void PlayBossFire() {
		if (Network.isClient)
		{
			GameObject.Find("Client Scripts").GetComponent<BGMusic>().PlayBossFire();
		}
	}

    IEnumerator Beam() {
        while (true) {
            if (inPlane) {
                // First open the iris shield
                networkView.RPC("irisShieldAnimation", RPCMode.All, 0.5f, 0.0f);
                yield return new WaitForSeconds(animation["Irishield"].length);
                irisOpen = true;
                int targetPlayer = PickTarget();
                if (targetPlayer != -1) {
                    character = GameObject.Find("Character" + targetPlayer);

                    // Then instantiate the two beam prefabs at the correct position, just in front of the cornea
                    chargingBeam = (ParticleSystem)Network.Instantiate(chargingBeamPrefab, beamSpawn.transform.position, Quaternion.identity, 100);
                    chargedBeam = (ParticleSystem)Network.Instantiate(chargedBeamPrefab, beamSpawn.transform.position, Quaternion.identity, 100);
                    networkView.RPC("modifyNames", RPCMode.All, chargingBeam.name, chargedBeam.name);

                    // Play the charging animation on all of the clients
                    networkView.RPC("chargingAnimation", RPCMode.All);

					// Play charging noise on all clients
					networkView.RPC("PlayBossFire", RPCMode.All);
                    
                    chargingBeam.Play();
                    //while (chargingBeam.isPlaying) {
                    //    yield return new WaitForSeconds(0.1f);
                    //}
                    yield return new WaitForSeconds(chargingBeam.duration);
                    Vector3 aimPos = character.transform.position;
                    iTween.LookTo(chargingBeam.gameObject, aimPos, 2.0f);
                    iTween.LookTo(gameObject, aimPos, 2.0f);
                    while (chargingBeam.isPlaying) {
                        yield return new WaitForSeconds(0.1f);
                    }
                    chargedBeam.transform.LookAt(aimPos, Vector3.forward);

                    networkView.RPC("chargedAnimation", RPCMode.All);
                    chargedBeam.Play();
                    beamCollider = (GameObject)Network.Instantiate(beamColliderPrefab, aimPos, Quaternion.identity, 100);
                    beamCollider.name = "BeamCollider";
                    EnemyBulletSettings weaponSettings = beamCollider.GetComponent<EnemyBulletSettings>();
                    weaponSettings.damage = beamPower;

                    networkView.RPC("chargedAnimation", RPCMode.All);
                    chargedBeam.Play();
                    while (chargedBeam.isPlaying) {
                        yield return new WaitForSeconds(0.1f);
                    }

                    Network.Destroy(chargingBeam.gameObject);
                    Network.Destroy(chargedBeam.gameObject);
                    Network.Destroy(beamCollider);
                    iTween.LookUpdate(gameObject, Vector3.left, 2);
                    //iTween.LookTo(gameObject, Vector3.left, 5);
                    networkView.RPC("irisShieldAnimation", RPCMode.All, -0.5f, animation["Irishield"].length);
                    yield return new WaitForSeconds(animation["Irishield"].length * 3f);
                    irisOpen = false;
                }
            }
            else yield return new WaitForSeconds(1f);
        }
    }

    void fireTurret() {
        int targetPlayer = PickTarget();
        if (targetPlayer != -1) {
            GameObject character = GameObject.Find("Character" + targetPlayer);
            // Fire from a random cannon each time
            int index = Random.Range(0, cannons.Length);
            GameObject cannon = cannons[index];
            Vector3 fireDirection = character.transform.position - cannon.transform.position;
            Vector3 force = fireDirection.normalized * forceMultiplier * typeForceMultiplier;
            fireDirection.y = Random.Range(fireDirection.y - 5f, fireDirection.y + 5f);
            Transform bullet = (Transform)Network.Instantiate(bulletPrefab, cannon.transform.position, cannon.transform.rotation, 200);
            NetworkViewID bulletID = bullet.networkView.viewID;

            // Play the turret animation on all of the clients
            //networkView.RPC("turretAnimation", RPCMode.All, index);

            NetworkViewID targetID = character.GetComponent<NetworkView>().viewID;
            networkView.RPC("fireBullet", RPCMode.All, cannon.transform.position, cannon.transform.rotation, targetID, bulletID, fireDirection, force);
        }
    }

    [RPC]
    void irisShieldAnimation(float speed, float time) {
        animation["Irishield"].speed = speed;
        animation["Irishield"].time = time;
        animation.Play("Irishield");
    }

    [RPC]
    void turretAnimation(int turretNumber) {
        animation.Play("Turret" + turretNumber);
    }

    [RPC]
    void chargingAnimation() {
        GameObject chargingBeam  = GameObject.Find("chargingBeam");
        ParticleSystem animation = chargingBeam.GetComponent<ParticleSystem>();
        animation.Play();
    }

    [RPC]
    void chargedAnimation() {
        GameObject chargedBeam   = GameObject.Find("chargedBeam");
        ParticleSystem animation = chargedBeam.GetComponent<ParticleSystem>();
        animation.Play();
    }

    [RPC]
    void modifyNames(string chargingBeamName, string chargedBeamName) {
        GameObject chargingBeam = GameObject.Find(chargingBeamName);
        GameObject chargedBeam  = GameObject.Find(chargedBeamName);
        chargingBeam.name = "chargingBeam";
        chargedBeam.name  = "chargedBeam";
    }

    [RPC]
    void fireBullet(Vector3 startPosition, Quaternion startRotation, NetworkViewID targetID, NetworkViewID bulletID, Vector3 fireDir, Vector3 force) {
        GameObject character = NetworkView.Find(targetID).gameObject;

        GameObject bullet = NetworkView.Find(bulletID).gameObject;
        EnemyBulletSettings ebs = bullet.GetComponent<EnemyBulletSettings>();

        bullet.transform.LookAt(character.transform, Vector3.forward);
        bullet.transform.Rotate(new Vector3(90, 0, 90));
        bullet.name = "EnemyBullet";
        Physics.IgnoreCollision(bullet.collider, gameObject.collider);
        bullet.rigidbody.AddForce(force);
        bullet.rigidbody.freezeRotation = true;
        ebs.damage = cannonPower;
    }

    private int universeN() {
        int length = transform.name.Length;
        string num = transform.name.Substring(length - 1, 1);
        if ("0123456789".Contains(num)) return (int.Parse(num));
        else return -1;
    }
}