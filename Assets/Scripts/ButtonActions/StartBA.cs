using UnityEngine;
using System.Collections;

public class StartBA : ButtonAction {
	public Player player;

	public override void Do() {
		player.ToggleGame();
	}
}
