using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NurseryManager : MonoBehaviour
{
	public GameManager gm;

	public BlobPanel blobPanel;
	public InfoPanel infoPanel;
	public UIButton breedButton;
	public UISlider breedProgressBar;
	
	public List<Blob> blobs;
	public List<Blob> maleBlobs;
	public List<Blob> femaleBlobs;
	
	int maxBlobs;
	public int curSelectedIndex;

	// Use this for initialization
	void Start () 
	{
		blobPanel.Init();
		curSelectedIndex = 0;
		maxBlobs = 20;

		blobs = new List<Blob>();

		Blob blob = new Blob();
		blob.color = BlobColor.Blue;
		blob.allele1 = blob.color;
		blob.male = true;
		blob.age = 5;
		blob.hasHatched = true;
		infoPanel.UpdateWithBlob(blob);
		blobs.Add(blob);
		blobPanel.UpdateBlobCellWithBlob(blobs.IndexOf(blob), blob);
		
		blob = new Blob();
		blob.color = BlobColor.Blue;
		blob.allele1 = blob.color;
		blob.male = false;
		blob.age = 5;
		blob.hasHatched = true;
		blobs.Add(blob);
		blobPanel.UpdateBlobCellWithBlob(blobs.IndexOf(blob), blob);

		PressGridItem(0);
		breedProgressBar.value = 1f;
	}


	void AddBlob(Blob newBlob)
	{
		blobs.Add(newBlob);
		if (CanEnableBreedButton())
			breedButton.isEnabled = false;
	}

	void DeleteBlob(Blob blob)
	{
		blobs.Remove(blob);
		if (curSelectedIndex >= blobs.Count)
			PressGridItem(curSelectedIndex - 1);

		blobPanel.UpdateBlobCellWithBlob(curSelectedIndex, null);
		infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);
	}


	bool CanEnableBreedButton()
	{
		if (MatingPairExists() == false)
			return false;
		
		if (breedProgressBar.value < 1f)
			return false;
		
		if (blobs.Count >= maxBlobs)
			return false;
		
		if (gm.gold < gm.breedCost)
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
		
		blobPanel.blobCells[blobs.IndexOf(maleBlob)].showProgressBar = true;
		blobPanel.blobCells[blobs.IndexOf(femaleBlob)].showProgressBar = true;
		
		Blob blob = new Blob();
		blob.hasHatched = false;
		blob.mom = femaleBlob;
		blob.dad = maleBlob;
		blob.color = Random.Range(0, 2) == 0 ? maleBlob.color : femaleBlob.color;
		blob.allele1 = blob.color;
		int totalEggs = GetTotalEggs();
		if (GetTotalEggs() <= 0)
			blob.male = false;
		else
			blob.male = (Random.Range(0, 2) == 0) ? true : false;
		
		blob.quality = Blob.GetNewQuality(maleBlob.quality, femaleBlob.quality);
		
		// Mutation chance
		float mutationRoll = Random.Range(0f,1f);
		if(mutationRoll >= gm.chanceForMutation)
		{
			BlobQuality q = Blob.GetQualityFromValue(blob.quality);
			if (q >= BlobQuality.Fair && blob.color == BlobColor.Blue)
			{
				blob.color = BlobColor.Red;
				blob.quality = 1;
			}	
			if (q >= BlobQuality.Good && blob.color == BlobColor.Red)
			{
				blob.color = BlobColor.Yellow;
				blob.quality = 1;
			}
			
			if (q >= BlobQuality.Excellent && blob.color == BlobColor.Yellow)
			{
				blob.color = BlobColor.Green;
				blob.quality = 1;
			}
			
			if (q >= BlobQuality.Outstanding && blob.color == BlobColor.Green)
			{
				blob.color = Random.Range(0f,1f) > .5f ? BlobColor.Orange : BlobColor.Purple;
				blob.quality = 1;
			}
		}
		
		AddBlob(blob);
		int index = blobs.IndexOf(blob);
		blobPanel.UpdateBlobCellWithBlob(index, blob);
		PressGridItem(index);
		
		BlobCell bc = blobPanel.blobCells[blobs.IndexOf(maleBlob)];
		bc.progressBar.value = 1f;
		
		bc = blobPanel.blobCells[blobs.IndexOf(femaleBlob)];
		bc.progressBar.value = 1f;
	}

	public void UpdateAllBlobCells()
	{
		foreach(Blob blob in blobs)
			blobPanel.UpdateBlobCellWithBlob(blobs.IndexOf(blob), blob);

	}


	public void PressGridItem(int index)
	{
		curSelectedIndex = index;
		infoPanel.UpdateWithBlob(blobs[index]);
		
		BlobCell bc = blobPanel.blobCells[index];
		bc.gameObject.SendMessage("OnClick");
	}
	

	public void PressBreedButton()
	{
		if(gm.gold < gm.breedCost || MatingPairExists() == false)
			return;
		
		breedButton.isEnabled = false;
		breedProgressBar.value = 0;

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

	// Update is called once per frame
	void Update () 
	{
		if(breedProgressBar.value < 1f)
		{
			breedProgressBar.value += Time.deltaTime * (1f / gm.breedBarFillTime);
			
			if(breedProgressBar.value >= 1f)
			{
				breedProgressBar.value = 1f;
				if (CanEnableBreedButton())
					breedButton.isEnabled = true;
			}
		}

		foreach(Blob blob in blobs)
		{
			if (blob.hasHatched == false)
			{
				blob.hatchTime += Time.deltaTime;

				if (blob.hatchTime > gm.blobHatchTime)
					blob.hatchTime = gm.blobHatchTime;
			}
		}
	}
}
