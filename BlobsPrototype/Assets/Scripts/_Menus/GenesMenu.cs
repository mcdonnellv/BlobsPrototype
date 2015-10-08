using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenesMenu : MonoBehaviour {
	
	public GameObject grid;
	public ItemInfoPopup popup;
	GameObject geneSlotHighlight;
	int selectedIndex = -1;

	public void Show() {
		GeneManager geneManager = GameObject.Find ("GeneManager").GetComponent<GeneManager>();

		foreach(Gene g in geneManager.storedGenes) {
			if(g == null)
				continue;
			Transform parentSocket = grid.transform.GetChild(geneManager.storedGenes.IndexOf(g));

			GameObject go = g.CreateGeneGameObject();
			go.transform.parent = parentSocket;
			go.transform.localScale = new Vector3(1f,1f,1f);
			go.transform.localPosition = new Vector3(0f,0f,0f);
		}

		if(selectedIndex == -1 && geneManager.storedGenes.Count > 0)
			ShowInfoForGene(geneManager.storedGenes[0]);
		else if(selectedIndex != -1 && selectedIndex < geneManager.storedGenes.Count)
			ShowInfoForGene(geneManager.storedGenes[selectedIndex]);
		else if(geneManager.storedGenes.Count == 0) {
			selectedIndex = -1;
			popup.nameLabel.text = "";
			popup.rarityLabel.text = "";
			popup.infoLabel1.text = "";
			popup.infoLabel2.text = "";
			popup.Hide();
		}
	}


	public void ShowInfoForGene(Gene gene) {
		GenePointer genePointer = null;
		foreach(Transform socket in grid.transform) {
			GenePointer gp = socket.GetComponentInChildren<GenePointer>();
			if(gp != null && gp.gene == gene)
				genePointer = gp;
		}

		if(genePointer != null)
			ShowInfoForGeneGameObject(genePointer);
	}


	public void ShowInfoForGeneGameObject(GenePointer genePointer) {
		popup.Show();
		Gene gene = genePointer.gene;
		Transform parentSocket = genePointer.transform.parent;
		selectedIndex = parentSocket.GetSiblingIndex();

		if(geneSlotHighlight != null)
			GameObject.Destroy(geneSlotHighlight);
		geneSlotHighlight = (GameObject)GameObject.Instantiate(Resources.Load("Gene Slot Highlight"));
		geneSlotHighlight.transform.parent = parentSocket;
		geneSlotHighlight.transform.localScale = new Vector3(1f,1f,1f);
		geneSlotHighlight.transform.localPosition = new Vector3(0f,0f,0f);
		geneSlotHighlight.GetComponent<UISprite>().depth = parentSocket.GetComponent<UISprite>().depth;

		
		popup.nameLabel.text = gene.itemName;
		popup.rarityLabel.text = ColorDefines.ColorToHexString(ColorDefines.ColorForQuality(gene.quality)) + gene.quality.ToString() + "[-]";
		popup.infoLabel1.text = gene.description;
		popup.infoLabel2.text = "";
		popup.infoLabel2.transform.DestroyChildren();
		UISprite originalIcon = genePointer.gameObject.GetComponent<UISprite>();
		popup.icon.atlas = originalIcon.atlas;
		popup.icon.spriteName = originalIcon.spriteName;

		foreach(Stat s in gene.stats) {
			int index = gene.stats.IndexOf(s);
			GameObject statGameObject = (GameObject)GameObject.Instantiate(Resources.Load("Stat Container"));
			statGameObject.transform.SetParent(popup.infoLabel2.transform);
			statGameObject.transform.localScale = new Vector3(1f,1f,1f);
			statGameObject.transform.localPosition = new Vector3(0f, -14f + index * -26f, 0f);
			UISprite sprite = statGameObject.GetComponentInChildren<UISprite>();
			sprite.alpha = 0f;
			UILabel[] labels = statGameObject.GetComponentsInChildren<UILabel>();
			labels[0].text = (s.modifier == Stat.Modifier.Added) ? ("+" + s.amount.ToString()) : ("+" + s.amount.ToString() + "%");
			labels[0].depth = popup.infoLabel2.depth + 2;
			labels[1].text = "[fist]" + s.id.ToString();
			labels[1].depth = popup.infoLabel2.depth + 2;
		}
	}

	void Update() {

		if(geneSlotHighlight != null)
			geneSlotHighlight.transform.localPosition = new Vector3(0f,0f,0f);
	}
}
