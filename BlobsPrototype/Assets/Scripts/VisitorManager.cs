using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class VisitorManager : MonoBehaviour {
//	public GameManager gm;
//	public TimeSpan visitorDelay;
//	public DateTime visitorTime;
//	public List<Blob> visitors  = new List<Blob>();
//	public List<int> visitorCost = new List<int>();
//	public List<DateTime> visitorTimers = new List<DateTime>();
//	int selectedIndex = -1;
//	int maxVisitors = 3;
//	
//	// Use this for initialization
//	void Start () 
//	{
//		visitorDelay = new TimeSpan(0, (int)(GetNewVisitorDelayMins()), 0);
//		visitorTime = DateTime.Now + visitorDelay;
//	}
//	
//	
//	int GetNewVisitorDelayMins()
//	{
//		int baseDelayMins = 1;//2;
//		baseDelayMins += gm.gameVars.visitorsSpawned * 2;
//		return (int)(Mathf.Clamp(baseDelayMins, 2, 60 * 4) * gm.timeScale);
//	}
//	
//	
//	public void GenerateVisitor()
//	{
//		Blob visitor = new Blob();
//		
//		visitor.male = true;
//		visitor.Hatch(true);
//		visitor.birthday = visitor.birthday - gm.breedingAge;
//		visitor.level = gm.GetAverageLevel();
//		visitor.quality = Blob.GetRandomQuality();
//		visitor.SetRandomTextures();
//		visitor.id = gm.gameVars.blobsSpawned++;
//		gm.gameVars.visitorsSpawned++;
//		
//		List<Gene> goodGenes = GeneManager.GetGenesWithPositiveEffect(gm.mum.genes);
//		List<Gene> badGenes = GeneManager.GetGenesWithNegativeEffect(gm.mum.genes);
//		
//		int allowedGenes = visitor.allowedGeneCount;
//		
//		for(int i=0; i<allowedGenes; i++)
//		{
//			Gene geneToadd = null;
//			if((i%3) == 0)
//			{
//				geneToadd = GeneManager.GetRandomGeneBasedOnRarity(goodGenes);
//				goodGenes.Remove(geneToadd);
//			}
//			else if(i==1)
//			{
//				geneToadd = badGenes[UnityEngine.Random.Range(0, badGenes.Count)];
//				badGenes.Remove(geneToadd);
//			}
//			
//			visitor.unprocessedGenes.Add(geneToadd);
//		}
//		
//		if(gm.gameVars.visitorsSpawned == 1)
//		{
//			visitor.unprocessedGenes.Clear();
//			visitor.unprocessedGenes.Add(gm.mum.GetGeneByName("Better Babies"));
//		}
//		
//		if(gm.gameVars.visitorsSpawned == 2)
//		{
//			visitor.unprocessedGenes.Clear();
//			visitor.unprocessedGenes.Add(gm.mum.GetGeneByName("Blue"));
//		}
//		
//		if(gm.gameVars.visitorsSpawned == 3)
//		{
//			visitor.unprocessedGenes.Clear();
//			visitor.unprocessedGenes.Add(gm.mum.GetGeneByName("Fertility"));
//		}
//		
//		visitor.SetGeneActivationForAll();
//		visitor.ApplyGeneEffects(visitor.activeGenes);
//		
//		visitors.Add(visitor);
//		visitorCost.Add(gm.gameVars.visitorsSpawned * 50);
//		visitorTimers.Add(DateTime.Now + new TimeSpan(0,1,0,0));
//		
//		gm.popup.Show("New Visitor", "A new Blob visitor has arrived to check out your kingdom!");
//	}
//	
//	
//	public void DontHireVisitor()
//	{
//		selectedIndex = -1;
//	}
//	
//	
//	public void HireVisitor()
//	{
//		int index = selectedIndex;
//		if (index < 0 || index >= visitors.Count)
//			return;
//		
//		if(gm.gameVars.nurseryBlobs.Count >= gm.nm.maxBlobs)
//		{
//			gm.popup.Show("Cannot Join", "Breeding Room is full.");
//			return;
//		}
//		
//		if(visitorCost[index] > gm.gameVars.gold)
//		{
//			gm.popup.Show("Cannot Join", "Not enough gold to hire this blob.");
//			return;
//		}
//		
//		
//		Blob visitor = visitors[index];
//		gm.AddGold(-visitorCost[index]);
//		visitors.RemoveAt(index);
//		visitorCost.RemoveAt(index);
//		visitorTimers.RemoveAt(index);
//		gm.visitorButtons[visitors.Count].gameObject.SetActive(false);
//		
//		gm.nm.AddBlob(visitor);
//		index = gm.nm.blobs.IndexOf(visitor);
//		gm.nm.blobPanel.UpdateBlobCellWithBlob(index, visitor);
//		gm.nm.PressGridItem(index);
//	}
//	
//	
//	public void ShowPopupForVisitor(int index)
//	{
//		Blob visitor = visitors[index];
//		//gm.blobPopupChoice.ShowChoice(visitor, "This blob wants to join you", "",        
//		//                              this, "HireVisitor",
//		//                              this, "DontHireVisitor");
//		selectedIndex = index;
//	}
//	
//	
//	bool IsVisitorPopupShowing()
//	{
//		//return (gm.blobPopupChoice.gameObject.activeInHierarchy && gm.blobPopupChoice.headerLabel.text == "This blob wants to join you");
//		return false;
//	}
//	
//	
//	public void VisitorButtonPressed(UIButton button)
//	{
//		int index = gm.visitorButtons.IndexOf(button);
//		ShowPopupForVisitor(index);
//	}
//	
//	
//	// Update is called once per frame
//	void Update () 
//	{
//		//		int aveLevel = gm.GetAverageLevel();
//		//
//		//		switch (aveLevel)
//		//		{
//		//		case BlobQuality.Abysmal:
//		//		case BlobQuality.Horrid:
//		//		case BlobQuality.Poor: maxVisitors = 1;break;
//		//		case BlobQuality.Fair: maxVisitors = 2;break;
//		//		case BlobQuality.Good: 
//		//		case BlobQuality.Excellent:
//		//		case BlobQuality.Outstanding: maxVisitors = 3;break;
//		//		}
//		
//		if (false)//visitorTime <= DateTime.Now)
//		{
//			if (visitors.Count < maxVisitors)
//			{
//				GenerateVisitor();
//				gm.visitorButtons[visitors.Count - 1].gameObject.SetActive(true);
//				visitorDelay = new TimeSpan(0, (int)(GetNewVisitorDelayMins()), 0);
//				visitorTime = DateTime.Now + visitorDelay;
//			}
//		}
//		
//		for (int i = 0; i < visitorTimers.Count; i++)
//		{
//			DateTime dt = visitorTimers[i];
//			if (dt <= DateTime.Now)
//			{
//				if(IsVisitorPopupShowing())
//					gm.popup.Hide();
//				
//				visitors.RemoveAt(i);
//				visitorCost.RemoveAt(i);
//				visitorTimers.RemoveAt(i);
//				gm.visitorButtons[visitors.Count].gameObject.SetActive(false);
//				i--;
//			}
//		}
//		
//		if (IsVisitorPopupShowing() && selectedIndex < visitors.Count && selectedIndex >= 0)
//		{
//			Blob visitor = visitors[selectedIndex];
//			List<Gene> genes = visitor.genes;
//			string genestr = "Genes: ";
//			foreach(Gene g in genes)
//				genestr += (g.negativeEffect ? "[FF9B9B]" : "[9BFF9B]") + g.itemName + ((genes.IndexOf(g) == (genes.Count - 1)) ? "[-]\n" : "[-], ");
//			TimeSpan ts = visitorTimers[selectedIndex] - DateTime.Now;
//			string timestr =  "Time Left: " + string.Format("{0:00}:{1:00}:{2:00} ", ts.TotalHours, ts.Minutes, ts.Seconds);
//			string bodyText =  "Gender: " + ((visitor.male) ? "Male\n" : "Female\n") +
//				"Quality: " + visitor.quality.ToString() + " (" + visitor.quality.ToString() + ")\n" + 
//					genestr + timestr;
//			//gm.blobPopupChoice.bodyLabel.text =  bodyText;
//			//gm.blobPopupChoice.bodyLabel.alignment = NGUIText.Alignment.Left;
//			//gm.blobPopupChoice.button1Label.text = "Recruit     [FFD700]" + visitorCost[selectedIndex].ToString() + "g[-]";
//		}
//		
//	}
}
