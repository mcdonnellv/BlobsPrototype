using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GeneManager : MonoBehaviour {
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
		foreach(BaseGene bg in genes) {
			storedGenes.Add(new Gene(bg)); //temorarily add all possibble genes to your inventory
		}
	}


	public void GeneTransferFromBlobToGenePool (Blob blob, Gene gene) {
		blob.genes.Remove(gene);
		storedGenes.Add(gene);
		blob.CalculateStats();
		blob.UpdateBlobInfoIfDisplayed();
	}


	public void GeneTransferFromGenePoolToBlob (Blob blob, Gene gene) {
		blob.genes.Add(gene);
		storedGenes.Remove(gene);
		blob.CalculateStats();
		blob.UpdateBlobInfoIfDisplayed();
	}


	public List<BaseGene> GetBaseGeneListFromGeneList (List<Gene> geneListParam) {
		List<BaseGene> baseGenes = new List<BaseGene>();
		foreach(Gene gene in geneListParam) {
			BaseGene baseGene = GetBaseGeneWithName(gene.itemName);
			baseGenes.Add(baseGene);
		}
		return baseGenes;
	}

	public List<Gene> CreateGeneListFromBaseGeneList (List<BaseGene> baseGeneListParam) {
		List<Gene> geneList = new List<Gene>();
		foreach(BaseGene baseGene in baseGeneListParam) {
			geneList.Add(new Gene(baseGene));
		}
		return geneList;
	}
}
