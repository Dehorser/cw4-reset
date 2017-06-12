using UnityEngine;
using System.Collections;

//start hopeShoulder
using UnityEngine.VR;
using System;
//end hopeShoulder

using System.Collections.Generic;

public class OnlineBodyView : MonoBehaviour {
	private static OnlineBodyView singleton;
	public static OnlineBodyView s {get {return singleton;}}
	protected void Awake(){
		singleton = this;
	}	

	public Material sphere;

	public OnlineBody[] bodies;

	public Transform avatarPrefab;

	private float handClosedDist = 0.19f;


	//start hopeShoulder
	private float leftKneeHeight;
	private float leftFootHeight;
	private float leftHipHeight;
	private float leftAnkleHeight;

	private float rightKneeHeight;
	private float rightFootHeight;
	private float rightHipHeight;
	private float rightAnkleHeight;

	private Vector3 leftKneePosition;
	private Vector3 leftFootPosition;
	private Vector3 leftHipPosition;
	private Vector3 leftAnklePosition;

	private Vector3 rightKneePosition;
	private Vector3 rightFootPosition;
	private Vector3 rightHipPosition;
	private Vector3 rightAnklePosition;

	public static float angleL;
	public static float angleR;
	//private float angleThreshold = 135; - it has temporarily been hard-coded
	public float ERROR = 43.0f;

	private float yaw;
	private float rad;
	private float xVal;
	private float zVal;
	public static int steps = 0;

	public static float stepStartTime = 0.001f;
	public static float stepEndTime = 0.001f;

	private bool stepping = false;

	public static float peakAngle = 180.0f;
	public static float peakTime = 0f;

	public static float avgStepTime = 0.65f;
	public static float lastStepTime = 1f;

	public static float avgPeakTime = 0.2f;
	public static float lastPeakTime = 0f;

	public static float velocity = 0.0f;

	public static float rate;

	public static float sequenceStartTime;

	public static float lastStepStart = 0f;
	public static float lastStepEnd = 0f;

	public static bool sequenceHasStarted;

	public static float maxRate = 1f;

	public static float firstFootTime = 0f;
	public static bool footIsLeft; 
	private bool footIsRight;
	private bool checkOtherFoot;
	private bool firstFootPass;
	private bool firstCheck = false;

	private float footTimeThreshold = 0.2f;
	private bool thresholdCheck = false;
	private float noiselessRiseTime = 0f;
	public static bool avgCheck = false;

	private bool lastTimeCheck = false;

	private bool noNewStepYet = false;
	//end hopeShoulder


	// Use this for initialization
	void Start () {

		bodies = new OnlineBody[6];
	}
	
	// Update is called once per frame
	void Update () {
		//MapAvatarRotation (); //this was originally uncommented

		//start hopeShoulder
		checkStep();
		//end hopeShoulder
	}

	public void UpdateBodyList (string data){
		string[] r = data.Split (","[0]);

		for (int i = 0; i<bodies.Length; i++) {
			if (bodies[i] != null){
				bool check = false;
				for (int x = 0; x<r.Length; x++) {

					if (bodies[i].name == r[x]) {
						
						check = true;
					}
				}
				if (!check){

					DeleteBody(bodies[i].name);
				}
			}
		}
		//Debug.Log (data);
	}

	public void CreateBody (string name){
		for (int i = 0; i<bodies.Length; i++) {
			if (bodies[i] == null){
			bodies[i] = new OnlineBody (name);
				//bodies[i].avatar = Instantiate (ObjectManager.s.avatarPrefab, bodies[i].go.transform.position,  ObjectManager.s.avatarPrefab.rotation) as Transform;
				//bodies[i].avatar.parent = bodies[i].go.transform;
				//bodies[i].avatar.localPosition = new Vector3(0,0,0);
				//bodies[i].anim = bodies[i].avatar.gameObject.GetComponent<Animator>();
				//bodies[i].character = Instantiate(avatarPrefab, new Vector3(0f, 0f, 0f), avatarPrefab.rotation) as Transform;
				//MapToKinect map = bodies[i].character.GetComponent<MapToKinect>();
				//map.AssignBody (name);

				break;
			}

		}
	}

