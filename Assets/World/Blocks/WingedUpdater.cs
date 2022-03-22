using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingedUpdater : BlockUpdater {
    public Vector3 wingp1, wingp2;

    public float rotateScl = 1f, floatScl = 1f, floatMag = 0.3f;
    public GameObject wingl, wingr;

    public bool attachBlocks = false;
    public Material[] faceMaterials;
    public int faceID = 1;

    private float time;
    private Vector3 ipos;
    private static List<Collider2D> clist = new List<Collider2D>();
    private static ContactFilter2D contactFilter = new ContactFilter2D();

    protected override void Start() {
        base.Start();
        updatesTile = true;
        ipos = transform.position;

        if (attachBlocks) AttachStart();
        if (faceMaterials.Length > 0) {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            Material[] mrs = mr.sharedMaterials;
            mrs[faceID] = faceMaterials[Random.Range(0, faceMaterials.Length)];
            mr.sharedMaterials = mrs;
        }

        time = -(ipos.x - pcon.transform.position.x) / PlayerControl.SPEED_MAX;
    }

    public override void UpdateTile() {
        time += Time.deltaTime;
        transform.position = ipos + new Vector3(0, Mathf.Sin(time / floatScl + 1.57f) * floatMag, 0);
        Vector3 wp = Vector3.Lerp(wingp1, wingp2, Mathf.Sin(time / rotateScl) * 0.5f + 0.5f);
        wingl.transform.rotation = Quaternion.Euler(wp);
        wp.y *= -1;
        wingr.transform.rotation = Quaternion.Euler(wp);
    }

    public void AttachStart() {
        contactFilter.useTriggers = true;
        int n = Physics2D.OverlapBox(transform.position, new Vector2(0.1f, 1.9f), 0f, contactFilter, clist);

        for (int i = 0; i < n; i++) {
            if (clist[i].gameObject != gameObject && Mathf.Abs(clist[i].transform.position.x - transform.position.x) < 0.2f && !clist[i].gameObject.CompareTag("Floor")) {
                Block b = clist[i].gameObject.GetComponent<BlockUpdater>().type;
                if (b != null && b.category != Vars.main.category.marker && b.category != Vars.main.category.terrain) Attach(clist[i].gameObject);
            }
        }
    }

    public void Attach(GameObject block) {
        block.transform.SetParent(transform, true);
    }
}
