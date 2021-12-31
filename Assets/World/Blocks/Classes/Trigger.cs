using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TriggerUpdater;

[CreateAssetMenu(fileName = "TriggerNew", menuName = "Blocks/Trigger", order = 9)]
public class Trigger : Block {
    public int maxData = 0;
    public int defaultData = 0;
    public string modifierName = "Modifier";
    public float modifierOffset = 0;
    public float modifierMultiplier = 1;

    public TagRange defaultRange = TagRange.Five;
    public bool defaultYCross = false; //whether to impose TagRange.CrossY in addition to the current range.
    public Color uiColor = Color.white;

    public override void init(float x, float y, byte ctype) {
        if(prefab != null) {
            GameObject newo = Instantiate(prefab, new Vector3(x, y, zLayer), Quaternion.identity);
            if(hasUpdate) {
                BlockUpdater bu = newo.GetComponent<BlockUpdater>();
                bu.type = this;
                bu.ctype = ctype;
            }
        }
    }

    public byte defaultCType() {
        return (byte)((int)defaultRange | (defaultData << 3));
    }

    public float dataToModifier(int data) {
        return data * modifierMultiplier + modifierOffset;
    }
}
