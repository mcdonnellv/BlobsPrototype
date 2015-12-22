using UnityEngine;
using System.Collections;

public class ItemInfoPopup : GenericGameMenu {

	public UIButton deleteButton;
	public UILabel deleteButtonLabel;
	public UILabel rarityLabel;
	public UILabel upperPanelInfoLabel;
	public UISprite icon;
	public UIGrid lowerPanelGrid;
	public UIWidget upperPanel;
	public UIWidget lowerPanel;
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
		if(!lowerPanel.gameObject.activeInHierarchy)
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
		upperPanelInfoLabel.text = "";
	}


	public void PopulateInfoFromGene(Gene g) {
		gene = g;

		headerLabel.text = gene.itemName;
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(gene.quality)) + gene.quality.ToString() + " Gene[-]";
		lowerPanelGrid.transform.DestroyChildren();
		icon.spriteName = g.iconName;
		icon.atlas = g.iconAtlas;
		upperPanelInfoLabel.text = gene.description;
		if(gene.state != GeneState.Active && gene.activationRequirements.Count > 0)
			upperPanelInfoLabel.text += " when activated";

		if(gene.activationRequirements.Count > 0) {
			lowerPanel.gameObject.SetActive(true);
			foreach(GeneActivationRequirement req in gene.activationRequirements) {
				int index = gene.activationRequirements.IndexOf(req);
				GameObject statGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Requirement Container"));
				statGameObject.transform.SetParent(lowerPanelGrid.transform);
				statGameObject.transform.localScale = new Vector3(1f,1f,1f);
				statGameObject.transform.localPosition = new Vector3(0f, -14f + index * -26f, 0f);
				UISprite[] sprites = statGameObject.GetComponentsInChildren<UISprite>();
				BaseItem item = itemManager.GetBaseItemByID(req.itemId);
				sprites[0].atlas = item.iconAtlas;
				sprites[0].spriteName = item.iconName;
				UILabel[] labels = statGameObject.GetComponentsInChildren<UILabel>();
				labels[0].text = "Feed " + req.amountNeeded.ToString() + " " + item.itemName;
				labels[1].text = req.amountConsumed.ToString() + " / " + req.amountNeeded.ToString();
				labels[0].color = (req.fulfilled) ? ColorDefines.positiveTextColor : ColorDefines.inactiveTextColor;
				labels[1].color = (req.fulfilled) ? ColorDefines.positiveTextColor : ColorDefines.inactiveTextColor;
			}
		}
		else
			lowerPanel.gameObject.SetActive(false);

		deleteButtonLabel.text = "Delete";
		lowerPanelGrid.Reposition();
		ResizeWindow();
	}

	public void PopulateInfoFromItem(Item i) {
		item = i;
		lowerPanel.gameObject.SetActive(false);
		headerLabel.text = item.itemName;
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(item.quality)) + item.quality.ToString() + " Item[-]";
		upperPanelInfoLabel.text = item.description;
		icon.atlas = item.iconAtlas;
		icon.spriteName = item.iconName;
		icon.color = ColorDefines.IconColorFromIndex(item.iconTintIndex);
		deleteButtonLabel.text = "Sell for " + item.sellValue.ToString() + "[gold]";
		ResizeWindow();
	}
}
