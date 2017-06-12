using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class MyNetworkServer : MonoBehaviour {
	const short MESSAGE_DATA = 880;
	const short MESSAGE_INFO = 881;
	const string SERVER_ADDRESS = "129.59.70.59";
	const string TRACKER_ADDRESS = "192.168.1.100";
	const int SERVER_PORT = 5000;

	public string message = "";
	public Text messageText;

	public int _connectionID;
	public static Vector3 _pos = new Vector3 ();
	public static Quaternion _quat = new Quaternion ();

	public int _updateCount = 0;
	public int _messageCount = 0;

	NetworkClient myClient;

	GameObject feather;
	GameObject featherDestination;

	// Use this for initialization
	void Start () {
		messageText = GetComponentInChildren<Text> ();
		feather = GameObject.Find ("The Lead Feather");
		featherDestination = GameObject.Find ("FeatherDestination");
		SetupClient ();
		message = "Discovered Android";
	}


	void Update () 
	{
		_updateCount++;
		resettingFSM ();
	}

	void OnGUI()
	{
		messageText.text = message;
	} 

	// Create a client and connect to the server port
	public void SetupClient()
	{
		myClient = new NetworkClient();
		myClient.RegisterHandler (MESSAGE_DATA, DataReceptionHandler);
		myClient.RegisterHandler(MsgType.Connect, OnConnected);     
		myClient.Connect(SERVER_ADDRESS, SERVER_PORT);
	}

	// client function
	public void OnConnected(NetworkMessage netMsg)
	{
		myClient.Send(MESSAGE_INFO, new VRPNInfo("GearVR", TRACKER_ADDRESS));
		_connectionID = netMsg.conn.connectionId;
		message = "Connected";
	}

	public void OnDisconnected(NetworkMessage netMsg)
	{
		_connectionID = -1;
		message = "Disconnected";
	}

	public void DataReceptionHandler(NetworkMessage _vrpnData)
	{
		VRPNMessage vrpnData = _vrpnData.ReadMessage<VRPNMessage>();
		_pos = vrpnData._pos;
		_quat = vrpnData._quat;
		//transform.eulerAngles = vrpnData._quat.eulerAngles;
		//message = transform.position.ToString();
	}
		

	private Vector3 intendedCenter = new Vector3 (-.6f, 0, -.3f);
	private float prevXAngle = 0f;
	private Vector3 prevPos = new Vector3();
	private bool resetNeeded = false;
	private bool hasNotReturnedToBounds = false;
	private float virtualAngleTurned = 0f; //each reset
	private float cumulativeAngleTurned = 0f; //total

	public void resettingFSM()
	{
		//Gather pertinent data
		Vector3 deltaTranslationByFrame = _pos - prevPos;
		float realWorldRotation = Camera.main.transform.localEulerAngles.y;
		float deltaRotationByFrame = realWorldRotation - prevXAngle;
		//if crossed threshold from + to - (1 to 359)
		if (deltaRotationByFrame > 90) {
			deltaRotationByFrame = deltaRotationByFrame - 360;
		}
		//if crossed threshold from - to + (359 to 1)
		else if (deltaRotationByFrame < -90) {
			deltaRotationByFrame = deltaRotationByFrame + 360;
		}

		//check to see if a reset is needed (only check if no reset has
		//	been triggered yet, and the subject has returned to inner bounds
		if (!resetNeeded && !hasNotReturnedToBounds && OutOfBounds ()) {
			resetNeeded = true;
			hasNotReturnedToBounds = true;
			virtualAngleTurned = 0f;
			feather.SetActive (true);
			Vector3 featherPosition = new Vector3 (featherDestination.transform.position.x, transform.position.y, featherDestination.transform.position.z);
			feather.transform.position = featherPosition;
			Vector3 featherEuler = new Vector3(90, featherDestination.transform.eulerAngles.y, 0);
			feather.transform.eulerAngles = featherEuler;
		}
		//perform reset by manipulating gain (to do this we will rotate the object in the opposite direction)
		else if (resetNeeded) {
			//Calculate the total rotation neccesary
			float calc1 = Mathf.Rad2Deg * Mathf.Atan2 (intendedCenter.x - _pos.x, intendedCenter.z - _pos.z);
			float rotationRemainingToCenter = calc1 - realWorldRotation;
			//fix rotation variables
			if (rotationRemainingToCenter < -360) {
				rotationRemainingToCenter += 360;
			}
			if (rotationRemainingToCenter < -180) {
				rotationRemainingToCenter = 360 + rotationRemainingToCenter;
			}
			float rotationRemaningToCenterP = 0;
			float rotationRemaningToCenterN = 0;
			//determine left and right angles to rotate
			if (rotationRemainingToCenter < 0) {
				rotationRemaningToCenterN = rotationRemainingToCenter;
				rotationRemaningToCenterP = 360 + rotationRemainingToCenter;
			} else {
				rotationRemaningToCenterP = rotationRemainingToCenter;
				rotationRemaningToCenterN = rotationRemainingToCenter - 360;
			}

			//determine gain based on direction subject has rotated already
			//tuned so that at 360 virtual angle turned the person is pointing back to the center
			float gain = 0;
			if (virtualAngleTurned > 0) {
				gain = (360f - virtualAngleTurned) / rotationRemaningToCenterP - 1;
			} else {
				gain = -(360f + virtualAngleTurned) / rotationRemaningToCenterN - 1;
			}
			//inject rotation
			float injectedRotation = (deltaRotationByFrame) * gain;
			virtualAngleTurned += deltaRotationByFrame; //baseline turn
			virtualAngleTurned += injectedRotation; //amount we make them turn as well
			cumulativeAngleTurned -= injectedRotation; //to keep the person moving in the correct direction

			//add the injected rotation to the parent object
			Vector3 tmp = transform.eulerAngles;
			tmp.y += injectedRotation;
			transform.eulerAngles = tmp;
			//if a full turn has occured then stop resetting
			if (Mathf.Abs (virtualAngleTurned) > 359.9f || ReturnedToBounds()) {
				resetNeeded = false;
			}
			message = "Please turn around";
		} 
		//Subject needs to walk forward two steps to prevent further triggers
		else if (hasNotReturnedToBounds) {
			if (ReturnedToBounds ()) {
				hasNotReturnedToBounds = false;
			}
			message = "Please walk forward";
			feather.SetActive (false);
			transform.Translate(deltaTranslationByFrame);
		}
		//General Operating
		else {
			message = "Please go to the destination";
			transform.Translate(deltaTranslationByFrame);
		}
		//update position incrementally using sin and cos
		float delX = Mathf.Cos(cumulativeAngleTurned * Mathf.Deg2Rad) * deltaTranslationByFrame.x + Mathf.Sin(cumulativeAngleTurned * Mathf.Deg2Rad) * deltaTranslationByFrame.z;
		float delZ = Mathf.Cos(cumulativeAngleTurned * Mathf.Deg2Rad) * deltaTranslationByFrame.z + Mathf.Sin(cumulativeAngleTurned * Mathf.Deg2Rad) * deltaTranslationByFrame.x;
		//transform.Translate(deltaTranslationByFrame);
		//store data for use next frame
		prevPos = _pos;
		prevXAngle = Camera.main.transform.localEulerAngles.y;
		message = feather.transform.position.ToString();
	}

	public bool OutOfBounds() {
		if (_pos.x > 1.4f)
			return true;
		if (_pos.x < -2.6f)
			return true;
		if (_pos.z > 1.2f)
			return true;
		if (_pos.z < -1.8f)
			return true;
		return false;
	}

	public bool ReturnedToBounds() {
		if (_pos.x > 1.1f)
			return false;
		if (_pos.x < -2.3f)
			return false;
		if (_pos.z > .9f)
			return false;
		if (_pos.z < -1.5f)
			return false;
		return true;
	}
}
