using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneManager : MonoBehaviour {
	public List<BaseGene> genes = new List<BaseGene>(); // All original genes
	public List<Gene> storedGenes = new List<Gene>();


	public void FirstTimeSetup() {
		foreach(BaseGene bg in genes) {
			storedGenes.Add(new Gene(bg)); //temorarily add all possibble genes to your inventory
		}
	}


	public BaseGene GetGeneByName(string nameParam){
		foreach(BaseGene m in genes)
			if (m.geneName == nameParam)
				return m;
		return null;
	}


	public bool DoesNameExistInList(string nameParam){
		foreach(BaseGene m in genes)
			if (m.geneName == nameParam)
				return true;
		return false;
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
}
