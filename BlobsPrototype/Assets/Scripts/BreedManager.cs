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
		int averageLevel = gameManager.GetAverageLevel();
		return 0;//averageLevel * 10;
	}

	public void FindSpouse(Blob blob) {
		List<Blob> blobs = blob.room.blobs;
		List<Blob> otherGenderSingleBlobs = new List<Blob>();
		Blob newSpouse = null;
		
		foreach(Blob b in blobs)
		{
			if(b == blob || 
			   b.spouseId != -1 || 
			   !b.hasHatched || 
			   blob.male == b.male || 
			   (b.female && b.unfertilizedEggs == 0))
				continue;
			otherGenderSingleBlobs.Add(b);
		}
		
		
		if(otherGenderSingleBlobs.Count > 0)
			newSpouse = otherGenderSingleBlobs[UnityEngine.Random.Range(0, otherGenderSingleBlobs.Count)];
		else
		{
			hudManager.popup.Show("Cannot Find a Mate", 
			                      "There are no available " + (blob.male ? "fertile female" : "male") + " blobs.");
			return;
		}
		
		PairBlobs(blob, newSpouse);
	}


	public void PairBlobs(Blob blob1, Blob blob2) {
		blob1.spouseId = blob2.id;
		blob2.spouseId = blob1.id;
		blob1.state = Blob.State.Dating;
		blob2.state = Blob.State.Dating;
		blob1.StartActionWithDuration(blob1.mateFindDelay);
		blob2.StartActionWithDuration(blob2.mateFindDelay);
	}


	public void UnPairBlob(Blob blob1) {
		Blob blob2 = blob1.GetSpouse();
		if(blob2 == null)
			return;
		blob1.spouseId = -1;
		blob2.spouseId = -1;
		blob1.state = Blob.State.Depressed;
		blob2.state = Blob.State.Depressed;
		blob1.StartActionWithDuration(blob1.depressedDelay);
		blob2.StartActionWithDuration(blob2.depressedDelay);
		blob1.ChangeColor("Default", ColorDefines.defaultBlobColor);
		blob2.ChangeColor("Default",ColorDefines.defaultBlobColor);
	}


	public void AskBlobsInteract(Blob blob1, Blob blob2) {

		if(blob2.isNugget || blob1.isNugget)
			return;

		if(!blob2.hasHatched || !blob1.hasHatched)
			return;

		if(blob1.actionDuration.TotalSeconds > 0 || blob2.actionDuration.TotalSeconds > 0)
			return;

		if(blob1.spouseId == -1)
			AttemptPair(blob1, blob2);
		else
			AttemptBreed(blob1, blob2);
	}


	public void AttemptPair(Blob blob1, Blob blob2) {
		if(CheckPairBlobErrors(blob1, blob2))
			return;
		
		hudManager.popup.Show("Pair Blobs", "Would you like to create a new breeding pair?", this, "PairBlobsConfirmed");
		potentialPairBlob1 = blob1;
		potentialPairBlob2 = blob2;
	}


	public void AttemptBreed(Blob blob1, Blob blob2) {
		int cost = GetBreedCost();

		if(CheckBreedBlobErrors(blob1, blob2))
			return;
		
		Blob spouse = blob1.GetSpouse();
		Blob male = blob1.male ? blob1 : spouse;
		Blob female = blob1.female ? blob1 : spouse;
		
		if(female.unfertilizedEggs == 0) {
			hudManager.popup.Show("Cannot Breed", "Female Blob has no more eggs.");
			return;
		}
		
		gameManager.AddGold(-cost);
		male.state = Blob.State.Breeding;
		female.state = Blob.State.Breeding;
		male.StartActionWithDuration(male.breedReadyDelay);
		female.StartActionWithDuration(female.breedReadyDelay);
		if(hudManager.blobInfoContextMenu.IsDisplayed())
			hudManager.blobInfoContextMenu.DisplayWithBlob(blob1);
	}


	public void BreedBlobs(Blob male, Blob female) {
		female.unfertilizedEggs--;
		Blob newBlob = CreateBlobFromParents(male, female);
		female.room.AddBlob(newBlob);
		newBlob.state = Blob.State.Hatching;
		newBlob.StartActionWithDuration(newBlob.blobHatchDelay);
	}


	public Blob CreateBlobFromParents(Blob dad, Blob mom) {
		GameObject blobGameObject = (GameObject)GameObject.Instantiate(Resources.Load("BlobSprites"));
		Blob blob = blobGameObject.AddComponent<Blob>();
		blob.dadId = dad.id;
		blob.momId = mom.id;
		blob.hasHatched = false;

		// figure out Rank
		float rng = UnityEngine.Random.Range(0f,1f);
		int lvl = (int)((dad.level + mom.level) / 2f);
		if(rng > .7f)
			lvl++;
		else if(rng < .3f)
			lvl--;
		blob.level = Mathf.Clamp(lvl, 1, 100);

		// spawn nugget or Blob
		blob.isNugget = (UnityEngine.Random.Range(0f,1f) > .4f);
		if(blob.isNugget) {
			blob.SetupNugget();
			return blob;
		}

		// figure gender
		if (GetTotalUnfertilizedEggs(dad.room.blobs) <= 0)
			blob.male = false;
		else
			blob.male = (UnityEngine.Random.Range(0, 2) == 0) ? true : false;

		// figure out quality
		blob.quality = Blob.GetRandomQuality();

		// figure out passed on genes
		List<Gene> parentGenes = dad.genes.Union(mom.genes).ToList();
		foreach(Gene g in parentGenes) {
			if(UnityEngine.Random.Range(0f,1f) <= Gene.PassOnChanceForQuality(g.quality))
				blob.genes.Add(g);
		}

		// figure out revealed genes if any
		float roll = UnityEngine.Random.Range(0f,1f);
		if(GeneQualityHelper(Quality.Legendary, roll, blob) == false)
			if(GeneQualityHelper(Quality.Epic, roll, blob) == false)
				if(GeneQualityHelper(Quality.Rare, roll, blob) == false)
					if(GeneQualityHelper(Quality.Common, roll, blob) == false)
						GeneQualityHelper(Quality.Standard, roll, blob);


		blob.Setup();
		return blob;
	}

	bool GeneQualityHelper(Quality q, float roll, Blob blob) {
		if(roll <= Gene.RevealChanceForQuality(q)) {
			blob.AddRandomGene(q);
			return true;
		}
		return false;
	}

	int GetTotalUnfertilizedEggs(List<Blob> blobs) {
		int total = 0;
		foreach(Blob blob in blobs) {
			if(blob.male)
				continue;
			total += blob.unfertilizedEggs;
		}
		return total;
	}


	bool CheckBreedBlobErrors(Blob blob1, Blob blob2) {
		int cost = GetBreedCost();

		if(blob1.room.IsRoomFull()) {
			hudManager.popup.Show("Cannot Breed", "Room is full. Sell or move a blob to free space");
			return true;
		}

		if(blob1.spouseId == -1) {
			hudManager.popup.Show("Cannot Breed", "This blob needs a partner before it can breed. Drag it on to another blob.");
			return true;
		}

		if(blob1.spouseId != blob2.id) {
			hudManager.popup.Show("Cannot Breed", "This blob can only breed with its pair");
			return true;
		}

		if(gameManager.gameVars.gold < cost) {
			hudManager.popup.Show("Cannot Breed", "You do not have enough gold.");
			return true;
		}
		
		return false;
	}

	bool CheckPairBlobErrors(Blob blob1, Blob blob2) {
		if(blob2.hasHatched == false)
			return true;

		if(blob1.isInfant || blob2.isInfant ) {
			hudManager.popup.Show("Pair Blobs", "You can onlt breed adult blobs/");
			return true;
		}

		if(blob2.spouseId != -1 || blob1.spouseId != -1) {
			hudManager.popup.Show("Pair Blobs", "The blob already has a pair.");
			return true;
		}

		if((blob1.male && blob2.male) || (blob1.female && blob2.female)) {
			hudManager.popup.Show("Pair Blobs", "You cannot pair blobs of the same gender.");
			return true;
		}

		return false;
	}


	void PairBlobsConfirmed() {
		PairBlobs(potentialPairBlob1, potentialPairBlob2);
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
