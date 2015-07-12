using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PaintingScript : _Mono {

	public GalleryScript gallery;
	private GameObject player;
	private GameObject light;
	private MeshRenderer mr;
	private Text text;
	private Canvas ui;
	bool hasFork = false;
	public GameObject forkPrefab;
	private GameObject fork;

	// Use this for initialization
	void Start () {
		mr = GetComponentInChildren<MeshRenderer>();
		player = GameObject.FindGameObjectWithTag("Player");
		light = player.GetComponentInChildren<Light>().gameObject;
		ui = GameObject.FindGameObjectWithTag("UI Hint").GetComponent<Canvas>();
		text = GameObject.FindGameObjectWithTag("Mouse Text").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		int paintingMask = 1 << LayerMask.NameToLayer("Painting");
		RaycastHit hit;
		if(Physics.Raycast(light.transform.position, light.transform.forward, out hit, 8, paintingMask)){
			ui.enabled = true;
			if(hit.collider.gameObject.Equals(gameObject)){
				if(Input.GetMouseButtonDown(0)){
					if(!hasFork && Globals.gameManager.numForks > 0){
						fork = Instantiate(forkPrefab);
						_Mono fm = fork.AddComponent<_Mono>();
						fm.x = x > 0? x - 0.3f : x + 0.3f;
						fm.y = y;
						fm.z = z;
						fm.angle = x > 0? -180f : 0f;
						hasFork = true;
						Globals.gameManager.numForks -= 1;
					}else if(hasFork){
						hasFork = false;
						Globals.gameManager.numForks += 1;
						Destroy(fork);
					}
				}

				if(hasFork){
					text.text = "Unseal";
				}else{
					text.text = "Seal";
				}
			}
		}else{
			ui.enabled = false;
		}
	}

	void OnGUI(){

		//Globals.GUIDrawRect(new Rect(0.1f, 0.1f, 100f, 100f), Color.white);

		if(Globals.developMode){

			Color c = new Color(1f, 1f, 1f, 0.5f);
			if(mr.isVisible){
				c = new Color(0f, 0f, 1f, 0.5f);
			}
			int paintingMask = 1 << LayerMask.NameToLayer("Painting");
			RaycastHit hit;
			if(Physics.Raycast(light.transform.position, light.transform.forward, out hit, 8, paintingMask)){
				if(hit.collider.gameObject.Equals(gameObject)){
					c = new Color(0f, 1f, 0f, 0.5f);
				}
			}

			float guiLocationX, guiLocationY;
			if(x < 0f){
				guiLocationX = z - gallery.z;
			}else{
				float test = gallery.z;
				guiLocationX = Globals.iLength - (z - gallery.z) + Globals.iLength + Globals.iWidth;
			}
			guiLocationY = y;

			Rect r = new Rect(guiLocationX - zs/2f, guiLocationY - ys/2f, zs, ys);

			Globals.GUIDrawRectPreScale(r, c);
		}

	}
}
