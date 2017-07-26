using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;
using System.IO;
using System;
using UnityEngine.UI;

public class ReOrienterAndTester : MonoBehaviour {

	private List<GameObject> _wayPoints;
	private List<Vector3> _startPositions;

	//this -- used to move the person to the start of a trial
	private GameObject _humanMover; //this, used to reset the subject and allow human movement
	private GameObject _maze; //this, used to turn the features of the maze on and off
	private GameObject _voronoi; //this, used to turn the features of the vornoi ground plane on and off
	private GameObject _textMessage;

	private int _index = 0;
	private int _index2 = 0;
	private int _trial = 0;

	public Text _stringMessage;

	// Use this for initialization
	void Start () {
		UnityEngine.Random.InitState(0);

		_wayPoints = new List<GameObject> (8);
		_wayPoints.Add (GameObject.Find ("Phonebooth"));
		_wayPoints.Add (GameObject.Find ("Chair and Table"));
		_wayPoints.Add (GameObject.Find ("Guitar"));
		_wayPoints.Add (GameObject.Find ("Snowman"));
		_wayPoints.Add (GameObject.Find ("Car"));
		_wayPoints.Add (GameObject.Find ("Well"));
		_wayPoints.Add (GameObject.Find ("Treasure Chest"));
		_wayPoints.Add (GameObject.Find ("Clock"));

		_startPositions = new List<Vector3> (8);
		_startPositions.Add (new Vector3 (-4.079f, 0f, 4.14f));
		_startPositions.Add (new Vector3 (-.85f, 0f, -4.117f));
		_startPositions.Add (new Vector3 (3.918f, 0f, -.673f));
		_startPositions.Add (new Vector3 (-4.11f, 0f, .09f));
		_startPositions.Add (new Vector3 (-3.981f, 0f, -1.406f));
		_startPositions.Add (new Vector3 (3.373f, 0f, 1.817f));
		_startPositions.Add (new Vector3 (3.516f,0f, -4.344f));
		_startPositions.Add (new Vector3 (.016f, 0f, 2.724f));

		_humanMover = GameObject.Find ("TestObject");
		_maze = GameObject.Find ("Maze");
		_voronoi = GameObject.Find ("Voronoi");
		_textMessage = GameObject.Find ("Canvas");
		_voronoi.SetActive (false);
		_textMessage.SetActive (false);
	}

	//public const int TRAINING = 0;
	public const int PRETRIAL = 1;
	public const int INTRIAL = 2;
	public const int POSTTRIAL = 3;
	private int state = PRETRIAL;
	private float lastButtonPress = 0;
//	private int skipCount = 0;
//	private float skipStart = 0;
//	private float lastPress = 0;

	// Update is called once per frame
	void Update () {
//		if (state == TRAINING) {
//			if (Time.fixedTime > 600) {
//				//Turn maze off
//				_maze.SetActive (false);
//				//Turn voronoi on
//				_voronoi.SetActive (true);
//				_textMessage.SetActive (true);
//				lastButtonPress = Time.fixedTime;
//				state = POSTTRIAL;
//			}
//			if (Time.fixedTime > skipStart + 5 && Input.GetMouseButton(0) && Time.fixedTime > lastPress + .25f) {
//				skipStart = Time.fixedTime;
//				skipCount = 1;
//				lastPress = Time.fixedTime;
//			}
//			else if (Input.GetMouseButton(0) && Time.fixedTime > lastPress + .25f) {
//				skipCount++;
//				lastPress = Time.fixedTime;
//				if (skipCount > 5)
//				{
//					transform.position = new Vector3(0, 0, -1.658f);
//				}
//				if (skipCount > 8)
//				{
//					//Turn maze off
//					_maze.SetActive(false);
//					//Turn voronoi on
//					_voronoi.SetActive(true);
//					_textMessage.SetActive(true);
//					lastButtonPress = Time.fixedTime;
//					state = POSTTRIAL;
//				}
//			}
//		} else 
		if (state == PRETRIAL) {
			if (Input.GetMouseButton (0) && Time.fixedTime > lastButtonPress + 1) {
				if (Mathf.Abs (_humanMover.transform.position.x - _wayPoints [_index].transform.position.x) < 1.2) {
					if (Mathf.Abs (_humanMover.transform.position.z - _wayPoints [_index].transform.position.z) < 1.2) {
						lastButtonPress = Time.fixedTime;
						state = INTRIAL;
						StartTrial ();
					}
				}
			}
		} else if (state == INTRIAL) {
			if (Input.GetMouseButton (0) && Time.fixedTime > lastButtonPress + 1) {
				lastButtonPress = Time.fixedTime;
				state = POSTTRIAL;
				UserEndTrial ();
				_trial++;
			}
			if (Time.fixedTime > lastButtonPress + 3) {
				_textMessage.SetActive (false);
			}
		} else if (state == POSTTRIAL) {
			if (Input.GetMouseButton (0) && Time.fixedTime > lastButtonPress + 1) {
				lastButtonPress = Time.fixedTime;
				state = PRETRIAL;
				ResetPerson ();
			}
		}
	}

	void ResetPerson() {
		//Generate random start
		_index = UnityEngine.Random.Range(0,7);
		//Generate random end
		do {
			_index2 = UnityEngine.Random.Range (0, 7);
		} while (_index2 == _index);
		_stringMessage.text = "Please go to " + _wayPoints [_index2].name;
		//Place this at corresponding waypoint
		transform.position = _startPositions[_index];
		//Place test object at (0, eyeHeight, 0)
		float eyeHeight = _humanMover.transform.localPosition.y;
		_humanMover.transform.localPosition = new Vector3(0f, eyeHeight, 0f);
		//Turn maze on
		_maze.SetActive(true);
		//Turn voronoi off
		_voronoi.SetActive(false);
		_textMessage.SetActive (false);
		//Record time, time, start, end, position, orientation
		LogData("PreTrialOrientation");
	}

	void StartTrial() {
		//Turn maze off
		_maze.SetActive(false);
		//Turn voronoi on
		_voronoi.SetActive(true);
		_textMessage.SetActive (true);
		//Give user instruction

		//Record time, time, start, end, position, orientation
		LogData("BeginTrial");
	}

	void UserEndTrial() {
		//Record time, time, start, end, position, orientation
		_textMessage.SetActive(true);
		_stringMessage.text = "Good Job";
		if (_trial > 48) {
			_stringMessage.text = "Testing Complete";
		}
		LogData("EndTrial");
	}

	void LogData(string action) {
		string path = Application.persistentDataPath + "/CW4Summary_Data.txt";

		string appendText = "\n" + DateTime.Now.ToString() + "\t" + 
			Time.time + "\t" + 

			_trial + "\t" + action + "\t" +

			_index.ToString() + "\t" + _index2.ToString() + "\t" +

			_humanMover.transform.position.x + "\t" + 
			_humanMover.transform.position.y + "\t" + 
			_humanMover.transform.position.z + "\t" +

			InputTracking.GetLocalRotation (VRNode.Head).eulerAngles.x + "\t" +
			InputTracking.GetLocalRotation (VRNode.Head).eulerAngles.y + "\t" +
			InputTracking.GetLocalRotation (VRNode.Head).eulerAngles.z;


		File.AppendAllText(path, appendText);
		Debug.Log(appendText);
	}
}
