using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Canvas menuTemplate;
	private Canvas curMenu;

	private Text prompt;
	private Button resetting;
	private Button CW4;

	private string practice;
	private string test;

	// Use this for initialization
	void Start () {
		InitMenu ();
        
    }

	void InitMenu() {
		curMenu = Instantiate (menuTemplate);

		prompt = curMenu.transform.Find ("Prompt").gameObject.GetComponent<Text>();

		resetting = curMenu.transform.Find ("Resetting").gameObject.GetComponent<Button>();

		CW4 = curMenu.transform.Find ("CW4").gameObject.GetComponent<Button>();

		ChoosePractice ();
	}

	void ChoosePractice() {
		prompt.text = "Choose Practice";

		resetting.onClick.AddListener(() => {
			practice = "Resetting Practice Phase";
			ChooseTest();
		});

		CW4.onClick.AddListener (() => {
			practice = "CW4 Practice Phase";
			ChooseTest();
		});
	}

	void ChooseTest() {
		resetting.onClick.RemoveAllListeners ();
		CW4.onClick.RemoveAllListeners ();

		prompt.text = "Choose Test";

		resetting.onClick.AddListener(() => {
			test = "Resetting Test Phase";
			StartExperiment();
		});

		CW4.onClick.AddListener (() => {
			test = "CW4 Test Phase";
			StartExperiment();
		});
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
	
