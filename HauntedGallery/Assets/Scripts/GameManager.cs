using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public int numForks = 3;
	public int level = 0;
	public int totalSeconds;

	private Text knifeText;
	private Text timerText;
	private bool isTimeRunning = true;
	public int levelSeconds = 120;
	public int observationSeconds = 15;
	public int switchInterval = 10;

	private int[] picturesByLevel = {3, 4, 6, 8, 3};
	private int[] possessedByLevel = {3, 3, 3, 3, 3};
	public int secondsToFlash;
	public float flashesPerSecond;
	public Color nonFlashingColor = Color.white;
	public Color flashingColor = Color.red;
	private float flashesTracker; //Keeps track of amount of time passed since last flash.
	private bool flashing = false;
	private GUIText guiText;
	private float secondTracker;
	private bool addedNewPictures;

	// Use this for initialization
	void Start () {
		level = 0;
		knifeText = GameObject.FindGameObjectWithTag("Knives Text").GetComponent<Text>();
		Debug.Log(knifeText);

		timerText = GameObject.FindGameObjectWithTag("Timer Text").GetComponent<Text>();

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		if(!addedNewPictures){
			StartNewLevel();
			addedNewPictures = true;
		}

		/*
		if(Input.GetKeyDown(KeyCode.Space)){
			Globals.developMode = !Globals.developMode;
		}
		
		if(Input.GetKeyDown(KeyCode.N)){
			EndOldLevel();
		}
		
		if(Input.GetKeyDown(KeyCode.M)){
			StartNewLevel();
		}

		
		if(Input.GetKeyDown(KeyCode.P)){
			Debug.Break();
		}
		
		if(Input.GetKeyDown(KeyCode.L)){
			GameObject.FindGameObjectWithTag("DebugLighting").GetComponent<Light>().enabled = 
				!GameObject.FindGameObjectWithTag("DebugLighting").GetComponent<Light>().enabled;
		}
		*/

		knifeText.text = "Knives: " + numForks;

		if(!isTimeRunning && numForks == 3){
			StartNewLevel ();
		}

		if(isTimeRunning){
			secondTracker += Time.deltaTime;
			if(secondTracker >= 1.0f){
				if(totalSeconds > 0){
					totalSeconds -= 1;
					
					//HERE IS WHERE THE HAUNT HAPPENS
					if(totalSeconds < levelSeconds - observationSeconds && totalSeconds % switchInterval == 0){
						//Sometimes don't haunt to reduce predictability
						Debug.Log("callign haunt");
						Globals.pictureManager.Haunt();
					}
				}else{
					Application.LoadLevel("GameOver");
				}
				secondTracker = 0.0f;
			}

			int minutes = totalSeconds / 60;
			int seconds = totalSeconds % 60;
			string result = "Deadline: ";
			if(minutes >= 10){
				result = result + minutes.ToString();
			}else{
				result = result + "0" + minutes.ToString();
			}
			result += ":";
			if(seconds >= 10){
				result = result + seconds.ToString();
			}else{
				result = result + "0" + seconds.ToString();
			}
			timerText.text = result;
			
			//Flashes
			flashesTracker += Time.deltaTime;
			// /2 accounts for the fact that two flashing switches counts as one flash.
			if (flashesTracker >= (1.0f / flashesPerSecond / 2)) {
				flashing = !flashing;
				flashesTracker = 0.0f;
			}

			if (flashing & (totalSeconds <= secondsToFlash)) {
				timerText.color = flashingColor;
			}else{
				timerText.color = nonFlashingColor;
			}
		}
	}
	
	public void EndOldLevel () {
		Debug.Log ("End of level " + level);
		numForks = 0;
		totalSeconds = levelSeconds;
		isTimeRunning = false;
		Globals.pictureManager.RemoveOldPictures();
	}

	public void StartNewLevel () {
		totalSeconds = levelSeconds;
		isTimeRunning = true;
		Debug.Log ("Start of level " + level);
		Globals.pictureManager.AddNewPictures(picturesByLevel[level], possessedByLevel[level]);
		level += 1;
	}
}
