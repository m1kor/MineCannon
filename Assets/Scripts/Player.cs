using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public Transform cameraTransform, floorColliderTransform, bg1Transform, bg2Transform, bg3Transform, platformTransform, leftWallColliderTransform;
	public TextMesh scoreTextMesh, pigsTextMesh, gameOverTextMesh, scoreNotificationTextMesh, highScoreTextMesh;
	public GameObject baloonTNTPrefab, tntPrefab, cactusPrefab, pigPrefab, springPrefab, lava1Prefab, lava2Prefab, baloonPlatformPrefab, forceLine, btnRetry, btnReturnToMenu;
	public bool launched, aimed;
	public FireBA fireBA;

	private bool mainMenu;
	private float distanceTraveled, screenW, bgW, traveled, prevCameraX, nextSpawn, rotSpeed, rotEdge;
	private int addBg;
	private List<GameObject> powerUps, toRemove;

	private Transform platformTransformBackup;
	private Vector3 bg1Pos, bg2Pos, bg3Pos, cameraPos, playerPos;
	private Quaternion platformRotation;

	void Awake () {
		Application.targetFrameRate = 60;
	}

	void Start() {
		screenW = Screen.width / (Screen.height * 0.5f) * 10.0f;
		leftWallColliderTransform.Translate(Vector3.left * screenW * 0.5f, Space.World);
		bgW = 846.0f / 32.0f;
		bg1Transform.position = new Vector3((bgW - screenW) * 0.5f, 0.0f, 0.0f);
		bg2Transform.position = new Vector3((bgW * 3.0f - screenW) * 0.5f, 0.0f, 0.0f);
		bg3Transform.position = new Vector3((bgW * 5.0f - screenW) * 0.5f, 0.0f, 0.0f);
		GetComponent<Rigidbody2D>().isKinematic = true;
		powerUps = new List<GameObject>();
		toRemove = new List<GameObject>();
		bg1Pos = bg1Transform.position;
		bg2Pos = bg2Transform.position;
		bg3Pos = bg3Transform.position;
		playerPos = transform.position;
		platformRotation = platformTransform.rotation;
		cameraPos = cameraTransform.position;
		platformTransformBackup = platformTransform;
		forceLine.transform.localPosition = new Vector3(-screenW * 0.5f, 9.5f, 8.0f);
		ToggleMainMenu();
	}

	public void ToggleMainMenu() {
		mainMenu = true;
		scoreTextMesh.renderer.enabled = false;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Controls"))
			go.renderer.enabled = false;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("MainMenu"))
			go.renderer.enabled = true;
		bg1Transform.position = bg1Pos;
		bg2Transform.position = bg2Pos;
		bg3Transform.position = bg3Pos;
		transform.position = playerPos;
		transform.rotation = platformRotation;
		cameraTransform.position = cameraPos;
		platformTransform = platformTransformBackup;
		platformTransform.rotation = platformRotation;
		GetComponent<Rigidbody2D>().isKinematic = true;
		int score = PlayerPrefs.GetInt("Pigs", 0);
		if (score == 1)
			pigsTextMesh.text = score.ToString() + " PIG";
		else
			pigsTextMesh.text = score.ToString() + " PIGS";
		gameOverTextMesh.renderer.enabled = false;
		scoreNotificationTextMesh.renderer.enabled = false;
		scoreNotificationTextMesh.GetComponent<Animator>().enabled = false;
		highScoreTextMesh.renderer.enabled = true;
		highScoreTextMesh.text = ScoreAsString(PlayerPrefs.GetInt("HighScore", 0));
		forceLine.renderer.enabled = false;
		fireBA.StopAnimatingForceLine();
		btnRetry.renderer.enabled = false;
		btnReturnToMenu.renderer.enabled = false;
		foreach (GameObject go in powerUps)
			Destroy(go);
		powerUps.Clear();
		GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}

	public void ToggleGame() {
		mainMenu = false;
		scoreTextMesh.renderer.enabled = true;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Controls"))
			go.renderer.enabled = true;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("MainMenu"))
			go.renderer.enabled = false;
		platformTransform = platformTransformBackup;
		bg1Transform.position = bg1Pos;
		bg2Transform.position = bg2Pos;
		bg3Transform.position = bg3Pos;
		transform.position = playerPos;
		transform.rotation = platformRotation;
		cameraTransform.position = cameraPos;
		platformTransform.rotation = platformRotation;
		GetComponent<Rigidbody2D>().isKinematic = true;
		addBg = 1;
		traveled = -bg1Transform.position.x - bgW * 0.5f;
		distanceTraveled = 0.0f;
		prevCameraX = 0.0f;
		nextSpawn = Random.Range(50.0f, 150.0f);
		transform.parent = platformTransform;
		gameOverTextMesh.renderer.enabled = false;
		highScoreTextMesh.renderer.enabled = false;
		launched = false;
		aimed = false;
		rotSpeed = Random.Range(35.0f, 135.0f);
		rotEdge = Random.Range(25.0f, 65.0f);
		btnRetry.renderer.enabled = false;
		btnReturnToMenu.renderer.enabled = false;
		scoreNotificationTextMesh.renderer.enabled = false;
		scoreNotificationTextMesh.GetComponent<Animator>().enabled = false;
		foreach (GameObject go in powerUps)
			Destroy(go);
		powerUps.Clear();
		GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}

	private string ScoreAsString(int s) {
		if (s < 10)
			return "000" + s.ToString();
		else if (s < 100)
			return "00" + s.ToString();
		else if (s < 1000)
			return "0" + s.ToString();
		else
			return s.ToString();
	}

	public void GameOver() {
		GetComponent<Rigidbody2D>().isKinematic = true;
		int score = Mathf.FloorToInt(distanceTraveled);
		if (score > PlayerPrefs.GetInt("HighScore", 0))
		{
			PlayerPrefs.SetInt("HighScore", score);
			scoreNotificationTextMesh.renderer.enabled = true;
			scoreNotificationTextMesh.GetComponent<Animator>().enabled = true;
		}
		gameOverTextMesh.renderer.enabled = true;
		btnRetry.renderer.enabled = true;
		btnReturnToMenu.renderer.enabled = true;
	}

	public void Pig() {
		PlayerPrefs.SetInt("Pigs", PlayerPrefs.GetInt("Pigs", 0) + 1);
	}

	void FixedUpdate() {
		if (!mainMenu) {
			floorColliderTransform.position = new Vector3 (transform.position.x, -8.194516f, -2.0f);
			if (launched && GetComponent<Rigidbody2D>().velocity.magnitude < 0.01f)
				GameOver();
		}
	}

	void Update() {
		if (!mainMenu) {
			if (!launched && !aimed) {
				if (platformTransform.rotation.eulerAngles.z < rotEdge) {
					platformTransform.Rotate(Vector3.forward, rotSpeed * Time.deltaTime);
					if (platformTransform.rotation.eulerAngles.z >= rotEdge) {
						rotEdge = Random.Range(5.0f, platformTransform.rotation.eulerAngles.z - 10.0f);
						rotSpeed = Random.Range(35.0f, 135.0f);
					}
				}
				else {
					platformTransform.Rotate(-Vector3.forward, rotSpeed * Time.deltaTime);
					if (platformTransform.rotation.eulerAngles.z <= rotEdge) {
						rotEdge = Random.Range(platformTransform.rotation.eulerAngles.z + 10.0f, 65.0f);
						rotSpeed = Random.Range(35.0f, 135.0f);
					}
				}
			}
			if (transform.position.x > prevCameraX)
				cameraTransform.position = new Vector3(Mathf.Max(0.0f, transform.position.x), 0.0f, -10.0f);
			traveled += Mathf.Max(0.0f, cameraTransform.position.x - prevCameraX);
			distanceTraveled += Mathf.Max(0.0f, cameraTransform.position.x - prevCameraX);
			scoreTextMesh.text = ScoreAsString(Mathf.FloorToInt(distanceTraveled));
			if (traveled >= bgW) {
				traveled -= bgW;
				switch (addBg) {
					case 1:
						bg1Transform.position = new Vector3(bg1Transform.position.x + bgW * 3.0f, 0.0f, 0.0f);
						break;
					case 2:
						bg2Transform.position = new Vector3(bg2Transform.position.x + bgW * 3.0f, 0.0f, 0.0f);
						break;
					case 3:
						bg3Transform.position = new Vector3(bg3Transform.position.x + bgW * 3.0f, 0.0f, 0.0f);
						break;
				}
				addBg++;
				if (addBg > 3)
					addBg = 1;
			}
			nextSpawn -= Mathf.Max(0.0f, cameraTransform.position.x - prevCameraX);
			if (nextSpawn < 0.0f) {
				nextSpawn = Random.Range(25.0f, 125.0f);
				GameObject powerUp = null;
				bool setPos = true;
				switch (Random.Range(0, 500) % 25) {
				case 1: // Pig
					powerUp = (GameObject)Instantiate(pigPrefab);
					break;
				default:
					switch (Random.Range(0, 99) % 4) {
					case 0: // TNT
						switch (Random.Range(0, 99) % 3) {
						case 0:
							powerUp = (GameObject)Instantiate(baloonTNTPrefab);
							setPos = false;
							powerUp.transform.position = new Vector3(cameraTransform.position.x + 30.0f, Random.Range(0.0f, 8.5f), -1.0f);
							powerUp.GetComponent<PowerUp>().yPos = powerUp.transform.position.y;
							powerUp.GetComponent<PowerUp>().tvar = Random.Range(0.0f, Mathf.PI);
							break;
						default:
							powerUp = (GameObject)Instantiate(tntPrefab);
							break;
						}
						break;
					case 1: // Cactus
						powerUp = (GameObject)Instantiate(cactusPrefab);
						break;
					case 2: // Spring
						switch (Random.Range(0, 99) % 5) {
						case 1: // Platform
							powerUp = (GameObject)Instantiate(baloonPlatformPrefab);
							setPos = false;
							powerUp.transform.position = new Vector3(cameraTransform.position.x + 30.0f, Random.Range(1.9f, 5.9f), -1.0f);
							powerUp.GetComponent<PowerUp>().yPos = powerUp.transform.position.y;
							powerUp.GetComponent<PowerUp>().tvar = Random.Range(0.0f, Mathf.PI);
							break;
						default:
							powerUp = (GameObject)Instantiate(springPrefab);
							break;
						}
						break;
					case 3: // Lava
						switch (Random.Range(0, 99) % 2) {
						case 0:
							powerUp = (GameObject)Instantiate(lava1Prefab);
							break;
						case 1:
							powerUp = (GameObject)Instantiate(lava2Prefab);
							break;
						}
						break;
					}
					break;
				}
				if (setPos)
					powerUp.transform.position = new Vector3(cameraTransform.position.x + 30.0f, -8.194516f, -1.0f);
				powerUps.Add(powerUp);
			}
			prevCameraX = cameraTransform.position.x;
			foreach (GameObject go in powerUps)
				if (go.transform.position.x < cameraTransform.position.x - 60.0f)
					toRemove.Add(go);
			foreach (GameObject go in toRemove) {
				powerUps.Remove(go);
				Destroy(go);
			}
			toRemove.Clear();
		}
	}
}
