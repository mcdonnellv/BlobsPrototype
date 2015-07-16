using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
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
	public UILabel mateButtonLabel;
	public UISlider mateProgressBar;
	public UISlider deleteProgressBar;
	public UIButton mateButton;
	public UIButton deleteButton;
	public GameObject gameOverObject;
	public GameObject winnerObject;
	public int maxBreedcount = 3;
	public float mateBarFillTime = 2f;
	public float deleteBarFillTime = 10f;
	public int maxBlobs = 20;
	public float maleMateDelay = 10f;
	public float femaleMateDelay = 3f;
	public float chanceForMutation = .8f;
	public int gold = 100;
	public int breedCost = 5;
	public int breedBaseCost = 5;
	public int breedCostMax = 50;
	public int sellValue = 5;
	public int curSelectedIndex;

	Blob recentFather;
	Blob recentMother;
	Blob recentBaby;
	bool culling;
	bool selectMode = false;
	List<Blob> maleBlobs;
	List<Blob> femaleBlobs;

	
	// Use this for initialization
	void Start ()
	{
		maleBlobs = new List<Blob>();
		femaleBlobs = new List<Blob>();

		breedingView.SetActive(true);
		missionView.SetActive(false);
		selectModeCover.SetActive(false);

		curSelectedIndex = 0;
		blobs = new List<Blob>();

		Blob blob = new Blob();
		blob.color = BlobColor.Blue;
		blob.allele1 = blob.color;
		blob.male = true;
		UpdateInfoTextWithBlob(blob);

		AddBlob(blob);

		blob = new Blob();
		blob.color = BlobColor.Blue;
		blob.allele1 = blob.color;
		blob.male = false;

		AddBlob(blob);
	
		UpdateGrid();
		AddGold(0);

		mateProgressBar.value = 1f;
		deleteProgressBar.value = 0f;
		deleteButton.isEnabled = false;
		culling = false;
	}


	public void EnableSelectMode(bool enable)
	{
		selectMode = enable;
		selectModeCover.SetActive(enable);
	}


	public void UpdateGrid()
	{
		int i=0;
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
				labels[1].text = (blob.male) ? "" : (maxBreedcount - blob.breedCount).ToString();
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

				bc.OnMissionLabel.SetActive(blob.onMission);

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
		mateButtonLabel.text = "Breed (" + breedCost.ToString() + "g)";
	}


	void AgeAllBlobs()
	{
		foreach(Blob blob in blobs)
		{
			blob.age++;
		}
	}


	void AddBlob(Blob newBlob)
	{
		blobs.Add(newBlob);

		List<Blob> maleBlobs = new List<Blob>();
		List<Blob> femaleBlobs = new List<Blob>();
		
		foreach(Blob blob in blobs)
		{
			if(blob.male && blob.alive)
				maleBlobs.Add(blob);
			else if(blob.alive)
				femaleBlobs.Add(blob);
		}
		
		blobs.Clear();
		
		foreach(Blob blob in maleBlobs)
			blobs.Add(blob);
		
		foreach(Blob blob in femaleBlobs)
			blobs.Add(blob);

		if (blobs.Count >= maxBlobs)
			mateButton.enabled = false;
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

	public void PressMateButton()
	{
		if(gold < breedCost || matingPairExists(false) == false)
			return;

		mateButton.isEnabled = false;
		mateProgressBar.value = 0;

		if(deleteProgressBar.value == 1f && culling)
		{
			deleteProgressBar.value = 0f;
			deleteButton.isEnabled = false;
			culling  = false;
		}

		Blob maleBlob = maleBlobs[Random.Range(0, femaleBlobs.Count)];
		Blob femaleBlob = femaleBlobs[Random.Range(0, femaleBlobs.Count)];
		AddGold(-breedCost);
		mateBlobs(maleBlob, femaleBlob);
	}


	public void PressDeleteButton()
	{
		culling = true;
		Blob blob = blobs[curSelectedIndex];
		BlobCell bc = getBlobCell(blob);
		bc.progressBar.value = 0f;
		blobs.RemoveAt(curSelectedIndex);
		AddGold(sellValue);
		UpdateGrid();
		UpdateInfoTextWithBlob(blobs[curSelectedIndex]);
	}


	public void AddGold(int toAdd)
	{
		gold += toAdd;
		goldLabel.text = "Gold: " + gold.ToString() + "g";
	}


	void mateBlobs(Blob maleBlob, Blob femaleBlob)
	{
		recentFather = maleBlob;
		recentMother = femaleBlob;
		
		maleBlob.breedCount++;
		femaleBlob.breedCount++;

		Blob blob = new Blob();
		blob.color = Random.Range(0, 2) == 0 ? maleBlob.color : femaleBlob.color;
		blob.allele1 = blob.color;
		blob.male = Random.Range(0, 2) == 0 ;
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
		bc.fillSpeed = maleMateDelay;

		bc = getBlobCell(recentMother);
		bc.progressBar.value = 1f;
		bc.fillSpeed = femaleMateDelay;

		AgeAllBlobs();
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


	public void BlobCellProgressDone(BlobCell blobCell)
	{
		if (matingPairExists(false) && mateProgressBar.value == 1f)
			mateButton.isEnabled = true;
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
				if (bc.progressBar.value > 0 || blob.onMission)
					continue;
			}

			if( !blob.alive)
				continue;
			
			if(blob.male)
				maleBlobs.Add(blob);
			else
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
	}


	// Update is called once per frame
	void Update () 
	{
		if(mateProgressBar.value < 1f)
		{
			mateProgressBar.value += Time.deltaTime * (1f / mateBarFillTime);

			if(mateProgressBar.value >= 1f)
			{
				mateProgressBar.value = 1f;
				if (matingPairExists(false))
					mateButton.isEnabled = true;
			}
		}

		if(deleteProgressBar.value < 1f)
		{
			deleteProgressBar.value += Time.deltaTime * (1f / deleteBarFillTime);
			
			if(deleteProgressBar.value >= 1f)
			{
				deleteProgressBar.value = 1f;
				deleteButton.isEnabled = true;
			}
		}
	}
}
