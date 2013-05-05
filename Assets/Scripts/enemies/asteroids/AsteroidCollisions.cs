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

    PlayerManager manager;
    //private bool showScore = false;

    // Destroy asteroid on collision with the character
    [RPC]
    void destroyAfterExplosion()
    {
        //Network.Destroy(gameObject);
        Instantiate(explosion, transform.position, transform.rotation);
    }



    void Start() {
        health = health * gameObject.transform.localScale.x;
    }

    private int universeN()
    {
        int length = transform.parent.parent.name.Length;
        string num = transform.parent.parent.name.Substring(length - 1, 1);
        if ("0123456789".Contains(num)) return (int.Parse(num));
        else return -1;
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
        bool hit = false;
        if (universeN() != -1)
        {
            manager = GameObject.Find("Character" + universeN()).GetComponent<PlayerManager>();
            GameObject collided = other.gameObject;
            string collidedTag = collided.tag;
            switch (collidedTag)
            {
                case "EnemyWeapon":
                    Network.Destroy(collided);
                    break;
                case "Enemy":
                    PlayerCollisions.Boom(collided);
                    Network.Destroy(collided);
                    break;
                case "Player":
                    manager.updateHitPoints(-asteroidDamage * gameObject.transform.localScale.x);
                    health = 0;
                    break;
                case "PlayerBeam":
                    // Do what we want for beam
                    PlayerCollisions.WeaponBoom(gameObject, 1);
                    Network.Destroy(collided);
                    health = health - (WeaponHandler.beamDamage);
                    hit = true;
                    break;
                case "PlayerCannon":
                    // Do what we want for cannon
                    PlayerCollisions.WeaponBoom(gameObject, 2);
                    Network.Destroy(collided);
                    iTween.MoveBy(gameObject, new Vector3(collided.rigidbody.velocity.x / 10, collided.rigidbody.velocity.y / 10, 0), 4f);
                    iTween.RotateAdd(gameObject, new Vector3(50, 50), 5f);
                    hit = true;
                    health = health - (WeaponHandler.cannonDamage);
                    break;
                case "PlayerMine":
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
                    Network.Destroy(collided);
                    hit = true;
                    health = health - (WeaponHandler.mineDamage);

                    break;
                case "MineFrag":
                    health = health - (WeaponHandler.mineFragmentDamage);
                    Network.Destroy(collided);
                    hit = true;
                    break;
                default:
                    break;
            }
            if (health <= 0 && hit)
            {
                int scoreAddition = (int)(100 * transform.localScale.x);
                manager.updateScore(scoreAddition);
                networkView.RPC("scoreXP", RPCMode.All, universeN(), scoreAddition);
                Network.Instantiate(explosion, transform.position, transform.rotation, 0);
                Network.Destroy(gameObject);
                
                //HudOn.score += scoreAddition;

                //StartCoroutine(XP("+" + scoreAddition));
            }
        }
    }

    [RPC]
    void scoreXP(int camNum, int score)
    {
        if (Network.isClient && GameObject.Find("Camera " + camNum))
        {
            StartCoroutine(XP("+" + score));
           //s networkView.RPC("showedScore", RPCMode.Server);
        }
    }

    [RPC]
    void showedScore()
    {
        //showScore = true;
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
