using UnityEngine;
using System.Collections;
using System;
using System.Net;

// Holds constants to be used in Networking
public class NetworkConstants
{
	// MasterServer
    public static string masterServerAddress = "54.243.193.180";
	
	// Server
    public static int serverPort = 23467;
	public static int connectionsAllowed = 32;
	
	// This can be done better with Network.player.ipAddress. Will be remove when all its uses are removed
    public static string GetIP()
	{
		Log.Warning("GetIP() is called, this can be done better with Network.player.ipAddress");
        string strHostName = "";
        strHostName = System.Net.Dns.GetHostName();
        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
        IPAddress[] addr = ipEntry.AddressList;
        return addr[addr.Length-1].ToString();
    }
}
