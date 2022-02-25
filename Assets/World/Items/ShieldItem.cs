using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerControl;

public class ShieldItem : Item {
    public const float PASSIVE_LOSS = 5f, DEF_MAX_HEALTH = 85f;
    public const float SHIELD_COOLDOWN = 0.4f; //cooldown until you can use a shield; stops spamming the shift button

    public float maxHealth = 85f, healthRegen = 0f;
    public float health;

    public float shieldTime = 0f;
    public float shieldSnap = 0f;
    public float shieldCooldown = 0f;

    public bool shielded = false; //real shielding tracker

    public override void Reset() {
        maxHealth = DEF_MAX_HEALTH;//todo enhanced shield accessory
        healthRegen = 0f;

        health = maxHealth; 
        shieldTime = 0;

        shieldTime = shieldSnap = shieldCooldown = 0f;
        shielded = false;
    }

    public override void UpdateAlways(PlayerControl pcon) {
        if(shieldCooldown > 0f) shieldCooldown -= Time.deltaTime;
        if(shieldSnap > 0f) shieldSnap -= Time.deltaTime;

        if(!shielded) health += healthRegen * Time.deltaTime;
        if(health > maxHealth) health = maxHealth;
        if(pcon.state != STATE.RUN) {
            shielded = false;
        }
    }

    public override bool Update(PlayerControl pcon) {
        if(pcon.state != STATE.RUN){
            return false;
        }

        //update
        if(pcon.landed && health > 0f && shieldCooldown <= 0f && KeyBinds.Shield()) {
            if(!shielded) {
                shieldSnap = 0.2f;
                shielded = true;
                shieldTime = 0f;
            }

            shieldTime += Time.deltaTime;
            if(shieldTime > 0.5f) health -= Time.deltaTime * PASSIVE_LOSS;
        }
        else{
            if(shielded) {
                shieldCooldown = SHIELD_COOLDOWN;
                shielded = false;
            }
        }
        return pcon.landed;
    }

    public override void BuildTable(GameObject table) {
        ShieldDisplay o = Object.Instantiate(Vars.main.prefabs.shieldDisplay).GetComponent<ShieldDisplay>();
        o.shield = this;
        o.transform.SetParent(table.transform, false);
    }

    public override TYPE Type() {
        return TYPE.SHIELD;
    }
}
