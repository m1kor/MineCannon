using UnityEngine;
using System.Collections;

public class ReturnToMenuBA : ButtonAction {
	public Player player;

	IEnumerator Action() {
		yield return new WaitForEndOfFrame();
		player.ToggleMainMenu();
	}

	public override void Do(){
		StartCoroutine(Action());
	}
}
