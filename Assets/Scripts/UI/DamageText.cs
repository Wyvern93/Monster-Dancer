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

    private Color baseColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        alpha = 2.0f;
        velocity = 100f;
        hvelocity = Random.Range(-20f, 20f);
    }

    // Update is called once per frame
    void Update()
    {
        alpha -= Time.unscaledDeltaTime * 3f;
        velocity -= Time.unscaledDeltaTime * 250f;
        rectTransform.anchoredPosition += new Vector2(Time.deltaTime * hvelocity, velocity * Time.unscaledDeltaTime);
        baseColor = new Color(color.r, color.g, color.b, alpha);
        text.color = baseColor;
        if (text.color.a <= 0.01f)
        {
            PoolManager.Return(gameObject, GetType());
        }
    }
}
