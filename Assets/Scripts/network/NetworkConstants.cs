using UnityEngine;
using System.Collections;
using System;
using System.Net;

public class NetworkConstants
{
    public static string serverAddress = "10.242.218.237"; //"127.0.0.1";
    public static int serverPort = 22044;
	//public static int serverPort = 25000;
	public static int connectionsAllowed = 32;

    public static string remoteIP = "127.0.0.1";
    public static int remotePort = 25000;
    public static int listenPort = 25000;
    public static bool useNAT = false;
    public static string yourIP = "";
    public static string yourPort = "";
	
	
	// This is set by menu
	public static bool usingMasterServer;
	public static string ipToConnectTo;

    public static string GetIP()
    {
        string strHostName = "";
        strHostName = System.Net.Dns.GetHostName();
        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
        IPAddress[] addr = ipEntry.AddressList;
        return addr[addr.Length-1].ToString();

    }
}
