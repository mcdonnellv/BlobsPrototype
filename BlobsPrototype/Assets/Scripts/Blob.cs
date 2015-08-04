using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum BlobQuality
{
	Poor = 1,
	Fair = 2, 
	Good = 3, 
	Excellent = 4, 
	Outstanding = 5, 
};

public enum BlobJob
{
	None,
	Farmer,
	Merchant,
	Builder,
	Fighter,
};

public enum BlobTrait
{
	None,
	Hardy,
	Wise,
	Smart,
	Charming,
	Strong,
	Nimble,
};


[Serializable]
public class Blob
{
	GameManager gm;
	//public Species species;
	public bool male;
	public int breedCount;
	public float quality;
	public BlobTrait trait;
	public BlobJob job;
	public bool onMission;
	public DateTime birthday;
	public Blob egg;
	public bool hasHatched;
	public DateTime hatchTime;
	public DateTime breedReadyTime;
	public DateTime goldProductionTime;
	public List<Gene> genes;
	List<int> activeGeneIndexes;
	public Color color { get{return GetBodyColor();} }
	public int allowedGeneCount { get{return GetGeneCountFromQuality(GetQualityFromValue(quality));} }
	public TimeSpan age {get {return DateTime.Now - birthday;}}
	public Dictionary<string, Texture> bodyPartSprites;


	public Blob()
	{
		gm = (GameManager)(GameObject.Find("GameManager")).GetComponent<GameManager>();
		quality = 1f;
		onMission = false;
		birthday = new DateTime(0);
		hasHatched = false;
		hatchTime = new DateTime(0);
		breedReadyTime = new DateTime(0);
		goldProductionTime = new DateTime(0);
		genes = new List<Gene>();
		activeGeneIndexes = new List<int>();
		bodyPartSprites = new Dictionary<string, Texture>();
	}
	

	public List<Gene> GetActiveGenes()
	{
		List<Gene> activeGeneList = new List<Gene>();
		foreach(Gene g in genes)
			if(IsGeneActive(g))
				activeGeneList.Add(g);
		return activeGeneList;
	}


	public List<Gene> GetInactiveGenes()
	{
		List<Gene> inactiveGeneList = new List<Gene>();
		foreach(Gene g in genes)
			if(IsGeneActive(g) == false)
				inactiveGeneList.Add(g);
		return inactiveGeneList;
	}


	public bool IsGeneActive(Gene g)
	{
		for(int i=0; i < activeGeneIndexes.Count; i++)
			if(genes[activeGeneIndexes[i]] == g)
				return true;
		return false;
	}


	public bool IsGeneTypeActive(Gene.Type t)
	{
		for(int i=0; i < activeGeneIndexes.Count; i++)
		{
			Gene activeGene = genes[activeGeneIndexes[i]];
			if(activeGene.type == t)
				return true;
		}
		return false;
	}


	public void ActivateGene(Gene g)
	{
		int index = genes.IndexOf(g);
		if(index == -1)
			return;

		DeactivateGenesOfType(g.type);
		activeGeneIndexes.Add(index);
	}


	public void DeactivateGenesOfType(Gene.Type t)
	{
		for(int i=0; i < activeGeneIndexes.Count; i++)
		{
			if(genes[activeGeneIndexes[i]].type == t)
			{
				activeGeneIndexes.RemoveAt(i);
				i--;
			}
		}
	}

	
	public void ActivateGenes()
	{
		foreach(Gene g in genes)
		{
			if(IsGeneTypeActive(g.type))
				continue;
			List<Gene> genesOfTheSameType = GeneManager.GetGenesOfType(genes, g.type);
			Gene geneToActivate = GeneManager.GetRandomGeneBasedOnStrength(genesOfTheSameType);
			if(GetActiveGenes().Count < 3)
				ActivateGene(geneToActivate);
		}
	}


	public void Hatch()
	{
		hasHatched = true;
		birthday = DateTime.Now;
	}


	public bool IsOfBreedingAge()
	{
		if(age >= gm.breedingAge)
			return true;
		return false;
	}


	public Color GetBodyColor()
	{
		for(int i=0; i < activeGeneIndexes.Count; i++)
			if(genes[activeGeneIndexes[i]].type == Gene.Type.BodyColor)
				return genes[activeGeneIndexes[i]].bodyColor;
		return new Color(0.165f, 0.745f, 0.925f, 1f);
	}


	public string GetBodyColorName()
	{
		for(int i=0; i < activeGeneIndexes.Count; i++)
			if(genes[activeGeneIndexes[i]].type == Gene.Type.BodyColor)
				return genes[activeGeneIndexes[i]].geneName;
		return "White";
	}
	

	public float BlobScale()
	{
		float s = .4f + (.6f * (float)(age.TotalSeconds / gm.breedingAge.TotalSeconds));
		return Mathf.Clamp(s, 0f, 1f);
	}

	public void SetRandomTextures()
	{
		SetBodyTexture();
		SetEyeTexture();
	}

	public void SetBodyTexture()
	{
		bodyPartSprites.Add("Body", gm.bpm.bodyTextures[UnityEngine.Random.Range(0, gm.bpm.bodyTextures.Count)]);
	}

	public void SetEyeTexture()
	{
		bodyPartSprites.Add("Eyes", gm.bpm.eyeTextures[UnityEngine.Random.Range(0, gm.bpm.eyeTextures.Count)]);
	}

	static public string GetQualityStringFromValue(float quality)
	{
		return GetQualityFromEnum(GetQualityFromValue(quality));
	}
	


	static public BlobQuality GetQualityFromValue(float quality)
	{
		if (quality < (float)BlobQuality.Fair)
			return BlobQuality.Poor;
		
		if (quality < (float)BlobQuality.Good)
			return BlobQuality.Fair;
		
		if (quality < (float)BlobQuality.Excellent)
			return BlobQuality.Good;
		
		if (quality < (float)BlobQuality.Outstanding)
			return BlobQuality.Excellent;
		
		return BlobQuality.Outstanding;
	}


	static public string GetQualityFromEnum(BlobQuality quality)
	{
		switch (quality)
		{
		case BlobQuality.Poor: return "Poor";
		case BlobQuality.Fair: return "Fair";
		case BlobQuality.Good: return "Good";
		case BlobQuality.Excellent: return "Excellent";
		case BlobQuality.Outstanding: return "Outstanding";
		}

		return "";
	} 

	static public int GetGeneCountFromQuality(BlobQuality quality)
	{
		switch (quality)
		{
		case BlobQuality.Poor: return 2;
		case BlobQuality.Fair: return 4;
		case BlobQuality.Good: return 6;
		case BlobQuality.Excellent: return 8;
		case BlobQuality.Outstanding: return 10;
		}
		
		return 2;
	}


	static public float GetNewQuality (float q1, float q2)
	{
		float combined = (q1 + q2) / 2f;
		float rand = UnityEngine.Random.Range(.1f, .4f);
		float f = (combined * .8f) + (combined * rand);
		return Mathf.Round(f * 10f) / 10f;
	}
}
