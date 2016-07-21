using UnityEngine;
using System.Collections;

public class RetryBA : ButtonAction {
	public Player player;
	
	IEnumerator Action() {
		yield return new WaitForEndOfFrame();
		player.ToggleGame();
	}
	
	public override void Do() {
		StartCoroutine(Action());
	}
}
