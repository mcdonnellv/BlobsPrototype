using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum BlobQuality
{
	Abysmal = 1,
	Horrid = 2,
	Poor = 3,
	Fair = 5, 
	Good = 8, 
	Excellent = 13, 
	Outstanding = 21, 
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
	BodyPartManager bpm;
	GameManager gm;
	public int id;
	public int momId;
	public int dadId;
	public int spouseId;
	public bool male;
	public bool female {get{return !male;}}
	public int unfertilizedEggs;
	public float quality;
	public float qualityBoostForOffspring;
	public BlobTrait trait;
	public BlobJob job;
	public bool onMission;
	public DateTime birthday;
	public Blob egg;
	public bool hasHatched;
	public DateTime hatchTime;
	public TimeSpan blobHatchDelay;
	public TimeSpan breedReadyDelay;
	public DateTime breedReadyTime;
	public DateTime goldProductionTime;
	public List<Gene> genes { get {return activeGenes.Union(inactiveGenes.Union(unprocessedGenes)).ToList();} }
	public List<Gene> unprocessedGenes;
	public List<Gene> activeGenes;
	public List<Gene> inactiveGenes;
	public Color color;
	public int allowedGeneCount { get{return GetGeneCountFromQuality(GetQualityFromValue(quality));} }
	public TimeSpan age {get {return DateTime.Now - birthday;}}
	public Dictionary<string, Texture> bodyPartSprites;
	public GameObject blobGameObject;
	public int tilePosX;
	public int tilePosY;


	public Blob()
	{
		bpm = (BodyPartManager)(GameObject.Find("BodyPartDatabase")).GetComponent<BodyPartManager>();
		quality = (float)BlobQuality.Poor;
		qualityBoostForOffspring = 0f;
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
		unfertilizedEggs = 2;
		color = new Color(0.863f, 0.863f, 0.863f, 1f);
		//blobHatchDelay = gm.blobHatchDelay;
		//breedReadyDelay = gm.breedReadyDelay;
		tilePosX = 0;
		tilePosY = 0;
	}

	public void Setup()
	{
		blobGameObject = (GameObject)GameObject.Instantiate(Resources.Load("BlobSprites"));
		BlobDragDropItem bddi = blobGameObject.GetComponent<BlobDragDropItem>();
		bddi.blob = this;
		SetBodyTexture();
		SetEyeTexture();
		List<UISprite> blobsprites = blobGameObject.GetComponentsInChildren<UISprite>().ToList();
		Texture tex = bodyPartSprites["Body"];
		blobsprites[0].spriteName = tex.name;
		tex = bodyPartSprites["Eyes"];
		blobsprites[1].spriteName = tex.name;
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
			case Gene.GeneActivationRequirements.QualityMustAtLeastBePoor:
				return (GetQualityFromValue(quality) >= BlobQuality.Poor);
			case Gene.GeneActivationRequirements.QualityMustAtLeastBeFair:
				return (GetQualityFromValue(quality) >= BlobQuality.Fair);
			case Gene.GeneActivationRequirements.QualityMustAtLeastBeGood:
				return (GetQualityFromValue(quality) >= BlobQuality.Good);
			case Gene.GeneActivationRequirements.QualityMustAtLeastBeExcellent:
				return (GetQualityFromValue(quality) >= BlobQuality.Excellent);
			case Gene.GeneActivationRequirements.QualityMustAtLeastBeOutstanding: 
				return (GetQualityFromValue(quality) >= BlobQuality.Outstanding);
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
		BlobQuality oldAveQuality = Blob.GetQualityFromValue(gm.GetAverageQuality());
		hasHatched = true;
		birthday = DateTime.Now;
		BlobQuality newAveQuality = Blob.GetQualityFromValue(gm.GetAverageQuality());

		if (oldAveQuality < newAveQuality)
			gm.popup.Show("Quality up!","Congratulations! Your average Blob quality is now " + Blob.GetQualityFromEnum(newAveQuality) + "!");
		else if(oldAveQuality > newAveQuality)
			gm.popup.Show("Quality Down!", "Oh no! Your average Blob quality is now " + Blob.GetQualityFromEnum(newAveQuality) + "!");
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
		bodyPartSprites.Add("Body", bpm.bodyTextures[UnityEngine.Random.Range(0, bpm.bodyTextures.Count)]);
	}

	public void SetEyeTexture()
	{
		bodyPartSprites.Add("Eyes", bpm.eyeTextures[UnityEngine.Random.Range(0, bpm.eyeTextures.Count)]);
	}

	static public string GetQualityStringFromValue(float quality)
	{
		return GetQualityFromEnum(GetQualityFromValue(quality));
	}
	


	static public BlobQuality GetQualityFromValue(float quality)
	{
		if (quality < (float)BlobQuality.Horrid)
			return BlobQuality.Abysmal;

		if (quality < (float)BlobQuality.Poor)
			return BlobQuality.Horrid;

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
		case BlobQuality.Abysmal: return "Abysmal";
		case BlobQuality.Horrid: return "Horrid";
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
		case BlobQuality.Abysmal: return 0;
		case BlobQuality.Horrid: return 0;
		case BlobQuality.Poor: return 2;
		case BlobQuality.Fair: return 4;
		case BlobQuality.Good: return 6;
		case BlobQuality.Excellent: return 8;
		case BlobQuality.Outstanding: return 10;
		}
		
		return 0;
	}


	static public BlobQuality GetQualityFromGeneCount(int genecount)
	{
		if(genecount <= 0)
			return BlobQuality.Horrid;
		if(genecount <= 2)
			return BlobQuality.Poor;
		if(genecount <= 4)
			return BlobQuality.Fair;
		if(genecount <= 6)
			return BlobQuality.Good;
		if(genecount <= 8)
			return BlobQuality.Excellent;
		if(genecount <= 10)
			return BlobQuality.Outstanding;

		return BlobQuality.Outstanding;
	}


	static public float GetNewQuality (Blob dad, Blob mom)
	{	
		float qualityBoost = (dad.qualityBoostForOffspring > mom.qualityBoostForOffspring) ? dad.qualityBoostForOffspring : mom.qualityBoostForOffspring;
		float rand = 0;

		if(UnityEngine.Random.Range(0,4) == 0)
			rand = UnityEngine.Random.Range(-1, 2) * 0.1f;

		float average = (dad.quality + mom.quality) / 2f + qualityBoost + rand;
		average = Mathf.Ceil(average * 10f) / 10f;
		return average;
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
				qualityBoostForOffspring = 0.1f;
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
