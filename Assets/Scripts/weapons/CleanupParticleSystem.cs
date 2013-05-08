using UnityEngine;
using System.Collections;

public class CleanupParticleSystem : MonoBehaviour {

    private ParticleSystem ps;

	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	void LateUpdate () {
        if (ps) {
            if (!ps.IsAlive()) {
                print("Destroying " + gameObject.name);
                Destroy(gameObject, ps.duration);
            }
        }
	}
}
