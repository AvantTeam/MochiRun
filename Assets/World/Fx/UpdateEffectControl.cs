using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateEffectControl : MonoBehaviour
{
    public GameObject mochiUpdateFx, floatFx;
    private GameObject current, currentOriginal;

    public GameObject[] flagFx;

    GameObject player;
    PlayerControl pcon;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        pcon = player.GetComponent<PlayerControl>();
    }

    void Update() {
        if(pcon.updateFxFlag > 0 && pcon.updateFxFlag <= flagFx.Length) {
            //flag starts from 1, but array starts from 0!
            setFx(flagFx[pcon.updateFxFlag - 1]);
        }
        else if(pcon.state == PlayerControl.STATE.RUN && pcon.landed) {
            //ground particle
            setFx(mochiUpdateFx);
        }
        else if(pcon.state == PlayerControl.STATE.FLOAT && pcon.gliding && pcon.courage > 0f) {
            setFx(floatFx);
        }
        else {
            if(current != null) {
                current.GetComponent<ParticleSystem>().Stop();
                current = currentOriginal = null;
            }
        }
    }

    private void setFx(GameObject newFx) {
        if(currentOriginal == newFx) return;
        if(current != null) {
            current.GetComponent<ParticleSystem>().Stop();
        }
        currentOriginal = newFx;
        current = Instantiate(newFx, player.transform.position, player.transform.rotation);
    }
}
