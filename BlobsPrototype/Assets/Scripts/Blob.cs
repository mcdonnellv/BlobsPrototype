using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


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
	public enum Quality
	{
		Standard,
		Rare,
		Epic,
		Legendary,
	};
	
	GameManager gm;
	public int id;
	public int momId;
	public int dadId;
	public int spouseId;
	public bool male;
	public bool female {get{return !male;}}
	public int unfertilizedEggs;
	public Quality quality;
	public int level;
	
	//stats
	public int goldProduction;
	public int sellValue;
	
	public float levelBoostForOffspring;
	public BlobTrait trait;
	public BlobJob job;
	public bool onMission;
	public DateTime birthday;
	public Blob egg;
	public bool hasHatched;
	public DateTime hatchTime;
	public TimeSpan blobHatchDelay;
	public TimeSpan breedReadyDelay;
	public TimeSpan heartbrokenRecoverDelay;
	public TimeSpan mateFindDelay;
	public DateTime breedReadyTime;
	public DateTime goldProductionTime;
	public DateTime heartbrokenRecoverTime;
	public DateTime mateFindTime;
	public List<Gene> genes { get {return activeGenes.Union(inactiveGenes.Union(unprocessedGenes)).ToList();} }
	public List<Gene> unprocessedGenes;
	public List<Gene> activeGenes;
	public List<Gene> inactiveGenes;
	public Color color;
	public int allowedGeneCount { get{return GetGeneCountFromQuality(quality);} }
	public TimeSpan age {get {return DateTime.Now - birthday;}}
	public Dictionary<string, Texture> bodyPartSprites;

	public int tilePosX;
	public int tilePosY;
	
	
	public Blob()
	{
		gm = (GameManager)(GameObject.Find("GameManager")).GetComponent<GameManager>();
		quality = Quality.Standard;
		levelBoostForOffspring = 0f;
		onMission = false;
		birthday = new DateTime(0);
		hasHatched = false;
		hatchTime = new DateTime(0);
		breedReadyTime = new DateTime(0);
		goldProductionTime = new DateTime(0);
		unprocessedGenes = new List<Gene>();
		activeGenes = new List<Gene>();
		inactiveGenes = new List<Gene>();
		bodyPartSprites = new Dictionary<string, Texture>();
		momId = -1;
		dadId = -1;
		spouseId = -1;
		unfertilizedEggs = 99;
		color = new Color(0.863f, 0.863f, 0.863f, 1f);
		blobHatchDelay = gm.blobHatchDelay;
		breedReadyDelay = gm.breedReadyDelay;
		//heartbrokenRecoverDelay = gm.heartbrokenRecoverDelay;
		//mateFindDelay = gm.mateFindDelay;
		goldProduction = 0;
		sellValue = 1;
		level = 1;
	}
	
	public string GetBlobStateString()
	{
		if (breedReadyTime > System.DateTime.Now)
			return "Breeding";
		if (heartbrokenRecoverTime > System.DateTime.Now)
			return "Depressed";
		if (!hasHatched && hatchTime > System.DateTime.Now)
			return "Incubating";
		if (mateFindTime > System.DateTime.Now)
			return "Dating";
		
		return "";
	}
	
	
	public Blob GetSpouse()
	{
		foreach(Blob b in gm.nm.blobs)
			if(spouseId == b.id)
				return b;
		
		return null;
	}
	
	
	public bool IsGeneActive(Gene g) { return activeGenes.Contains(g); }
	
	
	public void SetGeneActivationForAll()
	{
		while(unprocessedGenes.Count > 0)
		{
			Gene g = unprocessedGenes[0];
			SetGeneActivationForGene(g);
		}
	}
	
	
	public void SetGeneActivationForGene(Gene g)
	{
		unprocessedGenes.Remove(g);
		if(ShouldGeneBeActive(g))
			ActivateGene(g);
		else
			DeactivateGene(g);
	}
	
	
	public bool ShouldGeneBeActive(Gene g)
	{
		foreach(Gene.GeneActivationRequirements req in g.activationRequirements)
		{
			switch(req)
			{
			case Gene.GeneActivationRequirements.GenderMustBeFemale:
				return (female);
			case Gene.GeneActivationRequirements.GenderMustBeMale:
				return (male);
			case Gene.GeneActivationRequirements.MustHaveNoActiveGenesOfSameType:
				List<Gene> unprocessedGenesOfSameType = GeneManager.GetGenesOfType(unprocessedGenes, g.type);
				List<Gene> activeGenesOfSameType = GeneManager.GetGenesOfType(activeGenes, g.type);
				if(activeGenesOfSameType.Count > 0)
					return false;
				else if(unprocessedGenesOfSameType.Count > 1)
				{
					Gene chosen = GeneManager.GetRandomGeneBasedOnStrength(unprocessedGenesOfSameType);
					if(chosen != g)
					{
						SetGeneActivationForGene(chosen);
						return false;
					}
				}
				return true;
			}
		}
		return true;
	}
	
	
	public bool IsGeneTypeActive(Gene.Type t)
	{
		foreach(Gene g in genes)
			if(g.type == t && IsGeneActive(g))
				return true;
		return false;
	}
	
	
	public void ActivateGene(Gene g)
	{
		if(unprocessedGenes.Contains(g))
			SetGeneActivationForGene(g);
		else
		{
			if(!activeGenes.Contains(g))
				activeGenes.Add(g);
			if (inactiveGenes.Contains(g))
				inactiveGenes.Remove(g);
		}
	}
	
	
	public void DeactivateGene(Gene g)
	{
		if(unprocessedGenes.Contains(g))
			SetGeneActivationForGene(g);
		else
		{
			if(!inactiveGenes.Contains(g))
				inactiveGenes.Add(g);
			if (activeGenes.Contains(g))
				activeGenes.Remove(g);
		}
	}
	
	
	public void DeactivateGenesOfType(Gene.Type t)
	{
		foreach(Gene g in genes)
			if(g.type == t)
				DeactivateGene(g);
	}
	
	
	public void Hatch()
	{
		if(hasHatched)
			return;
		hasHatched = true;
		birthday = DateTime.Now;
	}
	
	
	public bool IsOfBreedingAge()
	{
		if(age >= gm.breedingAge)
			return true;
		return false;
	}
	
	
	public string GetBodyColorName()
	{
		foreach(Gene g in genes)
			if(g.type == Gene.Type.BodyColor && IsGeneActive(g))
				return g.geneName;
		return "White";
	}
	
	
	public float BlobScale()
	{
		float s = .4f + (.6f * (float)(age.TotalSeconds / gm.breedingAge.TotalSeconds));
		return Mathf.Clamp(s, 0f, 1f);
	}
	
	
	public void OrderGenes()
	{
		activeGenes = activeGenes.OrderByDescending( x => x.rarity).ThenBy(x => x.geneName).ToList();
		inactiveGenes = inactiveGenes.OrderByDescending( x => x.rarity).ThenBy(x => x.geneName).ToList();
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
	
	
	static public int GetGeneCountFromQuality(Blob.Quality q)
	{
		switch (q)
		{
		case Blob.Quality.Standard: return 0;
		case Blob.Quality.Rare: return 1;
		case Blob.Quality.Epic: return 2;
		case Blob.Quality.Legendary: return 3;
		}
		
		return 0;
	}
	
	
	static public Blob.Quality GetRandomQuality()
	{	
		Blob.Quality q = Blob.Quality.Standard;
		
		float r = UnityEngine.Random.Range(0f,1f);
		
		if(r <= 0.02140f)
			q = Blob.Quality.Rare;
		
		if(r <= 0.00428f)
			q = Blob.Quality.Epic;
		
		if(r <= 0.00108f)
			q = Blob.Quality.Legendary;
		
		return q;
	}
	
	
	public static Color ColorForQuality(Blob.Quality q)
	{
		switch (q)
		{
		case Blob.Quality.Rare:      return new Color(0.255f, 0.616f, 1f, .5f);
		case Blob.Quality.Epic:      return new Color(0.957f, 0.294f, 1f, .5f);
		case Blob.Quality.Legendary: return new Color(1f, 0.773f, 0.082f, .5f);
		}
		
		return Color.clear;
	}
	
	
	public void ApplyGeneEffects(List<Gene> geneList)
	{
		ApplyBreedingGeneEffects(GeneManager.GetGenesOfType(geneList, Gene.Type.Breeding));
		ApplyBodyColorGeneEffects(GeneManager.GetGenesOfType(geneList, Gene.Type.BodyColor));
	}
	
	
	void ApplyBodyColorGeneEffects(List<Gene> geneList)
	{
		if (geneList == null || geneList.Count == 0)
			return;
		
		if (geneList.Count > 1)
			Debug.LogError("This blob has more than 1 active Body color Gene");
		
		color = geneList[0].bodyColor;
	}
	
	
	void ApplyBreedingGeneEffects(List<Gene> geneList)
	{
		if (geneList == null || geneList.Count == 0)
			return;
		
		foreach(Gene g in geneList)
		{
			switch(g.geneName)
			{
			case "Fertility":
				unfertilizedEggs++;
				break;
			case "Virility":
				unfertilizedEggs++;
				break;
			case "Infertility":
				unfertilizedEggs = 1;
				break;
			case "Sterile":
				unfertilizedEggs = 0;
				break;
			case "Better Babies":
				levelBoostForOffspring = 0.1f;
				break;
			case "Quick Hatch":
				blobHatchDelay = new TimeSpan(0,0,(int)gm.blobHatchDelay.TotalSeconds/2);
				break;
			case "Quick Breed":
				breedReadyDelay = new TimeSpan(0,0,(int)gm.breedReadyDelay.TotalSeconds/2); 
				break;
			}
		}
	}
}
