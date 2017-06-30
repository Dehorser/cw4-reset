using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class MainMenu : MonoBehaviour {

    public Canvas menuTemplate;
    private Canvas curMenu;

    private Button up;
    private Button down;
    private Button left;
    private Button right;

    private string practice;
    private string test;

    [SerializeField] private VRInput myVRInput;

    Button getButton(string name) {
        return curMenu.transform.Find(name).gameObject.GetComponent<Button>();
    }

	// Use this for initialization
	void Start () {
		curMenu = Instantiate (menuTemplate);

        up = getButton("Up");
        down = getButton("Down");
        left = getButton("Left");
        right = getButton("Right");

        practice = "Practice Phase";
        test = "Test Phase";

        myVRInput = curMenu.gameObject.AddComponent<VRInput>();

        Selection();
	}

    // Sets either practice or test as CW4
    string setCW4(string str)
    {
        return "CW4 " + str;
    }

    string setResetting(string str) {
        return "Resetting " + str;
    }

    // Interprets swipes into actions
	void ChooseMode(VRInput.SwipeDirection swipe) {
        bool invalidSelection = false;

        switch(swipe)
        {
            case VRInput.SwipeDirection.UP:
                practice = setCW4(practice);
                test = setCW4(test);
                break;
            case VRInput.SwipeDirection.DOWN:
                practice = setResetting(practice);
                test = setResetting(test);
                break;
            case VRInput.SwipeDirection.LEFT:
                practice = setCW4(practice);
                test = setResetting(test);
                break;
            case VRInput.SwipeDirection.RIGHT:
                practice = setResetting(practice);
                test = setCW4(test);
                break;
            default:
                invalidSelection = true;
                break;
        }

        if (!invalidSelection) {
            Experiment();
        }
	}

    // Selection screen
    void Selection()
    {
        myVRInput.OnSwipe += ChooseMode;
    }

    // Experiment itself
    void Experiment() {
        // Destroy useless elements just in case
		Destroy (curMenu.gameObject);
        Destroy(myVRInput);

		StartCoroutine(SceneTimer("Resetting Learning Phase", 1, 6));
		StartCoroutine(SceneTimer("CW4 Learning Phase", 8, 6));
		StartCoroutine (SceneTimer (practice, 15, 6));
		StartCoroutine (SceneTimer (test, 22, 6));
	}

    // Handles timing of scenes
	IEnumerator SceneTimer (string sceneName, float startTime, float duration)
	{
        yield return new WaitForSecondsRealtime(startTime);
		SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
        yield return new WaitForSecondsRealtime(duration);
        SceneManager.UnloadScene(sceneName);
	}
}
	
