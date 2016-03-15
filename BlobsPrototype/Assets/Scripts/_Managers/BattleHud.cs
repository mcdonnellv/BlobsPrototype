using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleHud : MonoBehaviour {
	public UILabel commandLabel;
	public UILabel actionLabel;
	public UIProgressBar timeBar;
	public List<BattleBlobLifeBar> lifeBars;
	public Camera camera;
	public UILabel battleTimerLabel;
	public GameObject defeatObject;
	public GameObject victoryObject;
}
