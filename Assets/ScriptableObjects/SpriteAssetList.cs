using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteAssetList", menuName = "Scriptable Objects/SpriteAssetList", order = 0)]
public class SpriteAssetList : ScriptableObject
{
    public List<TMP_SpriteAsset> SpriteAssets;
}
