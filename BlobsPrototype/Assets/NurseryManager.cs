using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NurseryManager : MonoBehaviour
{
	public GameManager gm;

	public BlobPanel blobPanel;
	public InfoPanel infoPanel;
	public UIButton breedButton;
	public UIButton toVillageButton;
	public UIButton toCastleButton;
	public UILabel breedButtonLabel;
	public UILabel sellButtonLabel;
	public UISlider breedProgressBar;
	
	public List<Blob> blobs;
	public List<Blob> maleBlobs;
	public List<Blob> femaleBlobs;
	
	int maxBlobs;
	public int curSelectedIndex;
	float breedTime;

	// Use this for initialization
	void Start () 
	{
		blobPanel.Init();
		curSelectedIndex = 0;
		maxBlobs = 12;

		blobs = new List<Blob>();

		Blob blob = new Blob();

		blob.male = true;
		blob.age = 5;
		blob.hasHatched = true;
		blob.mutations.Add(gm.mum.GetMutationByName("Blue"));
		blob.color = blob.GetBodyColor();
		infoPanel.UpdateWithBlob(blob);
		blobs.Add(blob);
		blobPanel.UpdateBlobCellWithBlob(blobs.IndexOf(blob), blob);
		
		blob = new Blob();
		blob.male = false;
		blob.age = 5;
		blob.hasHatched = true;
		blob.mutations.Add(gm.mum.GetMutationByName("Blue"));
		blob.color = blob.GetBodyColor();
		blobs.Add(blob);
		blobPanel.UpdateBlobCellWithBlob(blobs.IndexOf(blob), blob);

		PressGridItem(0);
		breedProgressBar.value = 1f;
		toVillageButton.gameObject.SetActive(false);
		toCastleButton.gameObject.SetActive(false);
	}


	void AddBlob(Blob newBlob)
	{
		if (blobs.Count >= maxBlobs)
			return;

		blobs.Add(newBlob);
		newBlob.hatchTime = Time.time + gm.blobHatchDelay;
		if (CanEnableBreedButton())
			breedButton.isEnabled = false;
	}

	void DeleteBlob(Blob blob)
	{
		blobs.Remove(blob);
		if (curSelectedIndex >= blobs.Count)
			PressGridItem(curSelectedIndex - 1);

		for (int i = curSelectedIndex; i < blobs.Count; i++)
			blobPanel.UpdateBlobCellWithBlob(i, blobs[i]);

		blobPanel.UpdateBlobCellWithBlob(blobs.Count, null);
		infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);

		gm.UpdateAverageQuality();
		UpdateBreedCost();
	}

	int GetBreedCost()
	{
		float averageQuality = gm.GetAverageQuality();
		float mod = (float)Blob.GetQualityFromValue(averageQuality) - 1f;
		int breedCost = gm.breedBaseCost + (int)((mod / 4f) * (gm.breedCostMax - gm.breedBaseCost));

		return breedCost;
	}

	int GetSellValue()
	{
		return gm.sellValue;
	}

	public void UpdateBreedCost()
	{
		breedButtonLabel.text = "Breed (" + GetBreedCost().ToString() + "g)";
	}

	public void UpdateSellValue()
	{
		sellButtonLabel.text = "Sell (+" + GetSellValue().ToString() + "g)";
	}


	bool CanEnableBreedButton()
	{
		if (breedProgressBar.value < 1f)
			return false;
		
		if (blobs.Count >= maxBlobs)
			return false;
		
		if (gm.gold < gm.breedCost)
			return false;

		if (MatingPairExists() == false)
			return false;
		
		return true;
	}


	public bool MatingPairExists()
	{
		maleBlobs = new List<Blob>();
		femaleBlobs = new List<Blob>();

		foreach(Blob blob in blobs)
		{
			BlobCell bc = blobPanel.blobCells[blobs.IndexOf(blob)];
			if (bc.progressBar.value > 0 || blob.age < gm.breedingAge)
				continue;

			if (blob.male)
				maleBlobs.Add(blob);
			else if (blob.breedCount < gm.maxBreedcount)
				femaleBlobs.Add(blob);
		}
		
		return (femaleBlobs.Count > 0 && maleBlobs.Count > 0);
	}

	int GetTotalEggs()
	{
		int total = 0;
		foreach(Blob blob in blobs)
		{
			if(blob.male)
				continue;
			total += (gm.maxBreedcount - blob.breedCount);
		}
		return total;
	}

	void BreedBlobs(Blob maleBlob, Blob femaleBlob)
	{
		maleBlob.breedCount++;
		femaleBlob.breedCount++;
		maleBlob.breededThisYear = true;
		femaleBlob.breededThisYear = true;

		maleBlob.breedReadyTime = Time.time + gm.breedReadyDelay;
		femaleBlob.breedReadyTime = Time.time + gm.breedReadyDelay;
		

		Blob newBlob = GenerateNewBlobBasedOnBlobs(maleBlob, femaleBlob);
		femaleBlob.egg = newBlob;
	}

	public Blob GenerateNewBlobBasedOnBlobs(Blob dad, Blob mom)
	{
		Blob blob = new Blob();
		blob.hasHatched = false;
		blob.mom = mom;
		blob.dad = dad;
		int totalEggs = GetTotalEggs();

		//safety spawn female
		if (GetTotalEggs() <= 0)
			blob.male = false;
		else
			blob.male = (Random.Range(0, 2) == 0) ? true : false;
		
		blob.quality = Blob.GetNewQuality(dad.quality, mom.quality);


		// Mutation chance
		Mutation newMutation = null;
		List<Mutation> parentMutations = new List<Mutation>();
		List<Mutation> elligibleMutations = new List<Mutation>();
		parentMutations = dad.mutations.Union<Mutation>(mom.mutations).ToList<Mutation>();


		//check for mutations based on prerequisites
		foreach(Mutation parentMutation in parentMutations)
			foreach(Mutation potentialMutation in gm.mum.mutations)
				if(potentialMutation.preRequisite == parentMutation.name)
					elligibleMutations.Add(potentialMutation);
		

		// only one mutation of a type is allowed in the new blob's mutation list
		List<Mutation> temp = parentMutations.ToList();
		foreach(Mutation parentMutation in parentMutations)
		{
			temp.Remove(parentMutation);
			blob.mutations.Add(parentMutation);
			foreach(Mutation otherParentMutation in temp)
				if(parentMutation.type == otherParentMutation.type)
				{
					if(Random.Range(0,2) == 0)
					{
					   blob.mutations.Remove(parentMutation);
					   blob.mutations.Add(otherParentMutation);
					}
					break;
				}
		}

		//roll for a mutation
		float mutationRoll = Random.Range(0f,1f);
		foreach(Mutation elligibleMutation in elligibleMutations)
			if (mutationRoll <= elligibleMutation.revealChance)
			{
			    newMutation = elligibleMutation; //success!
				break;
			}

		if(newMutation != null)
		{
			//replace mutations of same type
			temp = blob.mutations.ToList();
			foreach(Mutation mutation in temp)
				if(mutation.type == newMutation.type)
			    {
				   blob.mutations.Add(newMutation);
				   blob.mutations.Remove(mutation);
			    }
		}


		blob.color = blob.GetBodyColor();

		return blob;
	}


	public void SpawnEgg(Blob egg)
	{
		if (blobs.Count >= maxBlobs)
			return;

		AddBlob(egg);
		int index = blobs.IndexOf(egg);
		blobPanel.UpdateBlobCellWithBlob(index, egg);
	}


	public void UpdateAllBlobCells()
	{
		if(blobs == null)
			return;

		foreach(Blob blob in blobs)
			blobPanel.UpdateBlobCellWithBlob(blobs.IndexOf(blob), blob);

		if(curSelectedIndex > blobs.Count)
			infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);
	}


	public void PressGridItem(int index)
	{
		if(index < 0)
			return;

		if(blobs.Count <= 0)
			return;

		curSelectedIndex = index;
		infoPanel.UpdateWithBlob(blobs[index]);
		UpdateSellValue();

		BlobCell bc = blobPanel.blobCells[index];
		bc.gameObject.SendMessage("OnClick");
	}
	

	public void PressBreedButton()
	{
		if(gm.gold < gm.breedCost)
		{
			gm.popup.Show("Cannot Breed", "Not enough gold.");
			return;
		}

		if(MatingPairExists() == false)
		{
			gm.popup.Show("Cannot Breed", "No breeding pair available.");
			return;
		}

		if(blobs.Count >= maxBlobs)
		{
			gm.popup.Show("Cannot Breed", "Room is full.");
			return;
		}
		
		breedButton.isEnabled = false;
		breedTime = gm.breedBarFillTime + Time.time;

		Blob maleBlob = maleBlobs[Random.Range(0, maleBlobs.Count)];
		Blob femaleBlob = femaleBlobs[Random.Range(0, femaleBlobs.Count)];
		gm.AddGold(-gm.breedCost);
		BreedBlobs(maleBlob, femaleBlob);
	}
	

	public void PressSellButton()
	{
		Blob blob = blobs[curSelectedIndex];
		BlobCell bc = blobPanel.blobCells[curSelectedIndex];

		if (blob.onMission)
			return;

		bc.Reset();
		gm.AddGold(gm.sellValue);
		DeleteBlob(blob);
	}


	public bool IsFull() {return(blobs.Count >= maxBlobs);}



	public void PressToVillageButton()
	{
		if (blobs.Count <= 0 || curSelectedIndex >= blobs.Count || gm.vm.IsFull())
			return;

		Blob blob = blobs[curSelectedIndex];
		if (!blob.hasHatched)
			return;

		BlobCell bc = blobPanel.blobCells[curSelectedIndex];
		if(bc.progressBar.value > 0f)
			return;
		bc.Reset();

		gm.vm.blobs.Add(blob);
		gm.vm.blobPanel.UpdateBlobCellWithBlob(gm.vm.blobs.IndexOf(blob), blob);

		blobs.Remove(blob);
		if (curSelectedIndex >= blobs.Count)
			PressGridItem(curSelectedIndex - 1);
		else
			PressGridItem(curSelectedIndex);
		
		for (int i = curSelectedIndex; i < blobs.Count; i++)
			blobPanel.UpdateBlobCellWithBlob(i, blobs[i]);
		
		blobPanel.UpdateBlobCellWithBlob(blobs.Count, null);
		if(curSelectedIndex < blobs.Count)
			infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);
		else
			infoPanel.UpdateWithBlob(null);

		gm.vm.newBlobAdded(blob);
	}

	public void PressToCastleButton()
	{
		if (blobs.Count <= 0 || curSelectedIndex >= blobs.Count || gm.cm.IsFull())
			return;
		
		Blob blob = blobs[curSelectedIndex];
		if (!blob.hasHatched)
			return;
		
		BlobCell bc = blobPanel.blobCells[curSelectedIndex];
		if(bc.progressBar.value > 0f)
			return;
		bc.Reset();
		
		gm.cm.blobs.Add(blob);
		gm.cm.blobPanel.UpdateBlobCellWithBlob(gm.cm.blobs.IndexOf(blob), blob);
		
		blobs.Remove(blob);
		if (curSelectedIndex >= blobs.Count)
			PressGridItem(curSelectedIndex - 1);
		else
			PressGridItem(curSelectedIndex);
		
		for (int i = curSelectedIndex; i < blobs.Count; i++)
			blobPanel.UpdateBlobCellWithBlob(i, blobs[i]);
		
		blobPanel.UpdateBlobCellWithBlob(blobs.Count, null);
		if(curSelectedIndex < blobs.Count)
			infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);
		else
			infoPanel.UpdateWithBlob(null);
		
		gm.cm.newBlobAdded(blob);
	}


	// Update is called once per frame
	void Update () 
	{
		breedProgressBar.value = 1f - ((breedTime - Time.time) / gm.breedBarFillTime);
		breedProgressBar.value = breedProgressBar.value > 1f ? 1f : breedProgressBar.value;

		if (breedTime <= Time.time && breedButton.isEnabled == false)
		{
			if (CanEnableBreedButton())
				breedButton.isEnabled = true;
		}
	}
}
