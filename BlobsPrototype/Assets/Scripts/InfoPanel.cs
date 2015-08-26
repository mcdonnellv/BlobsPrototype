using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InfoPanel : MonoBehaviour 
{
	public GameManager gm;
	public UILabel ageLabel;
	public UILabel eggsLabel;
	public UILabel genderLabel;
	public UILabel qualityLabel;
	public UILabel levelLabel;
	public UILabel bodyLabel;
	public UILabel breedButtonLabel;
	public UILabel deleteButtonLabel;
	public UISprite body;
	public UISprite face;
	public UISprite cheeks;
	public UISprite egg;
	public UISprite bg;
	public UISlider progress;

	public UIButton hatchButton;
	public UIButton addEggButton;
	public UIButton breedButton;
	public UIButton moveButton;
	public UIButton deleteButton;

	public UISprite genePanel;
	public GenePopup geneInfoPopup;
	public List<GeneCell> geneCells;

	Blob theBlob;

	public void UpdateWithBlobIfSelected(Blob blob)
	{
		if(gm.nm.blobs[gm.nm.curSelectedIndex] == blob)
			UpdateWithBlob(blob);
	}

	public void UpdateBreedButton(Blob blob)
	{
		breedButton.isEnabled = true;

		if(blob.spouseId == -1)
		{
			breedButton.isEnabled = true;
			breedButtonLabel.text = "Find a Partner";
		}
		else
		{
			breedButton.isEnabled = (theBlob.female ? (theBlob.unfertilizedEggs > 0) : (theBlob.GetSpouse().unfertilizedEggs > 0));
			breedButtonLabel.text = "Breed";
			UpdateBreedCost();
		}
		
		bool isIdle = blob.GetBlobStateString() == "";
		breedButton.isEnabled = !isIdle ? false : breedButton.isEnabled;
	}


	public void UpdateWithBlob(Blob blob)
	{
		theBlob = blob;

		if (blob == null)
		{
			UpdateAsNull();
			return;
		}

		if (!blob.hasHatched)
		{
			UpdateAsEgg();
			return;
		}

		blob.OrderGenes();
		genePanel.gameObject.SetActive(false);//blob.hasHatched);
		hatchButton.gameObject.SetActive(false);
		addEggButton.gameObject.SetActive(theBlob.female);
		breedButton.gameObject.SetActive(true);
		moveButton.gameObject.SetActive(true);
		deleteButton.gameObject.SetActive(true);
		progress.gameObject.SetActive(false);
		body.gameObject.SetActive(true);
		face.gameObject.SetActive(true);
		cheeks.gameObject.SetActive(!blob.male);
		egg.gameObject.SetActive(false);
		Texture tex = blob.bodyPartSprites["Body"];
		body.spriteName = tex.name;
		tex = blob.bodyPartSprites["Eyes"];
		face.spriteName = tex.name;
		body.color = blob.color;
		bg.color = (blob.male) ? new Color(0.62f, 0.714f, 0.941f,1f) : new Color(0.933f, 0.604f, 0.604f, 1f);
		eggsLabel.text = blob.unfertilizedEggs.ToString() + " eggs left";
		qualityLabel.text = blob.quality.ToString() + " quality";
		levelLabel.text = "Level   " + blob.level.ToString();
		if (blob.male)
		{
			genderLabel.text = "Male";
			eggsLabel.text = "";
		}
		else
			genderLabel.text = "Female";

		int pixels = (int)(blob.BlobScale() * 50f);
		body.SetDimensions(pixels, pixels);
		face.SetDimensions(pixels, pixels);
		cheeks.SetDimensions(pixels, pixels);

		//UpdateGenePanel()
		UpdateBreedButton(blob);

		moveButton.isEnabled = false;


		bool isIdle = blob.GetBlobStateString() == "";
		moveButton.isEnabled = false;
		deleteButton.isEnabled = isIdle;
		addEggButton.isEnabled = !isIdle ? false : addEggButton.isEnabled;
		UpdateSellValue();
	}


	void UpdateAsEgg()
	{
		UpdateAsNull();
		egg.gameObject.SetActive(true);
		progress.gameObject.SetActive(true);
		hatchButton.gameObject.SetActive(true);
		hatchButton.isEnabled = (theBlob.hatchTime <= System.DateTime.Now);
	}


	void UpdateAsNull()
	{
		eggsLabel.text = "";
		ageLabel.text = "";
		genderLabel.text = "";
		qualityLabel.text = "";
		levelLabel.text = "";
		genePanel.gameObject.SetActive(false);
		hatchButton.gameObject.SetActive(false);
		addEggButton.gameObject.SetActive(false);
		breedButton.gameObject.SetActive(false);
		moveButton.gameObject.SetActive(false);
		deleteButton.gameObject.SetActive(false);
		body.gameObject.SetActive(false);
		face.gameObject.SetActive(false);
		cheeks.gameObject.SetActive(false);
		egg.gameObject.SetActive(false);
		progress.gameObject.SetActive(false);
		bg.color = Color.gray;
	}


	void UpdateGenePanel()
	{
		//Gene Cells
		
		//		int i = 0;
		//		foreach(Gene g in theBlob.genes)
		//		{
		//			GeneCell gc = geneCells[i];
		//
		//			if(i >= theBlob.allowedGeneCount)
		//			{
		//				HideGeneCell(gc);
		//				continue;
		//			}
		//
		//			gc.cellSprite.alpha = 1f;
		//			gc.gene = g;
		//			gc.nameLabel.text = g.geneName + (theBlob.IsGeneActive(g) ? "" : " (inactive)");
		//			gc.infoLabel.text = "";
		//			gc.nameLabel.color = theBlob.IsGeneActive(g) ? Color.white : Color.gray;
		//			gc.rarityIndicator.gameObject.SetActive(true);
		//			gc.rarityIndicator.color = Gene.ColorForRarity(g.rarity);
		//			gc.button.gameObject.SetActive(true);
		//			gc.addGeneButton.gameObject.SetActive(false);
		//			i++;
		//		}
		//
		//		for(;i<geneCells.Count;i++)
		//		{
		//			GeneCell gc = geneCells[i];
		//			if(i == theBlob.allowedGeneCount)
		//			{
		//				HideGeneCell(gc);
		//				gc.cellSprite.alpha = .3f;
		//				BlobQuality bq = Blob.GetQualityFromGeneCount(i+1);
		//				gc.infoLabel.text = "Requires " + Blob.GetQualityFromEnum(bq) + " Quality to Unlock";
		//				continue;
		//			}
		//			else if(i > theBlob.allowedGeneCount)
		//			{
		//				HideGeneCell(gc);
		//				continue;
		//			}
		//
		//			gc.cellSprite.alpha = 1f;
		//			gc.nameLabel.color = Color.gray;
		//			gc.nameLabel.text = "No Gene";
		//			gc.rarityIndicator.gameObject.SetActive(false);
		//			gc.button.gameObject.SetActive(false);
		//			gc.addGeneButton.gameObject.SetActive(true);
		//		}
	}

	void HideGeneCell(GeneCell gc)
	{
		gc.cellSprite.alpha = 0f;
		gc.nameLabel.text = "";
		gc.infoLabel.text = "";
		gc.rarityIndicator.gameObject.SetActive(false);
		gc.button.gameObject.SetActive(false);
		gc.addGeneButton.gameObject.SetActive(false);
	}


	public void GeneInfoPressed(int index)
	{
		GeneCell gc = geneCells[index];
		geneInfoPopup.Show(gc.gene);
	}


	public void PressAddEggButton()
	{
		int cost = 1;
		gm.blobPopupChoice.ShowChoice(theBlob, "Add an Egg?", "Add an egg to this Blob for [C59F76]" + cost.ToString() + "c[-]?",
		                              this, "AddEgg", null, null);
	}


	public void AddEgg()
	{
		int cost = 1;
		if (gm.chocolate < cost) 
		{gm.blobPopup.Show(theBlob, "Cannot Add an Egg", "You do not have enough Chocolate."); return;}

		gm.AddChocolate(-1);
		theBlob.unfertilizedEggs++;
		UpdateWithBlob(theBlob);
		gm.nm.blobPanel.UpdateBlobCellWithBlob(gm.nm.blobs.IndexOf(theBlob), null);
	}


	public void AddGenePressed(int index)
	{
		GeneCell gc = geneCells[index];
		gm.geneAddPopup.Show("Add Gene", theBlob);
	}


	public void PressHatchButton()
	{
		foreach(Gene m in theBlob.genes)
		{
			if(m.revealed == false)
			{
				m.revealed = true;
				Gene mp = gm.mum.GetGeneByName(m.preRequisite);
				string colorString = (m.negativeEffect) ? "[FF9B9B]" : "[9BFF9BFF]";

				if (mp == null || (mp != null && m.type != mp.type))
					gm.blobPopup.Show(theBlob, "New Gene Discovery", "This blob has been born with the new " + colorString + m.geneName + " gene[-]!");
				else  
					gm.blobPopup.Show(theBlob, "New Gene Discovery", "This blob's [9BFF9B]" + m.preRequisite + " gene[-] has mutated into the " + colorString + m.geneName + " gene[-]!");
			}
		}


		gm.blobDetailsPopup.ShowChoice(theBlob, "New Baby Blob","", 
		                               this, "KeepBlob", this, "SellBlob");
		gm.blobDetailsPopup.button2Label.text = "Sell     +[FFD700]" + theBlob.sellValue.ToString() + "g[-]";
		gm.blobDetailsPopup.button1Label.text = "Keep";
	}


	public void SellBlob()
	{
		theBlob.Hatch();
		gm.nm.SellBlobFinal();
	}


	public void KeepBlob()
	{
		theBlob.Hatch();
		UpdateWithBlob(theBlob);
		gm.nm.blobPanel.UpdateBlobCellWithBlob(gm.nm.blobs.IndexOf(theBlob), theBlob);
		
		gm.UpdateAverageLevel();
	}


	public void PressBreedButton()
	{
		if(theBlob.spouseId == -1)
		{
			gm.nm.FindSpouse(theBlob);
			UpdateWithBlob(theBlob);
		}
		else
		{
			gm.nm.BreedBlobWithSpouse(theBlob);
		}
	
	}


	public void PressDeleteButton()
	{
		gm.TryDeleteBlob(theBlob, gm.nm, false);
	}

	
	public void PressMoveButton()
	{
		//TODO
	}


	public void UpdateBreedCost() { breedButtonLabel.text = "Breed     [FFD700]" + gm.nm.GetBreedCost().ToString() + "g[-]"; }


	public void UpdateSellValue() { deleteButtonLabel.text = "Sell    +[FFD700]" + theBlob.sellValue.ToString() + "g[-]"; }


	void ConfirmRemoveSpouse()
	{
		gm.nm.RemoveSpouse(theBlob);
		UpdateWithBlob(theBlob);
	}


	void Update () 
	{
		if (theBlob != null)
		{
			if (theBlob.hasHatched)
			{
				TimeSpan blobAge = theBlob.age;
				if (blobAge.Days > 0)
					ageLabel.text =  string.Format("{0:0} days", blobAge.Days) + " old";
				else if (blobAge.Hours > 0)
					ageLabel.text = string.Format("{0:0} hrs", blobAge.Hours) + " old";
				else
					ageLabel.text = string.Format("{0:0} mins", blobAge.Minutes) + " old";

			
			}
			else
			{
				if (theBlob.hatchTime > System.DateTime.Now)
				{
					System.TimeSpan ts = (theBlob.hatchTime - System.DateTime.Now);
					progress.value = 1f - (float)(ts.TotalSeconds / theBlob.blobHatchDelay.TotalSeconds);
				}
				else if(theBlob.hatchTime != new System.DateTime(0))
				{
					theBlob.hatchTime = new System.DateTime(0);
					progress.value = 0f;
					gm.nm.infoPanel.UpdateWithBlobIfSelected(theBlob);
				}
			}

			if (theBlob.hasHatched && theBlob.age < gm.breedingAge)
			{
				int pixels = (int)(theBlob.BlobScale() * 50f);
				body.SetDimensions(pixels, pixels);
				face.SetDimensions(pixels, pixels);
				cheeks.SetDimensions(pixels, pixels);
			}
		}
	}
}
