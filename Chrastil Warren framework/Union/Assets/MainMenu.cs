using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(SceneTimer("Resetting Learning Phase", 2, 5));
        StartCoroutine(SceneTimer("CW4 Learning Phase", 9, 5));
    }
	

	IEnumerator SceneTimer (string sceneName, float startTime, float duration)
	{
        yield return new WaitForSecondsRealtime(startTime);
		SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
        yield return new WaitForSecondsRealtime(duration);
        SceneManager.UnloadScene(sceneName);
	}
}
	
