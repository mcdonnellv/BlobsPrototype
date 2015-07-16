using UnityEngine;
using System.Collections;


public class Mission
{
	public float duration;
	public float durationCounting;
	public int reward;
	public float baseSuccessChance;
	public float requirementBonus;
	public BlobJob jobRequirement;
	public BlobTrait traitRequirement;
	public BlobColor colorRequirement;
	public Blob blob;
	public bool active;


	public Mission()
	{
		duration = 30f;
		durationCounting = 0f;
		baseSuccessChance = 1f;
		requirementBonus = 0f;
		reward = 30;
		jobRequirement = BlobJob.None;
		traitRequirement = BlobTrait.None;
		colorRequirement = BlobColor.None;
		blob = null;
		active = false;
	}

	public float GetSuccessChance(Blob blob)
	{
		float chance = baseSuccessChance;
		float otherbonusTotal = 1f - baseSuccessChance;
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

		return chance;
	}

}
