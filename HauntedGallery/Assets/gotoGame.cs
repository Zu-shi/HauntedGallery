using UnityEngine;
using System.Collections;

public class gotoGame : MonoBehaviour {

	float timer = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer > 6f){
			Application.LoadLevel("MainGame");
		}
	}
}
