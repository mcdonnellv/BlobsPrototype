using UnityEngine;
using System.Collections;

public class BlobCell : MonoBehaviour 
{

	public GameManager gm;
	public UISlider progressBar;
	public UIButton button;
	public GameObject onMissionLabel;
	public bool showProgressBar = false;

	// Use this for initialization
	public void Pressed () 
	{
		int index = transform.GetSiblingIndex();
		gm.PressGridItem(index);
	}

	void Update () 
	{
		if(showProgressBar)
			progressBar.value = 1f - gm.yearProgressBar.value;
		else
			progressBar.value = 0f;
	}

}
