using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewItem", menuName = "G.H.S/ItemAsset", order = 0)]
public class ItemAsset : ScriptableObject
{
    [TextArea]
    public string Desc = "無し";
    public Sprite Icon;
}