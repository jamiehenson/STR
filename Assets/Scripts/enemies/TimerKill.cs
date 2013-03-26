using UnityEngine;
using System.Collections;

public class TimerKill : MonoBehaviour {

    public float time = 5;

    IEnumerator timedkill() {
        yield return new WaitForSeconds(time);
      //  if(Network.isServer)
        {
            Destroy(gameObject);
        }
    }

	void Start () {
        StartCoroutine("timedkill");	
	}
}
