using UnityEngine;
using System.Collections;

public class BlobCell : MonoBehaviour 
{
	public GameManager gm;
	public GameObject parent;
	public UISlider progressBar;
	public UIButton button;
	public GameObject onMissionLabel;
	public UISprite body;
	public UISprite face;
	public UISprite lashes;
	public UISprite egg;
	public UILabel eggLabel;
	public Blob blob;

	// Use this for initialization
	public void Pressed () 
	{
		int index = transform.GetSiblingIndex();
		if(parent == gm.nm.gameObject)
			gm.nm.PressGridItem(index);

		if(parent == gm.vm.gameObject)
			gm.vm.PressGridItem(index);
	}

	public void Reset()
	{
		progressBar.value = 0f;
		eggLabel.text = "";
	}

	void Update () 
	{
		//blob belongs to nursery
		if (blob != null && parent == gm.nm.gameObject)
		{
			if (blob.breedReadyTime > 0f)
			{
				progressBar.value = (blob.breedReadyTime - Time.time) / gm.breedReadyDelay;
				progressBar.value = progressBar.value < 0f ? 0f : progressBar.value;

				if (blob.breedReadyTime < Time.time)
				{
					blob.breedReadyTime = 0f;
					if(blob.egg != null)
					{
						gm.nm.SpawnEgg(blob.egg);
						gm.nm.blobPanel.UpdateBlobCellWithBlob(gm.nm.blobs.IndexOf(blob), blob);
					}
				}
			}

			if (blob.hatchTime > 0f)
			{
				progressBar.value = (blob.hatchTime - Time.time) / gm.blobHatchDelay;
				progressBar.value = progressBar.value < 0f ? 0f : progressBar.value;
				
				if (blob.hatchTime < Time.time)
					blob.hatchTime = 0f;
			}

		}


		//blob belongs to village
		if (blob != null && parent == gm.vm.gameObject)
		{
			if(gm.vm.IsMaxTributeReached())
			{
				blob.goldProductionTime = 0f;
				progressBar.value = 0f;
			}

			progressBar.value = 1f - (blob.goldProductionTime - Time.time) / gm.blobGoldProductionSpeed;
			progressBar.value = progressBar.value < 0f ? 0f : progressBar.value;

			if (blob.goldProductionTime < Time.time)
			{
				blob.goldProductionTime = Time.time + gm.blobGoldProductionSpeed;
				gm.vm.AddTribute(blob.quality);
			}
		}

	}

}
