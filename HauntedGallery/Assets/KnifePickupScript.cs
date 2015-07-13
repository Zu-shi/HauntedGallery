using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KnifePickupScript : _Mono {
	
	private Canvas ui;
	private Text text;
	private GameObject player;
	private GameObject light;

	// Use this for initialization
	void Start () {	
		ui = GameObject.FindGameObjectWithTag("UI Hint").GetComponent<Canvas>();
		player = GameObject.FindGameObjectWithTag("Player");
		text = GameObject.FindGameObjectWithTag("Mouse Text").GetComponent<Text>();
		light = player.GetComponentInChildren<Light>().gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		int paintingMask = 1 << LayerMask.NameToLayer("Knife");
		RaycastHit hit;
		if(Physics.Raycast(light.transform.position, light.transform.forward, out hit, 10, paintingMask)){
			ui.enabled = true;
			Debug.Log ("Collide");
			if(hit.collider.gameObject.Equals(gameObject)){
				Debug.Log ("Collide2");
				if(Input.GetMouseButtonDown(0)){
					Globals.gameManager.numForks += 1;
					Destroy(gameObject);
				}
			}
			text.text = "Pick up";
		}else{
			ui.enabled = false;
		}
	}
}
