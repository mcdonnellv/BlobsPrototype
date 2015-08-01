using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CastleManager : MonoBehaviour 
{

	public GameManager gm;	
	public BlobPanel blobPanel;
	public InfoPanel infoPanel;
	public List<Blob> blobs {get{return gm.gameVars.castleBlobs;}}
	public UILabel sellButtonLabel;
	public UILabel moveLabel;
	public bool castleExists;
	int maxBlobs;
	int curSelectedIndex;


	void Start () 
	{
		blobPanel.Init();
		infoPanel.UpdateWithBlob(null);;
		maxBlobs = 20;
		PressGridItem(0);
		castleExists = false;
	}
	
	
	public void newBlobAdded(Blob blob)
	{
		int index = blobs.IndexOf(blob);
		BlobCell bc = blobPanel.blobCells[index];
		bc.progressBar.value = 0f;
		PressGridItem(index);
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
		
		moveLabel.text = "Move To\nNursery (-" + ((int)(bc.blob.quality * 30f)).ToString() + "g)";
	}
	
	
	void DeleteBlob(Blob blob)
	{
		blobs.Remove(blob);
		if (curSelectedIndex >= blobs.Count)
			PressGridItem(curSelectedIndex - 1);
		
		for (int i = curSelectedIndex; i < blobs.Count; i++)
			blobPanel.UpdateBlobCellWithBlob(i, blobs[i]);
		
		blobPanel.UpdateBlobCellWithBlob(blobs.Count, null);
		infoPanel.UpdateWithBlob(blobs[curSelectedIndex]);
		
		gm.UpdateAverageQuality();
		gm.nm.UpdateBreedCost();
	}
	
	
	public void PressSellButton()
	{
		Blob blob = blobs[curSelectedIndex];
		BlobCell bc = blobPanel.blobCells[curSelectedIndex];
		if(bc.progressBar.value > 0f)
			return;
		
		if (blob.onMission)
			return;
		
		bc.Reset();
		gm.AddGold(gm.sellValue);
		DeleteBlob(blob);
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
		if (blobs.Count <= 0 || curSelectedIndex >= blobs.Count || gm.nm.IsFull())
			return;
		
		Blob blob = blobs[curSelectedIndex];
		
		int cost = (int)(blob.quality * 30f);
		if (gm.gold < cost)
			return;
		
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
	}
	
	
	public bool IsFull() {return(blobs.Count >= maxBlobs);}


	// Update is called once per frame
	void Update () 
	{
	}
}
