using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PictureManager : _Mono {

	float lengthMargin = 2f;
	float heightMarginBottom = 1f;
	float heightMarginTop = 2f;
	float pictureRatio = 1.33333f;
	float maxWidth = 4f;
	int maxRetriesPerSize = 30;
	List<PaintingScript> leftPaintings = new List<PaintingScript>();
	List<PaintingScript> rightPaintings = new List<PaintingScript>();
	List<PaintingScript> possessedPaintings = new List<PaintingScript>();
	List<int> usedNumbers = new List<int>();

	//public Texture2D[] allTextures;
	public Object[] allTextures;
	public List<Texture> unusedTextures = new List<Texture>();

	public GameObject unitPaintingPrefab;
	public GalleryScript gallery;

	// Use this for initialization
	void Start () {
		allTextures = Resources.LoadAll("Portraits", typeof(Texture));
		Debug.Log ("AllTextures length" + allTextures.Length);
		if(allTextures.Length == 0){
			Debug.Break();
		}

		foreach(Object t in allTextures){
			unusedTextures.Add((Texture)t);
		}
		Debug.Log ("unusedTextures length" + unusedTextures.Count);
		if(unusedTextures.Count == 0){
			Debug.Break();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(possessedPaintings.Count != 0){
			bool levelEnding = true;
			foreach(PaintingScript pos in possessedPaintings){
				if(!pos.hasFork){
					levelEnding = false;
				}
			}

			if(levelEnding){
				Globals.gameManager.EndOldLevel();
			}
		}
	}

	//Use brute force method to find a place for our picture
	void GeneratePictures(Vector3 direction, int numImages, Vector3 corridorLocation) {
		List<PaintingScript> paintingList = direction.x < 0f ? leftPaintings : rightPaintings;

		for(int i = 0; i < numImages; i++){
//			Debug.Log ("Test");
			float idealWidth = maxWidth;
			Bounds proposedBounds = GenerateProposedBounds(direction, idealWidth, corridorLocation);
			int retries = 0;
			while( !IsAcceptableBounds(direction, proposedBounds) ){
				retries++;
				if(retries == maxRetriesPerSize){
					retries = 0;
					idealWidth -= 0.5f;
					if(idealWidth < 0.9f){
						Debug.LogError("cannot fit painting anywhere, most likely an error has occured");
						Debug.Break();
					}
				}
				proposedBounds = GenerateProposedBounds(direction, idealWidth, corridorLocation);
			}

			GameObject paintingGo = Instantiate(unitPaintingPrefab);
			PaintingScript painting = paintingGo.GetComponent<PaintingScript>();
			painting.xyz = proposedBounds.center;

			int index = Random.Range(0, unusedTextures.Count);
			Debug.Log (unusedTextures.Count + ":" + index);
			Texture selectedTexture = unusedTextures[index];
			unusedTextures.RemoveAt(index);
/*
			if(selectedTexture.height / selectedTexture.width > pictureRatio){
				painting.ys = idealWidth * pictureRatio;
				painting.zs = painting.ys * (selectedTexture.width / selectedTexture.height);
			}else{
				painting.zs = idealWidth;
				painting.ys = idealWidth / (selectedTexture.width / selectedTexture.height);
			}

*/
			painting.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material.mainTexture = selectedTexture;
			painting.zs = idealWidth;
			painting.ys = idealWidth * pictureRatio;
			//painting.angle = direction.x > 0f ? 0f : 180f;
			painting.xs = direction.x > 0f? painting.xs : -painting.xs;
			//painting.ys = direction.x > 0f? -painting.ys : painting.ys;
			//fm.transform.rotation = x > 0? Quaternion.Euler(new Vector3(0f, 0f, 180f)) : Quaternion.identity;
			painting.gallery = gallery;
			painting.Emerge();
			paintingList.Add(painting);
		}
	}

	Bounds GenerateProposedBounds(Vector3 direction, float idealWidth, Vector3 corridorLocation){
		Bounds b = new Bounds();
		float ypos = Random.Range(corridorLocation.y - Globals.iHalfHeight + heightMarginBottom + idealWidth * pictureRatio / 2f,
		                          corridorLocation.y + Globals.iHalfHeight - heightMarginTop - idealWidth * pictureRatio / 2f);
		float zpos = Random.Range(corridorLocation.z + lengthMargin + idealWidth / 2f,
		                          corridorLocation.z + Globals.iLength - lengthMargin - idealWidth /2f);
		b.center = new Vector3(direction.x < 0f ? -Globals.iHalfWidth - 0.01f : Globals.iHalfWidth + 0.01f, ypos, zpos);
		b.size = new Vector3(8f, idealWidth * pictureRatio, idealWidth);
		return b;
	}

	bool IsAcceptableBounds(Vector3 direction, Bounds proposedBounds){
		List<PaintingScript> paintingList = direction.x < 0f ? leftPaintings : rightPaintings;
		foreach(PaintingScript p in paintingList){
			if (proposedBounds.Intersects(p.gameObject.GetComponent<BoxCollider>().bounds)){
				return false;
			}
		}
//		Debug.Log("Bounds accepted, center: " + proposedBounds.center + " size: " + proposedBounds.size);
		return true;
	}

	public void AddNewPictures(int numPics, int numPossessed){
		GeneratePictures(Globals.LEFT, numPics, gallery.transform.position);
		GeneratePictures(Globals.RIGHT, numPics, gallery.transform.position);

		if(Globals.gameManager.level < 4){
			for(int i = 0; i < numPossessed; i++){
				Debug.Log (i);
				int s = 0;
				while(true){
					s = Random.Range(0, numPics * 2);
					bool selected = false;
					foreach(int i2 in usedNumbers){
						if(i2 == s){
							selected = true;
						}
					}
					if(!selected){
						usedNumbers.Add (s);
						break;
					}
				}

				if(s < numPics){
					Debug.Log ("s is " + s);
					possessedPaintings.Add(leftPaintings[s]);
					leftPaintings[s].possessed = true;
				}else{
					Debug.Log ("s is " + s);
					possessedPaintings.Add(rightPaintings[s - numPics]);
					rightPaintings[s - numPics].possessed = true;
				}
			}
		}else{
			allTextures = Resources.LoadAll("Final", typeof(Texture));
			unusedTextures.Clear();
			foreach(Object t in allTextures){
				unusedTextures.Add((Texture)t);
			}
			GeneratePictures(Globals.LEFT, numPics, gallery.transform.position);
			GeneratePictures(Globals.RIGHT, numPics, gallery.transform.position);
		}
	}
	
	public void RemoveOldPictures(){
		foreach(PaintingScript ps in leftPaintings){
			ps.Dispose();
		}
		leftPaintings.Clear();
		foreach(PaintingScript ps in rightPaintings){
			ps.Dispose();
		}
		rightPaintings.Clear();
		possessedPaintings.Clear();
		unusedTextures.Clear();
		foreach(Object t in allTextures){
			unusedTextures.Add((Texture)t);
		}
	}

	public void Haunt(){
		foreach(PaintingScript pos in possessedPaintings){
			Debug.Log ("calling individual haunt");
			if(pos.Haunt()){break;}
		}
	}

	public void SwitchTexture(PaintingScript ps){
		Debug.Log ("texture switched");
		if(unusedTextures.Count > 0){
			int index = Random.Range(0, unusedTextures.Count);
			//Debug.Log (unusedTextures.Count + ":" + index);
			Texture selectedTexture = unusedTextures[index];
			unusedTextures.RemoveAt(index);
			ps.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material.mainTexture = selectedTexture;
		}
	}
}
