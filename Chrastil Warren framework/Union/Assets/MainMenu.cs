using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class MainMenu : MonoBehaviour {

	public Canvas menuTemplate;
	private Canvas curMenu;

	private Text prompt;
	private Button resetting;
	private Button CW4;

	private string practice;
	private string test;

	private VRInput myVRInput;

	// Use this for initialization
	void Start () {
		InitMenu ();
    }

	void InitMenu() {
		curMenu = Instantiate (menuTemplate);

		prompt = curMenu.transform.Find ("Prompt").gameObject.GetComponent<Text>();

		resetting = curMenu.transform.Find ("Resetting").gameObject.GetComponent<Button>();

		CW4 = curMenu.transform.Find ("CW4").gameObject.GetComponent<Button>();

		myVRInput = new VRInput();

		ChoosePractice ();
	}

	void SetPractice(string sceneName) {
		practice = sceneName;
		ChooseTest();
	}

	void SetPracticeSwipe(VRInput.SwipeDirection swipe) {
		if (swipe == VRInput.SwipeDirection.UP) {
			SetPractice ("CW4 Practice Phase");
		} else if (swipe == VRInput.SwipeDirection.DOWN) {
			SetPractice ("Resetting Practice Phase");
		}
	}

	void ChoosePractice() {
		prompt.text = "Choose Practice";

		resetting.onClick.AddListener(()=>{ SetPractice ("Resetting Practice Phase"); });

		myVRInput.OnSwipe += SetPracticeSwipe;

		CW4.onClick.AddListener (() => {SetPractice ("CW4 Practice Phase"); });
	}

	void SetTest(string sceneName) {
		test = sceneName;
		StartExperiment();
	}

	void SetTestSwipe(VRInput.SwipeDirection swipe) {
		if (swipe == VRInput.SwipeDirection.UP) {
			SetTest ("CW4 Test Phase");
		} else if (swipe == VRInput.SwipeDirection.DOWN) {
			SetTest ("Resetting Test Phase");
		}
	}

	void ChooseTest() {
		resetting.onClick.RemoveAllListeners ();
		CW4.onClick.RemoveAllListeners ();
		myVRInput.OnSwipe -= SetPracticeSwipe;

		prompt.text = "Choose Test";

		resetting.onClick.AddListener(() => {SetTest("Resetting Test Phase");} );

		CW4.onClick.AddListener (() => {SetTest("CW4 Test Phase");} );

		myVRInput.OnSwipe += SetTestSwipe;
	}

	void StartExperiment() {
		Destroy (curMenu.gameObject);
		StartCoroutine(SceneTimer("Resetting Learning Phase", 2, 5));
		StartCoroutine(SceneTimer("CW4 Learning Phase", 9, 5));
		StartCoroutine (SceneTimer (practice, 16, 5));
		StartCoroutine (SceneTimer (test, 23, 5));
	}

	IEnumerator SceneTimer (string sceneName, float startTime, float duration)
	{
        yield return new WaitForSecondsRealtime(startTime);
		SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
        yield return new WaitForSecondsRealtime(duration);
        SceneManager.UnloadScene(sceneName);
	}
}
	
