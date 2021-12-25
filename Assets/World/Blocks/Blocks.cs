using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//temporary utility class holding widely used Blocks. Kinda useless now.
public class Blocks
{
    public static Block floorBlock, pitStart, pitEnd, spike, potion, jumpOrb, balloon, balloonLow, balloonHigh;

    public void Load() {
        floorBlock = find("Floor");
        pitStart = find("PitStarter");
        pitEnd = find("PitEnder");
        spike = find("Spike");
        potion = find("Potion");
        jumpOrb = find("JumpOrb");
        balloon = find("JumpBalloon");
        balloonLow = find("JumpBalloonLow");
        balloonHigh = find("JumpBalloonHigh");
    }

    private Block find(string name) {
        return Vars.main.content.block(name);
    }

    /*
    private GameObject find(string prefab) {
        foreach(GameObject o in BlockPrefabCollector.list) {
            if(o.name.Equals(prefab)) return o;
        }
        return null;
    }

    private GameObject findPath(string prefab) {
        return (GameObject)AssetDatabase.LoadAssetAtPath(prefab, typeof(GameObject));
    }

    private int findIndex(GameObject prefab) {
        for(int i = 0; i < BlockPrefabCollector.list.Length; i++) {
            if(BlockPrefabCollector.list[i] == prefab) return i;
        }
        return -1;
    }*/
}
