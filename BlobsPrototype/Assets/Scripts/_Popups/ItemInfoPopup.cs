using UnityEngine;
using System.Collections;

public class ItemInfoPopup : GenericGameMenu {

	public UIButton deleteButton;
	public UILabel rarityLabel;
	public UILabel infoLabelSingle;
	public UILabel infoLabelDouble;
	public UISprite icon;
	public GameObject singlePanel;
	public GameObject doublePanel;
	public GameObject singlePanelLabel;
	public UIGrid doublePanelUpperGrid;
	public UIGrid doublePanelLowerGrid;
	public Gene gene = null;
	public Item item = null;
	public static Vector3 position1 = new Vector3(330f, -650f, 0f);
	public static Vector3 position2 = new Vector3(-30f, -650f, 0f);

	public void Show() {
		gameObject.SetActive(true);
		if(displayed) // We are just changing the displayed info
			FlashChangeAnim();
		else 
			base.Show();
		deleteButton.gameObject.SetActive(false);
		AdjustPosition();
	}


	public void AdjustPosition() {
		HudManager hudManager = GameObject.Find("HudManager").GetComponent<HudManager>();
		transform.localPosition = position2;
		if(hudManager.inventoryMenu.displayed)
			transform.localPosition = position1;
	}


	public void DeleteButtonPressed() {}

	public void ClearFields() {
		headerLabel.text = "";
		rarityLabel.text = "";
		infoLabelSingle.text = "";
		infoLabelDouble.text = "";
	}


	public void ShowInfoForGene(Gene g) {
		gene = g;
		doublePanel.gameObject.SetActive(true);
		singlePanel.gameObject.SetActive(false);

		headerLabel.text = gene.geneName;
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(gene.quality)) + gene.quality.ToString() + " Gene[-]";
		infoLabelSingle.text = gene.description;
		doublePanelUpperGrid.transform.DestroyChildren();
		doublePanelLowerGrid.transform.DestroyChildren();
		icon.spriteName = Gene.GetSpriteNameWithQuality(gene.quality);

		infoLabelDouble.text = gene.description;
		if(gene.state != GeneState.Active)
			infoLabelDouble.text += " when activated";
		
		infoLabelDouble.color = (gene.state == GeneState.Active)?Color.white : Color.gray;

		foreach(GeneActivationRequirement req in gene.activationRequirements) {
			int index = gene.activationRequirements.IndexOf(req);
			GameObject statGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Requirement Container"));
			statGameObject.transform.SetParent(doublePanelLowerGrid.transform);
			statGameObject.transform.localScale = new Vector3(1f,1f,1f);
			statGameObject.transform.localPosition = new Vector3(0f, -14f + index * -26f, 0f);
			UISprite[] sprites = statGameObject.GetComponentsInChildren<UISprite>();
			sprites[0].atlas = req.item.iconAtlas;
			sprites[0].spriteName = req.item.iconName;
			UILabel[] labels = statGameObject.GetComponentsInChildren<UILabel>();
			labels[0].text = "Feed " + req.amountNeeded.ToString() + " " + req.item.itemName;
			labels[1].text = req.amountConsumed.ToString() + " / " + req.amountNeeded.ToString();
			labels[0].color = (req.fulfilled)?Color.white : Color.gray;
			labels[1].color = (req.fulfilled)?Color.white : Color.gray;
		}

		doublePanelUpperGrid.Reposition();
		doublePanelLowerGrid.Reposition();
	}

	public void ShowInfoForItem(Item i) {
		item = i;
		doublePanel.gameObject.SetActive(false);
		singlePanel.gameObject.SetActive(true);
		
		headerLabel.text = item.itemName;
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(item.quality)) + item.quality.ToString() + " Item[-]";
		infoLabelSingle.text = item.description;
		icon.atlas = item.iconAtlas;
		icon.spriteName = item.iconName;
	}

	public void FlashChangeAnim() {
		animationWindow.ResetToBeginning();
		animationWindow.PlayForward();
	} 
}
