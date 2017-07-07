using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.VR;
using System.IO;
using System;

public class MyNetworkServer : MonoBehaviour {
	const short MESSAGE_DATA = 880;
	const short MESSAGE_INFO = 881;
	const string SERVER_ADDRESS = "192.168.1.2";
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
	GameObject HUD;

	// Use this for initialization
	void Start () {
		//messageText = GetComponentInChildren<Text> ();
		feather = GameObject.Find ("The Lead Feather");
		featherDestination = GameObject.Find ("FeatherDestination");
		HUD = GameObject.Find ("HUD");
		HUD.SetActive (false);
		SetupClient ();
		message = "Discovered Android";
	}


	void Update () 
	{
		_updateCount++;
		resettingFSM ();
	}

	void FixedUpdate() //was previously FixedUpdate()
	{
		string path = Application.persistentDataPath + "/CW4Test_Data.txt";

		// This text is always added, making the file longer over time if it is not deleted
		string appendText = "\n" + DateTime.Now.ToString() + "\t" + 
			Time.time + "\t" + 

			Input.GetMouseButtonDown(0) + "\t" +

			Input.gyro.userAcceleration.x + "\t" + 
			Input.gyro.userAcceleration.y + "\t" + 
			Input.gyro.userAcceleration.z + "\t" + 

			gameObject.transform.position.x + "\t" + 
			gameObject.transform.position.y + "\t" + 
			gameObject.transform.position.z + "\t" +

			InputTracking.GetLocalRotation (VRNode.Head).eulerAngles.x + "\t" +
			InputTracking.GetLocalRotation (VRNode.Head).eulerAngles.y + "\t" +
			InputTracking.GetLocalRotation (VRNode.Head).eulerAngles.z;

		File.AppendAllText(path, appendText);

        move();
	}

	void OnGUI()
	{
        //messageText.text = message;
    }


    private float yaw;
    private float rad;
    private float xVal;
    private float zVal;

    public static float velocity = 0f;
    public static float method1StartTimeGrow = 0f;
    public static float method1StartTimeDecay = 0f;
    public static bool wasOne = false; //phase one when above (+/-) 0.105 threshold
    public static bool wasTwo = true; //phase two when b/w -0.105 and 0.105 thresholds


    void move()
    {
        yaw = InputTracking.GetLocalRotation(VRNode.Head).eulerAngles.y;
        rad = yaw * Mathf.Deg2Rad;
        zVal = 0.55f * Mathf.Cos(rad);
        xVal = 0.55f * Mathf.Sin(rad);

        if ((Input.gyro.userAcceleration.y >= 0.105f || Input.gyro.userAcceleration.y <= -0.105f) &&
            (Input.gyro.userAcceleration.z < 0.08f && Input.gyro.userAcceleration.z > -0.08f))
        {
            if (wasTwo)
            { //we are transitioning from phase 2 to 1
                method1StartTimeGrow = Time.time;
                wasTwo = false;
                wasOne = true;
            }
        }
        else
        {
            if (wasOne)
            {
                method1StartTimeDecay = Time.time;
                wasOne = false;
                wasTwo = true;
            }
        }

        if ((Input.gyro.userAcceleration.y >= 0.105f || Input.gyro.userAcceleration.y <= -0.105f) &&
            (Input.gyro.userAcceleration.z < 0.08f && Input.gyro.userAcceleration.z > -0.08f))
        { //0.08 is an arbitrary threshold

            velocity = 3f - (3f - velocity) * Mathf.Exp((method1StartTimeGrow - Time.time) / 1.6f); //grow
        }
        else
        {

            velocity = 0f - (0f - velocity) * Mathf.Exp((method1StartTimeDecay - Time.time) / 1.6f); //decay
        }

        transform.Translate(xVal * velocity * Time.fixedDeltaTime, 0, zVal * velocity * Time.fixedDeltaTime);

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
		Debug.Log (_pos);
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
			HUD.SetActive (true);
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
				HUD.SetActive (false);
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
			Vector3 tmp = transform.position;
			tmp.y = _pos.y;
			transform.position = tmp;
			//transform.position.y = _pos.y;
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
