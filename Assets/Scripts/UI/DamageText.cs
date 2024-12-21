using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageText : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    private float velocity, hvelocity;
    public Color color;
    public TextMeshProUGUI text;
    private float alpha = 2.0f;
    float sizeAngle = 0;

    private Color baseColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        alpha = 2.0f;
        velocity = Random.Range(200f, 400f);
        hvelocity = Random.Range(-50f, 50f);
        transform.localScale = Vector3.one;
        sizeAngle = 0;
    }

    // Update is called once per frame
    void Update()
    {
        sizeAngle = Mathf.MoveTowards(sizeAngle, 180, Time.unscaledDeltaTime * 360f);
        alpha -= Time.unscaledDeltaTime * 4f;
        //velocity -= Time.unscaledDeltaTime * 1000f;
        hvelocity = Mathf.Lerp(hvelocity, 0, Time.unscaledDeltaTime * 10f);
        velocity = Mathf.Lerp(velocity, 0, Time.unscaledDeltaTime * 10f);
        velocity = Mathf.Clamp(velocity, 0, 200);
        rectTransform.anchoredPosition += new Vector2(Time.deltaTime * hvelocity, velocity * Time.unscaledDeltaTime);
        baseColor = new Color(color.r, color.g, color.b, alpha);
        text.color = baseColor;
        transform.localScale = Vector3.one * Mathf.Clamp((1 + Mathf.Cos(sizeAngle * Mathf.Deg2Rad)), 1f, 1.5f);
        //transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.unscaledDeltaTime * 10);
        if (text.color.a <= 0.01f)
        {
            PoolManager.Return(gameObject, GetType());
        }
    }
}
