using UnityEngine;
using System.Collections;

public class InfoPanel : MonoBehaviour 
{
	public GameManager gm;
	public UILabel age;
	public UILabel eggs;
	public UILabel gender;
	public UILabel color;
	public UILabel quality;
	public UISprite body;
	public UISprite face;
	public UISprite lashes;
	public UISprite egg;
	public UISprite bg;
	public UISlider progress;
	public UIButton button;
	Blob theBlob;

	// Use this for initialization
	void Start () 
	{
	
	}

	public void UpdateWithBlob(Blob blob)
	{
		theBlob = blob;

		if (blob == null)
		{
			color.text = "";
			eggs.text = "";
			age.text = "";
			gender.text = "";
			quality.text = "";
			body.gameObject.SetActive(false);
			face.gameObject.SetActive(false);
			lashes.gameObject.SetActive(false);
			bg.color = Color.grey;
			button.gameObject.SetActive(false);
			progress.gameObject.SetActive(false);
			return;
		}

		button.gameObject.SetActive(!blob.hasHatched);
		progress.gameObject.SetActive(!blob.hasHatched);

		body.gameObject.SetActive(blob.hasHatched);
		face.gameObject.SetActive(blob.hasHatched);
		lashes.gameObject.SetActive(!blob.male && blob.hasHatched);
		egg.gameObject.SetActive(!blob.hasHatched);
		body.color = blob.color;
		bg.color = (blob.male) ? new Color(0.62f, 0.714f, 0.941f,1f) : new Color(0.933f, 0.604f, 0.604f, 1f);
		bg.color = blob.hasHatched ? bg.color : Color.gray;
		color.text = "Color: " + blob.GetBodyColorName();
		eggs.text = "Eggs Left: " + (gm.maxBreedcount - blob.breedCount).ToString();
		quality.text = "Quality: " + blob.quality.ToString() + " (" + Blob.GetQualityStringFromValue(blob.quality) + ")";
		age.text = "Age: " + blob.age.ToString();
		if (blob.male)
		{
			gender.text = "Gender: Male";
			eggs.text = "";
		}
		else
			gender.text = "Gender: Female";

		float a = (float)(blob.age > 3 ? 3 : blob.age);
		float s = .3f + (.7f * (a / 3f));
		s = (s > 1f) ? 1f : s;
		int pixels = (int)(s * 50f);
		body.SetDimensions(pixels, pixels);
		face.SetDimensions(pixels, pixels);
		lashes.SetDimensions(pixels, pixels);

		if (!blob.hasHatched)
		{
			color.text = "";
			eggs.text = "";
			age.text = "";
			gender.text = "";
			quality.text = "";
		}


	}

	public void pressed()
	{
		foreach(Mutation m in theBlob.mutations)
		{
			if(m.revealed == false)
			{
				m.revealed = true;
				gm.popup.Show("New Mutation Revealed", "You have discovered the " + m.mutationName + " mutation!" );
				gm.popup.SetBlob(theBlob);
			}
		}


		theBlob.hasHatched = true;
		theBlob.age = 0;
		UpdateWithBlob(theBlob);
		gm.nm.blobPanel.UpdateBlobCellWithBlob(gm.nm.blobs.IndexOf(theBlob), theBlob);

		gm.UpdateAverageQuality();
		gm.nm.UpdateBreedCost();
	}

	// Update is called once per frame
	void Update () 
	{
		if (theBlob != null && !theBlob.hasHatched)
		{
			if (theBlob != null && !theBlob.hasHatched)
			{

				System.TimeSpan ts = (theBlob.hatchTime - System.DateTime.Now);
				progress.value = 1f - (float)(ts.TotalSeconds / gm.blobHatchDelay.TotalSeconds);
				progress.value = progress.value > 1f ? 1f : progress.value;
			}

			if (progress.value >= 1f && button.isEnabled == false)
				button.isEnabled = true;

			if (progress.value < 1f && button.isEnabled == true)
				button.isEnabled = false;
		}
	}
}
