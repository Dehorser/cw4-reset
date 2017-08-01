﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class MainMenu : MonoBehaviour {

    public Canvas menuTemplate;
    private Canvas curMenu;
	private GameObject canvasCamera;

//    private Button up;
//    private Button down;
//    private Button left;
//    private Button right;

    [SerializeField] private VRInput myVRInput;

	private Phases myPhases;

	// Use this for initialization
	void Start () {
		curMenu = Instantiate (menuTemplate);
		canvasCamera = GameObject.Find ("Camera");

//        up = getButton("Up");
//        down = getButton("Down");
//        left = getButton("Left");
//        right = getButton("Right");

		myPhases = new Phases();

        myVRInput = curMenu.gameObject.AddComponent<VRInput>();

        Selection();
	}

	Button getButton(string name) {
		return curMenu.transform.Find(name).gameObject.GetComponent<Button>();
	}

    // Interprets swipes into actions
	void ChooseMode(VRInput.SwipeDirection swipe) {
        bool invalidSelection = true;

        switch(swipe)
        {
            case VRInput.SwipeDirection.UP:
				invalidSelection = false;
				break;
            case VRInput.SwipeDirection.DOWN:
				invalidSelection = false;
                break;
            case VRInput.SwipeDirection.LEFT:
				invalidSelection = false;
                break;
            case VRInput.SwipeDirection.RIGHT:
				invalidSelection = false;
                break;
            default:
                break;
        }

		if (!invalidSelection) {
			myVRInput.OnSwipe -= ChooseMode;
			//Experiment();
        }
	}

    // Selection screen
    void Selection()
    {
        myVRInput.OnSwipe += ChooseMode;
		//myVRInput.OnDoubleClick += SkipCurrentTask;
    }

//	#region Experimental Procedure
//
//    // Experiment itself
//	private Coroutine prac1TaskCoroutine;
//	private Coroutine prac2TaskCoroutine;
//	private Coroutine learnTaskCoroutine;
//	private Coroutine testTaskCoroutine;
//	private int currentTask = -1;
//    void Experiment() {
//        // Destroy useless elements just in case
//		canvasCamera.SetActive(false);
//		//Destroy (curMenu.gameObject);
//        //Destroy(myVRInput);
//
//
//		prac1TaskCoroutine = StartCoroutine(SceneTimer(practice1, 0, 0, practice1Time));
//		prac2TaskCoroutine = StartCoroutine(SceneTimer(practice2, 1, practice1Time, practice2Time));
//		learnTaskCoroutine = StartCoroutine (SceneTimer (learning, 2, practice1Time + practice2Time, learningTime));
//		testTaskCoroutine = StartCoroutine (SceneTimer (test, 3, practice1Time + practice2Time + learningTime, 
//			testTime));
//	}
//
//    // Handles timing of scenes
//	IEnumerator SceneTimer (string sceneName, int order, float startTime, float duration)
//	{
//        yield return new WaitForSecondsRealtime(startTime);
//		currentTask = order;
//		SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
//        yield return new WaitForSecondsRealtime(duration);
//        SceneManager.UnloadSceneAsync(sceneName);
//	}
//
//	void SkipCurrentTask()
//	{
//		//canvasCamera.SetActive (true);
//		if (prac1TaskCoroutine != null) StopCoroutine (prac1TaskCoroutine);
//		if (prac2TaskCoroutine != null) StopCoroutine (prac2TaskCoroutine);
//		if (learnTaskCoroutine != null) StopCoroutine (learnTaskCoroutine);
//		if (testTaskCoroutine != null) StopCoroutine (testTaskCoroutine);
//		switch(currentTask)
//		{
//		case 0:
//			SceneManager.UnloadSceneAsync (practice1);
//			prac2TaskCoroutine = StartCoroutine (SceneTimer (practice2, 1, 0, practice2Time));
//			learnTaskCoroutine = StartCoroutine (SceneTimer (learning, 2, practice2Time, learningTime));
//			testTaskCoroutine = StartCoroutine (SceneTimer (test, 3, practice2Time + learningTime, testTime));
//			break;
//		case 1:
//			SceneManager.UnloadSceneAsync(practice2);
//			learnTaskCoroutine = StartCoroutine (SceneTimer (learning, 2, 0, learningTime));
//			testTaskCoroutine = StartCoroutine (SceneTimer (test, 3, learningTime, testTime));
//			break;
//		case 2:
//			SceneManager.UnloadSceneAsync(learning);
//			testTaskCoroutine = StartCoroutine (SceneTimer (test, 3, 0, testTime));
//			break;
//		}
//	}

	//#endregion
}
	
