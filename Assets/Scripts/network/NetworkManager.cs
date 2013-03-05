using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
//using UnityEngine;
//using System.N

public class NetworkManager : MonoBehaviour {
	float btnX;
	float btnY;
	float btnW;
	float btnH;
	//LANBroadcastService lanservice;
	/*
	void Start()
	{
		this.btnX = (float) Screen.width * (float) 0.1;
		this.btnY = (float) Screen.height * (float) 0.1;
		this.btnW = (float) Screen.width * (float) 0.2;
		this.btnH = (float) Screen.width * (float) 0.2;
		lanservice = gameObject.GetComponent<LANBroadcastService>();
	}
	
	void startServer()
	{
		//Network.InitializeServer(32,25001,!Network.HavePublicAddress());
		NetworkSettings.currentBroastcastPort = NetworkSettings.serverBroadcastPort;
		lanservice.StartAnnounceBroadCasting(NetworkSettings.serverBroadcastListener);
		
	}
	
	void lanServiceJoinServer(string strip)
	{
		Debug.Log("In lanServiceJoinServer");
	}
	
	void lanSericeStartService()
	{
		Debug.Log ("In lanServiceStartService");
	}
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(btnX, btnY, btnW, btnH),"Start Server"))
		{
			startServer();
			//Debug.Log("Starting Server");
		}
	
		if (GUI.Button(new Rect(btnX, btnY * (float)1.2 + btnH, btnW, btnH),"Refresh Hosts"))
		{
			//Debug.Log("Referesh Host");
			NetworkSettings.currentBroastcastPort = NetworkSettings.clientBroadcastPort;
			this.lanservice.StartSearchBroadCasting( this.lanServiceJoinServer, this.lanSericeStartService );
		}
	}*/
}
