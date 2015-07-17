using UnityEngine;
using System.Collections;

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


public class Blob 
{
	public bool male;
	public BlobColor color;
	public int breedCount;
	public BlobColor allele1;
	public BlobColor allele2;
	public bool alive = true;
	public float quality = 1f;
	public BlobTrait trait;
	public BlobJob job;
	public bool onMission = false;
	public int age = 0;
	public bool matedThisYear = false;

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
		float rand = Random.Range(.1f, .4f);
		float f = (combined * .8f) + (combined * rand);
		return Mathf.Round(f * 10f) / 10f;
	}
}
