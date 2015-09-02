using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InfoPanel : MonoBehaviour 
{
	public GameManager gm;
	public UILabel age;
	public UILabel eggs;
	public UILabel gender;
	public UILabel quality;
	public UISprite body;
	public UISprite face;
	public UISprite cheeks;
	public UISprite egg;
	public UISprite bg;
	public UISlider progress;
	public UIButton multiButton;
	public UIButton addEggButton;
	public UISprite genePanel;
	public GenePopup geneInfoPopup;
	public List<GeneCell> geneCells;

	Blob theBlob;
	
	public void UpdateWithBlob(Blob blob)
	{
		theBlob = blob;

		if (blob == null)
		{
			eggs.text = "";
			age.text = "";
			gender.text = "";
			quality.text = "";
			body.gameObject.SetActive(false);
			face.gameObject.SetActive(false);
			cheeks.gameObject.SetActive(false);
			bg.color = Color.grey;
			multiButton.gameObject.SetActive(false);
			progress.gameObject.SetActive(false);
			return;
		}

		blob.OrderGenes();
		genePanel.gameObject.SetActive(blob.hasHatched);
		multiButton.gameObject.SetActive(!blob.hasHatched);
		addEggButton.gameObject.SetActive(blob.hasHatched && theBlob.female);
		progress.gameObject.SetActive(!blob.hasHatched);

		body.gameObject.SetActive(blob.hasHatched);
		face.gameObject.SetActive(blob.hasHatched);
		cheeks.gameObject.SetActive(!blob.male && blob.hasHatched);
		egg.gameObject.SetActive(!blob.hasHatched);
		Texture tex = blob.bodyPartSprites["Body"];
		body.spriteName = tex.name;
		tex = blob.bodyPartSprites["Eyes"];
		face.spriteName = tex.name;
		body.color = blob.color;
		bg.color = (blob.male) ? new Color(0.62f, 0.714f, 0.941f,1f) : new Color(0.933f, 0.604f, 0.604f, 1f);
		bg.color = blob.hasHatched ? bg.color : Color.gray;

		eggs.text = "Eggs: " + blob.unfertilizedEggs.ToString();
		quality.text = "Quality: " + blob.quality.ToString();
		age.text = "Age: " + blob.age.ToString();
		if (blob.male)
		{
			gender.text = "Gender: Male";
			eggs.text = "";
		}
		else
			gender.text = "Gender: Female";

		int pixels = (int)(blob.BlobScale() * 50f);
		body.SetDimensions(pixels, pixels);
		face.SetDimensions(pixels, pixels);
		cheeks.SetDimensions(pixels, pixels);

		if (!blob.hasHatched)
		{
			eggs.text = "";
			age.text = "";
			gender.text = "";
			quality.text = "";
		}

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


	public void PressMultiButton()
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

		theBlob.Hatch();
		UpdateWithBlob(theBlob);
		gm.nm.blobPanel.UpdateBlobCellWithBlob(gm.nm.blobs.IndexOf(theBlob), theBlob);

		//gm.UpdateAverageQuality();
		//gm.nm.UpdateBreedCost();
	}


	// Update is called once per frame
	void Update () 
	{
		if (theBlob != null)
		{
			if (theBlob.hasHatched)
			{
				TimeSpan blobAge = theBlob.age;
				if (blobAge.Days > 0)
					age.text = "Age: " + string.Format("{0:0} day {1:0} hr", blobAge.Days, blobAge.Hours);
				else if (blobAge.Hours > 0)
					age.text = "Age: " + string.Format("{0:0} hr {1:0} min", blobAge.Hours, blobAge.Minutes);
				else if (blobAge.Minutes > 0)
					age.text = "Age: " + string.Format("{0:0 }min {1:0} sec", blobAge.Minutes, blobAge.Seconds);
				else
					age.text = "Age: " + string.Format("{0:0} sec", blobAge.Seconds);
			
			}
			else
			{
				System.TimeSpan ts = (theBlob.hatchTime - System.DateTime.Now);
				progress.value = 1f - (float)(ts.TotalSeconds / gm.blobHatchDelay.TotalSeconds);
				progress.value = progress.value > 1f ? 1f : progress.value;
				
				
				if (progress.value >= 1f && multiButton.isEnabled == false)
					multiButton.isEnabled = true;
				
				if (progress.value < 1f && multiButton.isEnabled == true)
					multiButton.isEnabled = false;
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
