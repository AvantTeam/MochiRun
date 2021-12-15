using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantsLoader : MonoBehaviour
{
    public Mesh[] variants;
    void Start()
    {
        GetComponent<MeshFilter>().mesh = variants[Random.Range(0, variants.Length)];
    }
}
