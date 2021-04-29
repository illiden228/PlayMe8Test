using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class Item
{
    public AssetReference Icon;
    public Money MoneyType;
    public int Cost;
    public string Name;
}
