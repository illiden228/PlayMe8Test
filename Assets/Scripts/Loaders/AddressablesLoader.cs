using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AddressablesLoader : MonoBehaviour
{
    public void LoadSprite(AssetReference reference, Image image)
    {
        if (reference.OperationHandle.IsValid())
        {
            if (reference.OperationHandle.IsDone)
                image.sprite = reference.OperationHandle.Result as Sprite;
            else
                reference.OperationHandle.Completed += obj => image.sprite = obj.Result as Sprite;
        }
        else
        {
            reference.LoadAssetAsync<Sprite>().Completed += obj => image.sprite = obj.Result;
        }
    }
}
