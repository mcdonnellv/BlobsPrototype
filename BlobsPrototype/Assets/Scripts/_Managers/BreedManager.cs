using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

public class BreedManager : MonoBehaviour {

	HudManager hudManager;
	GameManager2 gameManager;
	RoomManager roomManager;

	Blob potentialPairBlob1;
	Blob potentialPairBlob2;

	public int GetBreedCost() {
		return 0;
	}


	public void AskBlobsInteract(Blob blob1, Blob blob2) {

		if(!blob2.hasHatched || !blob1.hasHatched)
			return;

		if(blob1.actionDuration.TotalSeconds > 0 || blob2.actionDuration.TotalSeconds > 0)
			return;

		AttemptBreed(blob1, blob2);
	}



	public void AttemptBreed(Blob blob1, Blob blob2) {
		int cost = GetBreedCost();

		if(CheckBreedBlobErrors(blob1, blob2))
			return;

		Blob male = blob1.male ? blob1 : blob2;
		Blob female = blob1.female ? blob1 : blob2;
		male.spouseId = female.id;
		female.spouseId = male.id;
		gameManager.AddGold(-cost);
		male.state = BlobState.Breeding;
		female.state = BlobState.Breeding;
		male.StartActionWithDuration(male.breedReadyDelay);
		female.StartActionWithDuration(female.breedReadyDelay);
		if(hudManager.blobInfoContextMenu.IsDisplayed())
			hudManager.blobInfoContextMenu.DisplayWithBlob(blob1);
	}


	public void BreedBlobs(Blob male, Blob female) {
		Blob newBlob = CreateBlobFromParents(male, female);
		female.room.AddBlob(newBlob);
		newBlob.state = BlobState.Hatching;
		newBlob.StartActionWithDuration(newBlob.blobHatchDelay);
	}


	public Blob CreateBlobFromParents(Blob dad, Blob mom) {
		GameObject blobGameObject = (GameObject)GameObject.Instantiate(Resources.Load("BlobSprites"));
		Blob blob = blobGameObject.AddComponent<Blob>();

		// figure gender
		blob.gender = (UnityEngine.Random.Range(0, 2) == 0) ? Gender.Male : Gender.Female;

		// figure out quality
		blob.quality = Blob.GetRandomQuality();

		// passed on genes
		blob.genes = dad.genes.Intersect(mom.genes).ToList();

		// activate/deactivate genes
		foreach(Gene g in blob.genes) {
			g.state = GeneState.Passive;
		}
		blob.genes[UnityEngine.Random.Range(0, blob.genes.Count)].state = GeneState.Available;

		blob.Setup();
		return blob;
	}


	bool CheckBreedBlobErrors(Blob blob1, Blob blob2) {
		int cost = GetBreedCost();

		if(blob1.male == blob2.male) {
			hudManager.popup.Show("Cannot Breed", "Must be different gender.");
			return true;
		}

		if(blob1.room.IsRoomFull()) {
			hudManager.popup.Show("Cannot Breed", "Room is full. Sell or move a blob to free space");
			return true;
		}

		if(gameManager.gameVars.gold < cost) {
			hudManager.popup.Show("Cannot Breed", "You do not have enough gold.");
			return true;
		}
		
		return false;
	}
	

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find("GameManager2").GetComponent<GameManager2>();
		hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
		ColorDefines.BuildColorDefines();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
