using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonUpdater : BlockUpdater 
{
    public Vector2 velocity = new Vector2(0f, 5f);
    public bool overrideVelocityY = true;
    public bool destroyOnHit = true;
    public GameObject hitFx = null;

    public float floatRadius = 0.1f;
    public float floatScl = 1f;


    protected override void Start()
    {
        base.Start();
        updatesTile = true;
        Debug.Log("ctype:" + ctype);
        /*if(ctypeMaterials != null && ctype < ctypeMaterials.Length) {
            Material mat = ctypeMaterials[ctype];
            if(mat != null) gameObject.GetComponent<MeshRenderer>().materials[targetMaterial] = mat;
        }*/
    }

    public override void UpdateTile()
    {
        //float pitch = gameObject.transform.rotation.eulerAngles.y;
        //float yaw = gameObject.transform.position.x + Time.time * spinSpeed;
        //Vector3 offset = new Vector3(Mathf.Cos(yaw) * Mathf.Cos(pitch) * floatRadius, Mathf.Sin(yaw) * Mathf.Cos(pitch) * floatRadius, Mathf.Sin(pitch) * floatRadius);
        //gameObject.GetComponent<MeshRenderer>().transform.position = gameObject.transform.position + offset;

        Vector3 pos = gameObject.transform.position;
        float offset = Mathf.Sin(Time.time / floatScl + pos.x) * floatRadius;
        
        pos.z = type.zLayer + offset;
        gameObject.transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            if(overrideVelocityY){
                pcon.SetVelocityY(velocity.y);
                pcon.Impulse(velocity.x, 0f);
            }
            else pcon.Impulse(velocity.x, velocity.y);

            if(hitFx != null) pcon.Fx(hitFx, transform.position, Quaternion.identity);
            if(destroyOnHit) Destroy(gameObject);
        }
    }
}
