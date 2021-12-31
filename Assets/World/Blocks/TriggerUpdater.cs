using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//updator for tag triggers.
public class TriggerUpdater : BlockUpdater
{
    public enum TagRange {
        One,
        Three,
        Five,
        Seven,
        Nine,
        CrossX,
        CrossY,
        Infinite
    }

    public TagRange range;
    public int data;
    public Trigger trigger;

    protected override void Start() {
        base.Start();
        trigger = type as Trigger;
        range = CtypeRange(ctype);
        data = ctype >> 3;
        updatesTile = true;
    }

    public override void UpdateTile() {
        if(CanTrigger()) {
            Triggered();
            Destroy(gameObject);
        }
    }

    public virtual void Triggered() {

    }

    public static TagRange CtypeRange(byte ctype) {
        return (TagRange)(ctype & 0b111);
    }

    public bool CanTrigger() {
        if((trigger.defaultYCross && range != TagRange.Infinite) && !YCross()) return false;
        switch(range) {
            case TagRange.Infinite: return true;
            case TagRange.CrossX: return pcon.gameObject.transform.position.x >= transform.position.x;
            case TagRange.CrossY: return YCross();
            default: return InSquare(pcon.gameObject.transform.position, transform.position, ((int)range) * 2 + 1);
        }
    }

    public bool InSquare(Vector3 a, Vector3 b, int r) {
        float range = r / 2f;
        return Mathf.Abs(a.x - b.x) <= range && Mathf.Abs(a.y - b.y) <= range;
    }

    public bool YCross() {
        return pcon.gameObject.transform.position.y >= transform.position.y;
    }
}
