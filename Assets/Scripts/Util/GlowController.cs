using UnityEngine;

[ExecuteAlways]
public class GlowController : MonoBehaviour
{
    [SerializeField] SpriteRenderer spr;
    private Material spr_mat;

    [ColorUsage(showAlpha:true, hdr:true)] public Color glowColor;
    public void Update()
    {
        if (spr == null) return;
        if (spr_mat == null) spr_mat = spr.sharedMaterial;

        spr_mat.SetColor("_GlowColor", glowColor);
    }
}