using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneManager: MonoBehaviour
{
	public List<Gene> genes = new List<Gene>();

	public void FirstTimeSetup()
	{
		Gene g = GetGeneByName("Better Babies");
		g.revealed = true;

		g = GetGeneByName("Fertility");
		g.revealed = true;

		foreach(Gene gene in genes)
			if(gene.negativeEffect == false && gene.quality <= Quality.Common)
				gene.revealed = true;
	}

	public Gene GetGeneByName(string nameParam)
	{
		foreach(Gene m in genes)
			if (m.geneName == nameParam)
				return m;
		return null;
	}


	public bool DoesNameExistInList(string nameParam)
	{
		foreach(Gene m in genes)
			if (m.geneName == nameParam)
				return true;
		return false;
	}


	public static List<Gene> GetGenesOfType(List<Gene> geneList, Gene.Type t)
	{
		List<Gene> genesOfTheSameType = new List<Gene>();
		foreach(Gene g in geneList)
			if(g.type == t)
				genesOfTheSameType.Add(g);
		return genesOfTheSameType;
	}


	public static List<Gene> GetGenesWithNegativeEffect(List<Gene> geneList)
	{
		List<Gene> badGenes = new List<Gene>();
		foreach(Gene g in geneList)
			if(g.negativeEffect)
				badGenes.Add(g);
		return badGenes;
	}


	public static List<Gene> GetGenesWithPositiveEffect(List<Gene> geneList)
	{
		List<Gene> goodGenes = new List<Gene>();
		foreach(Gene g in geneList)
			if(!g.negativeEffect)
				goodGenes.Add(g);
		return goodGenes;
	}


	public static List<Gene> GetGenesWithPrerequisite(List<Gene> geneList, string name)
	{
		List<Gene> elligibleGenes = new List<Gene>();
		foreach(Gene potentialGene in geneList)
			if(potentialGene.preRequisite == name)
				elligibleGenes.Add(potentialGene);
		return elligibleGenes;
	}


	public static Gene GetRandomGeneBasedOnStrength(List<Gene> geneList)
	{
		int cummulativeStrength = 0;
		foreach(Gene g in geneList)
			cummulativeStrength += g.geneStrength;
		
		int rand = UnityEngine.Random.Range(0, cummulativeStrength);
		cummulativeStrength = 0;
		foreach(Gene g in geneList)
		{
			cummulativeStrength += (int) g.geneStrength;
			if(rand < cummulativeStrength)
				return g;
		}

		return null;
	}


	public static Gene GetRandomGeneBasedOnRarity(List<Gene> geneList)
	{
		float combinedChances = 0f;
		foreach(Gene g in geneList)
			combinedChances += g.revealChance;

		float rand = UnityEngine.Random.Range(0f, combinedChances);
		combinedChances = 0;
		foreach(Gene g in geneList)
		{
			combinedChances += g.revealChance;
			if(rand < combinedChances)
				return g;
		}
		
		return null;
	}
}
