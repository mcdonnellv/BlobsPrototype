using UnityEngine;
using System;
using System.Collections;

public class InfoPanel : MonoBehaviour 
{
	public GameManager gm;
	public UILabel age;
	public UILabel eggs;
	public UILabel gender;
	public UILabel activeGenes;
	public UILabel inactiveGenes;
	public UILabel quality;
	public UISprite body;
	public UISprite face;
	public UISprite cheeks;
	public UISprite egg;
	public UISprite bg;
	public UISlider progress;
	public UIButton button;
	public UISprite genePanel;
	public UISprite genePanelActive;
	public UISprite genePanelInactive;
	Blob theBlob;

	public void UpdateWithBlob(Blob blob)
	{
		theBlob = blob;

		if (blob == null)
		{
			activeGenes.text = "";
			eggs.text = "";
			age.text = "";
			gender.text = "";
			quality.text = "";
			body.gameObject.SetActive(false);
			face.gameObject.SetActive(false);
			cheeks.gameObject.SetActive(false);
			bg.color = Color.grey;
			button.gameObject.SetActive(false);
			progress.gameObject.SetActive(false);
			return;
		}

		genePanel.gameObject.SetActive(blob.hasHatched && blob.genes.Count > 0);
		button.gameObject.SetActive(!blob.hasHatched);
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
		activeGenes.text = "";
		inactiveGenes.text = "";
		foreach(Gene g in blob.genes)
		{
			if (blob.IsGeneActive(g))
			{
				string colorString = (g.negativeEffect) ? "[FF9B9B]" : "[9BFF9BFF]";
				activeGenes.text += colorString + g.geneName + "[-]\n";
			}
			else
				inactiveGenes.text += g.geneName + "\n";
		}

		if(inactiveGenes.text == "")
		{
			genePanelActive.width = 180;
			genePanelInactive.gameObject.SetActive(false);
		}
		else
		{
			genePanelActive.width = 92;
			genePanelInactive.gameObject.SetActive(true);
		}

		activeGenes.text = activeGenes.text == "" ? "" : activeGenes.text.Remove(activeGenes.text.Length - 1);
		inactiveGenes.text = inactiveGenes.text == "" ? "" : inactiveGenes.text.Remove(inactiveGenes.text.Length - 1); ;
		eggs.text = "Eggs: " + (gm.maxBreedcount - blob.breedCount).ToString();
		quality.text = "Quality: " + blob.quality.ToString() + " (" + Blob.GetQualityStringFromValue(blob.quality) + ")";
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
			activeGenes.text = "";
			eggs.text = "";
			age.text = "";
			gender.text = "";
			quality.text = "";
		}
	}

	public void pressed()
	{
		foreach(Gene m in theBlob.genes)
		{
			if(m.revealed == false)
			{
				m.revealed = true;
				Gene mp = gm.mum.GetGeneByName(m.preRequisite);
				string colorString = (m.negativeEffect) ? "[FF9B9B]" : "[9BFF9BFF]";

				if (mp == null || (mp != null && m.type != mp.type))
					gm.popup.Show("New Gene Discovery", "This blob has been born with the new " + colorString + m.geneName + " gene[-]!");
				else  
					gm.popup.Show("New Gene Discovery", "This blob's [9BFF9B]" + m.preRequisite + " gene[-] has mutated into the " + colorString + m.geneName + " gene[-]!");
				gm.popup.SetBlob(theBlob);
			}
		}


		theBlob.Hatch();
		UpdateWithBlob(theBlob);
		gm.nm.blobPanel.UpdateBlobCellWithBlob(gm.nm.blobs.IndexOf(theBlob), theBlob);

		gm.UpdateAverageQuality();
		gm.nm.UpdateBreedCost();
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
				
				
				if (progress.value >= 1f && button.isEnabled == false)
					button.isEnabled = true;
				
				if (progress.value < 1f && button.isEnabled == true)
					button.isEnabled = false;
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
