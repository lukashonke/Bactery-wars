using UnityEngine;

namespace Assets.scripts.Mono
{
	public class NetworkSystem : MonoBehaviour
	{
		int socketId;
		int socketPort = 8880;

		int connectionId;

		int reliableChannel;
		int unreliableChannel;

		// Use this for initialization
		void Start()
		{
			/*NetworkTransport.Init();

			ConnectionConfig config = new ConnectionConfig();

			reliableChannel = config.AddChannel(QosType.Reliable);
			unreliableChannel = config.AddChannel(QosType.Unreliable);

			HostTopology topology = new HostTopology(config, 10);

			socketId = NetworkTransport.AddHost(topology, socketPort);

			Debug.Log("opened socket at " + socketId);

			UdpClient udpServer = new UdpClient(8881);*/
		}


		public void Connect()
		{
			/*byte error;
			connectionId = NetworkTransport.Connect(socketId, "127.0.0.1", 8880, 0, out error);
			Debug.Log("Connected to server. ConnectionId: " + connectionId);*/

			/*IPEndPoint RemoteEndPoint = new IPEndPoint(
			IPAddress.Parse("127.0.0.1"), 8885);
			Socket server = new Socket(AddressFamily.InterNetwork,
									   SocketType.Dgram, ProtocolType.Udp);

			string welcome = "Hello, are you there?";
			var data = Encoding.ASCII.GetBytes(welcome);
			server.SendTo(data, data.Length, SocketFlags.None, RemoteEndPoint);*/
		}

		public void Disconnect()
		{
			/*byte error;
			NetworkTransport.Disconnect(socketId, connectionId, out error);
			Debug.Log("Disconnected");*/
		}

		public void SendMsg()
		{
			/*byte error;
			byte[] buffer = new byte[1024];
			Stream stream = new MemoryStream(buffer);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, "HelloServer");

			int bufferSize = 1024;
        
			NetworkTransport.Send(socketId, connectionId, reliableChannel, buffer, bufferSize, out error);
			Debug.Log("sent msg to conn id " + connectionId);*/
		}

		// Update is called once per frame
		void Update()
		{
			/*int recHostId;
			int recConnectionId;
			int recChannelId;
			byte[] recBuffer = new byte[1024];
			int bufferSize = 1024;
			int dataSize;
			byte error;

			NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

			switch(recNetworkEvent)
			{
				case NetworkEventType.Nothing:
					break;
				case NetworkEventType.ConnectEvent:
					Debug.Log("incoming connection event received");
					break;
				case NetworkEventType.DataEvent:
					Stream stream = new MemoryStream(recBuffer);
					Debug.Log(stream.ToString()); 
					break;
				case NetworkEventType.DisconnectEvent:
					Debug.Log("remote client event disconnected");
					break;
			}*/


			/*IPEndPoint ServerEndPoint = new IPEndPoint(IPAddress.Any, 9050);
			Socket WinSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			WinSocket.Bind(ServerEndPoint);


        

		   // while (true)
			{
				var remoteEP = new IPEndPoint(IPAddress.Any, 11000);
				var data = udpServer.Receive(ref remoteEP); // listen on port 11000

				Debug.Log("receive data from " + remoteEP.ToString());
				Debug.Log(Encoding.ASCII.GetString(data, 0, 1));

				udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
			}*/
		}
	}


}
