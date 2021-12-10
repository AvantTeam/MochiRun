using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateEffectControl : MonoBehaviour
{
    public GameObject mochiUpdateFx, floatFx;
    private GameObject current, currentOriginal;

    GameObject player;
    PlayerControl pcon;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        pcon = player.GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update() {
        if(pcon.state == PlayerControl.STATE.RUN && pcon.landed) {
            //ground particle
            setFx(mochiUpdateFx);
        }
        else if(pcon.state == PlayerControl.STATE.FLOAT && pcon.courageTime > 0f && pcon.courage > 0f) {
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
