using UnityEngine;
using System.Collections;

public class EnemyBulletDestroy : MonoBehaviour {

	void Destroy() {
		networkView.RPC("DestoryOnServer", RPCMode.Server);
	}

	void DestroyOnServer() {
		Network.Destroy(gameObject);
	}
}
