﻿using UnityEngine;
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
	
	public List<Blob> blobs {get{return gm.gameVars.nurseryBlobs;}}
	public List<Blob> maleBlobs;
	public List<Blob> femaleBlobs;
	
	public int maxBlobs;
	public int curSelectedIndex;
	System.DateTime breedTime;

	// Use this for initialization
	void Start () 
	{
		blobPanel.Init();
		curSelectedIndex = 0;
		maxBlobs = 12;
		breedProgressBar.value = 1f;
		toVillageButton.gameObject.SetActive(false);
		toCastleButton.gameObject.SetActive(false);
	}


	public void FirstTimeSetup()
	{
		Blob blob = new Blob();
		blob.male = true;
		blob.Hatch();
		blob.birthday = blob.birthday - gm.breedingAge;
		blob.SetRandomTextures();
		blob.id = gm.gameVars.blobsSpawned++;
		//blob.unprocessedGenes.Add(gm.mum.GetGeneByName("Better Babies"));
		blob.SetGeneActivationForAll();
		blob.ApplyGeneEffects(blob.activeGenes);
		blobs.Add(blob);
		
		blob = new Blob();
		blob.male = false;
		blob.Hatch();
		blob.birthday = blob.birthday - gm.breedingAge;
		blob.SetRandomTextures();
		blob.id = gm.gameVars.blobsSpawned++;
		//blob.unprocessedGenes.Add(gm.mum.GetGeneByName("Better Babies"));
		//blob.unprocessedGenes.Add(gm.mum.GetGeneByName("Fertility"));
		blob.SetGeneActivationForAll();
		blob.ApplyGeneEffects(blob.activeGenes);
		blobs.Add(blob);
	}
	
	public void AddBlob(Blob newBlob)
	{
		if (blobs.Count >= maxBlobs)
			return;

		blobs.Add(newBlob);
		if(newBlob.hasHatched == false)
			newBlob.hatchTime = System.DateTime.Now + gm.blobHatchDelay;
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
		if(curSelectedIndex < blobs.Count)
			infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);
		else
			infoPanel.UpdateWithBlob(null);

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
		breedButtonLabel.text = "Breed [FFD700]" + GetBreedCost().ToString() + "g[-]";
	}


	public void UpdateSellValue()
	{
		sellButtonLabel.text = "Sell +[FFD700]" + GetSellValue().ToString() + "g[-]";
	}


	bool CanEnableBreedButton()
	{
		if (breedTime > System.DateTime.Now)
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

		maleBlob.breedReadyTime = System.DateTime.Now + gm.breedReadyDelay;
		femaleBlob.breedReadyTime = System.DateTime.Now + gm.breedReadyDelay;

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
		
		blob.quality = Blob.GetNewQuality(dad, mom);
		blob.SetRandomTextures();

		//check for possible new genes
		List<Gene> parentGenesRaw = dad.genes.Union<Gene>(mom.genes).ToList<Gene>();
		//List<Gene> parentGenesRaw = dad.genes.Concat(mom.genes).ToList<Gene>();
		List<Gene> GenesToPassOn = new List<Gene>();

		foreach(Gene g in parentGenesRaw)
		{
			int str = 0;
			if(dad.genes.Contains(g))
				str++;
			if(mom.genes.Contains(g))
				str++;

			float mod = (str > 1) ? 1f : .75f;

			float rand = UnityEngine.Random.Range(0f, 1f);
			if(rand < g.passOnChance * mod)
				GenesToPassOn.Add(g);
		}

		//Gene newGene = RollForNewGene(blob, GenesToPassOn);


		//if(newGene == null)
		//{
		//	// Gene passing
			blob.unprocessedGenes = CleanupGeneList(GenesToPassOn);
			LimitGenesTo(blob, blob.allowedGeneCount);
		//}
		//else
		//{
		//	blob.genes.Add(newGene);
		//	blob.quality = (float)BlobQuality.Poor;
		//}

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

	
	public Gene RollForNewGene(Blob blob, List<Gene> preReqList)
	{
		// poor quality blobs cannot mutate genes
		if (Blob.GetQualityFromValue(blob.quality) == BlobQuality.Poor)
			return null;
		
		List<Gene> allEligibleGenes = new List<Gene>();
		foreach(Gene gene in preReqList)
		{
			List<Gene> eligibleGenes = GeneManager.GetGenesWithPrerequisite(gm.mum.genes, gene.geneName);
			allEligibleGenes = allEligibleGenes.Union<Gene>(eligibleGenes).ToList<Gene>();
		}

		allEligibleGenes = allEligibleGenes.Union<Gene>(GeneManager.GetGenesWithPrerequisite(gm.mum.genes, "")).Distinct ().ToList<Gene>();

		//prune genes that currently exist amongthe player's blob population
		foreach(Blob b in blobs)//for now only look at nursery blobs
			foreach(Gene gene in b.genes)
				foreach(Gene eg in allEligibleGenes)
				    if(eg.geneName == gene.geneName)
					{
						allEligibleGenes.Remove(eg);
						break;
					}

		List<Gene> rollSuccessGenes = new List<Gene>();
		float geneRoll = UnityEngine.Random.Range(0f,1f);
		foreach(Gene eligibleGene in allEligibleGenes)
			if (geneRoll <= eligibleGene.revealChance)
				rollSuccessGenes.Add(eligibleGene);
			
		Gene newGene = GeneManager.GetRandomGeneBasedOnRarity(rollSuccessGenes);
		return newGene;
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
		UpdateSellValue();

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
				cell.infoLabel.text = "";

			if(b.id == blob.spouseId && b.spouseId == blob.id)
			{
				cell.heart.gameObject.SetActive(true);
				bc.heart.gameObject.SetActive(true);
			}
		}


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
		breedTime = System.DateTime.Now + gm.breedBarFillDelay;

		Blob maleBlob = null;
		Blob femaleBlob = null;
		List<Blob> singleFemales = new List<Blob>();
		foreach(Blob b in femaleBlobs)
			if(b.spouseId == -1)
				singleFemales.Add(b);

		while(femaleBlob == null)
		{
			maleBlob = maleBlobs[UnityEngine.Random.Range(0, maleBlobs.Count)];

			if(maleBlobs.Count == 1) //last male
				femaleBlob = femaleBlobs[UnityEngine.Random.Range(0, femaleBlobs.Count)];
			else if (maleBlob.spouseId == -1) //no partner
			{
				if(singleFemales.Count > 0)
					femaleBlob = singleFemales[UnityEngine.Random.Range(0, singleFemales.Count)];
				else
				{
					// this male blob will just have to wait. try select another male blob
					maleBlobs.Remove(maleBlob);
					maleBlob = maleBlobs[UnityEngine.Random.Range(0, maleBlobs.Count)];
				}
			}
			else //had a previous partner
			{
				foreach(Blob b in femaleBlobs)
					if (b.id == maleBlob.spouseId)
						femaleBlob = b;

				if(femaleBlob == null && singleFemales.Count > 0)
					femaleBlob = singleFemales[UnityEngine.Random.Range(0, singleFemales.Count)];
				else
					maleBlobs.Remove(maleBlob);
			}
		}


		if (maleBlob.spouseId != -1)
		{
			//had a previous partner
			foreach(Blob b in femaleBlobs)
			{
				if (b.id == maleBlob.spouseId)
				{
					// found her!
					femaleBlob = b;
					break;
				}
			}
		}

		if(femaleBlob == null) // shes gone! get a new wife.
			femaleBlob = femaleBlobs[UnityEngine.Random.Range(0, femaleBlobs.Count)];
		
		gm.AddGold(-gm.breedCost);
		BreedBlobs(maleBlob, femaleBlob);
	}
	

	public void PressSellButton()
	{
		gm.TrySellBlob(blobs[curSelectedIndex], this);
	}


	void SellBlobFinal() 
	{
		Blob blob = blobs[curSelectedIndex];
		BlobCell bc = blobPanel.blobCells[curSelectedIndex];
		bc.Reset();
		gm.AddGold(gm.sellValue);
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
		System.TimeSpan ts = (breedTime - System.DateTime.Now);
		breedProgressBar.value = 1f - (float)(ts.TotalSeconds / gm.breedBarFillDelay.TotalSeconds);
		breedProgressBar.value = breedProgressBar.value > 1f ? 1f : breedProgressBar.value;

		if (breedTime <= System.DateTime.Now && breedButton.isEnabled == false)
		{
			if (CanEnableBreedButton())
				breedButton.isEnabled = true;
		}
	}
}
