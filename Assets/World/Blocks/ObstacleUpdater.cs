using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this object has a Trigger Collider
public class ObstacleUpdater : BlockUpdater
{
    public float damage = 15f;
    public bool shieldable = false;
    public float shieldMultiplier = 1f;

    public bool destroyOnHit = false;
    public Vector3 angleOffset;
    public float rotateSpeed = 0f;

    public GameObject hitFx = null;

    protected override void Start() {
        base.Start();
        Vector3 rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(angleOffset + rot);
        updatesTile = rotateSpeed > 0.1f || rotateSpeed < -0.1f;
    }

    public override void UpdateTile() {
        Vector3 rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rot.x, rot.y + rotateSpeed * Time.deltaTime, rot.z);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            if(hitFx != null) pcon.Fx(hitFx, transform.position, Quaternion.identity);

            if(shieldable && pcon.shield != null && (pcon.shield.shieldSnap > 0f || pcon.shield.shielded)) {
                if(pcon.shield.shieldSnap > 0f) {
                    pcon.shield.shieldSnap = 0.0001f; //only allow snaps in the same frame
                    pcon.SnapShield();
                    OnSnap();
                }
                else {
                    pcon.shield.health = Mathf.Max(0f, pcon.shield.health - damage * shieldMultiplier);
                    Destroy(gameObject);
                }
            }
            else {
                pcon.Damage(damage, gameObject);
                OnHit();
                if(destroyOnHit) Destroy(gameObject);
            }
        }
    }

    public virtual void OnHit() {

    }

    public virtual void OnSnap() {
        Destroy(gameObject);
    }
}
