using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class CustomTile : TileBase
{
    public string label;
    public AssetBundle assetBundle;
    public Color color;
}