using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//creates Blocks from prefabs.
public class Blocks
{
    public static Block floorBlock, pitStart, pitEnd, spike, potion, jumpOrb, balloon, balloonLow, balloonHigh;

    public void Load() {
        floorBlock = new Floor(findPath("Assets/World/FloorPrefab.prefab"));
        pitStart = new PitStarter();
        pitEnd = new PitEnder();
        spike = new Block(find("Spike")) {
            onFloor = true, rotate = true
        };
        potion = new Block(find("Potion"));
        jumpOrb = new Balloon(find("JumpOrb")) {
            zLayer = 0.1f
        };
        balloon = new Balloon(find("JumpBalloon"));
        balloonLow = new Balloon(find("JumpBalloonLow"));
        balloonHigh = new Balloon(find("JumpBalloonHigh"));
    }

    private GameObject find(string prefab) {
        return (GameObject)AssetDatabase.LoadAssetAtPath("Assets/World/Blocks/" + prefab + ".prefab", typeof(GameObject));
    }

    private GameObject findPath(string prefab) {
        return (GameObject)AssetDatabase.LoadAssetAtPath(prefab, typeof(GameObject));
    }
}
