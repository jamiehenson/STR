using UnityEngine;
using System.Collections;

public class Vortex : MonoBehaviour {
    private bool scaleswitch;
    private int i;
	public int leadsToUniverse;
	public int inUniverse;
    private float growth = 0.015f;
	public Vector3 vortPos;
	public string label;
	private Texture2D bg;
	public static bool labelIsSet = false;
	private Font deco = (Font) Resources.Load ("Belgrad");
	public bool isBeingShrunk = false;

    public static IEnumerator shrink(GameObject vortex)
    {

		labelIsSet = false;
		float x = 1;
        while (x > 0)
        {
            x -= 0.04f;
            yield return new WaitForSeconds(0.005f);
        }
        Destroy(vortex);
    }

	public void setLabel(Vector3 pos, string lab)
	{
		vortPos = pos;
		label = lab;
		labelIsSet = true;
	}



	public void OnGUI()
	{
		int currentUniverse = PlayerManager.Instance.universeNumber;

		if (labelIsSet && currentUniverse == inUniverse)
		{
			Vector3 screenPoint = Camera.main.ViewportToScreenPoint(vortPos);
			screenPoint.y = (Screen.height/2 - (screenPoint.y - Screen.height/2)); // Flip y about center line (lord knows why)
			int bgw = 140; // Vortex note bg width
			int bgh = 30; // Vortex note bg height
			int x = 10;
			int y = 5;
			GUIStyle style = new GUIStyle();
	    	style.font = deco;
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 18;
			label = label.ToUpper();
			GUI.DrawTexture(new Rect(screenPoint.x-(bgw/2-5),screenPoint.y-(bgh/2)-40,bgw,bgh),bg,ScaleMode.StretchToFill, true, 0);
			GUI.Label(new Rect(screenPoint.x,screenPoint.y-40,x,y), label, style);
		}
	}

    void Start()
    {
		StopCoroutine("shrink");
		bg = (Texture2D) Resources.Load ("menu/blankfull");
    }

	void Update () {
	    transform.Rotate(Vector3.up * Time.deltaTime * 40f);
        transform.Rotate(Vector3.forward * Time.deltaTime * 10f);

        if (i == 0) scaleswitch = true;
        else if (i == 150) scaleswitch = false;

        if (scaleswitch)
        {
            transform.localScale += new Vector3(growth, 0, growth);
            i++;
        }
        else
        {
            transform.localScale += new Vector3(-growth, 0, -growth);
            i--;
        }
	}

    void OnTriggerEnter(Collider other) {
        GameObject collided = other.gameObject;
        if (collided.tag == "Player" && collided.GetComponent<PlayerMovement>().myCharacter) {
            HudOn.Instance.enteredVortex(leadsToUniverse);
            HudOn.vortpointOut = gameObject.transform.position;
        }
    }

    void OnTriggerExit(Collider other) {
        GameObject collided = other.gameObject;
        if (collided.tag == "Player" && collided.GetComponent<PlayerMovement>().myCharacter)
            HudOn.Instance.leftVortex();
    }
}
