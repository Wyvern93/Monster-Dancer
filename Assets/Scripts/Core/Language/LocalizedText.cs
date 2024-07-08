using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    private TextMeshProUGUI textmesh;
    [SerializeField] private string defaultTextId;

    public void OnEnable()
    {
        textmesh = GetComponent<TextMeshProUGUI>();
        SetText(defaultTextId);
    }

    public void SetText(string id)
    {
        textmesh.text = Localization.GetLocalizedString(id);
        textmesh.rectTransform.sizeDelta = new Vector2(textmesh.preferredWidth, textmesh.rectTransform.sizeDelta.y);
    }
}
