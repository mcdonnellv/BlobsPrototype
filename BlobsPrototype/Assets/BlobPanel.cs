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
		if(blob == null)
		{
			bc.gameObject.SetActive(false);
			return;
		}

		bc.gameObject.SetActive(true);

		bc.egg.gameObject.SetActive(!blob.hasHatched);
		bc.body.gameObject.SetActive(blob.hasHatched);
		bc.face.gameObject.SetActive(blob.hasHatched);
		bc.lashes.gameObject.SetActive(blob.hasHatched && !blob.male);


		UISprite bg = bc.GetComponent<UISprite>();
		UIButton button = bc.GetComponentInChildren<UIButton>();
		bg.color = (blob.male) ? new Color(0.62f, 0.714f, 0.941f,1f) : new Color(0.933f, 0.604f, 0.604f, 1f);
		bg.color = (blob.hasHatched) ? bg.color : Color.grey;
		button.defaultColor = button.hover = bg.color;

		UILabel[] labels = bc.GetComponentsInChildren<UILabel>();
		labels[0].text = (blob.male || blob.age < gm.breedingAge) ? "" : (gm.maxBreedcount - blob.breedCount).ToString();

		bc.body.color = Blob.GetColorFromEnum(blob.color);

		float a = (float)(blob.age > 3 ? 3 : blob.age);
		float s = .3f + (.7f * (a / 3f));
		s = (s > 1f) ? 1f : s;
		int pixels = (int)(s * 50f);
		bc.body.SetDimensions(pixels, pixels);
		bc.face.SetDimensions(pixels, pixels);
		bc.lashes.SetDimensions(pixels, pixels);

		bc.onMissionLabel.SetActive(blob.onMission);

	}
	

	// Update is called once per frame
	void Update () 
	{
	
	}
}
