using UnityEngine;
using System.Collections;

public class ItemInfoPopup : MonoBehaviour {

	public UITweener animationWindow;
	public UIButton deleteButton;
	public UILabel nameLabel;
	public UILabel rarityLabel;
	public UILabel infoLabelSingle;
	public UILabel infoLabelDouble;
	public UISprite icon;
	public GameObject singlePanel;
	public GameObject doublePanel;
	public GameObject singlePanelLabel;
	public UIGrid doublePanelUpperGrid;
	public UIGrid doublePanelLowerGrid;

	public void Show() {
		gameObject.SetActive(true);
		transform.localScale = new Vector3(0,0,0);
		animationWindow.onFinished.Clear();
		animationWindow.PlayForward();
		deleteButton.gameObject.SetActive(false);
	}
	
	public void Hide() {
		animationWindow.onFinished.Add(new EventDelegate(this, "DisableWindow"));
		animationWindow.PlayReverse();
	}

	void DisableWindow() {
		animationWindow.onFinished.Clear();
		gameObject.SetActive(false);
	}

	public void DeleteButtonPressed() {}


	public void ShowInfoForGene(Gene gene) {
		doublePanel.gameObject.SetActive(true);
		singlePanel.gameObject.SetActive(false);

		nameLabel.text = gene.geneName;
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(gene.quality)) + gene.quality.ToString() + " Gene[-]";
		infoLabelSingle.text = gene.description;
		doublePanelUpperGrid.transform.DestroyChildren();
		doublePanelLowerGrid.transform.DestroyChildren();

		//UISprite originalIcon = gene.CreateGeneGameObject .genePointer.gameObject.GetComponent<UISprite>();
		//icon.atlas = null;//originalIcon.atlas;
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

	public void ShowInfoForItem(Item item) {
		doublePanel.gameObject.SetActive(false);
		singlePanel.gameObject.SetActive(true);
		
		nameLabel.text = item.itemName + "   x" + item.count;
		rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(item.quality)) + item.quality.ToString() + " Item[-]";
		infoLabelSingle.text = item.description;
		icon.atlas = item.iconAtlas;
		icon.spriteName = item.iconName;
	}
}
