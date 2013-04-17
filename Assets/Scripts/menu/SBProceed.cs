using UnityEngine;
using System.Collections;

public class SBProceed : MonoBehaviour
{
    private bool proceed = false;

    void Start()
    {
        iTween.FadeTo(gameObject, 0.6f, 0.1f);
    }

    void OnMouseEnter()
    {
        iTween.FadeTo(gameObject, 1.0f, 0.5f);
    }

    void OnMouseExit()
    {
        iTween.FadeTo(gameObject, 0.6f, 0.5f);
    }

    void OnMouseUp()
    {
        proceed = true;
    }

    void Update()
    {
        if (MP.hostScreen && proceed)
        {
            Server.countUniverse = int.Parse(MP.playerLimit);
            Application.LoadLevel("server");
            proceed = false;
        }
        else if (MP.joinScreen && proceed)
        {
            Application.LoadLevel("OnlineClient");
            proceed = false;
        }
    }
}
