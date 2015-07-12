using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public int numForks = 3;
	public int level = 0;
	public int startingLevel = 1;

	private Text knifeText;

	// Use this for initialization
	void Start () {
		knifeText = GameObject.FindGameObjectWithTag("Knives Text").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			Globals.developMode = !Globals.developMode;
		}

		knifeText.text = "Knives: " + numForks;
	}

	void StartNewLevel () {

	}
}
