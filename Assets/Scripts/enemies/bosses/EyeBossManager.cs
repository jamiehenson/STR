using UnityEngine;
using System.Collections;

public class EyeBossManager : BossManager
{
    public ParticleSystem chargingBeamPrefab, chargedBeamPrefab;
    public GameObject beamColliderPrefab;
    public Transform bulletPrefab;
    public bool inPlane = false;
    private BossMovement bossMovement;
    private GameObject character;
    private GameObject[] characters;
    private GameObject[] cannons;
    private Vector3 beamPos;
    private Bounds bounds;
    private GameObject cornea;
    private GameObject iris;

    private float beamPower, cannonPower;

    new public void Start() {
        if (Network.isServer) {
            base.Start();
            characters = GameObject.FindGameObjectsWithTag("Player");
            cornea     = GameObject.Find("Cornea");
            bounds     = cornea.GetComponent<MeshRenderer>().renderer.bounds;
            cannons    = GameObject.FindGameObjectsWithTag("BossCannon");
            initStats();

            StartCoroutine(Shoot());
            StartCoroutine(Beam());
        }
    }

    void initStats() {
        if (Network.isServer) {
            gameObject.name     = "Boss0";
            health              = characters.Length*200;
            beamPower           = 0.01f;
            cannonPower         = 10f;
            killPoints          = 10000;
            speed               = 0.3f;
            firingDelay         = 0.4f;
            moveDelay           = 5f;
            forceMultiplier     = 500;
            rotation            = 0.0f;
            typeForceMultiplier = 1f;
        }
    }

    public override IEnumerator Shoot() {
        while (true) {
            if (inPlane) {
                int targetPlayer = PickTarget();
                if (targetPlayer != -1) {
                    GameObject character  = GameObject.Find("Character" + targetPlayer);
                    // Fire from a random cannon each time
                    int index = Random.Range(0, cannons.Length);
                    GameObject cannon     = cannons[index];
                    Vector3 fireDirection = character.transform.position - cannon.transform.position;
                    Vector3 force         = fireDirection.normalized * forceMultiplier * typeForceMultiplier;
                    fireDirection.y       = Random.Range(fireDirection.y - 2.5f, fireDirection.y + 2.5f);
                    Debug.Log(cannon.transform.position);
                    Debug.Log(cannon.transform.localPosition);
                    Transform bullet = (Transform)Network.Instantiate(bulletPrefab, cannon.transform.position, cannon.transform.rotation, 200);
                    NetworkViewID bulletID = bullet.networkView.viewID;

                    // Play the turret animation on all of the clients
                    //networkView.RPC("turretAnimation", RPCMode.All, index);

                    NetworkViewID targetID = character.GetComponent<NetworkView>().viewID;
                    networkView.RPC("fireBullet", RPCMode.All, cannon.transform.position, cannon.transform.rotation, targetID, bulletID, fireDirection, force);
                }
                yield return new WaitForSeconds(firingDelay);
            }
            else yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Beam() {
        while (true) {
            if (inPlane) {
                // First open the iris shield
                networkView.RPC("irisShieldAnimation", RPCMode.All, 0.5f, 0.0f);
                yield return new WaitForSeconds(animation["Irishield"].length);
                int targetPlayer = PickTarget();
                if (targetPlayer != -1) {
                    character = GameObject.Find("Character" + targetPlayer);

                    // Then instantiate the two beam prefabs at the correct position, just in front of the cornea
                    beamPos = new Vector3(cornea.transform.position.x - bounds.size.x - 6, cornea.transform.position.y, cornea.transform.position.z);
                    ParticleSystem chargingBeam = (ParticleSystem)Network.Instantiate(chargingBeamPrefab, beamPos, Quaternion.identity, 100);
                    ParticleSystem chargedBeam = (ParticleSystem)Network.Instantiate(chargedBeamPrefab, beamPos, Quaternion.identity, 100);
                    networkView.RPC("modifyNames", RPCMode.All, chargingBeam.name, chargedBeam.name);

                    // Play the charging animation on all of the clients
                    networkView.RPC("chargingAnimation", RPCMode.All);
                    chargingBeam.Play();
                    while (chargingBeam.isPlaying) {
                        yield return new WaitForSeconds(0.1f);
                    }

                    chargedBeam.transform.LookAt(character.transform, Vector3.forward);
                    //networkView.RPC("aimWeapon", RPCMode.All, character);

                    networkView.RPC("chargedAnimation", RPCMode.All);
                    chargedBeam.Play();
                    GameObject beamCollider = (GameObject)Network.Instantiate(beamColliderPrefab, character.transform.position, Quaternion.identity, 100);
                    beamCollider.name = "BeamCollider";
                    EnemyBulletSettings weaponSettings = beamCollider.GetComponent<EnemyBulletSettings>();
                    weaponSettings.damage = beamPower;
                    while (chargedBeam.isPlaying) {
                        yield return new WaitForSeconds(0.1f);
                    }

                    Network.Destroy(chargingBeam.gameObject);
                    Network.Destroy(chargedBeam.gameObject);
                    Network.Destroy(beamCollider);
                    networkView.RPC("irisShieldAnimation", RPCMode.All, -0.5f, animation["Irishield"].length);
                    yield return new WaitForSeconds(animation["Irishield"].length * 3f);
                }
            }
            else yield return new WaitForSeconds(1f);
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