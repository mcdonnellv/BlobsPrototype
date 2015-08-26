using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectDifficulty : MonoBehaviour {
	public int index;
	public Toggle[] toggles;

	public void OnSelect(){
		for (int i=0; i< toggles.Length; i++) {
			if(i != index){
				toggles[i].isOn=false;
			}
		}
		toggles [index].isOn = true;
	}
}
