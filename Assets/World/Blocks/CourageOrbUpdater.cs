using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourageOrbUpdater : BlockUpdater
{
    public Vector2 velocity = new Vector2(0f, 5f);
    public bool overrideVelocityX = false;
    public bool overrideVelocityY = true;
    public bool destroyOnHit = false;
    public GameObject hitFx = null;

    override public void Couraged(PlayerControl pcon) {
        if(overrideVelocityX) pcon.SetVelocityX(velocity.x);
        else pcon.Impulse(velocity.x, 0f);
        if(overrideVelocityY) pcon.SetVelocityY(velocity.y);
        else pcon.Impulse(0f, velocity.y);


        if(hitFx != null) pcon.Fx(hitFx, transform.position, Quaternion.identity);
        if(destroyOnHit) Destroy(gameObject);
    }
}
