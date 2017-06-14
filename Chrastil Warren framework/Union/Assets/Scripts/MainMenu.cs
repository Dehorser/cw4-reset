using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SceneManager.LoadSceneAsync ("Resetting Learning Phase");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
