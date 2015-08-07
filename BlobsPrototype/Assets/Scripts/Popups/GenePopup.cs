using UnityEngine;
using System.Collections;

public class GenePopup : Popup 
{
	public UILabel nameLabel;
	public UILabel rarityLabel;

	public void Show(Gene gene)
	{
		string rarityString = Gene.RarityStringFromrarity(gene.rarity);
		string colorString = Gene.HexColorStringFromRarity(gene.rarity);

		base.Show("Gene Info", gene.description);
		nameLabel.text = gene.geneName;
		rarityLabel.text = colorString + rarityString + "[-]";
		gameObject.SetActive(true);
	}
}