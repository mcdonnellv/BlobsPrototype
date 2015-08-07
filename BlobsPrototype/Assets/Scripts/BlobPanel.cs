using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlobPanel : MonoBehaviour 
{
	public GameManager gm;
	public List<BlobCell> blobCells;
	public int maxCells = 20;

	// Use this for initialization
	void Start () 
	{

	}

	public void Init()
	{
		blobCells = new List<BlobCell>();
		
		foreach(Transform blobCellTransform in transform)
		{
			BlobCell bc = blobCellTransform.GetComponent<BlobCell>();
			blobCells.Add(bc);
			bc.gameObject.SetActive(false);
		}
	}


	public void UpdateBlobCellWithBlob(int index, Blob blob)
	{
		BlobCell bc = blobCells[index];
		bc.blob = blob;

		if(blob == null)
		{
			bc.gameObject.SetActive(false);
			return;
		}

		bc.gameObject.SetActive(true);

		bc.egg.gameObject.SetActive(!blob.hasHatched);
		bc.body.gameObject.SetActive(blob.hasHatched);
		bc.face.gameObject.SetActive(blob.hasHatched);
		bc.cheeks.gameObject.SetActive(blob.hasHatched && !blob.male);


		UISprite bg = bc.GetComponent<UISprite>();
		UIButton button = bc.GetComponentInChildren<UIButton>();
		float c = 1.0f;
		bg.color = (blob.male) ? new Color(0.62f*c, 0.714f*c, 0.941f*c,1f) : new Color(0.933f*c, 0.604f*c, 0.604f*c, 1f);
		bg.color = (blob.hasHatched) ? bg.color : Color.grey;
		button.defaultColor = button.hover = bg.color;

		UILabel[] labels = bc.GetComponentsInChildren<UILabel>();
		bc.eggLabel.text = (blob.male || !blob.hasHatched) ? "" : (gm.maxBreedcount - blob.breedCount).ToString();

		Texture tex = blob.bodyPartSprites["Body"];
		bc.body.spriteName = tex.name;
		tex = blob.bodyPartSprites["Eyes"];
		bc.face.spriteName = tex.name;

		bc.body.color = blob.color;

		int pixels = (int)(blob.BlobScale() * 50f);
		bc.body.SetDimensions(pixels, pixels);
		bc.face.SetDimensions(pixels, pixels);
		bc.cheeks.SetDimensions(pixels, pixels);

		bc.onMissionLabel.SetActive(blob.onMission);
	}
	

	// Update is called once per frame
	void Update () 
	{
		foreach(BlobCell bc in blobCells)
		{
			if (bc.blob != null && bc.blob.hasHatched && bc.blob.age < gm.breedingAge)
			{
				int pixels = (int)(bc.blob.BlobScale() * 50f);
				bc.body.SetDimensions(pixels, pixels);
				bc.face.SetDimensions(pixels, pixels);
				bc.cheeks.SetDimensions(pixels, pixels);
			}
		}
	}
}
