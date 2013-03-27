using UnityEngine;
using System.Collections;

public class AsteroidCollisions : MonoBehaviour {

    private float asteroidDamage = 30f;
    private float health = 5;

    private float force = 500;
    public Transform MineFrag;
    public Transform explosion;
	
	private GameObject xp;
    private bool exploded;


    // Destroy asteroid on collision with the character
    [RPC]
    void destroyAfterExplosion()
    {
        Network.Destroy(gameObject);
    }

    void Start() {
        health = health * gameObject.transform.localScale.x;
    }
	
	IEnumerator XP(string points)
    {
        xp = new GameObject("XP");
        xp.AddComponent("GUIText");
        xp.guiText.font = (Font)Resources.Load("Belgrad");
        xp.guiText.fontSize = 28;
        Vector3 placeOfDeath = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        xp.transform.position = new Vector3(placeOfDeath.x / Screen.width, placeOfDeath.y / Screen.height,0.5f);
        xp.guiText.anchor = TextAnchor.MiddleCenter;
        xp.guiText.text = points;
        xp.guiText.material.color = Color.white;
        iTween.FadeTo(xp, 0f, 2f);
        yield return new WaitForSeconds(2);
        Destroy(xp);
    }

    // Set to destroy all in their path!
    void OnTriggerEnter(Collider other) {
		if (Network.isClient)
			return;
		
        GameObject collided = other.gameObject;
        string collidedName = collided.name;
        string collidedNamePrefix;

        if (collidedName.StartsWith("Enemy") && !collidedName.StartsWith("EnemyBullet")) collidedNamePrefix = "Enemy";
        else if (collidedName.StartsWith("Character")) collidedNamePrefix = "Character";
        else if (collidedName.StartsWith("beam")) collidedNamePrefix = "Beam";
        else if (collidedName.StartsWith("cannon")) collidedNamePrefix = "Cannon";
        else if (collidedName.StartsWith("mine")) collidedNamePrefix = "Mine";
        else collidedNamePrefix = collidedName; 

        switch (collidedNamePrefix) {
            case "EnemyBullet":
                if (Network.isServer) Network.Destroy(collided);
                break;
            case "Enemy":
                PlayerCollisions.Boom(collided);
                if (Network.isServer) Network.Destroy(collided);
                break;
            case "Character":
                PlayerManager.hitPoints -= asteroidDamage * gameObject.transform.localScale.x;
                // Explode  asteroid only on the Client (don't network.instantiate it), then destroy from the Server
                if (Network.isClient)
                {
                    Instantiate(explosion, transform.position, transform.rotation);
                    networkView.RPC("destroyAfterExplosion", RPCMode.Server);
                }
                break;
            case "Beam":
                // Do what we want for beam
                PlayerCollisions.WeaponBoom(gameObject, 1);
                if (Network.isServer) Network.Destroy(collided);
                health = health - (WeaponHandler.beamDamage);
                break;
            case "Cannon":
                // Do what we want for cannon
                PlayerCollisions.WeaponBoom(gameObject, 2);
                iTween.MoveBy(gameObject, new Vector3(collided.rigidbody.velocity.x/10,collided.rigidbody.velocity.y/10,0), 4f);
                iTween.RotateAdd(gameObject, new Vector3(50,50), 5f);
                
                health = health - (WeaponHandler.cannonDamage);
                break;
            case "Mine":
                // Do what we want for mine
                if (Network.isClient)
                {
                    for (int i = 0; i < 1; i++)
                    {

                        Transform fragment = (Transform)Instantiate(MineFrag, gameObject.transform.position, Random.rotation);
                        fragment.name = "MineFragment";
                        Physics.IgnoreCollision(fragment.collider, gameObject.collider);
                        fragment.rigidbody.AddForce((Random.insideUnitSphere.normalized * 2) * force);
                    }
                    for (int i = 0; i < 1; i++)
                    {
                        Transform fragment = (Transform)Instantiate(MineFrag, gameObject.transform.position, Random.rotation);
                        fragment.name = "Mine Fragment";
                        Physics.IgnoreCollision(fragment.collider, gameObject.collider);
                        fragment.rigidbody.AddForce((Random.insideUnitCircle.normalized) * force);
                    }
                    networkView.RPC("destroyAfterExplosion", RPCMode.Server);
                }
                health = health - (WeaponHandler.mineDamage);
                
                break;
            case "Mine Fragment":
                health = health - (WeaponHandler.mineFragmentDamage);
                Destroy(collided);
                break;
            default:
                break;
        }
        if (health <= 0 && Network.isServer) {
            Network.Instantiate(explosion, transform.position, transform.rotation,300);
            Network.Destroy(gameObject);
            int scoreAddition = (int) (100 * transform.localScale.x);
            HudOn.score += scoreAddition;
            StartCoroutine(XP("+" + scoreAddition));
        }
    }

    void OnDestroy() {
        if (Network.isServer)
        {
            string num = transform.name.Substring(transform.name.Length - 1, 1);
            int universeNum = int.Parse(num);
            Commander.asteroidCount[universeNum]--;
        }
    }
}
