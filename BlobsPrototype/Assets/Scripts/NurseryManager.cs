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
	
	public List<Blob> blobs {get{return gm.gameVars.nurseryBlobs;}}
	public List<Blob> maleBlobs;
	public List<Blob> femaleBlobs;
	
	int maxBlobs;
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
		blobs.Add(blob);
		
		blob = new Blob();
		blob.male = false;
		blob.Hatch();
		blob.birthday = blob.birthday - gm.breedingAge;
		blob.SetRandomTextures();
		blobs.Add(blob);
	}
	
	void AddBlob(Blob newBlob)
	{
		if (blobs.Count >= maxBlobs)
			return;

		blobs.Add(newBlob);
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

		maleBlob.breedReadyTime = System.DateTime.Now + gm.breedReadyDelay;
		femaleBlob.breedReadyTime = System.DateTime.Now + gm.breedReadyDelay;

		Blob newBlob = GenerateNewBlobBasedOnBlobs(maleBlob, femaleBlob);
		femaleBlob.egg = newBlob;
	}


	public Blob GenerateNewBlobBasedOnBlobs(Blob dad, Blob mom)
	{
		Blob blob = new Blob();
		blob.hasHatched = false;
		int totalEggs = GetTotalEggs();

		//safety spawn female
		if (GetTotalEggs() <= 0)
			blob.male = false;
		else
			blob.male = (UnityEngine.Random.Range(0, 2) == 0) ? true : false;
		
		blob.quality = Blob.GetNewQuality(dad.quality, mom.quality);
		blob.SetRandomTextures();

		//check for possible new genes
		List<Gene> parentGenes = dad.genes.Union<Gene>(mom.genes).ToList<Gene>();
		Gene newGene = RollForNewGene(blob, parentGenes);

		if(newGene == null)
		{
			// Gene passing
			blob.genes = CleanupGeneList(parentGenes);
			LimitGenesTo(blob, blob.allowedGeneCount);
		}
		else
		{
			blob.genes.Add(newGene);
			blob.quality = 1f;
		}

		blob.ActivateGenes();

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

		// a blob may only have 6 genes, 3 active genes and 3 inactive genes at most

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

			blob.genes = genesNew;
		}
	}

	
	public Gene RollForNewGene(Blob blob, List<Gene> preReqList)
	{
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
		float geneRoll = 0f;//UnityEngine.Random.Range(0f,1f);
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
		breedTime = System.DateTime.Now + gm.breedBarFillDelay;

		Blob maleBlob = maleBlobs[UnityEngine.Random.Range(0, maleBlobs.Count)];
		Blob femaleBlob = femaleBlobs[UnityEngine.Random.Range(0, femaleBlobs.Count)];
		gm.AddGold(-gm.breedCost);
		BreedBlobs(maleBlob, femaleBlob);
	}
	

	public void PressSellButton()
	{
		Blob blob = blobs[curSelectedIndex];
		BlobCell bc = blobPanel.blobCells[curSelectedIndex];
		
		if (blob.onMission) 
		{gm.popup.Show("Cannot Sell", "Blob is on a mission."); gm.popup.SetBlob(blob); return;}
		
		if (blob.hasHatched == false)
		{gm.popup.Show("Cannot Sell", "Blob has not been hatched."); return;}
		
		if (blob.breedReadyTime > System.DateTime.Now)
		{gm.popup.Show("Cannot Sell", "Blob is still breeding."); gm.popup.SetBlob(blob); return;}
		
		bool lastOfGender = true;
		foreach(Blob b in blobs)
			if (b != blob && blob.male == b.male && b.hasHatched)
				lastOfGender = false;
		
		if (lastOfGender)
		{gm.popup.Show("Cannot Sell", "Cannot sell your last " + ((blob.male == true) ? "male" : "female") +" blob."); gm.popup.SetBlob(blob); return;}

		Gene lastGene = null;
		foreach(Gene g1 in blob.genes)
		{
			bool geneDupeFound = false;
			foreach(Blob b in blobs)
			{
				if(b == blob)
					continue;

				foreach(Gene g2 in b.genes)
				{
					if(g1 == g2)
					{
						geneDupeFound = true;
						break;
					}
				}

				if(geneDupeFound)
					break;
			}

			if(!geneDupeFound)
			{
				lastGene = g1;
				break;
			}
		}
				
		if (lastGene != null)
		{
			gm.popup.ShowChoice("Warning!", 
			                     "This is your last blob with the [9BFF9B]" + lastGene.geneName + " gene[-]. Are you sure you want to sell this blob?", 
			                    this, "SellBlobFinal"); gm.popup.SetBlob(blob); 
			return;
		}

		gm.popup.ShowChoice("Sell Blob", "Are you sure you want to sell this blob?", this, "SellBlobFinal");
		gm.popup.SetBlob(blob);
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
