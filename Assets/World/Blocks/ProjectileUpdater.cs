using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileUpdater : ObstacleUpdater {
    public Vector3 speed = new Vector3(-1f, 0f, 0f);
    public bool reflectable = true;

    public GameObject despawnFx;
    public float lifetime = 1f;

    private Vector3 realSpeed;
    private float time = 0;
    private bool reflected = false;

    protected override void Start() {
        base.Start();
        updatesTile = true;
        reflected = false;
        realSpeed = speed;

        float t = (transform.position.x - pcon.transform.position.x) / PlayerControl.SPEED_MAX;
        Vector3 newp = speed * -1f * t;
        transform.position = newp + transform.position;
    }

    public override void UpdateTile() {
        base.UpdateTile();
        transform.position = realSpeed * Time.deltaTime + transform.position;
        if(reflected) {
            time += Time.deltaTime;
            if(time > lifetime) {
                if(despawnFx != null) pcon.Fx(despawnFx, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    public override void OnSnap() {
        if(!reflectable) {
            base.OnSnap();
            return;
        }
        if(reflected) return;
        time = 0f;
        reflected = true;
        realSpeed = -speed + new Vector3(PlayerControl.SPEED_MAX, Random.Range(-0.1f, 0.3f), Random.Range(-0.2f, 0.2f));
        GetComponent<Collider2D>().enabled = false;
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
