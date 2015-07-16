using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MissionManager : MonoBehaviour 
{
	public GameManager gm;
	public List<Mission> missions;
	public GameObject missionPanel;

	float mins5 = 60f * 5f;

	// Use this for initialization
	void Start () 
	{
		missions = new List<Mission>();

		Mission mission = new Mission();
		missions.Add(mission);
		updateMisisonList();
	}


	MissionCell GetMissionCell(Mission mission)
	{
		int index = missions.IndexOf(mission);
		GameObject cell = missionPanel.transform.GetChild(index).gameObject;
		return cell.GetComponent<MissionCell>();
	}


	void updateMisisonList()
	{
		foreach(Transform missionCell in missionPanel.transform)
		{
			GameObject cell = missionCell.gameObject;

			if (missionCell.GetSiblingIndex() < missions.Count)
			{
				cell.gameObject.SetActive(true);
				Mission mission = missions[missionCell.GetSiblingIndex()];
				MissionCell mc = cell.GetComponent<MissionCell>();
				int timeMin = (int)((mission.duration - mission.durationCounting) / 60f);
				int timeSec = (int)((mission.duration - mission.durationCounting) % 60f);
				mc.durationLabel.text = timeMin.ToString() + "m " + timeSec.ToString() + "s";
				mc.chanceLabel.text = "0%";//((int)(mission.GetSuccessChance() * 100f)).ToString() + "%";
				mc.rewardLabel.text = ((int)mission.reward).ToString() + "g";
				mc.requirementLabel.text = "Requirements: none";

				if(mission.blob == null)
				{
					mc.blobContainer.SetActive(false);
					mc.startButton.isEnabled = false;
				}
				else
				{
					mc.blobContainer.SetActive(true);
					mc.startButton.isEnabled = true;

					UISprite[] sprites = cell.GetComponentsInChildren<UISprite>();
					sprites[1].color = Blob.GetColorFromEnum(mission.blob.color);
					sprites[2].color = Color.white;
					sprites[3].enabled = !mission.blob.male;
				}
			}
			else
			{
				cell.gameObject.SetActive(false);
			}
		}
	}


	public void PressMissionItemBlob(int index)
	{
		Mission mission = missions[index];

		if (mission.blob == null)
		{
			mission.blob = gm.blobs[0];
			mission.blob.onMission = true;
		}
		else
		{
			mission.blob.onMission = false;
			mission.blob = null;
		}

		updateMisisonList();
	}

	
	public void PressMissionItemStart(int index)
	{
		Mission mission = missions[index];

		if(!mission.active)
		{
			mission.active = true;
			
			MissionCell mc = GetMissionCell(mission);
			UILabel startButtonLabel = mc.startButton.GetComponentInChildren<UILabel>();
			startButtonLabel.text = "Collect";
			mc.startButton.isEnabled = false;
			mc.blobButton.isEnabled = false;
		}
		else
		{
			mission.blob.onMission = false;
			gm.AddGold(mission.reward);
			missions.Remove(mission);
			updateMisisonList();
		}
	}


	void MissionReady(Mission mission)
	{
		MissionCell mc = GetMissionCell(mission);
		mc.startButton.isEnabled = true;
	}


	// Update is called once per frame
	void Update () 
	{
		foreach(Mission mission in missions)
		{
			if (!mission.active)
				continue;

			mission.durationCounting += Time.deltaTime;
			MissionCell mc = GetMissionCell(mission);
			mc.slider.value = 1f - (mission.durationCounting / mission.duration);
			int timeMin = (int)((mission.duration - mission.durationCounting) / 60f);
			int timeSec = (int)((mission.duration - mission.durationCounting) % 60f);
			mc.durationLabel.text = timeMin.ToString() + "m " + timeSec.ToString() + "s";

			if (mission.durationCounting >= mission.duration)
			{
				mission.durationCounting = mission.duration;
				MissionReady(mission);
			}
		}
	}

	public void BreedingButtonPressed()
	{
		gm.breedingView.SetActive(true);
		gm.missionView.SetActive(false);
		gm.UpdateGrid();
	}
}