	public void DeleteBody (string name){

		for (int i = 0; i<bodies.Length; i++) {
			if (bodies[i].name == name){
				//Destroy(bodies[i].character.gameObject);
				//OnlineBodyPhysics physScript = bodies[i].go.GetComponent<OnlineBodyPhysics>();
				//Destroy(physScript.heightSphere.gameObject);
				Destroy(bodies[i].go);
				//Destroy (bodies[i].avatar.gameObject);
				bodies[i] = null;
				if (PlayerManager.s.currentPlayer != null && name == PlayerManager.s.currentPlayer.name){
						PlayerManager.s.currentPlayer = null;
					}
				break;
			}
		}
	}

	public void SyncState (string[] data){
		Debug.Log ("Sync State");
		for (int i = 1; i<data.Length-1; i++) {
			bool check = false;
			for (int j = 0; j<bodies.Length; j++){
				if (bodies[j] != null)
				if (data[i] == bodies[j].name){
					check = true;
					break;
				}
			}
			Debug.Log (check);
			if (!check){
				CreateBody(data[i]);
			}
		}

		for (int r = 0; r<bodies.Length; r++) {
			if (bodies[r] != null){
				bool check = false;
				for (int k = 0; k<data.Length; k++){
						if(bodies[r].name == data[k])
							check = true;
				}
				if (check == false){
					DeleteBody (bodies[r].name);
				}
			}
		}

	}



	public void RefreshBody (string[] data){

		//print (data [8] + " " + data[9]);

		//name = data [1];
			var bodyname = data [1];
			for (int i = 0; i<bodies.Length; i++) {
				if (bodies [i] != null)
				if (bodies [i].name == bodyname) {
					for (int j = 2; j<data.Length-1; j++) {
						string[] part = data [j].Split ("," [0]);
						for (int r = 0; r<bodies[i].parts.Length; r++) {
							if (bodies [i].parts [r].name == part [0]) {
							
						
								Vector3 pos = new Vector3 (0f, 0f, 0f);
								Quaternion rot = new Quaternion (0f, 0f, 0f, 0f);
                                
								float.TryParse (part [1], out pos.x);
								float.TryParse (part [2], out pos.y);
								float.TryParse (part [3], out pos.z);

								float.TryParse (part [4], out rot.x);
								float.TryParse (part [5], out rot.y);
								float.TryParse (part [6], out rot.z);
								float.TryParse (part [7], out rot.w);

								bodies [i].parts [r].go.transform.localPosition = pos; 
								//bodies [i].parts [r].go.transform.localRotation = rot;					

								
							bodies [i].parts [r].infered = int.Parse(part[8]);


							if (part[8] == "0"){
								bodies [i].parts [r].r.material.color = ObjectManager.s.coolMat.color;
								}else{
								bodies [i].parts [r].r.material.color = new Color(1f,0f,0f);
								}

							}


						}
						
					}

					break;

				}
			}


	}

