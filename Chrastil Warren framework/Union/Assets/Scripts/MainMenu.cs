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

	private string practice1, practice2;
    private string learning;
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

		practice1 = practice2 = "Practice Phase";

		learning = "Learning Phase";
        test = "Learning Phase";

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
                learning = setCW4(learning);
                test = setCW4(test);
				practice1 = setCW4(practice1);
				practice2 = setCW4(practice2);
                break;
            case VRInput.SwipeDirection.DOWN:
                learning = setResetting(learning);
                test = setResetting(test);
				practice1 = setResetting(practice1);
				practice2  = setResetting(practice2 );
                break;
            case VRInput.SwipeDirection.LEFT:
                learning = setCW4(learning);
                test = setResetting(test);
				practice1 = setCW4(practice1);
				practice2  = setResetting(practice2 );
                break;
            case VRInput.SwipeDirection.RIGHT:
                learning = setResetting(learning);
                test = setCW4(test);
				practice1 = setResetting(practice1);
				practice2  = setCW4(practice2 );
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

		StartCoroutine(SceneTimer(practice1, 0, 30));
		StartCoroutine(SceneTimer(practice2, 30, 30));
		StartCoroutine (SceneTimer (learning, 60, 60));
		StartCoroutine (SceneTimer (test, 120, 360));
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
	
