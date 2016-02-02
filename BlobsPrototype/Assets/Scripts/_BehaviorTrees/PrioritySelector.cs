using UnityEngine;
using System;
using System.Collections;

// Priority selectors will always check the 1st child's behavior first every tick
// So on 2nd 3rd etc ticks it always executes the 1st child, and resets everything else that it touched on the 1st tick.
// example, an orc peon's priority selector has 2 child behaviors 1)defend self, and 2)gather wood. an orc is gathering wood (running),
// when it gets attacked, the priority selector will check defend self node first. which means it will stop gathering wood and fight.
// where an ordinary selector would have kept the orc gathering wood.

public class PrioritySelector : Selector {

	private int _lastSelector;

	public PrioritySelector() {
		Update = () => {
			_selector = 0;
			for(;;) {
				Status s = GetChild(_selector).Tick();
				if(s != Status.BhFailure) {
					for(int i = _selector + 1; i <= _lastSelector; i++) 
						GetChild(i).Reset();
					_lastSelector = _selector;
					return s;
				}

				if(++_selector == ChildCount) 
					return Status.BhFailure;
			}
		}; 
	}
}