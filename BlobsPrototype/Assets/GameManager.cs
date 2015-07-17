using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public MissionManager mm;
	public GameObject missionView;
	public GameObject breedingView;
	public List<Blob> blobs;
	public GameObject grid;
	public GameObject selectModeCover;
	public UILabel genderLabel;
	public UILabel colorLabel;
	public UILabel genotypeLabel;
	public UILabel countLabel;
	public UILabel qualityLabel;
	public UILabel averageQualityLabel;
	public UILabel goldLabel;
	public UILabel ageLabel;
	public UILabel breedButtonLabel;
	public UILabel yearLabel;
	public UILabel missionButtonLabel;
	public UISlider breedProgressBar;
	public UISlider yearProgressBar;
	public UIButton breedButton;
	public UIButton sellButton;
	public UIButton missionButton;
	public GameObject gameOverObject;
	public GameObject winnerObject;

	public float yearFillTime;
	public float breedBarFillTime;
	public float chanceForMutation;
	public int maxBlobs;
	public int gold;
	public int breedCost;
	public int breedBaseCost;
	public int breedCostMax;
	public int sellValue;
	public int curSelectedIndex;
	public int breedingAge;
	public int breedTimesPerYear;
	public int maxBreedcount;

	Blob recentFather;
	Blob recentMother;
	Blob recentBaby;
	bool selectMode;
	List<Blob> maleBlobs;
	List<Blob> femaleBlobs;
	int year;

	
	// Use this for initialization
	void Start ()
	{
		chanceForMutation = .8f;
		maxBlobs = 20;
		gold = 100;
		breedCost = 10;
		breedBaseCost = 10;
		breedCostMax = 100;
		sellValue = 3;
		curSelectedIndex = 0;
		breedingAge = 2;
		breedTimesPerYear = 4;
		maxBreedcount = 3;
		year = 0;
		selectMode = false;

		maleBlobs = new List<Blob>();
		femaleBlobs = new List<Blob>();
		blobs = new List<Blob>();

		breedingView.SetActive(true);
		missionView.SetActive(false);
		selectModeCover.SetActive(false);


		Blob blob = new Blob();
		blob.color = BlobColor.Blue;
		blob.allele1 = blob.color;
		blob.male = true;
		blob.age = 5;
		UpdateInfoTextWithBlob(blob);

		AddBlob(blob);

		blob = new Blob();
		blob.color = BlobColor.Blue;
		blob.allele1 = blob.color;
		blob.male = false;
		blob.age = 5;

		AddBlob(blob);
	
		UpdateGrid();
		AddGold(0);

		breedProgressBar.value = 1f;
		yearProgressBar.value = 0f;

		yearFillTime = 30f;
		breedBarFillTime = yearFillTime / breedTimesPerYear;
	}


	public void EnableSelectMode(bool enable)
	{
		selectMode = enable;
		selectModeCover.SetActive(enable);
	}


	public void UpdateGrid()
	{
		if (breedingView.activeSelf == false)
			return;

		if (CanEnableBreedButton())
			breedButton.isEnabled = true;


		int i = 0;
		float cummulativeQuality = 0;

		foreach(Transform blobCell in grid.transform)
		{
			if (i < blobs.Count)
			{
				Blob blob = blobs[i];
				cummulativeQuality += blob.quality;
				GameObject cell = blobCell.gameObject;
				cell.SetActive(true);

				UIButton button = cell.GetComponentInChildren<UIButton>();
				BlobCell bc = cell.GetComponent<BlobCell>();
				UISprite bg = cell.GetComponent<UISprite>();
				UILabel[] labels = cell.GetComponentsInChildren<UILabel>();
				UISprite[] sprites = cell.GetComponentsInChildren<UISprite>();

				bg.color = (blob.male) ? new Color(0.62f, 0.714f, 0.941f,1f) : new Color(0.933f, 0.604f, 0.604f, 1f);
				button.defaultColor = bg.color;
				button.hover = bg.color;
	
				labels[0].text = "";
				labels[1].text = (blob.male || blob.age < breedingAge) ? "" : (maxBreedcount - blob.breedCount).ToString();
				labels[2].text = (blob == recentFather) ? "Dad" : (blob == recentMother) ? "Mom" : (blob == recentBaby) ? "Baby" : "";
				sprites[3].enabled = !blob.male;

				if(blob.alive == false)
				{
					sprites[1].color = Color.black;
					sprites[2].color = Color.black;
					labels[1].color = Color.red;
					labels[0].text = "";
					labels[1].text = "";
					labels[2].text = "Dead";
					labels[2].color = Color.gray;
					sprites[0].color = new Color( 0.69f, 0.588f, 0.408f, .5f);
				}
				else
				{
					sprites[1].color = Blob.GetColorFromEnum(blob.color);
					sprites[2].color = Color.white;
					labels[1].color = Color.white;
				}

				if (blob.onMission)
				{
					bc.onMissionLabel.SetActive(true);
					UILabel missionLabel = bc.onMissionLabel.GetComponentInChildren<UILabel>();
					missionLabel.text = "On Mission";
					foreach (Mission mission in mm.missions)
						if(mission.blob == blob && mm.isMissionReadyForCollecting(mission))
							missionLabel.text = "Mission Done";
				}
				else 
					bc.onMissionLabel.SetActive(false);

				//scale
				float a = (float)(blob.age > 3 ? 3 : blob.age);
				float s = .3f + (.7f * (a / 3f));
				if(s > 1f)
					s = 1f;
				int pixels = (int)(s * 50f);
				sprites[1].SetDimensions(pixels, pixels);
				sprites[2].SetDimensions(pixels, pixels);
				sprites[3].SetDimensions(pixels, pixels);
			}
			else
			{
				blobCell.gameObject.SetActive(false);
			}
			i++;
		}

		float averageQuality = cummulativeQuality / blobs.Count;
		averageQuality = Mathf.Round(averageQuality * 10f) / 10f;
		averageQualityLabel.text = "Average Quality: " + Blob.GetQualityStringFromValue(averageQuality);

		float mod = (float)Blob.GetQualityFromValue(averageQuality) - 1f;
		breedCost = breedBaseCost + (int)((mod / 4f) * (breedCostMax - breedBaseCost));
		breedButtonLabel.text = "Breed (" + breedCost.ToString() + "g)";
	}


	bool CanEnableBreedButton()
	{
		if (matingPairExists(false) == false)
			return false;

		if (breedProgressBar.value < 1f)
			return false;

		if (blobs.Count >= maxBlobs)
			return false;

		if (gold < breedCost)
			return false;

		return true;
	}


	void AgeAllBlobs()
	{
		foreach(Blob blob in blobs)
		{
			BlobCell bc = getBlobCell(blob);
			bc.showProgressBar = false;
			bc.progressBar.value = 0f;
			blob.age++;
			blob.breededThisYear = false;
		}

		UpdateGrid();
	}


	int GetTotalEggs()
	{
		int total = 0;
		foreach(Blob blob in blobs)
		{
			if(blob.male)
				continue;
			total += (maxBreedcount - blob.breedCount);
		}
		return total;
	}


	void AddBlob(Blob newBlob)
	{
		blobs.Add(newBlob);
		if (blobs.Count >= maxBlobs)
			breedButton.isEnabled = false;
	}


	public void PressGridItem(int index)
	{
		curSelectedIndex = index;
		UpdateInfoTextWithBlob(blobs[index]);

		Transform blobCell = grid.transform.GetChild(index);
		blobCell.gameObject.SendMessage("OnClick");
	}


	public void UpdateInfoTextWithBlob(Blob blob)
	{

		colorLabel.text = "Color: " + Blob.GetNameFromEnum(blob.color);
		countLabel.text = "Eggs Left: " + (maxBreedcount - blob.breedCount).ToString();
		genotypeLabel.text = "Genotype: " + Blob.GetNameFromEnum(blob.allele1) + 
			(blob.allele2 != 0 ? ("/" + Blob.GetNameFromEnum(blob.allele2)) : "");
		qualityLabel.text = "Quality: " + blob.quality.ToString() + " (" + Blob.GetQualityStringFromValue(blob.quality) + ")";
		ageLabel.text = "Age: " + blob.age.ToString();
		if (blob.male)
		{
			genderLabel.text = "Gender: Male";
			countLabel.text = "";
		}
		else
			genderLabel.text = "Gender: Female";
	
	}

	public void PressBreedButton()
	{
		if(gold < breedCost || matingPairExists(false) == false)
			return;

		breedButton.isEnabled = false;
		breedProgressBar.value = 0;

		Blob maleBlob = maleBlobs[Random.Range(0, maleBlobs.Count)];
		Blob femaleBlob = femaleBlobs[Random.Range(0, femaleBlobs.Count)];
		AddGold(-breedCost);
		BreedBlobs(maleBlob, femaleBlob);
	}


	public void PressSellButton()
	{
		Blob blob = blobs[curSelectedIndex];

		if (blob.onMission)
			return;

		BlobCell bc = getBlobCell(blob);
		bc.progressBar.value = 0f;
		blobs.RemoveAt(curSelectedIndex);
		if (curSelectedIndex >= blobs.Count)
			PressGridItem(curSelectedIndex - 1);
		AddGold(sellValue);
		UpdateGrid();
		UpdateInfoTextWithBlob(blobs[curSelectedIndex]);
		CheckGameOver();
	}


	public void AddGold(int toAdd)
	{
		gold += toAdd;
		goldLabel.text = "Gold: " + gold.ToString() + "g";
	}


	void BreedBlobs(Blob maleBlob, Blob femaleBlob)
	{
		recentFather = maleBlob;
		recentMother = femaleBlob;
		
		maleBlob.breedCount++;
		femaleBlob.breedCount++;
		maleBlob.breededThisYear = true;
		femaleBlob.breededThisYear = true;

		getBlobCell(maleBlob).showProgressBar = true;
		getBlobCell(femaleBlob).showProgressBar = true;

		Blob blob = new Blob();
		blob.color = Random.Range(0, 2) == 0 ? maleBlob.color : femaleBlob.color;
		blob.allele1 = blob.color;
		int totalEggs = GetTotalEggs();
		if (GetTotalEggs() <= 0)
			blob.male = false;
		else
			blob.male = (Random.Range(0, 2) == 0) ? true : false;

		blob.quality = Blob.GetNewQuality(maleBlob.quality, femaleBlob.quality);

		// Mutation chance
		float mutationRoll = Random.Range(0f,1f);
		if(mutationRoll >= chanceForMutation)
		{
			BlobQuality q = Blob.GetQualityFromValue(blob.quality);
			if (q >= BlobQuality.Fair && blob.color == BlobColor.Blue)
			{
				blob.color = BlobColor.Red;
				blob.quality = 1;
			}	
			if (q >= BlobQuality.Good && blob.color == BlobColor.Red)
			{
				blob.color = BlobColor.Yellow;
				blob.quality = 1;
			}

			if (q >= BlobQuality.Excellent && blob.color == BlobColor.Yellow)
			{
				blob.color = BlobColor.Green;
				blob.quality = 1;
			}

			if (q >= BlobQuality.Outstanding && blob.color == BlobColor.Green)
			{
				blob.color = Random.Range(0f,1f) > .5f ? BlobColor.Orange : BlobColor.Purple;
				blob.quality = 1;
			}
		}

		AddBlob(blob);
		recentBaby = blob;
		UpdateGrid();
		PressGridItem(blobs.IndexOf(blob));
		CheckGameOver();

		BlobCell bc = getBlobCell(recentFather);
		bc.progressBar.value = 1f;

		bc = getBlobCell(recentMother);
		bc.progressBar.value = 1f;
	}


	BlobCell getBlobCell(Blob blob)
	{
		int index = blobs.IndexOf(blob);
		Transform blobCell = grid.transform.GetChild(index);
		GameObject cell = blobCell.gameObject;
		return cell.GetComponent<BlobCell>();
	}

	void KillBlob(Blob blob)
	{
		blob.alive = false;
		UpdateGrid();
		CheckGameOver();
	}


	void CheckGameOver()
	{
		if (matingPairExists(true) == false)
			gameOverObject.SetActive(true);

		bool hasBlue = false;
		bool hasRed = false;
		bool hasYellow = false;
		bool hasGreen = false;
		bool hasPurple = false;
		bool hasOrange = false;

		foreach(Blob blob in blobs)
		{
			hasBlue = blob.color == BlobColor.Blue;
			hasRed = blob.color == BlobColor.Red;
			hasYellow = blob.color == BlobColor.Yellow;
			hasGreen  = blob.color == BlobColor.Green;
			hasPurple = blob.color == BlobColor.Purple;
			hasOrange = blob.color == BlobColor.Orange;
		}

		if (hasBlue && hasRed && hasYellow && hasOrange && hasGreen && hasPurple)
		{
			winnerObject.SetActive(true);
		}
	}


	bool matingPairExists(bool gameOverCheck)
	{
		maleBlobs = new List<Blob>();
		femaleBlobs = new List<Blob>();
		
		foreach(Blob blob in blobs)
		{
			BlobCell bc = getBlobCell(blob);
			if(!gameOverCheck)
			{
				if (bc.progressBar.value > 0 || blob.onMission || blob.age < breedingAge)
					continue;
			}

			if( !blob.alive)
				continue;
			
			if (blob.male)
				maleBlobs.Add(blob);
			else if (blob.breedCount < maxBreedcount)
				femaleBlobs.Add(blob);
		}

		if (femaleBlobs.Count > 0 && maleBlobs.Count > 0)
			return true;

		return false;
	}


	public void MissionsButtonPressed()
	{
		breedingView.SetActive(false);
		missionView.SetActive(true);
		mm.UpdateMissionList();
	}
	

	// Update is called once per frame
	void Update () 
	{
		if(yearProgressBar.value < 1f)
		{
			yearProgressBar.value += Time.deltaTime * (1f / yearFillTime);

			if(yearProgressBar.value >= 1f)
			{
				yearProgressBar.value = 0f;
				year++;
				AgeAllBlobs();

				if(missionView.activeSelf)
					mm.UpdateMissionList();

				yearLabel.text = "Year: " + year.ToString();
				UpdateInfoTextWithBlob(blobs[curSelectedIndex]);
			}
		}

		if(breedProgressBar.value < 1f)
		{
			breedProgressBar.value += Time.deltaTime * (1f / breedBarFillTime);

			if(breedProgressBar.value >= 1f)
			{
				breedProgressBar.value = 1f;
				if (CanEnableBreedButton())
					breedButton.isEnabled = true;
			}
		}
	}
}
