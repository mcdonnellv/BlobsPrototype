using UnityEngine;
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

		return blob;
	}


	List<Gene> PassGenes(List<BaseGene> dadBaseGenes, List<BaseGene> momBaseGenes, BlobInteractAction blobInteractAction) {
		List<BaseGene> childBaseGenes = dadBaseGenes.Union(momBaseGenes).ToList();
		List<Gene> childGenes = geneManager.CreateGeneListFromBaseGeneList(childBaseGenes);

		// Prune genes that cannot exist with each other
		childGenes = PruneGeneListOfExclusiveGenes(childGenes);
		
		// morph genes if needed
		for(int i = 0; i < childGenes.Count; i++) {
			Gene g = childGenes[i];
			childGenes[i] = g.MorphIfNeeded();
		}

		return childGenes;
	}


	List<Gene> PruneGeneListOfExclusiveGenes(List<Gene> childGenes) {
		List<Gene> prunedGenes = childGenes.ToList();
		List<TraitType> traitTypesProcessed = new List<TraitType>();

		foreach(Gene g in childGenes) {
			TraitType t = g.traitType;
			if(traitTypesProcessed.Contains(t))
				continue;
			traitTypesProcessed.Add(t);
			List<Gene> toChooseFrom = new List<Gene>();
			toChooseFrom.Add(g);
			foreach(Gene g1 in childGenes)
				if(g != g1 && g.functionality.CanExistWithWith(g1.traitType, g) == false)
					toChooseFrom.Add(g1);
			if(toChooseFrom.Count > 1) {
				Gene keep = toChooseFrom[UnityEngine.Random.Range(0,toChooseFrom.Count)];
				toChooseFrom.Remove(keep);
				foreach(Gene toRemove in toChooseFrom)
					if(prunedGenes.Contains(toRemove))
						prunedGenes.Remove(toRemove);
			}
		}
		return prunedGenes;
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
