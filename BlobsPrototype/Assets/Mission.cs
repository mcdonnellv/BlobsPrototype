using UnityEngine;
using System.Collections;


public class Mission
{
	public float duration;
	public float durationCounting;
	public int reward;
	public int ageRequirement;
	public float requirementBonus;
	public BlobJob jobRequirement;
	public BlobTrait traitRequirement;
	public BlobColor colorRequirement;
	public Blob blob;
	public bool active;
	public bool successful;


	public Mission()
	{
		ageRequirement = 1;
		duration = 3f;
		durationCounting = 0f;
		requirementBonus = 0f;
		reward = 30;
		jobRequirement = BlobJob.None;
		traitRequirement = BlobTrait.None;
		colorRequirement = BlobColor.None;
		blob = null;
		active = false;
		successful = true;
	}

	public float GetSuccessChance(Blob blob)
	{
		float chance = 1f;
		float otherbonusTotal = 1f - chance;
		float reqCount = 0;
		if (jobRequirement != BlobJob.None)
			reqCount++;

		if (traitRequirement != BlobTrait.None)
			reqCount++;

		if (colorRequirement != BlobColor.None)
			reqCount++;

		if (reqCount > 0)
		{
			float piece = otherbonusTotal / reqCount;

			if (jobRequirement != BlobJob.None && jobRequirement == blob.job)
				chance += piece;

			if (traitRequirement != BlobTrait.None && traitRequirement == blob.trait)
				chance += piece;

			if (colorRequirement != BlobColor.None && colorRequirement == blob.color)
				chance += piece;
		}


		chance = Mathf.Round(chance * 100f) / 100f;

		return chance;
	}


	public void SetRandomMissionValues()
	{
		int random = Random.Range(0,10);

		duration = 60f;
		switch (random)
		{
		case 0: 
			duration *= 5f;
			reward = 100;
			ageRequirement = 3;
			break;

		case 1: 
			duration *= 5f;
			reward = 200;
			ageRequirement = 10;
			break;

		case 3: 
			duration *= 5f;
			reward = 250;
			ageRequirement = 15;
			break;

		case 4: 
			duration *= 2f;
			reward = 20;
			ageRequirement = 1;
			break;

		case 5: 
			duration *= 2f;
			reward = 30;
			ageRequirement = 3;
			break;

		case 6: 
			duration *= 2f;
			reward = 40;
			ageRequirement = 10;
			break;

		case 7: 
			duration *= 1f;
			reward = 10;
			ageRequirement = 1;
			break;

		case 8: 
			duration *= 1f;
			reward = 15;
			ageRequirement = 5;
			break;

		case 9: 
			duration *= 1f;
			reward = 20;
			ageRequirement = 10;
			break;

		}
	}

}
