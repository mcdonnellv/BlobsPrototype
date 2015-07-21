using UnityEngine;
using System.Collections;

public class BlobCell : MonoBehaviour 
{

	public NurseryManager nm;
	public UISlider progressBar;
	public UIButton button;
	public GameObject onMissionLabel;
	public bool showProgressBar = false;
	public UISprite body;
	public UISprite face;
	public UISprite lashes;
	public UISprite egg;

	// Use this for initialization
	public void Pressed () 
	{
		int index = transform.GetSiblingIndex();
		nm.PressGridItem(index);
	}

	public void Reset()
	{
		progressBar.value = 0f;
	}

	void Update () 
	{
		//if(showProgressBar)
		//	progressBar.value = 1f - gm.yearProgressBar.value;
		//else
		//	progressBar.value = 0f;
	}

}
