using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneManager : MonoBehaviour {
	public List<BaseGene> genes = new List<BaseGene>(); // All original genes
	public List<Gene> storedGenes = new List<Gene>();


	public bool DoesNameExistInList(string nameParam){return (GetBaseGeneWithName(nameParam) != null); }
	
	public BaseGene GetBaseGeneWithName(string nameParam) {
		foreach(BaseGene g in genes)
			if (g.itemName == nameParam)
				return g;
		return null;
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
