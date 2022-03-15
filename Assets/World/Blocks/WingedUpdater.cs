using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingedUpdater : BlockUpdater {
    public Vector3 wingp1, wingp2;

    public float rotateScl = 1f, floatScl = 1f, floatMag = 0.3f;
    public GameObject wingl, wingr;

    public bool attachBlocks = false;

    private float time;
    private Vector3 ipos;

    protected override void Start() {
        base.Start();
        updatesTile = true;
        ipos = transform.position;
    }

    public override void UpdateTile() {
        time += Time.deltaTime;
        transform.position = ipos + new Vector3(0, Mathf.Sin(time / floatScl) * floatMag, 0);
        Vector3 wp = Vector3.Lerp(wingp1, wingp2, Mathf.Sin(time / rotateScl) * 0.5f + 0.5f);
        wingl.transform.rotation = Quaternion.Euler(wp);
        wp.y *= -1;
        wingr.transform.rotation = Quaternion.Euler(wp);
    }
}
