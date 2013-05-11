using UnityEngine;
using System.Collections;

public class scrolling : MonoBehaviour {

    private float speed = 50.0f;
    private ParticleSystem stars;
    private float currentSpeed;

	// Use this for initialization
	void Start () {
        stars = gameObject.GetComponent<ParticleSystem>();
        stars.startSpeed = speed;
        stars.emissionRate = speed;
	}

}
