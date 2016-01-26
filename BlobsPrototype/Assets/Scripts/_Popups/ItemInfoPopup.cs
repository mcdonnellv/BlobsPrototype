using UnityEngine;
using System.Collections;

public class ItemInfoPopup : GenericGameMenu {

	public UIButton deleteButton;
	public UILabel deleteButtonLabel;
	public UILabel rarityLabel;
	public UILabel infoLabel;
	public UISprite icon;
	ItemManager itemManager { get { return ItemManager.itemManager; } }
	[HideInInspector] public Gene gene = null;
	[HideInInspector] public Item item = null;
	[HideInInspector] public GenericGameMenu owner = null;
	public int defaultWindowHeight = 414;


	public void GameMenuClosing(GenericGameMenu menu) { if(menu == owner) Hide();}
	public override void Cleanup() { base.Cleanup(); owner = null; }
	public void DeleteButtonPressed() { owner.SendMessage("DeleteButtonPressed"); }


	public void Show(GenericGameMenu caller, Item i) {
		Show(caller);
		if(i != null) PopulateInfoFromItem(i);
	}

	public void Show(GenericGameMenu caller, Gene g) {
		Show(caller);
		if(g != null) PopulateInfoFromGene(g);
	}

	public void Show(GenericGameMenu caller) {
		gameObject.SetActive(true);
		if(IsDisplayed() && owner == caller) // We are just changing the displayed info
			FlashChangeAnim();
		else 
			base.Show();
		owner = caller;
		ShowDeleteButton(false);
	}


	public void ResizeWindow() {
		int height = defaultWindowHeight;
		if(!deleteButton.gameObject.activeInHierarchy)
			height -= 40;
		height -= 120;
		window.GetComponent<UISprite>().height = height;
	}


	public void ShowDeleteButton(bool enable) { 
		deleteButton.gameObject.SetActive(enable);
		ResizeWindow();
	}


	public void ClearFields() {
		headerLabel.text = "";
		rarityLabel.text = "";
		infoLabel.text = "";
	}


	public void PopulateInfoFromGene(Gene g) {
		gene = g;
		headerLabel.text = gene.itemName.ToUpper();
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(gene.quality)) + gene.quality.ToString() + " Gene[-]";
		icon.spriteName = g.iconName;
		icon.atlas = g.iconAtlas;
		infoLabel.text = gene.description;
		deleteButtonLabel.text = "Delete";
		ResizeWindow();
	}

	public void PopulateInfoFromItem(Item i) {
		item = i;
		headerLabel.text = item.itemName;
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(item.quality)) + item.quality.ToString() + " Item[-]";
		infoLabel.text = item.description;
		icon.atlas = item.iconAtlas;
		icon.spriteName = item.iconName;
		icon.color = ColorDefines.IconColorFromIndex(item.iconTintIndex);
		deleteButtonLabel.text = "Sell for " + item.sellValue.ToString() + "[gold]";
		ResizeWindow();
	}
}
