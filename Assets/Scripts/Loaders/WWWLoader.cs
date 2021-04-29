using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WWWLoader : MonoBehaviour
{
    public void LoadImage(string url, RawImage image)
    {
        StartCoroutine(Load(url, image));
    }

    private IEnumerator Load(string url, RawImage image)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            image.texture = www.texture;
        }
    }
}
