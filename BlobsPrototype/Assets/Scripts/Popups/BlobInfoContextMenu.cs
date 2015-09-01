using UnityEngine;
using System.Collections;

public class BlobInfoContextMenu : MonoBehaviour {

	public Blob blob;
	public UILabel statusLabel;
	public UILabel levelLabel;
	public UILabel qualityLabel;
	public UILabel genderLabel;
	public UILabel eggsLabel;
	public UILabel ageLabel;
	public UISprite genderSprite;
	public UISprite eggSprite;

	public void DisplayWithBlob(Blob blob)
	{
		genderLabel.text = blob.male ? "Male" : "Female";
		//genderQuality.text = 
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
