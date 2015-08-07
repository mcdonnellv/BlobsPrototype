using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class VisitorManager : MonoBehaviour
{
	public GameManager gm;
	public TimeSpan visitorDelay;
	public DateTime visitorTime;
	public List<Blob> visitors  = new List<Blob>();
	public List<int> visitorCost = new List<int>();
	public List<DateTime> visitorTimers = new List<DateTime>();
	int selectedIndex = -1;
	int maxVisitors = 3;

	// Use this for initialization
	void Start () 
	{
		visitorDelay = new TimeSpan(0, (int)(UnityEngine.Random.Range(20,30)  * gm.timeScale), 0);
		visitorTime = DateTime.Now + visitorDelay;
	}


	public void GenerateVisitor()
	{
		Blob visitor = new Blob();

		visitor.male = (UnityEngine.Random.Range(0,2) == 0);
		visitor.Hatch();
		visitor.birthday = visitor.birthday - gm.breedingAge;
		visitor.quality = gm.GetAverageQuality();
		visitor.SetRandomTextures();
		visitor.id = gm.gameVars.blobsSpawned++;

		List<Gene> colorGenes = GeneManager.GetGenesOfType(gm.mum.genes, Gene.Type.BodyColor);
		List<Gene> badGenes = GeneManager.GetGenesWithNegativeEffect(gm.mum.genes);


		Gene goodGene = colorGenes[UnityEngine.Random.Range(0, colorGenes.Count)];
		Gene badGene1 = badGenes[UnityEngine.Random.Range(0, badGenes.Count)];
		badGenes.Remove(badGene1);
		Gene badGene2 = badGenes[UnityEngine.Random.Range(0, badGenes.Count)];

		visitor.genes.Add(goodGene);
		visitor.genes.Add(badGene1);
		visitor.genes.Add(badGene2);

		visitor.ActivateGenes();

		visitors.Add(visitor);
		visitorCost.Add(100);
		visitorTimers.Add(DateTime.Now + new TimeSpan(0,0,0,10));
	}
	

	public void DontHireVisitor()
	{
		selectedIndex = -1;
	}


	public void HireVisitor()
	{
		int index = selectedIndex;
		if (index < 0 || index >= visitors.Count)
			return;

		if(gm.gameVars.nurseryBlobs.Count >= gm.nm.maxBlobs)
		{
			gm.popup.Show("Cannot Join", "Breeding Room is full.");
			return;
		}

		if(visitorCost[index] > gm.gameVars.gold)
		{
			gm.popup.Show("Cannot Join", "Not enough gold to hire this blob.");
			return;
		}


		Blob visitor = visitors[index];
		gm.AddGold(-visitorCost[index]);
		visitors.RemoveAt(index);
		visitorCost.RemoveAt(index);
		visitorTimers.RemoveAt(index);
		gm.visitorButtons[visitors.Count].gameObject.SetActive(false);

		gm.nm.AddBlob(visitor);
		index = gm.nm.blobs.IndexOf(visitor);
		gm.nm.blobPanel.UpdateBlobCellWithBlob(index, visitor);
		gm.nm.PressGridItem(index);
	}


	public void ShowPopupForVisitor(int index)
	{
		gm.blobPopupChoice.ShowChoice(visitors[index], "This blob wants to join you", "",
		                    this, "HireVisitor",
		                    this, "DontHireVisitor");
		selectedIndex = index;
	}


	bool IsVisitorPopupShowing()
	{
		return (gm.popup.gameObject.activeInHierarchy && gm.popup.headerLabel.text == "This blob wants to join you");
	}


	public void VisitorButtonPressed(UIButton button)
	{
		int index = gm.visitorButtons.IndexOf(button);
		ShowPopupForVisitor(index);
	}


	// Update is called once per frame
	void Update () 
	{
		BlobQuality aveQuality = Blob.GetQualityFromValue(gm.GetAverageQuality());

		switch (aveQuality)
		{
		case BlobQuality.Poor: maxVisitors = 0;break;
		case BlobQuality.Fair: maxVisitors = 1;break;
		case BlobQuality.Good: maxVisitors = 2;break;
		case BlobQuality.Excellent:
		case BlobQuality.Outstanding: maxVisitors = 3;break;
		}

		if (visitorTime <= DateTime.Now)
		{
			if (visitors.Count < maxVisitors)
			{
				GenerateVisitor();
				gm.visitorButtons[visitors.Count - 1].gameObject.SetActive(true);
				visitorDelay = new TimeSpan(0, (int)(UnityEngine.Random.Range(20,30) * gm.timeScale), 0);
				visitorTime = DateTime.Now + visitorDelay;
			}
		}

		for (int i = 0; i < visitorTimers.Count; i++)
		{
			DateTime dt = visitorTimers[i];
			if (dt <= DateTime.Now)
			{
				if(IsVisitorPopupShowing())
					gm.popup.Hide();

				visitors.RemoveAt(i);
				visitorCost.RemoveAt(i);
				visitorTimers.RemoveAt(i);
				gm.visitorButtons[visitors.Count].gameObject.SetActive(false);
				i--;
			}
		}

		if (IsVisitorPopupShowing())
		{
			Blob visitor = visitors[selectedIndex];
			string genestr = "Genes: ";
			foreach(Gene g in visitor.genes)
				genestr += (g.negativeEffect ? "[FF9B9B]" : "[9BFF9B]") + g.geneName + ((visitor.genes.IndexOf(g) == (visitor.genes.Count - 1)) ? "[-]\n" : "[-], ");
			TimeSpan ts = visitorTimers[selectedIndex] - DateTime.Now;
			string timestr =  string.Format("{0:00}:{1:00}:{2:00} ", ts.TotalHours, ts.Minutes, ts.Seconds);
			gm.popup.bodyLabel.text = " Will you hire him for [FFD700]" + "100" + "g[-]?\n" + genestr +
				"Time Left: " + timestr;
		}

	}
}
