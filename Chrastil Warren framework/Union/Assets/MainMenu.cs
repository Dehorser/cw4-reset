using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	private static string[] learningScenes = {"Resetting Practice Phase", 
		"CW4 Practice Phase"
	};

	// Use this for initialization
	void Start () {
		Learning ();
	}
	
	// Update is called once per frame
	void Learning() {
		foreach (string s in learningScenes) {
			StartCoroutine(SceneTimer (s, 10f));
		}
	}

	IEnumerator SceneTimer (string sceneName, float seconds)
	{
		yield return new WaitForSeconds (seconds);
		SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
	}
}
	
