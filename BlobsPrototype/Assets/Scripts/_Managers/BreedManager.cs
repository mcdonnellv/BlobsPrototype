﻿using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;


public class BreedManager : MonoBehaviour {
	private static BreedManager _breedManager;
	public static BreedManager breedManager { get {if(_breedManager == null) _breedManager = GameObject.Find("BreedManager").GetComponent<BreedManager>(); return _breedManager; } }
	HudManager hudManager { get { return HudManager.hudManager; } }
	GameManager2 gameManager { get { return GameManager2.gameManager; } }
	RoomManager roomManager  { get { return RoomManager.roomManager; } }
	GeneManager geneManager  { get { return GeneManager.geneManager; } }
	ItemManager itemManager  { get { return ItemManager.itemManager; } }
	Blob potentialPairBlob1;
	Blob potentialPairBlob2;
	public int GetBreedCost() { return 5; }
	public int GetMergeCost() { return 0; }


	public void AttemptBreedNoParam() { AttemptBreed(potentialPairBlob1, potentialPairBlob2); }


	public void AskBlobsInteract(Blob blob1, Blob blob2, BlobInteractAction blobInteractAction) {
		if(!blob2.hasHatched || !blob1.hasHatched)
			return;
		if(blob1.actionDuration.TotalSeconds > 0 || blob2.actionDuration.TotalSeconds > 0)
			return;
		if(blobInteractAction == BlobInteractAction.Breed) {
			potentialPairBlob1 = blob1;
			potentialPairBlob2 = blob2;
			AskConfirmBreed();
		}
		if(blobInteractAction == BlobInteractAction.Merge)
			AttemptMerge(blob1, blob2);
	}


	public void AskConfirmBreed() {
		int cost = GetBreedCost();
		if(gameManager.gameVars.chocolate  >= cost)
			hudManager.popup.Show("Breed", "Breed these blobs for " + cost.ToString() + " [token]?", this, "AttemptBreedNoParam");
		else
			hudManager.ShowError("Not enough [token]");
	}


	public void AttemptBreed(Blob blob1, Blob blob2) {
		int cost = GetBreedCost();

		if(CheckBreedBlobErrors(blob1, blob2))
			return;

		gameManager.AddChocolate(-cost);

		Blob male = blob1.male ? blob1 : blob2;
		Blob female = blob1.female ? blob1 : blob2;
		male.spouseId = female.id;
		female.spouseId = male.id;
		male.gameObject.StartActionWithDuration(BlobState.Breeding, male.breedReadyDelay);
		female.gameObject.StartActionWithDuration(BlobState.Breeding, female.breedReadyDelay);
		if(hudManager.blobInfoContextMenu.IsDisplayed())
			hudManager.blobInfoContextMenu.Show(blob1.id);
	}


	public void BreedBlobs(Blob male, Blob female) {
		Blob newBlob = CreateBlobFromParents(male, female, BlobInteractAction.Breed);
		female.room.AddBlob(newBlob);
		newBlob.gameObject.StartActionWithDuration(BlobState.Hatching, newBlob.blobHatchDelay);
	}


	public Blob CreateBlobFromParents(Blob dad, Blob mom, BlobInteractAction blobInteractAction) {
		GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("BlobGameObject"));
		BlobGameObject blobGameObject = go.GetComponent<BlobGameObject>();
		blobGameObject.Setup();
		Blob blob = blobGameObject.blob;

		// pass on genes
		blob.genes = PassGenes(geneManager.GetBaseGeneListFromGeneList(dad.genes), geneManager.GetBaseGeneListFromGeneList(mom.genes), blobInteractAction);
		blob.hiddenGenes = PassGenes(geneManager.GetBaseGeneListFromGeneList(dad.hiddenGenes), geneManager.GetBaseGeneListFromGeneList(mom.hiddenGenes), blobInteractAction);

		//trigger any OnBirth gene logic
		blob.OnBirth();

		// give random element and sigil
		switch(blobInteractAction) {
		case BlobInteractAction.Merge:
			blob.SetNativeElement((UnityEngine.Random.Range(0, 2) == 0) ? dad.element : mom.element);
			blob.sigil = (UnityEngine.Random.Range(0, 2) == 0) ? dad.sigil : mom.sigil;
			break;
		}

		return blob;
	}


	List<Gene> PassGenes(List<BaseGene> dadBaseGenes, List<BaseGene> momBaseGenes, BlobInteractAction blobInteractAction) {
		List<BaseGene> childBaseGenes = dadBaseGenes.Union(momBaseGenes).ToList();
		List<BaseGene> exclusiveGenes = new List<BaseGene>();
		List<TraitType> traitTypesProcessed = new List<TraitType>();

		// prune genes marked as exclusive
		foreach(BaseGene b in childBaseGenes)
			if(Trait.IsExclusive(b.traitType))
				exclusiveGenes.Add(b);

		foreach(BaseGene b in exclusiveGenes) {
			TraitType t = b.traitType;
			if(traitTypesProcessed.Contains(t))
				continue;
			List<BaseGene> genesOfTrait = new List<BaseGene>();
			foreach(BaseGene b1 in exclusiveGenes)
				if(t == b1.traitType)
					genesOfTrait.Add(b1);
			BaseGene keep = genesOfTrait[UnityEngine.Random.Range(0, genesOfTrait.Count)];
			foreach(BaseGene b1 in genesOfTrait)
				if(b1 != keep)
					childBaseGenes.Remove(b1);
			traitTypesProcessed.Add(t);
		}


		List<Gene> childGenes = geneManager.CreateGeneListFromBaseGeneList(childBaseGenes);

		//TODO: Prune genes that cannot exist with each other
		foreach(Gene g in childGenes) {
			TraitType t = g.traitType;
			if(traitTypesProcessed.Contains(t))
				continue;
			traitTypesProcessed.Add(t);
			foreach(Gene g1 in childGenes)
				if(g != g1 && !g.functionality.CanExistWithWith(t))
					;
		}
			
		// morph genes if needed
		for(int i = 0; i < childGenes.Count; i++) {
			Gene g = childGenes[i];
			childGenes[i] = g.MorphIfNeeded();
		}

		return childGenes;
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

		//if(gameManager.gameVars.gold < cost) {
		//	hudManager.popup.Show("Cannot Breed", "You do not have enough gold.");
		//	return true;
		//}
		
		return false;
	}


	bool CheckMergeBlobErrors(Blob blob1, Blob blob2) {
		if(gameManager.gameVars.gold < GetMergeCost()) {
			hudManager.popup.Show("Cannot Merge", "You do not have enough gold.");
			return true;
		}
		
		return false;
	}


	public void AttemptMerge(Blob blob1, Blob blob2) {
		int cost = GetMergeCost();
		
		if(CheckMergeBlobErrors(blob1, blob2))
			return;

		gameManager.AddGold(-cost);
		Room room = blob1.room;
		Blob newBlob = CreateBlobFromParents(blob1, blob2, BlobInteractAction.Merge);
		newBlob.gameObject.StartActionWithDuration(BlobState.Hatching, newBlob.blobHatchDelay);
		blob1.tilePosX = -1;
		blob1.tilePosY = -1;
		blob2.tilePosX = -1;
		blob2.tilePosY = -1;
		room.blobs.Remove(blob1);
		room.blobs.Remove(blob2);
		room.AddBlob(newBlob);
		room.DeleteBlob(blob1);
		room.DeleteBlob(blob2);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
