using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageManager : MonoBehaviour 
{
	public GameManager gm;	
	public BlobPanel blobPanel;
	public InfoPanel infoPanel;
	public List<Blob> blobs {get{return gm.gameVars.villageBlobs;}}
	public UILabel sellButtonLabel;
	public UILabel tributeLabel;
	public UILabel moveLabel;
	public bool villageExists;
	int maxBlobs;
	int curSelectedIndex;
	float tribute;
	float maxTribute;


	void Start () 
	{
		return;
		blobPanel.Init();
		infoPanel.UpdateWithBlob(null);
		maxBlobs = 20;
		tribute = 0;
		maxTribute = 0;
		PressGridItem(0);
		villageExists = false;
	}


	public void newBlobAdded(Blob blob)
	{
		int index = blobs.IndexOf(blob);
		BlobCell bc = blobPanel.blobCells[index];
		bc.progressBar.value = 0f;
		PressGridItem(index);
		UpdateMaxTribute();
		blob.goldProductionTime = System.DateTime.Now + gm.blobGoldProductionDelay;
	}


	int GetSellValue()
	{
		return gm.sellValue;
	}


	public void UpdateSellValue()
	{
		sellButtonLabel.text = "Sell (+" + GetSellValue().ToString() + "g)";
	}


	public void PressGridItem(int index)
	{
		if(index < 0)
			return;

		if(blobs.Count <= 0)
			return;

		curSelectedIndex = index;
		infoPanel.UpdateWithBlob(blobs[index]);
		UpdateSellValue();
		
		BlobCell bc = blobPanel.blobCells[index];
		bc.gameObject.SendMessage("OnClick");

		moveLabel.text = "Move To\nNursery (-" + ((int)(bc.blob.level * 30f)).ToString() + "g)";
	}


	void DeleteBlob(Blob blob)
	{
		blobs.Remove(blob);
		if (curSelectedIndex >= blobs.Count)
			PressGridItem(curSelectedIndex - 1);
		
		for (int i = curSelectedIndex; i < blobs.Count; i++)
			blobPanel.UpdateBlobCellWithBlob(i, blobs[i]);
		
		blobPanel.UpdateBlobCellWithBlob(blobs.Count, null);
		if(curSelectedIndex < blobs.Count)
			infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);
		else
			infoPanel.UpdateWithBlob(null);
		gm.UpdateAverageLevel();
		UpdateMaxTribute();
	}


	public void PressSellButton()
	{
		gm.TrySellBlob(blobs[curSelectedIndex], this);
	}


	void SellBlobFinal() 
	{
		Blob blob = blobs[curSelectedIndex];
		BlobCell bc = blobPanel.blobCells[curSelectedIndex];
		bc.Reset();
		gm.AddGold(gm.sellValue);
		DeleteBlob(blob);
	}


	public void PressCollectButton()
	{
		gm.AddGold((int)tribute);
		tribute = 0;
		UpdateTributeLabel();

		foreach(Blob blob in blobs)
		{
			if(blob.goldProductionTime < System.DateTime.Now)
				blob.goldProductionTime = System.DateTime.Now + gm.blobGoldProductionDelay;
		}

	}

	public void UpdateMaxTribute()
	{
		maxTribute = (int)(blobs.Count * gm.GetAverageLevel() * gm.tributeMaxMulitplier);
		UpdateTributeLabel();
	}


	public void UpdateTributeLabel()
	{
		tributeLabel.text = "Tribute: " + ((int)tribute).ToString() + "g\nMax: " + ((int)maxTribute).ToString() + "g";
	}


	public void UpdateAllBlobCells()
	{
		if(blobs == null)
			return;

		foreach(Blob blob in blobs)
			blobPanel.UpdateBlobCellWithBlob(blobs.IndexOf(blob), blob);
		if(curSelectedIndex > blobs.Count)
			infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);
	}


	public void PressToNurseryButton()
	{
		if (blobs.Count <= 0 || curSelectedIndex >= blobs.Count)
			return;
		
		Blob blob = blobs[curSelectedIndex];

		if (gm.nm.IsFull()) 
		{gm.blobPopup.Show(blob, "Cannot Move", "There is no more space in the Nursery."); return;}
		
		int cost = (int)(blob.level * 30f);
		if (gm.gold < cost) 
		{gm.blobPopup.Show(blob, "Cannot Move", "You do not have enough Gold."); return;}


		gm.AddGold(-cost);
		BlobCell bc = blobPanel.blobCells[curSelectedIndex];
		bc.Reset();
		
		gm.nm.blobs.Add(blob);
		gm.nm.blobPanel.UpdateBlobCellWithBlob(gm.nm.blobs.IndexOf(blob), blob);
		
		blobs.Remove(blob);
		if (curSelectedIndex >= blobs.Count)
			PressGridItem(curSelectedIndex - 1);
		else
			PressGridItem(curSelectedIndex);
		
		for (int i = curSelectedIndex; i < blobs.Count; i++)
			blobPanel.UpdateBlobCellWithBlob(i, blobs[i]);
		
		blobPanel.UpdateBlobCellWithBlob(blobs.Count, null);
		if(curSelectedIndex < blobs.Count)
			infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);
		else
			infoPanel.UpdateWithBlob(null);

		UpdateMaxTribute();
	}


	public bool IsFull() {return(blobs.Count >= maxBlobs);}


	public bool IsMaxTributeReached() {return (tribute >= maxTribute);}


	public void AddTribute(float value)
	{
		tribute += value;
		if(tribute >= maxTribute)
			tribute = maxTribute;
		UpdateTributeLabel();
	}

	// Update is called once per frame
	void Update () 
	{
	}
}
