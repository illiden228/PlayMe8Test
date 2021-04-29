using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewMoney", order = 55, menuName = "Moneys/NewMoney")]
public class Money : ScriptableObject
{
    public AssetReference Icon;
}
