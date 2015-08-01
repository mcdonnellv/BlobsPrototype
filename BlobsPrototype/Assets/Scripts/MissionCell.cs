using UnityEngine;
using System.Collections;

public class MissionCell : MonoBehaviour 
{
	public UILabel requirementLabel;
	public UILabel chanceLabel;
	public UILabel rewardLabel;
	public UILabel durationLabel;
	public UISlider slider;
	public GameObject blobContainer;
	public UIButton startButton;
	public UIButton blobButton;
	public MissionManager mm;

	public void BlobButtonPressed()
	{
		int index = transform.GetSiblingIndex();
		mm.PressMissionItemBlob(index);
	}

	public void StartButtonPressed()
	{
		int index = transform.GetSiblingIndex();
		mm.PressMissionItemStart(index);
	}
}
