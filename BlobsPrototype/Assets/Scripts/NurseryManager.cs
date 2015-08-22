using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NurseryManager : MonoBehaviour
{
	public GameManager gm;

	public BlobPanel blobPanel;
	public InfoPanel infoPanel;
	
	public List<Blob> blobs {get{return gm.gameVars.nurseryBlobs;}}
	public List<Blob> maleBlobs;
	public List<Blob> femaleBlobs;
	
	public int maxBlobs;
	public int curSelectedIndex;
	System.DateTime breedTime;


	void Start () 
	{
		blobPanel.Init();
		curSelectedIndex = 0;
		maxBlobs = 12;
	}


	public void FirstTimeSetup()
	{
		Blob blob = new Blob();
		blob.male = true;
		blob.Hatch();
		blob.birthday = blob.birthday - gm.breedingAge;
		blob.SetRandomTextures();
		blob.id = gm.gameVars.blobsSpawned++;
		blob.SetGeneActivationForAll();
		blob.ApplyGeneEffects(blob.activeGenes);
		blob.sellValue = 10 + Mathf.FloorToInt(blob.level * 1.5f);
		blobs.Add(blob);
		
		blob = new Blob();
		blob.male = false;
		blob.Hatch();
		blob.birthday = blob.birthday - gm.breedingAge;
		blob.SetRandomTextures();
		blob.id = gm.gameVars.blobsSpawned++;
		blob.SetGeneActivationForAll();
		blob.ApplyGeneEffects(blob.activeGenes);
		blob.sellValue = 10 + Mathf.FloorToInt(blob.level * 1.5f);
		blobs.Add(blob);
	}


	public void AddBlob(Blob newBlob)
	{
		if (blobs.Count >= maxBlobs)
			return;

		blobs.Add(newBlob);
		if(newBlob.hasHatched == false)
			newBlob.hatchTime = System.DateTime.Now + newBlob.blobHatchDelay;
	}


	void DeleteBlob(Blob blob)
	{

		if(blob.spouseId != -1)
			RemoveSpouse(blob);

		blobs.Remove(blob);
		if (curSelectedIndex >= blobs.Count)
			PressGridItem(curSelectedIndex - 1);

		for (int i = curSelectedIndex; i < blobs.Count; i++)
			blobPanel.UpdateBlobCellWithBlob(i, blobs[i]);

		blobPanel.UpdateBlobCellWithBlob(blobs.Count, null);
		if(curSelectedIndex < blobs.Count)
			infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);
		else
			infoPanel.UpdateWithBlob(null);

		gm.UpdateAverageLevel();
	}


	public int GetBreedCost()
	{
		int averageLevel = gm.GetAverageLevel();
		return averageLevel * 10;
	}


	int GetSellValue() { return gm.sellValue; }


	bool CanEnableBreedButton()
	{
		if (breedTime > System.DateTime.Now)
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
			if (bc.progressBar.value > 0 || blob.age < gm.breedingAge || !blob.hasHatched)
				continue;

			if (blob.male)
				maleBlobs.Add(blob);
			else if (blob.unfertilizedEggs > 0)
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
			total += blob.unfertilizedEggs;
		}
		return total;
	}


	void BreedBlobs(Blob maleBlob, Blob femaleBlob)
	{
		//break previous spouse bonds
		foreach(Blob b in blobs)
			if(b.id == maleBlob.spouseId || b.id == femaleBlob.spouseId)
				b.spouseId = -1;
		// make new bonds
		maleBlob.spouseId = femaleBlob.id;
		femaleBlob.spouseId = maleBlob.id;

		femaleBlob.unfertilizedEggs--;

		maleBlob.breedReadyTime = System.DateTime.Now + maleBlob.breedReadyDelay;
		femaleBlob.breedReadyTime = System.DateTime.Now + femaleBlob.breedReadyDelay;

		Blob newBlob = GenerateNewBlobBasedOnBlobs(maleBlob, femaleBlob);
		newBlob.SetGeneActivationForAll();
		newBlob.ApplyGeneEffects(newBlob.activeGenes);
		femaleBlob.egg = newBlob;
	}


	public Blob GenerateNewBlobBasedOnBlobs(Blob dad, Blob mom)
	{
		Blob blob = new Blob();
		blob.id = gm.gameVars.blobsSpawned++;
		blob.dadId = dad.id;
		blob.momId = mom.id;
		blob.hasHatched = false;
		int totalEggs = GetTotalEggs();

		//safety spawn female
		if (GetTotalEggs() <= 0)
			blob.male = false;
		else
			blob.male = (UnityEngine.Random.Range(0, 2) == 0) ? true : false;
		
		blob.quality = Blob.GetRandomQuality();
		blob.level = (int)(dad.level + mom.level / 2f) + UnityEngine.Random.Range (-1,2);
		blob.level = Mathf.Clamp(blob.level, 1, 100);
		blob.sellValue = 10 + Mathf.FloorToInt(blob.level * 1.5f);
		blob.SetRandomTextures();

		List<Gene> parentGenesRaw = dad.genes.Union<Gene>(mom.genes).ToList<Gene>();
		List<Gene> GenesToPassOn = new List<Gene>();

		foreach(Gene g in parentGenesRaw)
		{
			int str = 0;
			if(dad.genes.Contains(g))
				str++;
			if(mom.genes.Contains(g))
				str++;

			float mod = (str > 1) ? .8f : .5f;

			float rand = UnityEngine.Random.Range(0f, 1f);
			if(rand < g.passOnChance * mod)
				GenesToPassOn.Add(g);
		}

		blob.unprocessedGenes = CleanupGeneList(GenesToPassOn);
		LimitGenesTo(blob, blob.allowedGeneCount);

		blob.SetGeneActivationForAll();
		blob.ApplyGeneEffects(blob.activeGenes);

		return blob;
	}


	public List<Gene> CleanupGeneList(List<Gene> genes)
	{
		//remove dupe genes first
		genes = genes.Distinct().ToList();

		for (int i = 0; i < genes.Count; i++)
		{
			List<Gene> genesOfTheSameType = GeneManager.GetGenesOfType(genes, genes[i].type);

			// a blob can only have 2 of a gene type
			if (genesOfTheSameType.Count <= 2)
				continue;

			// if there are more, randomly select the surviving genes based on their gene strength
			int genesKept = 0;
			while (genesKept < 2)
			{
				int cummulativeStrength = 0;
				foreach(Gene g in genesOfTheSameType)
					cummulativeStrength += (int)g.geneStrength;
				
				int rand = UnityEngine.Random.Range(0, cummulativeStrength);
				cummulativeStrength = 0;
				for (int j = 0; j < genesOfTheSameType.Count; j++)
				{
					Gene g = genesOfTheSameType[j];
					cummulativeStrength += (int)g.geneStrength;
					if(rand <= cummulativeStrength)
					{
						genesKept++;
						genesOfTheSameType.Remove(g);
						break;
					}
				}
			}
			
			// remove unlucky the genes from the main list
			for (int j = 0; j < genes.Count; j++)
			{
				foreach(Gene removeMe in genesOfTheSameType)
				{
					if(genes[j].geneName == removeMe.geneName)
					{
						genes.Remove(genes[j]);
						j--;
					}
				}
			}
		}
		return genes;
	}


	public void LimitGenesTo(Blob blob, int count)
	{
		if(blob.genes.Count > count)
		{
			List<Gene> genesNew = new List<Gene>();
			List<Gene> genesOld = blob.genes.ToList();

			for (int i=0; i<count; i++)
			{
				Gene g = GeneManager.GetRandomGeneBasedOnStrength(genesOld);
				genesNew.Add(g);
				genesOld.Remove(g);
			}

			blob.unprocessedGenes = genesNew;
		}
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
		Blob blob = blobs[index];
		BlobCell bc = blobPanel.blobCells[index];
		infoPanel.UpdateWithBlob(blob);

		foreach(Blob b in blobs)
		{
			BlobCell cell = blobPanel.blobCells[blobs.IndexOf(b)];
			cell.heart.gameObject.SetActive(false);
		}

		foreach(Blob b in blobs)
		{
			BlobCell cell = blobPanel.blobCells[blobs.IndexOf(b)];
			if(b.id == blob.dadId)
				cell.infoLabel.text = "Dad";
			else if(b.id == blob.momId)
				cell.infoLabel.text = "Mom";
			else
				cell.infoLabel.text = b.GetBlobStateString();

			if(b.id == blob.spouseId && b.spouseId == blob.id)
			{
				cell.heart.gameObject.SetActive(true);
				bc.heart.gameObject.SetActive(true);
			}
		}
		bc.gameObject.SendMessage("OnClick");
	}


	public void FindSpouse(Blob blob)
	{
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
		{gm.popup.Show("Cannot Find a Mate", "There are no available " + (blob.male ? "fertile female" : "male") + " blobs."); return;}

		blob.spouseId = newSpouse.id;
		newSpouse.spouseId = blob.id;
		infoPanel.UpdateWithBlob(blob);

		blob.mateFindTime = System.DateTime.Now + blob.mateFindDelay;
		newSpouse.mateFindTime = System.DateTime.Now + newSpouse.mateFindDelay;
	}


	public void RemoveSpouse(Blob blob)
	{
		Blob spouse = blob.GetSpouse();

		if(spouse == null)
			blob.spouseId = -1;
		else
		{
			blob.spouseId = -1;
			spouse.spouseId = -1;
			blob.heartbrokenRecoverTime = System.DateTime.Now + blob.heartbrokenRecoverDelay;
			spouse.heartbrokenRecoverTime = System.DateTime.Now + spouse.heartbrokenRecoverDelay;
			BlobCell bc = blobPanel.blobCells[blobs.IndexOf(blob)];
			bc.heart.gameObject.SetActive(false);
			bc = blobPanel.blobCells[blobs.IndexOf(spouse)];
			bc.heart.gameObject.SetActive(false);
			infoPanel.UpdateWithBlob(blob);
		}
	}


	public void BreedBlobWithSpouse(Blob blob)
	{
		if(blob.spouseId == -1)
		{gm.popup.Show("Cannot Breed", "Blob has no mate."); return;}

		if(gm.gold < GetBreedCost())
		{gm.popup.Show("Cannot Breed", "Not enough gold.");return;}
		
		if(blobs.Count >= maxBlobs)
		{gm.popup.Show("Cannot Breed", "Room is full.");return;}

		if(blob.breedReadyTime > System.DateTime.Now)
		{gm.popup.Show("Cannot Breed", "Blob is still breeding");return;}

		if(blob.heartbrokenRecoverTime > System.DateTime.Now)
		{gm.popup.Show("Cannot Breed", "Blob is depressed");return;}

		Blob spouse = blob.GetSpouse();

		if((blob.female && blob.unfertilizedEggs == 0) || (spouse.female && spouse.unfertilizedEggs == 0))
		{gm.popup.Show("Cannot Breed", "Female Blob cannot produce anymore eggs.\nThis Blob must find a new mate to be able to breed.");return;}

		gm.AddGold(-GetBreedCost());
		if(blob.male)
			BreedBlobs(blob, spouse);
		else
			BreedBlobs(spouse, blob);
		infoPanel.UpdateWithBlob(blob);
	}


	public void GetMateFindResult(Blob blob)
	{
		if(blob.female)
			return;

		Blob spouse = blob.GetSpouse();
		int levelDifference = Mathf.Clamp(spouse.level - blob.level, 0, 100);
		float penalty = Mathf.Clamp(levelDifference * .1f, 0f, 1f);
		float roll = UnityEngine.Random.Range(0f,1f);
		bool success = (roll < (1f - penalty));

		if(success)
		{
			gm.blobPopup.Show(spouse, "Find a Mate", "Successfully courted a female!\nThis blob can now breed.");
			BlobCell bc = gm.nm.blobPanel.blobCells[gm.nm.blobs.IndexOf(blob)];
			bc.heart.gameObject.SetActive(true);
			bc = gm.nm.blobPanel.blobCells[gm.nm.blobs.IndexOf(spouse)];
			bc.heart.gameObject.SetActive(true);
		}
		else
		{
			gm.blobPopup.Show(blob, "Find a Mate", "Busted!\nThis blob was rejected by a female.");
			blob.heartbrokenRecoverTime = System.DateTime.Now + blob.heartbrokenRecoverDelay;
			blob.spouseId = -1;
			spouse.spouseId = -1;
		}
	}


	public void SellBlobFinal() 
	{
		Blob blob = blobs[curSelectedIndex];
		gm.AddGold(blob.sellValue);
		DeleteBlobFinal();
	}


	public void DeleteBlobFinal() 
	{
		Blob blob = blobs[curSelectedIndex];
		BlobCell bc = blobPanel.blobCells[curSelectedIndex];
		bc.Reset();
		DeleteBlob(blob);
	}


	public bool IsFull() {return(blobs.Count >= maxBlobs);}


	public void PressToVillageButton()
	{
		if (blobs.Count <= 0 || curSelectedIndex >= blobs.Count)
			return;

		Blob blob = blobs[curSelectedIndex];

		if (gm.vm.IsFull()) 
		{gm.blobPopup.Show(blob, "Cannot Move", "There is no more space in the Village."); return;}


		if (!blob.hasHatched)
		{gm.blobPopup.Show(blob, "Cannot Move", "Blob has not been hatched."); return;}

		BlobCell bc = blobPanel.blobCells[curSelectedIndex];
		if (blob.breedReadyTime > System.DateTime.Now)
		{gm.blobPopup.Show(blob, "Cannot Move", "Blob is still breeding."); return;}
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
		if (gm.cm.IsFull()) 
		{gm.blobPopup.Show(blob, "Cannot Move", "There is no more space in the Village."); return;}
		
		
		if (!blob.hasHatched)
		{gm.blobPopup.Show(blob, "Cannot Move", "Blob has not been hatched.");  return;}
		
		BlobCell bc = blobPanel.blobCells[curSelectedIndex];
		if (blob.breedReadyTime > System.DateTime.Now)
		{gm.blobPopup.Show(blob, "Cannot Move", "Blob is still breeding."); return;}
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
	}
}
