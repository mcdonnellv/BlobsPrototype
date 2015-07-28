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
	public float successChance;


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
		successChance = 1f;
	}

	public float GetSuccessChance(Blob blob)
	{
		float agePenalty = .25f;
		float chance = 1f;
		float otherbonusTotal = 1f - chance;
		float reqCount = 0;

		if (jobRequirement != BlobJob.None)
			reqCount++;

		if (traitRequirement != BlobTrait.None)
			reqCount++;

		if (colorRequirement != BlobColor.None)
			reqCount++;

		if (ageRequirement > 0)
			reqCount++;

		if (reqCount > 0)
		{
			float piece = otherbonusTotal / reqCount;

			if (jobRequirement != BlobJob.None && jobRequirement == blob.job)
				chance += piece;

			if (traitRequirement != BlobTrait.None && traitRequirement == blob.trait)
				chance += piece;

			if (ageRequirement > 0)
				chance -= agePenalty * (ageRequirement - blob.age);

			chance = (chance < 0f) ? 0f : ((chance > 1f) ? 1f : chance);
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
			duration *= 10f;
			reward = 100;
			ageRequirement = 20;
			break;

		case 1: 
			duration *= 10f;
			reward = 300;
			ageRequirement = 30;
			break;

		case 3: 
			duration *= 10f;
			reward = 500;
			ageRequirement = 40;
			break;

		case 4: 
			duration *= 5f;
			reward = 40;
			ageRequirement = 1;
			break;

		case 5: 
			duration *= 5f;
			reward = 60;
			ageRequirement = 10;
			break;

		case 6: 
			duration *= 2f;
			reward = 80;
			ageRequirement = 20;
			break;

		case 7: 
			duration *= 2f;
			reward = 20;
			ageRequirement = 1;
			break;

		case 8: 
			duration *= 2f;
			reward = 30;
			ageRequirement = 5;
			break;

		case 9: 
			duration *= 2f;
			reward = 40;
			ageRequirement = 10;
			break;

		}
	}

}