	public void MapAvatarRotation (){
		foreach (OnlineBody body in bodies){
			if (body != null){
				//Quaternion newrot = Quaternion.FromToRotation(body.partsDic["WristRight"].go.transform.position,body.partsDic["HandRight"].go.transform.position);

				// Line Renderer Positions
				body.partsDic["SpineShoulder"].lr.SetPosition(0, body.partsDic["SpineShoulder"].go.transform.position);
				body.partsDic["SpineShoulder"].lr.SetPosition(1, body.partsDic["Neck"].go.transform.position);

				body.partsDic["Neck"].lr.SetPosition(0, body.partsDic["Neck"].go.transform.position);
				body.partsDic["Neck"].lr.SetPosition(1, body.partsDic["Head"].go.transform.position);

				body.partsDic["WristRight"].lr.SetPosition(0, body.partsDic["WristRight"].go.transform.position);
				body.partsDic["WristRight"].lr.SetPosition(1, body.partsDic["ElbowRight"].go.transform.position);

				body.partsDic["ElbowRight"].lr.SetPosition(0, body.partsDic["ElbowRight"].go.transform.position);
				body.partsDic["ElbowRight"].lr.SetPosition(1, body.partsDic["ShoulderRight"].go.transform.position);

				body.partsDic["ShoulderRight"].lr.SetPosition(0, body.partsDic["ShoulderRight"].go.transform.position);
				body.partsDic["ShoulderRight"].lr.SetPosition(1, body.partsDic["SpineShoulder"].go.transform.position);

				body.partsDic["WristLeft"].lr.SetPosition(0, body.partsDic["WristLeft"].go.transform.position);
				body.partsDic["WristLeft"].lr.SetPosition(1, body.partsDic["ElbowLeft"].go.transform.position);
				
				body.partsDic["ElbowLeft"].lr.SetPosition(0, body.partsDic["ElbowLeft"].go.transform.position);
				body.partsDic["ElbowLeft"].lr.SetPosition(1, body.partsDic["ShoulderLeft"].go.transform.position);
				
				body.partsDic["ShoulderLeft"].lr.SetPosition(0, body.partsDic["ShoulderLeft"].go.transform.position);
				body.partsDic["ShoulderLeft"].lr.SetPosition(1, body.partsDic["SpineShoulder"].go.transform.position);

				body.partsDic["SpineShoulder"].lr.SetPosition(0, body.partsDic["SpineShoulder"].go.transform.position);
				body.partsDic["SpineShoulder"].lr.SetPosition(1, body.partsDic["SpineMid"].go.transform.position);

				body.partsDic["SpineMid"].lr.SetPosition(0, body.partsDic["SpineMid"].go.transform.position);
				body.partsDic["SpineMid"].lr.SetPosition(1, body.partsDic["SpineBase"].go.transform.position);

				body.partsDic["HipRight"].lr.SetPosition(0, body.partsDic["HipRight"].go.transform.position);
				body.partsDic["HipRight"].lr.SetPosition(1, body.partsDic["SpineBase"].go.transform.position);

				body.partsDic["KneeRight"].lr.SetPosition(0, body.partsDic["KneeRight"].go.transform.position);
				body.partsDic["KneeRight"].lr.SetPosition(1, body.partsDic["HipRight"].go.transform.position);

				body.partsDic["AnkleRight"].lr.SetPosition(0, body.partsDic["AnkleRight"].go.transform.position);
				body.partsDic["AnkleRight"].lr.SetPosition(1, body.partsDic["KneeRight"].go.transform.position);

				body.partsDic["FootRight"].lr.SetPosition(0, body.partsDic["FootRight"].go.transform.position);
				body.partsDic["FootRight"].lr.SetPosition(1, body.partsDic["AnkleRight"].go.transform.position);

				body.partsDic["HipLeft"].lr.SetPosition(0, body.partsDic["HipLeft"].go.transform.position);
				body.partsDic["HipLeft"].lr.SetPosition(1, body.partsDic["SpineBase"].go.transform.position);
				
				body.partsDic["KneeLeft"].lr.SetPosition(0, body.partsDic["KneeLeft"].go.transform.position);
				body.partsDic["KneeLeft"].lr.SetPosition(1, body.partsDic["HipLeft"].go.transform.position);
				
				body.partsDic["AnkleLeft"].lr.SetPosition(0, body.partsDic["AnkleLeft"].go.transform.position);
				body.partsDic["AnkleLeft"].lr.SetPosition(1, body.partsDic["KneeLeft"].go.transform.position);
				
				body.partsDic["FootLeft"].lr.SetPosition(0, body.partsDic["FootLeft"].go.transform.position);
				body.partsDic["FootLeft"].lr.SetPosition(1, body.partsDic["AnkleLeft"].go.transform.position);


				// Joint Rotations
				body.partsDic["WristRight"].go.transform.LookAt(body.partsDic["HandTipRight"].go.transform.position);
				body.partsDic["WristLeft"].go.transform.LookAt(body.partsDic["HandTipLeft"].go.transform.position);
				//Debug.Log (newrot);

				if (Vector3.Distance(body.partsDic["HandTipRight"].go.transform.position, body.partsDic["HandRight"].go.transform.position) < handClosedDist){
					body.handRight.Close();
				}else{
					body.handRight.Open();
				}

				if (Vector3.Distance(body.partsDic["HandTipLeft"].go.transform.position, body.partsDic["HandLeft"].go.transform.position) < handClosedDist){
					body.handLeft.Close();
				}else{
					body.handLeft.Open();
				}

			}
		}

	} 


