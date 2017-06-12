using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using System;

public class hopeShoulder : MonoBehaviour {
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

	private float angleL;
	private float angleR;
	private float angleThreshold = 135;
	public float ERROR = 43;

	private float yaw;
	private float rad;
	private float xVal;
	private float zVal;


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

		angleL = getAngle(leftKneePosition, leftHipPosition, leftAnklePosition);
		angleR = getAngle (rightKneePosition, rightHipPosition, rightAnklePosition);

		if (angleR < angleThreshold || angleL < angleThreshold) {
			move ();
		}
	}

	//takes a step
	void move() {
		yaw = InputTracking.GetLocalRotation(VRNode.Head).eulerAngles.y;
		rad = yaw * Mathf.Deg2Rad;
		zVal = 0.35f * Mathf.Cos(rad);
		xVal = 0.35f * Mathf.Sin(rad);

		transform.Translate(xVal, 0, zVal);
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
}