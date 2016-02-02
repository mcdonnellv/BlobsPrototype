using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AiManager : MonoBehaviour {
	private static AiManager _aiManager;
	public static AiManager aiManager { get {if(_aiManager == null) _aiManager = GameObject.Find("AiManager").GetComponent<AiManager>(); return _aiManager; } }

	public T FindClosest<T>(Vector3 myPosition) where T : MonoBehaviour {
		float minDistance = float.MaxValue;
		T ret = null;
		foreach (T obj in FindObjectsOfType(typeof(T))) {
			float dist = (obj.gameObject.transform.position - myPosition).magnitude;
			if(dist < minDistance) {
				minDistance = dist;
				ret = obj;
			}
		}
		return ret;
	}
}


public abstract class NPC : MonoBehaviour {
	List<Behavior> behaviors = new List<Behavior>();


	// Use this for initialization
	void Start () {
		behaviors.Add(new Hunger(this, AiManager.aiManager));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


public class Wolf : NPC {

}


public class Food : MonoBehaviour {
	
}


public class Hunger : Sequence {
	public int hunger = 0;
	private NPC _npc;
	private AiManager _aiManager;
	private Vector3 _target;
	private Food _targetFood;

	public Hunger(NPC npc, AiManager aiManager) {
		_npc = npc;
		_aiManager = aiManager;

		Add<Behavior>().Update = IncreaseHunger;
		Add<Condition>().CanRun = IsHungry;
		Add<Behavior>().Update = LocateFood;
		Selector selector = Add<Selector>();
		Sequence sequence = selector.Add<Sequence>();
		sequence.Add<Condition>().CanRun = NearFood;
		sequence.Add<Behavior>().Update = EatFood;
	}

	public Status IncreaseHunger() {
		hunger++;
		return Status.BhSuccess;
	}

	public bool IsHungry() {
		return hunger > 100;
	}

	public Status LocateFood() {
		if(_targetFood == null) {
			Food food = _aiManager.FindClosest<Food>(_npc.transform.position);
			if(food == null) {
				Debug.Log("Could not find food");
				return Status.BhFailure;
			}
			_target = food.transform.position;
			_targetFood = food;
			//TODO: set somehting here that triggers it to moveto position like target  = food.gameobject
		}
		return Status.BhSuccess;
	}

	public bool NearFood() {
		return (_npc.transform.position - _target).magnitude <= 0.25f;
	}

	
	public Status EatFood() {
		Debug.Log("Eating");
		hunger = 0;
		GameObject.Destroy(_targetFood);
		_targetFood = null;
		return Status.BhSuccess;
	}
}






