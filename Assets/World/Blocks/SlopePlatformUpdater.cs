using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopePlatformUpdater : BlockUpdater {
    public byte ignoreStart = 2;
    protected override void Start() {
        base.Start();
        if(ctype >= ignoreStart && ctype < ignoreStart + 2) {
            //a downwards slope platform is essentially a no-op
            GetComponent<EdgeCollider2D>().enabled = false;
            GetComponent<PlatformEffector2D>().enabled = false;
        }
    }
}
