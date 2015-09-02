using UnityEngine;
using System;
using System.Collections;

public class BlobInfoContextMenu : MonoBehaviour {

	Blob blob;
	public UILabel statusLabel;
	public UILabel levelLabel;
	public UILabel qualityLabel;
	public UILabel genderLabel;
	public UILabel eggsLabel;
	public UILabel ageLabel;
	public UISprite genderSprite;
	public UISprite eggSprite;

	public void DisplayWithBlob(Blob blobParam)
	{
		blob = blobParam;
		statusLabel.text = blob.GetBlobStateString();
		genderLabel.text = blob.male ? "Male" : "Female";
		levelLabel.text = "Level " + blob.level.ToString();
		qualityLabel.text = "[" + ColorToHex(Blob.ColorForQuality(blob.quality)) + "]" + blob.quality.ToString() + "[-]";
		eggsLabel.text = blob.male ? "None" : blob.unfertilizedEggs.ToString() + " Eggs";
		TweenPosition tp = gameObject.GetComponent<TweenPosition>();
		tp.PlayForward();
		tp.enabled = true;
	}

	string ColorToHex(Color32 color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (blob != null && blob.hasHatched)
		{
			TimeSpan blobAge = blob.age;
			if (blobAge.Days > 0)
				ageLabel.text =  string.Format("{0:0} days", blobAge.Days) + " old";
			else if (blobAge.Hours > 0)
				ageLabel.text = string.Format("{0:0} hrs", blobAge.Hours) + " old";
			else
				ageLabel.text = string.Format("{0:0} mins", blobAge.Minutes) + " old";
		}
	
	}
}
