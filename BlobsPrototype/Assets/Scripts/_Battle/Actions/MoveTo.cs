//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using TreeSharp;
//
////public class MoveTo : Action {
////}
//
//
//
//public class AtDestination : Action {
//	private Vector2 dest;
//	
//	public AtDestination(Vector2 d) {
//		dest = d;
//	}
//	
//	protected override RunStatus Run(object context) {
//		Combatant combatant = (Combatant)context;
//		if(combatant.transform.localPosition.x == dest.x && combatant.transform.localPosition.y == dest.y)
//			return RunStatus.Success;
//		return RunStatus.Failure;
//	}
//}
//
//// check to see if an enemy is inside our perception area
//public class EnemyInRange : Action {
//	protected override RunStatus Run(object context) {
//		Combatant combatant = (Combatant)context;
//		float perceptionArea = combatant.perception;
//		Faction opposingFaction = combatant.faction == Faction.blob ? Faction.enemy : Faction.blob;
//		List<Combatant> enemies = CombatManager.combatManager.GetCombatantsByFaction(opposingFaction);
//		foreach(Combatant c in enemies)
//			if(Vector3.Distance(c.transform.localPosition - combatant.transform.localPosition) > perceptionArea)
//				return RunStatus.Success;
//		return RunStatus.Failure;
//	}
//}
//
//
//// set target to the closest enemy if any
//public class TargetNearestEnemy : Action {
//	protected override RunStatus Run(object context) {
//		Combatant combatant = (Combatant)context;
//		Faction opposingFaction = combatant.faction == Faction.blob ? Faction.enemy : Faction.blob;
//		List<Combatant> enemies = CombatManager.combatManager.GetCombatantsByFaction(opposingFaction);
//		float leastDistance = combatant.perception;
//		Combatant leastDistanceEnemy = null;
//		foreach(Combatant c in enemies) {
//			float distance = Vector3.Distance(c.transform.localPosition - combatant.transform.localPosition);
//			if(distance < leastDistance) {
//				leastDistance = distance;
//				leastDistanceEnemy = c;
//			}
//		}
//
//		if(leastDistanceEnemy != null) {
//			combatant.target = leastDistanceEnemy;
//			return RunStatus.Success;
//		}
//
//		return RunStatus.Failure;
//	}
//}
//
//
