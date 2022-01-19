using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this object has a Trigger Collider
public class ObstacleUpdater : BlockUpdater
{
    public float damage = 15f;
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

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            pcon.Damage(damage, gameObject);
            OnHit();
            if(hitFx != null) pcon.Fx(hitFx, transform.position, Quaternion.identity);
            if(destroyOnHit) Destroy(gameObject);
        }
    }

    public virtual void OnHit() {

    }
}
