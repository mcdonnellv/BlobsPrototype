using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneAddPopup : Popup 
{
	public UILabel button2Label;
	public UIButton button2;
	public UIGrid cellContainer;
	public List<GeneAddCell> cells;
	public GeneAddCell geneAddCell;
	public GenePopup geneInfoPopup;
	public GameManager gm;
	public Blob blob;
	int selectedIndex = -1;
	
		
	public void Show (string header, Blob aBlob) 
	{
		base.Show(header);

		blob = aBlob;
		if(cells == null)
			cells = new List<GeneAddCell>();

		List<Gene> genes = gm.mum.genes;
		List<Gene> revealedGenes = new List<Gene>();
		foreach(Gene g in genes)
			if(g.revealed)
				revealedGenes.Add(g);
		foreach(Gene g in revealedGenes)
		{
			GeneAddCell gc = (GeneAddCell)GameObject.Instantiate(geneAddCell);
			gc.transform.SetParent(cellContainer.transform);
			gc.transform.localPosition = new Vector2(0f,0f);
			gc.transform.localScale = new Vector3(1f,1f,1f);
			gc.gameObject.SetActive(true);
			UIWidget wc = gc.GetComponent<UIWidget>();
			UIWidget wp = cellContainer.GetComponent<UIWidget>();
			wc.depth = wp.depth + 1;
			gc.geneAddPopup = this;


			gc.gene = g;
			gc.nameLabel.text = g.geneName;
			gc.rarityIndicator.color = Gene.ColorForRarity(g.rarity);
			UISprite sprite = gc.GetComponent<UISprite>();
			sprite.color = new Color(0.6f, 0.6f, 0.6f, 1f);

			cells.Add(gc);
			gc.name = "Cell" + string.Format("{0:00}", cells.IndexOf(gc));
		}

		cellContainer.Reposition();

		button1.onClick.Clear();
		button1.onClick.Add(new EventDelegate(this, "AddGeneToBlob"));

		button2.onClick.Clear();
		button2.onClick.Add(new EventDelegate(this, "Hide"));
		button2Label.text = "Cancel";

		if(cells.Count > 0)
			PressedCell(0);
	}

	public void Pressed(int index)
	{
		GeneAddCell gc = cells[index];
		geneInfoPopup.Show(gc.gene);
	}

	public void PressedCell(int index)
	{
		GeneAddCell gc = cells[index];
		selectedIndex = index;

		foreach(GeneAddCell gac in cells)
		{
			UISprite s = gac.GetComponent<UISprite>();
			s.color = new Color(0.6f, 0.6f, 0.6f, 1f);
		}

		UISprite sprite = gc.GetComponent<UISprite>();
		sprite.color = new Color(1f, 0.773f, 0.082f ,1f);

		int cost = 1;
		button1Label.text = "Add Gene [C59F76]" + cost.ToString()+ "c[-]";
	}


	public void Hide()
	{
		cellContainer.transform.DestroyChildren();
		cells.Clear();
		base.Hide();
	}

	public void AddGeneToBlob()
	{
		int cost = 1;
		if (gm.chocolate < cost) 
		{gm.blobPopup.Show(blob, "Cannot Add Gene", "You do not have enough Chocolate."); return;}

		GeneAddCell gc = cells[selectedIndex];
		Gene g = gc.gene;

		foreach(Gene g1 in blob.genes)
			if(g == g1)
		{gm.blobPopup.Show(blob, "Cannot Add Gene", "Blob already has this gene."); return;}


		Hide();
		gm.AddChocolate(-1);
		blob.unprocessedGenes.Add(g);
		List<Gene> temp = new List<Gene>();
		temp.Add(g);
		blob.SetGeneActivationForGene(g);
		blob.ApplyGeneEffects(temp);
		gm.nm.UpdateAllBlobCells();
		gm.nm.infoPanel.UpdateWithBlob(blob);
	}
}
