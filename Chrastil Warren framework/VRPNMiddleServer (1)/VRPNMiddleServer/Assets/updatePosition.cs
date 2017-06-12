using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class updatePosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		Vector3 pos = VRPN.vrpnTrackerPos ("mainHMD@129.59.70.136", 0);
//		Quaternion quat = VRPN.vrpnTrackerQuat ("mainHMD@129.59.70.136", 0);
//		Vector3 newPos = new Vector3 ();
//		newPos [0] = pos [0] / 1000f;
//		newPos [1] = pos [1] / 1000f;
//		newPos [2] = pos [2] / 1000f;
//		transform.position = newPos;
	}

	void OnGUI(){
//		GUI.Label(new Rect (0, 0, 100, 100), transform.position.ToString());
	}
}
