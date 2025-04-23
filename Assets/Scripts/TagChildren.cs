using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagChildren : MonoBehaviour
{
    public string tagToApply = "YourTag"; // Replace with your desired tag

    void Start()
    {
        ApplyTagRecursively(transform, tagToApply);
    }

    void ApplyTagRecursively(Transform parent, string tag)
    {
        parent.tag = tag;

        foreach (Transform child in parent)
        {
            ApplyTagRecursively(child, tag);
        }
    }
}

