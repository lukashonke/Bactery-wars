using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MultiplayerManager : NetworkManager
{
	public void HostGame()
	{
		NetworkManager.singleton.StartHost();
	}

	public void DedicatedServer()
	{
		NetworkManager.singleton.StartServer();
	}

	public void JoinGame()
	{
		NetworkManager.singleton.StartClient();
	}
}