	//start hopeShoulder
	//checks if a step has been taken
	void checkStep() {
		leftKneeHeight = OnlineBodyView.s.bodies [0].partsDic ["KneeLeft"].go.transform.position.y;
		leftFootHeight = OnlineBodyView.s.bodies [0].partsDic ["FootLeft"].go.transform.position.y;
		leftHipHeight = OnlineBodyView.s.bodies [0].partsDic ["HipLeft"].go.transform.position.y;
		leftAnkleHeight = OnlineBodyView.s.bodies [0].partsDic ["AnkleLeft"].go.transform.position.y;

		rightKneeHeight = OnlineBodyView.s.bodies[0].partsDic["KneeRight"].go.transform.position.y;
		rightFootHeight = OnlineBodyView.s.bodies[0].partsDic["FootRight"].go.transform.position.y;
		rightHipHeight = OnlineBodyView.s.bodies[0].partsDic["HipRight"].go.transform.position.y;
		rightAnkleHeight = OnlineBodyView.s.bodies[0].partsDic["AnkleRight"].go.transform.position.y;

		leftKneePosition = new Vector3(OnlineBodyView.s.bodies [0].partsDic ["KneeLeft"].go.transform.position.x, 
			OnlineBodyView.s.bodies [0].partsDic ["KneeLeft"].go.transform.position.y,
			OnlineBodyView.s.bodies [0].partsDic ["KneeLeft"].go.transform.position.z);
		leftFootPosition = new Vector3(OnlineBodyView.s.bodies [0].partsDic ["FootLeft"].go.transform.position.x,
			OnlineBodyView.s.bodies [0].partsDic ["FootLeft"].go.transform.position.y,
			OnlineBodyView.s.bodies [0].partsDic ["FootLeft"].go.transform.position.z);
		leftHipPosition = new Vector3 (OnlineBodyView.s.bodies [0].partsDic ["HipLeft"].go.transform.position.x,
			OnlineBodyView.s.bodies [0].partsDic ["HipLeft"].go.transform.position.y,
			OnlineBodyView.s.bodies [0].partsDic ["HipLeft"].go.transform.position.z);
		leftAnklePosition = new Vector3(OnlineBodyView.s.bodies [0].partsDic ["AnkleLeft"].go.transform.position.x, 
			OnlineBodyView.s.bodies [0].partsDic ["AnkleLeft"].go.transform.position.y,
			OnlineBodyView.s.bodies [0].partsDic ["AnkleLeft"].go.transform.position.z);

		rightKneePosition = new Vector3(OnlineBodyView.s.bodies[0].partsDic["KneeRight"].go.transform.position.x,
			OnlineBodyView.s.bodies[0].partsDic["KneeRight"].go.transform.position.y,
			OnlineBodyView.s.bodies[0].partsDic["KneeRight"].go.transform.position.z);
		rightFootPosition = new Vector3(OnlineBodyView.s.bodies[0].partsDic["FootRight"].go.transform.position.x,
			OnlineBodyView.s.bodies[0].partsDic["FootRight"].go.transform.position.y,
			OnlineBodyView.s.bodies[0].partsDic["FootRight"].go.transform.position.z);
		rightHipPosition = new Vector3(OnlineBodyView.s.bodies[0].partsDic["HipRight"].go.transform.position.x,
			OnlineBodyView.s.bodies[0].partsDic["HipRight"].go.transform.position.y,
			OnlineBodyView.s.bodies[0].partsDic["HipRight"].go.transform.position.z);
		rightAnklePosition = new Vector3(OnlineBodyView.s.bodies[0].partsDic["AnkleRight"].go.transform.position.x,
			OnlineBodyView.s.bodies[0].partsDic["AnkleRight"].go.transform.position.y,
			OnlineBodyView.s.bodies[0].partsDic["AnkleRight"].go.transform.position.z);

		//if Kinect glitches, do nothing
		if (leftFootHeight > leftHipHeight || rightFootHeight > rightHipHeight
			|| leftFootHeight > leftKneeHeight || rightFootHeight > rightKneeHeight) {
			return;
		}

		//angles in degrees
		angleL = getAngle(leftKneePosition, leftHipPosition, leftAnklePosition);
		angleR = getAngle (rightKneePosition, rightHipPosition, rightAnklePosition);


		//checks to see if step is the first of a sequence of steps
		if ((Time.time >= (stepStartTime + avgStepTime) + 2f) && !stepping && (angleR < 135 || angleL < 135)) {
			sequenceStartTime = Time.time;
		}

		//to determine start and end of a single step
		if (!stepping && (angleR < 135 || angleL < 135)) { //user is starting a step
			lastStepStart = stepStartTime;
			steps++;
			avgCheck = true;
			lastTimeCheck = true;
			noNewStepYet = false;
			stepStartTime = Time.time;
			peakAngle = 180; //to reset peakAngle for every step

			if (angleL < 135) {
				footIsLeft = true;
			} else {
				footIsLeft = false;
			}
		} else if (stepping && (angleR > 140 && angleL > 140)) { //user has ended step
			lastStepEnd = stepEndTime;
			stepEndTime = Time.time;
			firstFootPass = true; //bool that helps check when this foot has been lowered to beyond 165
			thresholdCheck = true; //bool that helps check when the other foot has been raised beyond 165 (gets set to false once we've detected footTimeThreshold)
			noNewStepYet = true;
		}

		//to determine when the first foot crosses 165 (on the way down)
		if (steps > 0 && (angleR >= 165 && angleL >= 165) && firstFootPass) {
			firstFootTime = Time.time;
			firstFootPass = false; //resets the bool
			firstCheck = true; //bool that ensures that we check the other foot's rising beyond 165 only one time
		}

		//checking if the other foot was raised while accounting for noise
		if (footIsLeft && thresholdCheck) { //if left foot is 'firstFoot'
			//need to check whether right foot is raised, while accounting for noise - if it wasn't, set checkOtherFoot to false, else true
			checkOtherFoot = (angleR <  150);

			//record at what time the right foot was actually raised
			if (checkOtherFoot) {
				noiselessRiseTime = Time.time;
				thresholdCheck = false; //resets the bool once we've recorded the needed time
			}

		} else if (!footIsLeft && thresholdCheck) { //if right foot is 'firstFoot'
			//need to check whether left foot is raised, while accounting for noise - if it wasn't, set checkOtherFoot to false, else true
			checkOtherFoot = (angleL < 150);

			//record at what time the left was actually raised
			if (checkOtherFoot) {
				noiselessRiseTime = Time.time;
				thresholdCheck = false;
			}
		}
			
		footTimeThreshold = noiselessRiseTime - firstFootTime; //update footTimeThreshold
			
		//to determine if player is stepping
		if (angleR < 135 || angleL < 135) {
			stepping = true;
		} else if (angleR > 140 && angleL > 140) {
			stepping = false;
		}
			
		if (steps > 1 && lastTimeCheck) { //lastTimeCheck to ensure this is calculated only once
			lastStepTime = stepStartTime - lastStepStart; //calculate last step's time
			rate = 2.5f / lastStepTime;

			if (rate > maxRate) {
				rate = maxRate;
			}

			if (1.3f * rate > 6f) {
				maxRate = 6f;
			} else {
				maxRate = 1.3f * rate;
			}

			lastPeakTime = peakTime - lastStepStart; //calculate last step's time to peak

			lastTimeCheck = false;
		}

		//calculate avg time taken for step, peak
		//should only be executed once every step
		if (steps == 1 && avgCheck) {
			avgStepTime = lastStepTime;
			avgPeakTime = lastPeakTime;
			avgCheck = false;
		} else if (steps > 1 && avgCheck) {
			avgStepTime = (lastStepTime + avgStepTime) / 2;
			avgPeakTime = (lastPeakTime + avgPeakTime) / 2;
			avgCheck = false;
		}
			
		move (); //moves player

		//calculates peak angle and time
		if (stepping) {
			if (angleL < peakAngle) {
				peakAngle = angleL;
				peakTime = Time.time;
			} else if (angleR < peakAngle) {
				peakAngle = angleR;
				peakTime = Time.time;
			}
		}

	}

