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
	public UISprite cheeks;
	public UISprite heart;
	public UISprite egg;
	public UISprite geneIcon;
	public UISprite eggIcon;
	public UISprite qualityIndicator;
	public UILabel eggLabel;
	public UILabel geneCountLabel;
	public UILabel infoLabel;
	public UILabel levelLabel;
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
		eggIcon.gameObject.SetActive(false);
		geneIcon.gameObject.SetActive(false);
		eggLabel.text = "";
		infoLabel.text = "";
		heart.gameObject.SetActive(false);
		qualityIndicator.color = Color.white;
	}


	void Update () 
	{
		//blob belongs to nursery
		if (blob != null && parent == gm.nm.gameObject)
		{
			if (blob.hasHatched)
			{
				if (blob.breedReadyTime > System.DateTime.Now)
				{
					System.TimeSpan ts = (blob.breedReadyTime - System.DateTime.Now);
					progressBar.value = (float)(ts.TotalSeconds / blob.breedReadyDelay.TotalSeconds);
				}
				else
				{
					progressBar.value = 0f;
					blob.breedReadyTime = new System.DateTime(0);
					if(blob.egg != null)
					{
						gm.nm.SpawnEgg(blob.egg);
						gm.nm.blobPanel.UpdateBlobCellWithBlob(gm.nm.blobs.IndexOf(blob), blob);
						blob.egg = null;
					}
				}
			}
			else
			{
				if (blob.hatchTime > System.DateTime.Now)
				{
					System.TimeSpan ts = (blob.hatchTime - System.DateTime.Now);
					progressBar.value = (float)(ts.TotalSeconds / blob.blobHatchDelay.TotalSeconds);
				}
				else
					progressBar.value = 0f;

			}

		}

		//blob belongs to village
		if (blob != null && parent == gm.vm.gameObject)
		{
			if (blob.goldProductionTime > System.DateTime.Now)
			{
				System.TimeSpan ts = (blob.goldProductionTime - System.DateTime.Now);
				progressBar.value = 1f - (float)(ts.TotalSeconds / gm.blobGoldProductionDelay.TotalSeconds);
			}
			else if(gm.vm.IsMaxTributeReached())
			{
				progressBar.value = 1f;
			}
			else
			{
				blob.goldProductionTime = System.DateTime.Now + gm.blobGoldProductionDelay;
				gm.vm.AddTribute(blob.goldProduction);
			}
		}

	}

}
