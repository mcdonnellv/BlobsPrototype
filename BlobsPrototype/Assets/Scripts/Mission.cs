using UnityEngine;
using System.Collections;


public class Mission
{
	public float duration;
	public float durationCounting;
	public int reward;
	public int ageRequirement;
	public float requirementBonus;
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
		blob = null;
		active = false;
		successful = true;
		successChance = 1f;
	}

	public float GetSuccessChance(Blob blob) {
		return 0f;
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