	//takes a step
	void move() {
		yaw = InputTracking.GetLocalRotation (VRNode.Head).eulerAngles.y;
		rad = yaw * Mathf.Deg2Rad;
		zVal = 0.35f * Mathf.Cos (rad);
		xVal = 0.35f * Mathf.Sin (rad);

		if ((Time.time < stepStartTime + avgStepTime) && (Time.time >= stepStartTime)) {
			velocity = rate + (velocity - rate) * Mathf.Exp ((stepStartTime - Time.time) / 1.6f); //1.6f is an arbitrary constant
		} else if (Time.time > stepEndTime && noNewStepYet) {		
			velocity = (0.5f * rate) + (velocity - (0.5f * rate)) * Mathf.Exp ((stepEndTime - Time.time) / 1.6f);

			if (Time.time >= firstFootTime + footTimeThreshold) { //time has passed
				if (!checkOtherFoot) { //other foot hasn't been raised
					velocity = 0f;
				}
			}
		} else if (Time.time >= stepStartTime + avgStepTime + 0.3f) {
			velocity = 0f;
		}
			
		transform.Translate(-xVal * velocity, 0, -zVal * velocity);

	}

	//takes three positions A, B, C and returns angle between AB and AC
	float getAngle(Vector3 A, Vector3 B, Vector3 C) {
		float theta;

		if (A == B && B == C && C == Vector3.zero) {
			return 0;
		}

		Vector3 AB = new Vector3(B.x - A.x, B.y - A.y, B.z - A.z);
		Vector3 AC = new Vector3(C.x - A.x, C.y - A.y, C.z - A.z);

		float dot = dotProduct(AB, AC);

		try {
			theta = Mathf.Acos(dot / AB.magnitude / AC.magnitude);
		}
		catch (Exception DivideByZeroException) {
			return 0;
		}

		return Mathf.Rad2Deg * theta;

	}

