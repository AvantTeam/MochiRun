using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinUpdater : ObstacleUpdater
{
    public int coins = 1;
    public override void OnHit() {
        pcon.coins += coins;
    }
}
