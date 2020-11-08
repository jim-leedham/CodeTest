using UnityEngine;
using System;
using System.IO;

#if (UNITY_EDITOR)
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PaletteRenderer : MonoBehaviour
{
    [SerializeField] public Texture2D LookupTex;
    [SerializeField] public Texture2D PaletteTex;
    [SerializeField] public Material PaletteMat;
    [SerializeField] public Color Color;
    [SerializeField] public int PaletteRow;

    [NonSerialized] public MaterialPropertyBlock MatBlock;
    [NonSerialized] public SpriteRenderer R;

    public int RowCount
    {
        get { return PaletteTex == null? 0 : PaletteTex.height; }
    }

    public void CheckRenderer()
    {
        if (R == null)
        {
            R = GetComponent<SpriteRenderer>();
        }
    }

    public void SetRow(int Value)
    {
        Value = Mathf.Clamp(Value, 0, RowCount-1);
        PaletteRow = Value;
        ApplyProps();
    }

    void OnEnable()
    {
        ApplyProps();
    }

    public void BeginMatBlock()
    {
        if (MatBlock == null)
        {
            MatBlock = new MaterialPropertyBlock();
        }
        R.GetPropertyBlock(MatBlock);
    }

    public void EndMatBlock()
    {
        R.SetPropertyBlock(MatBlock);
    }

    void SetProps(int PalleteRow)
    {
        if (R != null)
        {
            BeginMatBlock();
            R.sharedMaterial = PaletteMat;
            MatBlock.SetColor(ShaderProps.Color, Color);
            MatBlock.SetTexture(ShaderProps.PaletteTex, PaletteTex);
            MatBlock.SetFloat(ShaderProps.PaletteRow, PalleteRow);
            MatBlock.SetTexture(ShaderProps.AlphaTex, LookupTex);
            SetCustomProps();
            EndMatBlock();
        }
    }

    public virtual void SetCustomProps()
    {
    }

    public void ApplyProps()
    {
        CheckRenderer();

        if (LookupTex != null &&
            PaletteMat != null &&
            PaletteTex != null)
        {
            SetProps(PaletteRow);
        }
    }

#if (UNITY_EDITOR)
    void Update()
    {
        if (!Application.isPlaying)
        {
            if (PaletteMat == null)
            {
                PaletteMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/AssetBundles/Materials/MatPalette.mat");
            }

            CheckRenderer();

            ApplyProps();
        }
    }
#endif
}


public static class ShaderProps
{
    public static int PaletteRow;
    public static int AlphaTex;
    public static int PaletteTex;
    public static int Color;

    static ShaderProps()
    {
        Color = Shader.PropertyToID("_Color");
        PaletteRow = Shader.PropertyToID("_PaletteRow");
        AlphaTex = Shader.PropertyToID("_AlphaTex");
        PaletteTex = Shader.PropertyToID("_PaletteTex");
    }
}
