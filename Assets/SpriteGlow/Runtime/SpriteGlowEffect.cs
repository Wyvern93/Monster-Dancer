﻿using UnityEngine;
using UnityEngine.UI;

namespace SpriteGlow
{
    /// <summary>
    /// Adds an HDR outline over the <see cref="SpriteRenderer"/>'s sprite borders.
    /// Can be used in conjuction with bloom post-processing to create a glow effect.
    /// </summary>
    [AddComponentMenu("Effects/Sprite Glow")]
    [ExecuteInEditMode]
    public class SpriteGlowEffect : MonoBehaviour
    {
        public Image Renderer;
        private Material material;
        public Color GlowColor
        {
            get => glowColor; 
            set { if (glowColor != value) { glowColor = value; SetMaterialProperties(); } }
        }
        public float GlowBrightness
        {
            get => glowBrightness; 
            set { if (glowBrightness != value) { glowBrightness = value; SetMaterialProperties(); } }
        }
        public int OutlineWidth
        {
            get => outlineWidth; 
            set { if (outlineWidth != value) { outlineWidth = value; SetMaterialProperties(); } }
        }
        public float AlphaThreshold
        {
            get => alphaThreshold;
            set { if (alphaThreshold != value) { alphaThreshold = value; SetMaterialProperties(); } }
        }
        public bool DrawOutside
        {
            get => drawOutside;
            set { if (drawOutside != value) { drawOutside = value; SetMaterialProperties(); } }
        }
        public bool EnableInstancing
        {
            get => enableInstancing;
            set { if (enableInstancing != value) { enableInstancing = value; SetMaterialProperties(); } }
        }

        [Tooltip("Base color of the glow.")]
        [SerializeField] private Color glowColor = Color.white;
        [Tooltip("The brightness (power) of the glow."), Range(1, 10)]
        [SerializeField] private float glowBrightness = 2f;
        [Tooltip("Width of the outline, in texels."), Range(0, 10)]
        [SerializeField] private int outlineWidth = 1;
        [Tooltip("Threshold to determine sprite borders."), Range(0f, 1f)]
        [SerializeField] private float alphaThreshold = .01f;
        [Tooltip("Whether the outline should only be drawn outside of the sprite borders. Make sure sprite texture has sufficient transparent space for the required outline width.")]
        [SerializeField] private bool drawOutside = false;
        [Tooltip("Whether to enable GPU instancing.")]
        [SerializeField] private bool enableInstancing = false;

        private static readonly int isOutlineEnabledId = Shader.PropertyToID("_IsOutlineEnabled");
        private static readonly int outlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int outlineSizeId = Shader.PropertyToID("_OutlineSize");
        private static readonly int alphaThresholdId = Shader.PropertyToID("_AlphaThreshold");

        private void Awake ()
        {
            material = Instantiate(Renderer.material);
            Renderer.material = material;
        }

        private void OnEnable ()
        {
            SetMaterialProperties();
        }

        private void OnDisable ()
        {
            SetMaterialProperties();
        }

        private void OnValidate ()
        {
            if (!isActiveAndEnabled) return;

            // Update material properties when changing serialized fields with editor GUI.
            SetMaterialProperties();
        }

        private void OnDidApplyAnimationProperties ()
        {
            // Update material properties when changing serialized fields with Unity animation.
            SetMaterialProperties();
        }

        private void SetMaterialProperties ()
        {
            if (!Renderer) return;

            material.SetFloat(isOutlineEnabledId, isActiveAndEnabled ? 1 : 0);
            material.SetColor(outlineColorId, GlowColor * GlowBrightness);
            material.SetFloat(outlineSizeId, OutlineWidth);
            material.SetFloat(alphaThresholdId, AlphaThreshold);
        }
    }
}
