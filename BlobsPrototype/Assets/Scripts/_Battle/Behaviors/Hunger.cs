using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Patrol : Sequence {

	private NPC _npc;
	private int _targetWaypointIndex;
	private System.DateTime _restTimeUp;
	private List<GameObject> _waypoints;

	public Patrol(NPC npc, List<GameObject> waypoints) {
		_npc = npc;
		_restTimeUp = System.DateTime.MaxValue;
		_targetWaypointIndex = -1;
		_waypoints = waypoints;


		Add<Behavior>().Update = GetWaypoint;
		Add<Behavior>().Update = WalkToWaypoint;

		Selector selector = Add<Selector>();
		Sequence sequence = selector.Add<Sequence>();
		sequence.Add<Condition>().CanRun = IsNearWaypoint;
		sequence.Add<Condition>().Update = HangOut;
		Add<Condition>().CanRun = IsRestTimeExpired;
	}

	public Status GetWaypoint() {
		if(_waypoints == null || _waypoints.Count <= 0) {
			Debug.Log(">>>No waypoints");
			return Status.BhFailure;
		}

		if(_targetWaypointIndex == -1)
			_targetWaypointIndex = 0;
		else
			_targetWaypointIndex = (_targetWaypointIndex + 1) % _waypoints.Count;
		return Status.BhSuccess;
	}
	
	public Status WalkToWaypoint() {
		_npc.moveToTarget = _waypoints[_targetWaypointIndex];
		return Status.BhSuccess;
	}

	public bool IsNearWaypoint() {
		float mag = (_npc.transform.position - _waypoints[_targetWaypointIndex].transform.position).magnitude;
		return mag <= 0.25f;
	}

	public Status HangOut() {
		Debug.Log(">>>Reached waypoint. Hanging Out");
		_npc.moveToTarget = _npc.gameObject;
		_restTimeUp = System.DateTime.Now + new System.TimeSpan(0,0,3);
		return Status.BhSuccess;
	}

	public bool IsRestTimeExpired() {
		return _restTimeUp <= System.DateTime.Now;
	}
}


public class SelfPreservation : Sequence {

	private NPC _npc;
	private AiManager _aiManager;

	public SelfPreservation(NPC npc, AiManager aiManager) {
	}

	public bool IsCriticalHealth() {
		return (_npc.health / _npc.combatStats.health.combatValue < 0.2f);
	}
}


public class Food : MonoBehaviour{}

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
				Debug.Log(">>>Could not find food");
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
		Debug.Log(">>>Eating");
		hunger = 0;
		GameObject.Destroy(_targetFood);
		_targetFood = null;
		return Status.BhSuccess;
	}
}

