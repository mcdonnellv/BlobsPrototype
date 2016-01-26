using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreMenuListGrid : UIGrid {
	StoreManager storeManager  { get { return StoreManager.storeManager; } }
	GeneManager geneManager  { get { return GeneManager.geneManager; } }

	public void SetupStoreCells() {
		this.transform.DestroyChildren();
		foreach(GeneStoreItem gsi in storeManager.baseGenes) {
			StoreListCell cell = GetCellFromStoreItem(gsi);
			BaseGene baseGene = geneManager.GetBaseGeneByID(gsi.baseGeneId);
			GameObject cellGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Store Gene List Cell"));
			cell = cellGameObject.GetComponent<StoreListCell>();
			cell.transform.parent = transform;
			cell.transform.localScale = Vector3.one;
			cell.transform.localPosition = Vector3.zero;
			cell.titleLabel.text = baseGene.itemName;
			cell.icon.spriteName = baseGene.iconName;
			cell.icon.atlas = baseGene.iconAtlas;
			cell.newLabel.gameObject.SetActive(!gsi.alreadySeen);
			cell.baseGeneId = gsi.baseGeneId;
			if(gsi.researched)
				cell.costLabel.text = (baseGene.sellValue * 10f).ToString() + "[gold]";
			else
				cell.costLabel.text = "";
			if(gsi.researched && ItemManager.itemManager.HaveRequiredMaterialsForGene(baseGene)) {
				cell.titleLabel.color = Color.white;
				cell.costLabel.color = Color.white;
			}
			else {
				cell.titleLabel.color = ColorDefines.inactiveTextColor;
				cell.costLabel.color = ColorDefines.inactiveTextColor;
			}
		}
		
		base.Reposition();
	}


	public StoreListCell GetCellFromStoreItem(GeneStoreItem item) {
		int index = storeManager.baseGenes.IndexOf(item);
		return GetCellFromIndex(index);
	}


	public StoreListCell GetCellFromIndex(int index) {
		if(index >= 0 && index < transform.childCount) {
			Transform cellTransform = transform.GetChild(index);
			StoreListCell cell = cellTransform.GetComponent<StoreListCell>();
			return cell;
		}
		return null;
	}
}
