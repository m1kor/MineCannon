using UnityEngine;
using System.Collections;

public class FireBA : ButtonAction {
	public GameObject player, forceLine;
	public AnimationCurve forceCurve;

	private Player playerComp;
	private Rigidbody2D playerRB;
	private const float forceTimeLimit = 0.83333f;
	private bool animateForceLine;
	private float forceTime, screenW;

	void Start() {
		playerComp = player.GetComponent<Player>();
		screenW = Screen.width / (Screen.height * 0.5f) * 10.0f;
		playerRB = player.GetComponent<Rigidbody2D>();
		animateForceLine = false;
	}

	IEnumerator Launch() {
		yield return new WaitForSeconds(0.5f);
		playerComp.launched = true;
	}

	public void AnimateForceLine() {
		forceTime = 0.0f;
		animateForceLine = true;
		forceLine.renderer.enabled = true;
		StopCoroutine("HideForceLine");
	}

	public void StopAnimatingForceLine() {
		animateForceLine = false;
		StartCoroutine("HideForceLine");
	}

	IEnumerator HideForceLine() {
		yield return new WaitForSeconds(2.0f);
		forceLine.renderer.enabled = false;
	}

	public override void Do() {
		if (!playerComp.aimed) {
			playerComp.aimed = true;
			AnimateForceLine();
		}
		else {
			if (forceTime < 0.2f) {
				forceTime = 0.2f;
				forceLine.transform.localScale = new Vector3(screenW * forceCurve.Evaluate(forceTime), 512.0f, 1.0f);
			}
			playerRB.isKinematic = false;
			playerRB.AddForce(playerRB.transform.TransformDirection(Vector3.right) * playerRB.mass * 2000.0f * (forceCurve.Evaluate(forceTime) + 0.5f));
			playerRB.transform.parent = null;
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Controls"))
				obj.renderer.enabled = false;
			StartCoroutine(Launch());
			StopAnimatingForceLine();
		}
	}

	public void Update() {
		if (animateForceLine) {
			forceTime += Time.deltaTime;
			if (forceTime > forceTimeLimit)
				forceTime -= forceTimeLimit;
			forceLine.transform.localScale = new Vector3(screenW * forceCurve.Evaluate(forceTime), 512.0f, 1.0f);
		}
	}
}
