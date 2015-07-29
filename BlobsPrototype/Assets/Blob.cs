using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum BlobColor
{
	None,
	Blue, 
	Red, 
	Orange, 
	Purple, 
	Green, 
	Yellow
};

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
	public bool male;
	public Color color;
	public int breedCount;
	public float quality;
	public BlobTrait trait;
	public BlobJob job;
	public bool onMission;
	public int age;
	public Blob egg;
	public bool hasHatched;
	public DateTime hatchTime;
	public DateTime breedReadyTime;
	public DateTime goldProductionTime;
	public List<Mutation> mutations;

	public Blob()
	{
		quality = 1f;
		onMission = false;
      	age = 0;
		hasHatched = false;
		hatchTime = new DateTime(0);
		breedReadyTime = new DateTime(0);
		goldProductionTime = new DateTime(0);
		mutations = new List<Mutation>();
	}

	public Color GetBodyColor()
	{
		foreach(Mutation mutation in mutations)
			if(mutation.type == Mutation.Type.BodyColor)
				return mutation.bodyColor;
		return Color.white;
	}

	public string GetBodyColorName()
	{
		foreach(Mutation mutation in mutations)
			if(mutation.type == Mutation.Type.BodyColor)
				return mutation.name;
		return "White";
	}


	static public Color GetColorFromEnum(BlobColor blobColor)
	{
		switch (blobColor)
		{
		case BlobColor.Blue: return Color.cyan;
		case BlobColor.Red: return new Color (1f, 0.6f, 0.6f, 1f);
		case BlobColor.Yellow: return new Color (1f, 0.9f, 0.2f, 1f);
		case BlobColor.Orange: return new Color (1f, 0.5f, 0f, 1f);
		case BlobColor.Green: return Color.green;
		case BlobColor.Purple: return new Color (1f, 0.6f, 1f, 1f);
		}
		
		return Color.black;
	}

	static public string GetNameFromEnum(BlobColor blobColor)
	{
		switch (blobColor)
		{
		case BlobColor.Blue: return "Blue";
		case BlobColor.Red: return "Red";
		case BlobColor.Yellow: return "Yellow";
		case BlobColor.Orange: return "Orange";
		case BlobColor.Green: return "Green";
		case BlobColor.Purple: return "Purple";
		}
		
		return "";
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

	static public float GetNewQuality (float q1, float q2)
	{
		float combined = (q1 + q2) / 2f;
		float rand = UnityEngine.Random.Range(.1f, .4f);
		float f = (combined * .8f) + (combined * rand);
		return Mathf.Round(f * 10f) / 10f;
	}
}
