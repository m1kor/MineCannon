using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour
{
    public bool continuous = false;

    private float halfW, halfH, halfSH, screenEdge, mul;
    private ButtonAction act;
    private bool inBounds, acting;
    private SpriteRenderer spriteRenderer;
    private Vector3 initialScale;

	void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        halfW = spriteRenderer.sprite.texture.width / 960.0f;
        halfH = spriteRenderer.sprite.texture.height / 960.0f;
        halfSH = Screen.height * 0.5f;
        act = GetComponent<ButtonAction>();
        inBounds = false;
        initialScale = transform.localScale;
        screenEdge = Screen.width / (Screen.height * 0.5f) * 0.5f;
        acting = false;
        mul = 10.0f;
	}
	
    private bool checkTouch(Vector3 position)
    {
        position = new Vector3((position.x / halfSH - screenEdge) * mul, (position.y / halfSH - 1.0f) * mul, 0.0f);
		if (position.x >= transform.localPosition.x / initialScale.x - halfW * mul * 1.5f && position.x <= transform.localPosition.x / initialScale.x + halfW * mul * 1.5f && position.y >= transform.localPosition.y / initialScale.y - halfH * mul * 1.5f && position.y <= transform.localPosition.y / initialScale.y + halfH * mul * 1.5f)
            return true;
        else
            return false;
    }

	void Update()
    {
        if (spriteRenderer.enabled)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        inBounds = checkTouch(Input.GetTouch(0).position);
                        if (inBounds)
                        {
                            acting = true;
                            transform.localScale = initialScale * 0.8f;
                        }
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        if (inBounds && checkTouch(Input.GetTouch(0).position))
                        {
                            if (continuous)
                                acting = false;
                            else
                                act.Do();
                        }
                        transform.localScale = initialScale;
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Moved && !checkTouch(Input.GetTouch(0).position))
                    {
                        transform.localScale = initialScale;
                        acting = false;
                    }
                    if (continuous && acting)
                        act.Do();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    inBounds = checkTouch(Input.mousePosition);
                    if (inBounds)
                    {
                        acting = true;
                        transform.localScale = initialScale * 0.8f;
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (inBounds && checkTouch(Input.mousePosition))
                    {
                        if (continuous)
                            acting = false;
                        else
                            act.Do();
                    }
                    transform.localScale = initialScale;
                }
                else if (Input.GetMouseButton(0) && !checkTouch(Input.mousePosition))
                {
                    transform.localScale = initialScale;
                    acting = false;
                }
                if (continuous && acting)
                    act.Do();
            }
        }
	}
}

