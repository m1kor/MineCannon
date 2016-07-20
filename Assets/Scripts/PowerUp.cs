using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour
{
	public Sprite explosion, spring1, spring2, deadPig;
	public int kind;
	public Transform platformTransform;
	public float yPos, tvar;

	private bool zoomOut, drown, onBack, floating, isEnabled, onBackRand;
	private Transform playerTransform;
	private SpriteRenderer playerSpriteRenderer;

	void Start()
	{
		zoomOut = drown = onBack = false;
		isEnabled = true;
		floating = true;
		tvar = 1.0f;
	}

	IEnumerator Hide()
	{
		yield return new WaitForSeconds(0.25f);
		GetComponent<SpriteRenderer>().enabled = false;
	}

	IEnumerator Reverse()
	{
		yield return new WaitForSeconds(0.25f);
		GetComponent<SpriteRenderer>().sprite = spring1;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (isEnabled && coll.gameObject.tag == "Player")
		{
			Rigidbody2D rb = null;
			switch (kind)
			{
			case 0: // TNT
				rb = coll.gameObject.GetComponent<Rigidbody2D>();
				rb.AddForceAtPosition(new Vector2(1.0f, 0.8f).normalized * rb.mass * 1500.0f, new Vector2(coll.gameObject.transform.position.x - 1.75f, coll.gameObject.transform.position.y));
				GetComponent<SpriteRenderer>().sprite = explosion;
				StartCoroutine(Hide());
				break;
			case 1: // Cactus
				coll.gameObject.GetComponent<Player>().GameOver();
				break;
			case 2: // Pig
				coll.gameObject.GetComponent<Player>().Pig();
				rb = coll.gameObject.GetComponent<Rigidbody2D>();
				rb.AddForce(Vector2.up * rb.mass * 1200.0f);
				GetComponent<SpriteRenderer>().sprite = deadPig;
				onBack = true;
				onBackRand = (Random.Range(0, 99) % 2 == 0) ? true : false;
				tvar = 0.25f;
				isEnabled = false;
				break;
			case 3: // Spring
				rb = coll.gameObject.GetComponent<Rigidbody2D>();
				rb.AddForce(Vector2.up * rb.mass * 1200.0f);
				GetComponent<SpriteRenderer>().sprite = spring2;
				StartCoroutine(Reverse());
				break;
			case 4: // Lava
				coll.gameObject.GetComponent<Player>().GameOver();
				playerTransform = coll.gameObject.transform;
				playerSpriteRenderer = coll.gameObject.GetComponent<SpriteRenderer>();
				drown = true;
				break;
			case 5: // Baloon TNT
				rb = coll.gameObject.GetComponent<Rigidbody2D>();
				rb.AddForceAtPosition(new Vector2(1.0f, 0.5f).normalized * rb.mass * 1500.0f, new Vector2(coll.gameObject.transform.position.x - 2.5f, coll.gameObject.transform.position.y));
				GetComponent<SpriteRenderer>().sprite = explosion;
				transform.Translate(Vector3.down * 5.040815f);
				floating = false;
				StartCoroutine(Hide());
				break;
			case 6: // Baloon platform
				coll.gameObject.transform.position = transform.position + Vector3.down * 2.609126f;
				coll.gameObject.transform.parent = platformTransform;
				coll.gameObject.transform.rotation = Quaternion.identity;
				coll.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
				coll.gameObject.GetComponent<Player>().platformTransform = transform;
				coll.gameObject.GetComponent<Player>().launched = false;
				coll.gameObject.GetComponent<Player>().aimed = false;
				floating = false;
				foreach (GameObject go in GameObject.FindGameObjectsWithTag("Controls"))
					go.renderer.enabled = true;
				break;
			}
			isEnabled = false;
		}
	}

	void Update()
	{
		if (zoomOut)
		{
			tvar -= Time.deltaTime * 2.5f;
			if (tvar < 0.0f)
			{
				zoomOut = false;
				GetComponent<SpriteRenderer>().enabled = false;
			}
			transform.localScale = Vector3.one * tvar;
		}
		else if (drown)
		{
			playerTransform.Translate(Vector3.down * Time.deltaTime * 10.0f, Space.World);
			tvar -= Time.deltaTime * 3.0f;
			playerSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, tvar);
			if (tvar < 0.0f)
				drown = false;
		}
		else if (onBack)
		{
			tvar -= Time.deltaTime;
			GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f - tvar * 4.0f, 1.0f - tvar * 4.0f);
			transform.RotateAround(transform.position, Vector3.forward * ((onBackRand) ? 1.0f : -1.0f), Time.deltaTime * 800.0f);
			transform.Translate(Vector3.up * Time.deltaTime * 12.5f, Space.World);
			if (tvar < 0.0f)
			{
				GetComponent<SpriteRenderer>().color = Color.red;
				GetComponent<SpriteRenderer>().enabled = false;
				onBack = false;
			}
		}
		if ((kind == 5 || kind == 6) && floating)
		{
			tvar += Time.deltaTime * 3.0f;
			if (tvar >= Mathf.PI)
				tvar -= Mathf.PI;
			transform.localPosition = new Vector3(transform.localPosition.x, yPos + Mathf.Sin(tvar) * 3.0f, transform.localPosition.z);
		}
	}
}
