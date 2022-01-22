using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourageOrbUpdater : BlockUpdater
{
    public Vector2 velocity = new Vector2(0f, 5f);
    public bool overrideVelocityX = false;
    public bool overrideVelocityY = true;
    public bool destroyOnHit = false;

    public float rotateSpeed = 0f;
    public GameObject hitFx = null;

    private float rv = 0f;
    private bool hit = false;

    protected override void Start() {
        base.Start();
        updatesTile = rotateSpeed > 0.1f || rotateSpeed < -0.1f;
        rv = 0f;
        hit = false;
    }

    public override void UpdateTile() {
        rv *= 0.9f;
        Vector3 rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rot.x, rot.y + (rotateSpeed + rv) * Time.deltaTime, rot.z);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if(hit) return;
        if(collision.gameObject.CompareTag("Player") && pcon.gliding) {
            hit = true;

            if(overrideVelocityX) pcon.SetVelocityX(velocity.x);
            else pcon.Impulse(velocity.x, 0f);
            if(overrideVelocityY) pcon.SetVelocityY(velocity.y);
            else pcon.Impulse(0f, velocity.y);

            if(hitFx != null) pcon.Fx(hitFx, transform.position, Quaternion.identity);
            rv = rotateSpeed * 15f;
            if(destroyOnHit) Destroy(gameObject);
        }
    }
}
