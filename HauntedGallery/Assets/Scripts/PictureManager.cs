using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PictureManager : _Mono {

	float lengthMargin = 2f;
	float heightMarginBottom = 1f;
	float heightMarginTop = 2f;
	float pictureRatio = 1.33333f;
	float maxWidth = 4f;
	int maxRetriesPerSize = 30;
	List<PaintingScript> leftPaintings;
	List<PaintingScript> rightPaintings;
	public GameObject unitPaintingPrefab;
	public GalleryScript gallery;

	// Use this for initialization
	void Start () {
		leftPaintings = new List<PaintingScript>();
		rightPaintings = new List<PaintingScript>();
		GeneratePictures(Globals.LEFT, 12, gallery.transform.position);
		GeneratePictures(Globals.RIGHT, 4, gallery.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Use brute force method to find a place for our picture
	void GeneratePictures(Vector3 direction, int numImages, Vector3 corridorLocation) {
		List<PaintingScript> paintingList = direction.x < 0f ? leftPaintings : rightPaintings;

		for(int i = 0; i < numImages; i++){
			Debug.Log ("Test");
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
			painting.zs = idealWidth;
			painting.ys = idealWidth * pictureRatio;
			painting.angle = direction.x > 0f ? 0f : 180f;
			painting.gallery = gallery;
			paintingList.Add(painting);
		}
	}

	Bounds GenerateProposedBounds(Vector3 direction, float idealWidth, Vector3 corridorLocation){
		Bounds b = new Bounds();
		float ypos = Random.Range(corridorLocation.y - Globals.iHalfHeight + heightMarginBottom + idealWidth * pictureRatio / 2f,
		                          corridorLocation.y + Globals.iHalfHeight - heightMarginTop - idealWidth * pictureRatio / 2f);
		float zpos = Random.Range(corridorLocation.z + lengthMargin + idealWidth / 2f,
		                          corridorLocation.z + Globals.iLength - lengthMargin - idealWidth /2f);
		b.center = new Vector3(direction.x < 0f ? -Globals.iHalfWidth + 0.5f : Globals.iHalfWidth - 0.5f, ypos, zpos);
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
		Debug.Log("Bounds accepted, center: " + proposedBounds.center + " size: " + proposedBounds.size);
		return true;
	}
}
