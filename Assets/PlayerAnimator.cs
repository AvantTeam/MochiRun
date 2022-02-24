using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerAnimator : MonoBehaviour {
    private const string landed = "onGround", running = "isRunning", runSpeed = "runSpeed", deadTrigger = "dead", isStunned = "stunned", isShielded = "shielded";
    private const float BLINK_DIST = 3f, GLOW_TIME = 0.7f;
    public const float SHIELD_RAD = 2.5f;
    private WaitForSeconds WaitBlink = new WaitForSeconds(0.15f), WaitDamage = new WaitForSeconds(0.6f);

    [Serializable]
    public struct FaceFrame {
        public Texture2D face;
        public float duration;
    }
    public Material faceMaterial;
    public Texture2D idleFace, blinkFace, stunFace;
    public Texture2D[] jumpFace, damageFace;
    public FaceFrame[] deathFaceAnimation;

    public GameObject shield;
    public ShieldEffectUpdater shieldEffect;
    public bool stunEnded = true;
    private bool animatingFace = false, jumping = false, stunned = false;
    private float blinkTimer = BLINK_DIST, eyeGlow = 0f, shieldHeat = 0f;
    private string[] resetTriggers = {deadTrigger};

    GameObject player;
    PlayerControl pcon;
    Animator animator;
    Rigidbody2D rigid;

    LookAtConstraint shieldLooker;
    Material shieldMat;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        rigid = player.GetComponent<Rigidbody2D>();
        pcon = player.GetComponent<PlayerControl>();
        animator = GetComponent<Animator>();
        pcon.animator = this;

        shieldLooker = shield.GetComponent<LookAtConstraint>();
        shieldMat = shield.GetComponent<MeshRenderer>().material;
        reset();
    }

    void Update() {
        animator.SetBool(landed, pcon.landed);
        animator.SetBool(running, pcon.state == PlayerControl.STATE.RUN);
        animator.SetBool(isShielded, pcon.shielded);
        UpdateShield();

        bool stun = pcon.state == PlayerControl.STATE.STUNNED;
        bool stunFallen = stun && !(pcon.stateTime > PlayerControl.STUN_TIME && Mathf.Abs(rigid.velocity.x) < 1f && pcon.landed);
        animator.SetBool(isStunned, stunFallen);
        animator.SetFloat(runSpeed, Mathf.Abs(rigid.velocity.x));
        stunEnded = !stun || animator.GetCurrentAnimatorStateInfo(0).IsTag("Run");

        if(eyeGlow > 0 && animatingFace) {
            setGlow(Mathf.Clamp01(2 * eyeGlow / GLOW_TIME));
            eyeGlow -= Time.deltaTime;
            if(eyeGlow <= 0f) {
                setGlow(0f);
                animatingFace = false;
            }
        }
        else if(faceMaterial.color.a > 0f) {
            setGlow(0f);
        }

        if(!animatingFace && eyeGlow <= 0) {
            if(stunned) {
                if(!stunFallen) {
                    setFaceQuiet(idleFace);
                    stunned = false;
                }
                return;
            }
            else if(stunFallen) {
                setFaceQuiet(stunFace);
                stunned = true;
                return;
            }

            if(jumping) {
                if(pcon.landed || rigid.velocity.y < 0.1f) {
                    setFaceQuiet(idleFace);
                    jumping = false;
                }
            }
            else {
                if(!pcon.landed && rigid.velocity.y >= 0.1f) {
                    setRandomFace(jumpFace);
                    jumping = true;
                    return;
                }
                if(blinkTimer < 0) {
                    //blinking
                    blinkTimer += Time.deltaTime;
                    if(blinkTimer >= 0) {
                        setFaceQuiet(idleFace);
                        blinkTimer = BLINK_DIST + UnityEngine.Random.Range(-0.3f, 0.3f);
                    }
                }
                else if(pcon.landed){
                    //not blinking
                    blinkTimer -= Time.deltaTime;
                    if(blinkTimer <= 0) {
                        setFaceQuiet(blinkFace);
                        blinkTimer = -0.15f;
                    }
                }
            }
        }
        //don't add code here btw
    }

    private void UpdateShield() {
        shieldHeat = approachDelta(shieldHeat, pcon.shielded ? 1 : 0, pcon.shielded ? 4 : 4.6f);

        if(shieldHeat > 0.01f) {
            if(!shield.activeInHierarchy) shield.SetActive(true);
            shieldLooker.roll = Time.unscaledTime * 110f;

            float r = SHIELD_RAD * shieldHeat;
            shield.transform.localScale = new Vector3(r, r, r);
            shieldMat.SetFloat("_Alpha", shieldHeat);
        }
        else if(shield.activeInHierarchy) shield.SetActive(false);
    }

    public void Kill(bool deathEffect) {
        if(!deathEffect) { 
            animator.SetTrigger(deadTrigger);
            StopAllCoroutines();
            StartCoroutine(DyingFacial());
        }
    }

    public void Damage() {
        //this animation is important
        StopAllCoroutines();
        StartCoroutine(DamageFacial());
    }

    public void SnapShield() {
        shieldHeat = 1;
        shieldEffect.Play(pcon);

        if(animatingFace) return;
        eyeGlow = GLOW_TIME;
        animatingFace = true;
        setFaceQuiet(idleFace);
    }

    public void CourageBurst() {
        if(animatingFace) return;
        eyeGlow = GLOW_TIME;
        animatingFace = true;
        setFaceQuiet(idleFace);
    }

    public void reset() {
        foreach(string t in resetTriggers) {
            animator.ResetTrigger(t);
        }
        StopAllCoroutines();
        setFace();
        eyeGlow = 0f;
        setGlow(0f);
        shield.SetActive(false);
        shieldHeat = 0f;
    }

    private void setGlow(float a) {
        Color c = faceMaterial.color;
        c.a = a;
        faceMaterial.color = c;
    }

    private void setFace(Texture2D f) {
        if(f != idleFace){
            animatingFace = true;
            eyeGlow = 0f;
            setGlow(0f);
        }
        faceMaterial.mainTexture = f;
    }

    private void setFaceQuiet(Texture2D f) {
        faceMaterial.mainTexture = f;
    }

    private void setFace() {
        animatingFace = false;
        faceMaterial.mainTexture = idleFace;
    }

    //this is quiet!
    private void setRandomFace(Texture2D[] f) {
        faceMaterial.mainTexture = f[UnityEngine.Random.Range(0, f.Length)];
    }

    IEnumerator DamageFacial() {
        setFace(blinkFace);
        yield return WaitBlink;
        setRandomFace(damageFace);
        yield return WaitDamage;
        setFace(blinkFace);
        yield return WaitBlink;
        setFace();
    }

    IEnumerator DyingFacial() {
        foreach(FaceFrame f in deathFaceAnimation) {
            setFace(f.face);
            yield return new WaitForSeconds(f.duration);
        }
    }

    float lerpDelta(float fromValue, float toValue, float progress) {
        return fromValue + (toValue - fromValue) * Mathf.Clamp01(progress * Time.deltaTime * 60f);
    }

    float approachDelta(float from, float to, float speed) {
        return approach(from, to, speed * Time.deltaTime);
    }

    float approach(float from, float to, float speed) {
        return from + Mathf.Clamp(to - from, -speed, speed);
    }
}
