using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PaintingScript : _Mono {

	public GalleryScript gallery;
	private GameObject player;
	private GameObject light;
	private MeshRenderer mr;
	private Text text;
	private Canvas ui;
	public bool hasFork = false;
	public bool possessed = false;
	public int changedTimes = 0;
	public GameObject forkPrefab;
	public GameObject forkPickupPrefab;
	private GameObject fork;
	private bool fading = false;

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
		//GameObject forkPickup = (GameObject)Instantiate(forkPickupPrefab, Vector3.zero, Quaternion.identity);

		int paintingMask = 1 << LayerMask.NameToLayer("Painting");
		RaycastHit hit;
		if(Physics.Raycast(light.transform.position, light.transform.forward, out hit, 8, paintingMask)){
			ui.enabled = true;
			if(hit.collider.gameObject.Equals(gameObject)){
				if(Input.GetMouseButtonDown(0) && !fading){
					if(!hasFork && Globals.gameManager.numForks > 0){
						fork = Instantiate(forkPrefab);
						_Mono fm = fork.AddComponent<_Mono>();
						fm.x = x > 0? x - 0.3f : x + 0.3f;
						fm.y = y;
						fm.z = z;
						fm.xs = x > 0? -fm.xs : -fm.xs;
						fm.angle = x > 0? 0f : 180f;
						//fm.transform.rotation = x > 0? Quaternion.Euler(new Vector3(0f, 0f, 180f)) : Quaternion.identity;
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

			string s = "";
			if(possessed){
				s += "Pos" + changedTimes; 
				if(hasFork){
					s+= "Forked";
				}
			}

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

			Globals.GUIDrawRectPreScale(r, c, s);
		}

	}

	public void Dispose(){
		if(hasFork){
			GameObject forkPickup = (GameObject)Instantiate(forkPickupPrefab, fork.transform.position, fork.transform.rotation);
			//Debug.Break();
			_Mono forkPickupMono = forkPickup.AddComponent<_Mono>();
			forkPickupMono.xyzs = fork.GetComponent<_Mono>().xyzs;
			//forkPickupMono.x = 0f;
			Destroy(fork);
		}

		fading = true;
		Sequence s = DOTween.Sequence();
		//DOTween.To( ()=> y, x => y = x, -10f, 1f );
		foreach(MeshRenderer cmr in GetComponentsInChildren<MeshRenderer>()){
			cmr.material.DOFade(0f, 1.5f);
		}
		//DOTween.To( ()=> y, x => y = x, 0f, 1f );
		s.AppendInterval(1.5f);
		s.AppendCallback(Destruct);
	}

	public void Emerge(){
		Sequence s = DOTween.Sequence();
		foreach(MeshRenderer cmr in GetComponentsInChildren<MeshRenderer>()){
			Color c = cmr.material.color;
			c.a = 0f;
			cmr.material.color = c;
			cmr.material.DOFade(1f, 1.5f);
			//Debug.Log ("test");
		}
		s.AppendInterval(1.5f);
	}

	public void Destruct(){
		ui.enabled = false;
		Destroy(gameObject);
	}

	public bool Haunt(){
		if(!mr.isVisible && !hasFork){
			if(Globals.gameManager.totalSeconds >= 60 && Random.Range(0,1) < 0.7f){SwitchPicture(); return true;}
			else if(Globals.gameManager.totalSeconds < 60){SwitchPicture(); return true;}
		}
		return false;
	} 

	void SwitchPicture(){
		Debug.Log("Called switchPicture");
		Globals.pictureManager.SwitchTexture(this);
		changedTimes++;
	}
}
