using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine.VR;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ExperimentalState : MessageBase
{
	public Vector3 _pos;
	public Vector3 _euAngle;
	public int _trialNumber;

	public ExperimentalState()
	{
		_pos = new Vector3 ();
		_euAngle = new Vector3 ();
		_trialNumber = 0;
	}

	public ExperimentalState(Vector3 pos, Vector3 euAngle, int trialNumber)
	{
		_pos = pos;
		_euAngle = euAngle;
		_trialNumber = trialNumber;
	}
}

class NetworkControlCenter
{
	const short STATE_DATA = 890;
	const short CONNECTION_STATUS = 893;
	const string SERVER_ADDRESS = "192.168.11.2";
	const int SERVER_PORT = 5001;
	int framesSinceContact = 0;
	NetworkClient myClient;
	private bool isClient = false;

	private GameObject parent;
	private TrialManager trialManager;

	public void Start(GameObject newParent, TrialManager newTM, bool server)
	{
		parent = newParent;
		trialManager = newTM;
		if (!server) {
			SetupClient ();
		} else {
			StartServer ();
		}
	}

	public void Update()
	{
		//return;
		framesSinceContact++;
		if (isClient) {
			if (myClient.isConnected) {
				myClient.Send (CONNECTION_STATUS, new ExperimentalState ());
				Debug.Log ("Hello");
			}
			if (framesSinceContact > 240) {
				framesSinceContact = -600;
				//RestartClient ();
			}
		} else {
			NetworkServer.SendToAll (CONNECTION_STATUS, new ExperimentalState());
			if (framesSinceContact > 600) {
				framesSinceContact = -600;
				RestartServer ();
			}
		}
	}

	//client code

	private void SetupClient()
	{
		//This code runs on windows
		isClient = true;
		myClient = new NetworkClient ();
		myClient.RegisterHandler (STATE_DATA, StateUpdate);
		myClient.RegisterHandler (MsgType.Connect, OnClientConnected);
		myClient.RegisterHandler (CONNECTION_STATUS, OnKeepInTouch);
		myClient.Connect (SERVER_ADDRESS, SERVER_PORT);
	}

	private void RestartClient()
	{
		//This code runs on windows
		myClient.Disconnect();
		myClient.UnregisterHandler (STATE_DATA);
		myClient.UnregisterHandler (MsgType.Connect);
		myClient.UnregisterHandler (CONNECTION_STATUS);
		isClient = true;
		myClient = new NetworkClient ();
		myClient.RegisterHandler (STATE_DATA, StateUpdate);
		myClient.RegisterHandler (MsgType.Connect, OnClientConnected);
		myClient.RegisterHandler (CONNECTION_STATUS, OnKeepInTouch);
		myClient.Connect (SERVER_ADDRESS, SERVER_PORT);
	}

	public void SendClientUpdate()
	{
		//Debug.Log ("Sent Data to Server");
		myClient.Send(STATE_DATA, new ExperimentalState (parent.transform.position, parent.transform.eulerAngles, trialManager.GetOrderIndex ()));
	}
		

	public void StateUpdate(NetworkMessage ServerStateDataMessage)
	{
		//Run on client, recives stat update
		//Debug.Log("Recieved Data from Server");
		ExperimentalState ServerData = ServerStateDataMessage.ReadMessage<ExperimentalState> ();
		trialManager.SetOrderIndex (ServerData._trialNumber);
	}

	public void OnClientConnected(NetworkMessage netMsg)
	{
		//do nothing
		trialManager.SetOrderIndex(5);
		Debug.Log ("Client Connected");
	}

	//both somehow

	public void OnKeepInTouch(NetworkMessage connectionPresent)
	{
		framesSinceContact = -2400;
	}

	//server code

	private void StartServer()
	{
		//This code runs on android
		NetworkServer.Listen(SERVER_PORT);
		NetworkServer.RegisterHandler (MsgType.Connect, OnServerConnected);
		NetworkServer.RegisterHandler (STATE_DATA, OnStateRecieved);
		Debug.Log ("Hello");
	}

	private void RestartServer()
	{
		NetworkServer.Reset ();
		NetworkServer.Listen(SERVER_PORT);
		NetworkServer.RegisterHandler (MsgType.Connect, OnServerConnected);
		Debug.Log ("Hello");
		NetworkServer.RegisterHandler (STATE_DATA, OnStateRecieved);
	}

	public void OnServerConnected(NetworkMessage netMsg)
	{
		//Send out current state
		NetworkServer.SetClientReady(netMsg.conn);
		SendStateUpdate ();
	}

	public void SendStateUpdate()
	{
		NetworkServer.SendToAll (STATE_DATA, new ExperimentalState (parent.transform.position, parent.transform.eulerAngles, trialManager.GetOrderIndex ()));
	}

	public void OnStateRecieved(NetworkMessage ClientStateDataMessage)
	{
		//Run on server, recieves what new state should be from client
		ExperimentalState ClientData = ClientStateDataMessage.ReadMessage<ExperimentalState> ();
		trialManager.SetOrderIndex (ClientData._trialNumber);
		ReOrienterAndTester parentTest = parent.GetComponent<ReOrienterAndTester> ();
		parentTest.UpdateTrial ();
	}
		
}

