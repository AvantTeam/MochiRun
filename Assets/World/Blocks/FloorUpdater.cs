using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorUpdater : BlockUpdater {
    public Mesh[] variants;
    public float zRand = 0f;
    public float decorateChance = -1;
    public GameObject[] decorations;

    protected override void Start() {
        base.Start();
        GetComponent<MeshFilter>().mesh = variants[Random.Range(0, variants.Length)];
        transform.position = new Vector3(transform.position.x, transform.position.y, Random.Range(-zRand, zRand));

        if(decorateChance > 0f && Random.Range(0f, 1f) < decorateChance) {
            GameObject d = Instantiate(decorations[Random.Range(0, decorations.Length)]);
            d.transform.SetParent(transform, false);
        }
    }
}