	//returns dot product of two vectors
	float dotProduct(Vector3 A, Vector3 B) {
		return A.x * B.x + A.y * B.y + A.z * B.z;
	}
	//end hopeShoulder

}


public class OnlineBody
{
	public string name;
	public GameObject go;
	public Transform avatar;
	public BodyPart[] parts;
	public Animator anim;


	public Dictionary<string, BodyPart> partsDic;
	
	public HandModel handLeft;
	public HandModel handRight;


	public OnlineBody(string aName)
	{
		parts = new BodyPart[25];
		string bodypartlist = "FootLeft,AnkleLeft,KneeLeft,HipLeft,FootRight,AnkleRight,KneeRight,HipRight,HandTipLeft,ThumbLeft,HandLeft,WristLeft,ElbowLeft,ShoulderLeft,HandTipRight,ThumbRight,HandRight,WristRight,ElbowRight,ShoulderRight,SpineBase,SpineMid,SpineShoulder,Neck,Head";
		string[] bodypartarray;
		bodypartarray = bodypartlist.Split ("," [0]);
		name = aName;
		go = new GameObject ("OnlineBody:" + name);
		go.transform.position = KinectWorld.s.transform.position;
		go.transform.parent = OnlineBodyView.s.transform;

		partsDic = new Dictionary<string, BodyPart> ();


		//height.transform.parent = go.transform;
	
		GameObject height = new GameObject ("Height:" + name);
		OnlineBodyHeight h = height.AddComponent<OnlineBodyHeight> ();
		for (int i = 0; i<bodypartarray.Length; i++) {
			parts[i] = new BodyPart(bodypartarray[i],go,this);
			partsDic.Add (parts[i].name,parts[i]);

			if (parts[i].name == "SpineBase"){
				h.bodyName = name;
				h.transform.position = parts[i].go.transform.position;
				h.spineBase = parts[i].go.transform;

			}
		}
		//OnlineBodyPhysics phys = go.AddComponent<OnlineBodyPhysics> ();
		//phys.onlineBody = this;
		//phys.heightSphere = h;





		//OnlineBodyView.s.maptokinect.AssignBody (name);
	}
}

public class BodyPart
{
	public string name;
	public GameObject go;
	public Transform transform;
	public int infered;
	public Renderer r;
	public LineRenderer lr;
	public OnlineBody parentOnlineBody;

	public BodyPart(string aName, GameObject parentBody, OnlineBody body){
		parentOnlineBody = body;
		name = aName;
		go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		//go.GetComponent<Renderer> ().enabled = false;
		go.name = name;
		go.transform.parent = parentBody.transform;
		go.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
		go.layer = LayerMask.NameToLayer ("PlayerPoints");
		Rigidbody rb = go.AddComponent<Rigidbody> ();
		rb.useGravity = false;
		rb.isKinematic = true;
		go.AddComponent<SphereCollider> ();
		r = go.GetComponent<Renderer> ();
		r.material = ObjectManager.s.coolMat;

		lr = go.AddComponent<LineRenderer> ();
		lr.SetVertexCount (2);
		lr.SetWidth (0.1f, 0.1f);
		//lr.SetColors (Color.black, Color.black);
		lr.material = ObjectManager.s.lineMat;

		if (aName == "HandTipLeft" || aName == "HandTipRight" || aName == "HandLeft" || aName == "HandRight" || aName == "ThumbRight" || aName == "ThumbLeft") {
			r.enabled = false;
		} else if (aName == "WristLeft" || aName == "WristRight" ) {
			r.enabled = false;
			HandModel hand = go.AddComponent<HandModel>();

			if (aName == "WristLeft")
				parentOnlineBody.handLeft = hand;
			if (aName == "WristRight")
				parentOnlineBody.handRight = hand;

		} else {

		}

	}


}