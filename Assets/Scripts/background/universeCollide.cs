using UnityEngine;
using System.Collections;

public class universeCollide : MonoBehaviour {

    void OnTriggerExit(Collider collided)
    {
        if (Network.isServer)
        {
            string collidedName = collided.name;
            if (collidedName.StartsWith("Asteroid"))
            {
                Debug.Log("Asteroid Collision Here");
                Network.Destroy(collided.gameObject);
            }
            switch (collidedName)
            {
                case "EnemyBullet":
                    Network.Destroy(collided.gameObject);
                    break;
                case "beamUSA(Clone)":
                    Network.Destroy(collided.gameObject);
                    break;
                case "cannonUSA(Clone)":
                    Network.Destroy(collided.gameObject);
                    break;
                case "mineUSA(Clone)":
                    Network.Destroy(collided.gameObject);
                    break;
                case "beamChina(Clone)":
                    Network.Destroy(collided.gameObject);
                    break;
                case "cannonChina(Clone)":
                    Network.Destroy(collided.gameObject);
                    break;
                case "mineChina(Clone)":
                    Network.Destroy(collided.gameObject);
                    break;
                case "beamRussia(Clone)":
                    Network.Destroy(collided.gameObject);
                    break;
                case "cannonRussia(Clone)":
                    Network.Destroy(collided.gameObject);
                    break;
                case "mineRussia(Clone)":
                    Network.Destroy(collided.gameObject);
                    break;
                case "MineFrag(Clone)":
                    Network.Destroy(collided.gameObject);
                    break;
                default:
                    break;

            }
        }
    }


}
