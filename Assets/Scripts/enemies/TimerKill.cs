using UnityEngine;
using System.Collections;

public class TimerKill : MonoBehaviour {

    private float time = 1;

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
