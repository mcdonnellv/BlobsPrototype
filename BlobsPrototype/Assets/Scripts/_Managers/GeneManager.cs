using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class GeneManager : MonoBehaviour {
	private static GeneManager _geneManager;
	public static GeneManager geneManager { get {if(_geneManager == null) _geneManager = GameObject.Find("GeneManager").GetComponent<GeneManager>(); return _geneManager; } }

	public List<BaseGene> genes = new List<BaseGene>(); // All original genes
	public List<Gene> storedGenes = new List<Gene>();
	public UIAtlas iconAtlas;


	public bool DoesNameExistInList(string nameParam){return (GetBaseGeneWithName(nameParam) != null); }
	public bool DoesIdExistInList(int idParam) {return (GetBaseGeneByID(idParam) != null); }
	
	public BaseGene GetBaseGeneWithName(string nameParam) {
		foreach(BaseGene g in genes)
			if (g.itemName == nameParam)
				return g;
		return null;
	}

	public BaseGene GetBaseGeneByID(int idParam) {
		foreach(BaseGene i in genes)
			if (i.id == idParam)
				return i;
		return null;
	}

	public int GetNextAvailableID() {
		int lowestIdVal = 0;
		List<BaseGene> sortedByID = genes.OrderBy(x => x.id).ToList();
		foreach(BaseGene i in sortedByID)
			if (i.id == lowestIdVal)
				lowestIdVal++;
		return lowestIdVal;
	}

	public void FirstTimeSetup() {
		//foreach(BaseGene bg in genes) {
			//storedGenes.Add(new Gene(bg)); //temorarily add all possibble genes to your inventory
		//}
	}


	public void GeneTransferFromBlobToGenePool (Blob blob, Gene gene) {
		blob.genes.Remove(gene);
		storedGenes.Add(gene);
		blob.CalculateStatsFromGenes();
		blob.gameObject.UpdateBlobInfoIfDisplayed();
	}


	public void GeneTransferFromGenePoolToBlob(Blob blob, Gene gene) {
		blob.genes.Add(gene);
		storedGenes.Remove(gene);
		blob.CalculateStatsFromGenes();
		blob.gameObject.UpdateBlobInfoIfDisplayed();
	}


	public List<BaseGene> GetBaseGeneListFromGeneList(List<Gene> geneListParam) {
		List<BaseGene> baseGenes = new List<BaseGene>();
		foreach(Gene gene in geneListParam) {
			BaseGene baseGene = GetBaseGeneWithName(gene.itemName);
			baseGenes.Add(baseGene);
		}
		return baseGenes;
	}

	public List<Gene> CreateGeneListFromBaseGeneList(List<BaseGene> baseGeneListParam) {
		List<Gene> geneList = new List<Gene>();
		foreach(BaseGene baseGene in baseGeneListParam) {
			geneList.Add(new Gene(baseGene));
		}
		return geneList;
	}


	public List<BaseGene> GetBaseGenesWithKeyItem(int keyItemId) {
		List<BaseGene> geneList = new List<BaseGene>();
		foreach(BaseGene baseGene in genes) {
			if(baseGene.activationRequirements.Count > 0 && baseGene.activationRequirements[0].itemId == keyItemId)
				geneList.Add(baseGene);
		}
		return geneList;
	}


	static public List<Gene> LimitGenes(List<Gene> list, int limit) {
		if(list.Count < limit)
			return list;
		//shuffle
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = UnityEngine.Random.Range(0, n + 1) ;
			Gene g = list[k];  
			list[k] = list[n];  
			list[n] = g;  
		}
		list.RemoveRange(limit -1, list.Count - limit);
		return list.OrderBy(x => x.itemName).ToList();
	}
}
