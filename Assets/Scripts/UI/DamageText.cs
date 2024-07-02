using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    private float velocity, hvelocity;
    private Color Color = Color.white;
    public TextMeshProUGUI text;
    private float alpha = 4.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        alpha = 4.0f;
        Color = Color.white;
        velocity = 100f;
        hvelocity = Random.Range(-20f, 20f);
    }

    // Update is called once per frame
    void Update()
    {
        alpha -= Time.deltaTime * 3f;
        velocity -= Time.deltaTime * 150f;
        rectTransform.anchoredPosition += new Vector2(Time.deltaTime * hvelocity, velocity * Time.deltaTime);
        Color = new Color(1, 1, 1, alpha);
        if (Color.a <= 0.01f)
        {
            PoolManager.Return(gameObject, GetType());
        }
    }
}
