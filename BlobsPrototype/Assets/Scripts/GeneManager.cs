using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneManager : MonoBehaviour {
	public List<Gene> genes = new List<Gene>(); // All genes
	public List<Gene> storedGenes = new List<Gene>();


	public void FirstTimeSetup() {
		foreach(Gene g in genes) {
			storedGenes.Add(g);
			if(g.description == "") {
				foreach(Stat s in g.stats) {
					if(s.modifier == Stat.Modifier.Added)
						g.description += "+" + s.amount.ToString() + " to " + s.id.ToString() + "\n";
					else
						g.description += s.id.ToString() + " increased by " + s.amount + "%" + "\n";
				}
			}
		}
	}

	public Gene GetGeneByName(string nameParam){
		foreach(Gene m in genes)
			if (m.itemName == nameParam)
				return m;
		return null;
	}


	public bool DoesNameExistInList(string nameParam){
		foreach(Gene m in genes)
			if (m.itemName == nameParam)
				return true;
		return false;
	}


	public static List<Gene> GetGenesOfType(List<Gene> geneList, Gene.GeneType t){
		List<Gene> genesOfTheSameType = new List<Gene>();
		foreach(Gene g in geneList)
			if(g.type == t)
				genesOfTheSameType.Add(g);
		return genesOfTheSameType;
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
