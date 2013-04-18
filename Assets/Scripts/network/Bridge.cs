using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bridge : MonoBehaviour {
    public Server server;
    public OnlineClient client;

    Dictionary<NetworkPlayer, bool> hasClientLoaded;
    Dictionary<NetworkPlayer, int> setUniverseBuffer;
    Dictionary<NetworkPlayer, Dictionary<NetworkViewID, string>> updateUniverseNamesBuffer;
    Dictionary<NetworkPlayer, Dictionary<NetworkViewID, string>> updateCharacterNamesBuffer;

    // Use this for initialization
    void Start() {
        if (Network.isServer) {
            server = GameObject.Find("Network").GetComponent<Server>();
        }
        else {
            Network.isMessageQueueRunning = true;
            client = GameObject.Find("Client Scripts").GetComponent<OnlineClient>();
            print("Client Bridge initial" + Network.player);
            networkView.RPC("clientBridgeLoaded", RPCMode.Server, Network.player);
        }

        setUniverseBuffer = new Dictionary<NetworkPlayer, int>();
        updateUniverseNamesBuffer = new Dictionary<NetworkPlayer, Dictionary<NetworkViewID, string>>();
        updateCharacterNamesBuffer = new Dictionary<NetworkPlayer, Dictionary<NetworkViewID, string>>();
        hasClientLoaded = new Dictionary<NetworkPlayer, bool>();
    }

    public void addPlayer(NetworkPlayer player) {
       hasClientLoaded.Add(player, false);
    }

    [RPC]
    public void clientBridgeLoaded(NetworkPlayer player) {
        Debug.Log("ClientBridgeNowLoaded");

        hasClientLoaded[player] = true;

        if (updateUniverseNamesBuffer.ContainsKey(player)) {
            Dictionary<NetworkViewID, string> dic = updateUniverseNamesBuffer[player];
            foreach (KeyValuePair<NetworkViewID, string> pair in dic) {
                networkView.RPC("updateUniverseNames", player, pair.Key, pair.Value);
            }
            updateUniverseNamesBuffer.Remove(player);
        }

        if (updateCharacterNamesBuffer.ContainsKey(player))
        {
            Dictionary<NetworkViewID, string> dic = updateCharacterNamesBuffer[player];
            foreach (KeyValuePair<NetworkViewID, string> pair in dic)
            {
                networkView.RPC("updateCharacterNames", player, pair.Key, pair.Value);
            }
            updateCharacterNamesBuffer.Remove(player);
        }
        if (setUniverseBuffer.ContainsKey(player)) {
            networkView.RPC("setUniverse", player, setUniverseBuffer[player]);
            setUniverseBuffer.Remove(player);
        }
    }

    [RPC]
    public void setUniverse(int num) {
        if (Network.isClient) {
            client.setUniverse(num);
            Debug.Log("Bridge Client Set Universe RPC");
        }
    }


    public void setUniverse(int num, NetworkPlayer player) {
        if (Network.isServer) {
            if (hasClientLoaded[player]) {
                networkView.RPC("setUniverse", player, num);
            }
            else {
               setUniverseBuffer.Add(player, num);
            }
            Debug.Log("Bridge Client Set Universe");
        }

    }

    [RPC]
    public void updateUniverseNames(NetworkViewID ID, string name) {
        if (Network.isClient) {
            Debug.Log("Player" + ID + " " + name);
            client.updateUniverseNames(ID, name);
        }
    }

    public void updateUniverseNames(Dictionary<NetworkViewID, string> list, NetworkPlayer player) {
        if (Network.isServer) {
            if (hasClientLoaded[player]) {
                foreach (KeyValuePair<NetworkViewID, string> pair in list) {
                    Debug.Log("Player" + pair.Key + " " + pair.Value);
                    networkView.RPC("updateUniverseNames", player, pair.Key, pair.Value);
                }
            }
            else {
                updateUniverseNamesBuffer.Add(player, list);
            }
        }
    }

    [RPC]
    public void updateCharacterNames(NetworkViewID ID, string name)
    {
        if (Network.isClient)
        {
            Debug.Log("Player" + ID + " " + name);
            client.updateCharacterNames(ID, name);
        }
    }
    public void updateCharacterNames(Dictionary<NetworkViewID, string> list, NetworkPlayer player)
    {
        if (Network.isServer)
        {
            if (hasClientLoaded[player])
            {
                foreach (KeyValuePair<NetworkViewID, string> pair in list)
                {
                    Debug.Log("Player" + pair.Key + " " + pair.Value);
                    networkView.RPC("updateCharacterNames", player, pair.Key, pair.Value);
                }
            }
            else
            {
                updateCharacterNamesBuffer.Add(player, list);
            }
        }
    }

	public void moveCamera(int universeNum, NetworkPlayer player) {
		if (Network.isServer)
			networkView.RPC("moveCameraRPC", player, universeNum);
	}

	[RPC]
	public void moveCameraRPC(int universeNum) {
		if (Network.isClient)
			client.moveCamera(universeNum);
	}

  /*  public void sendCharacter(NetworkViewID viewID, Vector3 position, NetworkPlayer player)
    {
        if(Network.isClient)
            networkView.RPC("createCharacter", RPCMode.Server, viewID, position, player);
    }
    [RPC]
    public void createCharacter(NetworkViewID viewID, Vector3 position, NetworkPlayer player)
    {
        if (Network.isServer)
        {
            if(hasClientLoaded[player])
                server.createCharacter(viewID, position, player);
        }
    }*/
    // Update is called once per frame
    void Update() {

    }
}