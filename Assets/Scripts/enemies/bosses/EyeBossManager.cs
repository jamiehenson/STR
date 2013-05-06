using UnityEngine;
using System.Collections;

public class EyeBossManager : BossManager
{
    public ParticleSystem chargingBeamPrefab, chargedBeamPrefab;
    public GameObject beamColliderPrefab;
    public bool inPlane = false;
    private BossMovement bossMovement;
    private GameObject character;
    private GameObject[] characters;
    private Vector3 beamPos;
    private Bounds bounds;
    private GameObject cornea;
    private GameObject iris;

    new public void Start() {
        if (Network.isServer) {
            base.Start();
            initStats();
            characters = GameObject.FindGameObjectsWithTag("Player");
            cornea = GameObject.Find("Cornea");
            bounds = cornea.GetComponent<MeshRenderer>().renderer.bounds;

            StartCoroutine(Shoot());
        }
    }

    void initStats() {
        gameObject.name = "Boss0";
        health = 30;
        weaponPower = 1f;
        killPoints = 8000;
        speed = 0.3f;
        firingDelay = 3f;
        moveDelay = 5f;
        force = 500;
    }

    public override IEnumerator Shoot() {
        while (true) {
            if (inPlane) {
                // First open the iris shield
                networkView.RPC("irisShieldAnimation", RPCMode.All, 0.5f, 0.0f);
                yield return new WaitForSeconds(animation["Irishield"].length);
                int target = PickTarget();
                //if (target != -1) {
                    character = GameObject.Find("Character" + 1);

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

                    GameObject beamCollider = (GameObject)Network.Instantiate(beamColliderPrefab, character.transform.position, Quaternion.identity, 100);
                    EnemyBulletSettings weaponSettings = beamCollider.GetComponent<EnemyBulletSettings>();
                    weaponSettings.damage = weaponPower;

                    networkView.RPC("chargedAnimation", RPCMode.All);
                    chargedBeam.Play();
                    while (chargedBeam.isPlaying) {
                        yield return new WaitForSeconds(0.1f);
                    }

                    Network.Destroy(chargingBeam.gameObject);
                    Network.Destroy(chargedBeam.gameObject);
                    Network.Destroy(beamCollider);
                    networkView.RPC("irisShieldAnimation", RPCMode.All, -0.5f, animation["Irishield"].length);
                    yield return new WaitForSeconds(animation["Irishield"].length*3f);
            }
            else yield return new WaitForSeconds(1f);
        }
    }

    [RPC]
    void aimWeapon(GameObject target) {
        GameObject chargedBeam = GameObject.Find("chargedBeam");
        //Vector3 fireDirection = target.transform.position - chargedBeam.transform.position;
        
        //beam.transform.Rotate(new Vector3(90, 0, 90));
        //Physics.IgnoreCollision(beam.collider, gameObject.collider);
    }

    [RPC]
    void irisShieldAnimation(float speed, float time) {
        animation["Irishield"].speed = speed;
        animation["Irishield"].time = time;
        animation.Play("Irishield");
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

    private int universeN() {
        int length = transform.name.Length;
        string num = transform.name.Substring(length - 1, 1);
        if ("0123456789".Contains(num)) return (int.Parse(num));
        else return -1;
    }
}