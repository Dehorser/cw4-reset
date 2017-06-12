using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class VRPNMessage : MessageBase
{
	public Vector3 _pos;
	public Quaternion _quat;

	public VRPNMessage()
	{
		_pos = new Vector3 ();
		_quat = new Quaternion ();
	}

	public VRPNMessage(Vector3 pos, Quaternion quat)
	{
		_pos = pos;
		_quat = quat;
	}
}

public class MyNetworkServer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public bool isAtStartup = true;
	public bool isAndroid = false;
	public bool foundMessage = false;
	public string message = "";

	public int _connectionID;
	public Vector3 _pos = new Vector3 ();
	public Quaternion _quat = new Quaternion ();

	NetworkClient myClient;

	void Update () 
	{
		if (isAtStartup) {
			if (Input.GetKeyDown (KeyCode.S)) {
				SetupServer ();
			}

			if (Application.platform == RuntimePlatform.Android || Input.GetKeyDown (KeyCode.C)) {
				SetupClient ();
				isAndroid = true;
			}
		} else if (isAndroid) {
			//do nothing
		} else {
			Vector3 tmpPos = VRPN.vrpnTrackerPos ("GearVR@192.168.1.100", 0);
			Quaternion tmpQuat = VRPN.vrpnTrackerQuat ("GearVR@192.168.1.100", 0);
			Vector3 pos = new Vector3 (10*tmpPos.x, 10*tmpPos.z, 10*tmpPos.y);
			//Quaternion quat = new Quaternion (-tmpQuat.x, -tmpQuat.y, -tmpQuat.z, tmpQuat.w);
			Quaternion quat = new Quaternion (0,0,0,1);
			if (_connectionID > 0) {
				NetworkServer.SendToClient (_connectionID, (short)880, new VRPNMessage (pos, quat));
			}
			transform.position = pos;
			transform.eulerAngles = quat.eulerAngles;
		}
	}

	void OnGUI()
	{
		if (isAtStartup) {
			GUI.Label (new Rect (2, 10, 150, 100), "Press S for server");      
			GUI.Label (new Rect (2, 50, 150, 100), "Press C for client");
		} else if (isAndroid) {
			
		} else {
			
		}
	} 

	// Create a server and listen on a port
	public void SetupServer()
	{
		NetworkServer.Listen(5000);
		NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
		isAtStartup = false;
	}

	// Create a client and connect to the server port
	public void SetupClient()
	{
		myClient = new NetworkClient();
		myClient.RegisterHandler ((short)880, DataReceptionHandler);
		myClient.RegisterHandler(MsgType.Connect, OnConnected);     
		myClient.Connect("129.59.70.59", 5000);
		isAtStartup = false;
	}

	// client function
	public void OnConnected(NetworkMessage netMsg)
	{
		_connectionID = netMsg.conn.connectionId;
		message = "Connected";
	}

	public void DataReceptionHandler(NetworkMessage _vrpnData)
	{
		VRPNMessage vrpnData = _vrpnData.ReadMessage<VRPNMessage>();
		transform.position = vrpnData._pos;
		transform.eulerAngles = vrpnData._quat.eulerAngles;
	}
}
