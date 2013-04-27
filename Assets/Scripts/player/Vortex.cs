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

	public static IEnumerator playerGrow(GameObject player)
    {
		yield break;

        double o = player.transform.localScale.x;
		float x = 0;
        player.transform.localScale = new Vector3(0, 0, 0);
        while (x <= o)
        {
            x += 0.004f;
            player.transform.localScale = new Vector3(x, x, x);
            yield return new WaitForSeconds(0.05f);
        }
    }

	public static IEnumerator playerShrink(GameObject player)
    {
		yield break;
		print ("No no no no no, I got here!");
		labelIsSet = false;
		float x = player.transform.localScale.x;
        while (x > 0)
        {
            x -= 0.0002f;
            player.transform.localScale = new Vector3(x, x, x);
            yield return new WaitForSeconds(0.1f);
        }
    }

	public static IEnumerator grow(GameObject vortex)
    {
		yield break;

        float x = 0;
        vortex.transform.localScale = new Vector3(0, 0, 0);
        while (x <= 4)
        {
            x += 0.06f;
            vortex.transform.localScale = new Vector3(x, 0, x);
            yield return new WaitForSeconds(0.005f);
        }
    }

    public static IEnumerator shrink(GameObject vortex)
    {
		//yield break;

		labelIsSet = false;
		float x = 1;
        while (x > 0)
        {
            x -= 0.04f;
            //vortex.transform.localScale = new Vector3(x, x, x);
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
		StartCoroutine(grow(gameObject));
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

    void OnCollisionEnter(Collision collision)
    {
		GameObject obj = collision.gameObject;
		if (obj.tag == "Player" && obj.GetComponent<PlayerMovement>().myCharacter)
			HudOn.Instance.enteredVortex(leadsToUniverse);
			HudOn.vortpointOut = gameObject.transform.position;
    }
	
	void OnCollisionExit(Collision collision)
    {
		GameObject obj = collision.gameObject;
		if (obj.tag == "Player" && obj.GetComponent<PlayerMovement>().myCharacter)
			HudOn.Instance.leftVortex();
    }
}
