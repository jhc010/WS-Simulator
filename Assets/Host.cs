using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Host : MonoBehaviour {
    NetworkManager net;
    
	public void StartHost() {
        net = GameObject.Find("Network").GetComponent<NetworkManager>();
        net.StartHost();
    }
	
	public void StartClient() {
        net = GameObject.Find("Network").GetComponent<NetworkManager>();
        net.StartClient();
    }
}
