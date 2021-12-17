using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public const float PLAYER_RADIUS = 0.5f;

    public static float JUMP_MAX = 0f; //y velocity of the jump. To set it using blocks instead, use setJumpHeight().
    public static float SPEED_MAX = 4f;
    public static float INVIN_TIME = 0.75f; //invincibility seconds after taking any damage
    public static float MAX_HP = 100f;
    public static float HP_LOSS = 4f; //per second
    public static int COURAGES = 2;
    public static float COURAGE_SLOT = 1f; //max courage = SLOT * COURAGES
    public static float FLOAT_YVEL = -0.7f;
    private const float JUMP_RELEASE_REDUCE = 0.75f;
    private const float JUMP_GRACE = 0.3f; //the motive to "press jump" lasts for this long; i.e. press jump up to X seconds before touching the ground to immediately jump after
    private const float ACCEL = 10f;

    public enum STATE {
        NONE = -1,
        RUN = 0,
        JUMP,
        FLOAT,
        MISS,
        STOP,
        CUTSCENE,
        NUM
    };

    public STATE state = STATE.NONE;
    public STATE nextState = STATE.NONE;
    public float stateTime = 0f; //time after a state change
    public float health = 0f;
    public float courage = 0f;
    public float courageTime = 0f;

    public bool landed = false;
    public bool usedCourage = false;
    private bool jumpReleased = false;
    private float jumpPressTimer = JUMP_GRACE + 0.1f;
    private float invincibility = 0f;
    private Collider2D[] colresult = new Collider2D[25]; //attempting to get all overlapping colliders will get 20 max
    private ContactFilter2D triggerContactFilter;
    private Vector2 externalForce = Vector2.zero, overrideForce = Vector2.zero;
    private bool overrideForceX, overrideForceY;

    Rigidbody2D rigid;
    Collider2D collider2d;
    CameraController cameraControl;
    public PlayerAnimator animator;
    public GameObject costume, burstFx, courageStartFx, deathFx, damageFx, courageFailFx;
    // Start is called before the first frame update
    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
        cameraControl = GameObject.Find("Main Camera").GetComponent<CameraController>();
        triggerContactFilter = new ContactFilter2D();
        triggerContactFilter.useTriggers = true;

        reset();
    }

    // Update is called once per frame
    void Update() {
        Vector2 vel = rigid.velocity;
        bool inputJump = input_Space();
        stateTime += Time.deltaTime;
        checkLanded();
        checkDeath();

        if(input_SpaceDown()) {
            jumpPressTimer = 0f;
        }
        else if(jumpPressTimer < JUMP_GRACE) {
            jumpPressTimer += Time.deltaTime;
        }
        if(invincibility > 0f) invincibility -= Time.deltaTime;

        //AUTO STATE CHANGE
        if(nextState == STATE.NONE) {
            switch(state) {
                case STATE.RUN:
                    if(!landed) {
                        stateAir();
                    }
                    else {
                        if(jumpPressTimer < JUMP_GRACE) {
                            jumpPressTimer = JUMP_GRACE + 0.1f;
                            nextState = STATE.JUMP;
                        }
                    }
                    break;
                case STATE.FLOAT:
                case STATE.JUMP:
                    stateAir();
                    break;
            }
        }

        //STATE INIT (per state)
        if(nextState != STATE.NONE) {
            state = nextState;
            nextState = STATE.NONE;
            switch(state){
                case STATE.JUMP:
                    vel.y = JUMP_MAX;
                    jumpReleased = false;
                    break;
                case STATE.FLOAT:
                    if(!usedCourage) Fx(courageStartFx);
                    usedCourage = true;
                    courageTime = 0f;
                    break;
                case STATE.RUN:
                    usedCourage = false;
                    courage = COURAGES * COURAGE_SLOT;
                    break;
            }
            stateTime = 0f;
        }

        if(state != STATE.STOP && (state == STATE.RUN || vel.x > SPEED_MAX * 0.1f)) { //keep speed constantly big except when you try slamming onto a wall
            if(state == STATE.RUN && landed && Mathf.Abs(externalForce.x) < 0.1f) vel.x = SPEED_MAX;
            else if(Mathf.Abs(vel.x) > SPEED_MAX) vel.x = SPEED_MAX * Mathf.Sign(vel.x); //todo fix this mess
            else vel.x += ACCEL * Time.deltaTime;
        }
        else if(state == STATE.STOP){
            vel.x *= 0.7f; //abrupt stop when dead
        }

        //UPDATE (per state)
        switch(state) {
            case STATE.RUN:
                
            break;
            case STATE.JUMP:
                if(inputJump || jumpReleased) break;
                if(vel.y <= 0f){
                    jumpReleased = true;
                    break; //nothing to do here
                }
                vel.y *= JUMP_RELEASE_REDUCE; //early jump key up; reduce jump height
                jumpReleased = true;
                break;
            case STATE.FLOAT:
                if(courage > 0f) {
                    if(inputJump) {
                        if(vel.y <= -1f) {
                            //glide if you are falling down
                            if(vel.y < FLOAT_YVEL) vel.y = FLOAT_YVEL;
                            courage -= Time.deltaTime;
                        }
                        courageTime += Time.deltaTime; //even if you are jumping up, you can still use the burst
                    }
                    else{
                        if(courageTime < 0.2f && courageTime > 0f) {
                            //fast tap
                            courage = (Mathf.Ceil(courage / COURAGE_SLOT) - 1f) * COURAGE_SLOT; //use one full slot
                            courageBurst();
                        }
                        courageTime = 0f;
                    }
                }
                break;
        }
        
        if(overrideForceX) {
            vel.x = overrideForce.x;
            overrideForceX = false;
        }
        if(overrideForceY) {
            vel.y = overrideForce.y;
            overrideForceY = false;
        }
        rigid.velocity = vel + externalForce;
        externalForce = Vector2.zero;
        costume.transform.position = transform.position;
    }

    public int CourageSlots() {
        return Mathf.FloorToInt(courage / COURAGE_SLOT);
    }

    //note: Reset is reserved
    public void reset() {
        state = STATE.NONE;
        nextState = STATE.RUN;
        jumpPressTimer = JUMP_GRACE + 0.1f;
        setJumpHeight(2f); //todo levelmeta
        showPlayer(true);

        health = MAX_HP;
        usedCourage = false;
        courage = COURAGES * COURAGE_SLOT;
        invincibility = 0f;
        externalForce = Vector2.zero;
        overrideForce = Vector2.zero;
        overrideForceX = overrideForceY = false;
        if(animator != null) animator.reset();
    }

    public void setJumpHeight(float h) {
        JUMP_MAX = heightToVel(h);
    }

    public float heightToVel(float h) {
        return Mathf.Sqrt(2f * 9.8f * h);//v = sqrt(2gh)
    }

    private void checkLanded() {
        landed = false;
        Vector2 s = new Vector2(transform.position.x, transform.position.y);
        Vector2 e = s + Vector2.down * 0.6f;
        Collider2D collider = Physics2D.Linecast(s, e).collider;
        if(collider == null || collider.isTrigger) {
            return;
        }
        if(collider.gameObject.CompareTag("Floor")) {
            cameraControl.targetFloorY = collider.gameObject.transform.localScale.y / 2f + collider.gameObject.transform.position.y + PLAYER_RADIUS;
        }

        //there is a floor under me
        if(state == STATE.JUMP && stateTime < Time.deltaTime * 3f) {
            return; //don't check for ground right after a jump so the player *can* jump
        }
        landed = true;
    }

    private void checkDeath() {
        if(state == STATE.STOP || state == STATE.CUTSCENE) return;
        if(transform.position.y < cameraControl.targetFloorY - cameraControl.boundsDownY) {
            //fell down a pit
            GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
            float minFloorY = cameraControl.targetFloorY;
            foreach(GameObject floor in floors) {
                Vector3 pos = floor.transform.position;
                if(pos.x - ChunkLoader.FLOOR_WIDTH / 2f < transform.position.x || pos.x > transform.position.x + ChunkLoader.FLOOR_WIDTH / 2f + rigid.velocity.x) continue;
                float yBound = floor.transform.localScale.y / 2f + pos.y + PLAYER_RADIUS;
                if(yBound < minFloorY) minFloorY = yBound;
            }

            if(transform.position.y < minFloorY - cameraControl.boundsDownY) {
                //it really was a pit. Tragic.
                Kill(true);
                return;
            }
            else {
                //there was a floor beneath
                cameraControl.targetFloorY = minFloorY;
            }
        }

        health -= HP_LOSS * Time.deltaTime;

        if(health <= 0) {
            //died because running was no longer an available option
            Kill(false);
        }
    }

    private void stateAir() {
        if(landed) nextState = STATE.RUN;
        else if(courage > 0f && input_SpaceDown() && (jumpReleased || rigid.velocity.y <= 0f) && state != STATE.FLOAT) nextState = STATE.FLOAT;
    }

    private void courageBurst() {
        //todo only play burstFx if bursting succeeds
        Fx(courageFailFx);
        int n = collider2d.OverlapCollider(triggerContactFilter, colresult);
        for(int i = 0; i < n; i++) {
            GameObject o = colresult[i].gameObject;
            if(o.CompareTag("CourageTrigger")) {
                BlockUpdater build = o.GetComponent<BlockUpdater>();
                if(build != null) build.Couraged(this);
            }
        }
    }

    public void Impulse(float x, float y) {
        externalForce.x += x;
        externalForce.y += y;
    }

    public void SetVelocity(float x, float y) {
        SetVelocityX(x);
        SetVelocityY(y);
    }

    public void SetVelocityX(float x) {
        if(overrideForceX && Mathf.Abs(overrideForce.x) > Mathf.Abs(x)) return;
        overrideForceX = true;
        overrideForce.x = x;
    }

    public void SetVelocityY(float y) {
        if(overrideForceY && Mathf.Abs(overrideForce.y) > Mathf.Abs(y)) return;
        overrideForceY = true;
        overrideForce.y = y;
    }

    public void Damage(float damage) {
        Damage(damage, null);
    }

    public void Damage(float damage, GameObject? source) {
        health -= damage;
        if(damage < 0.5f) {
            //heal effect?
        }
        else if(damage > 5f) { //status effects (dot damage, <5) are not prevented by invincibility
            if(invincibility > 0.1f) return;
            invincibility = INVIN_TIME;

            Fx(damageFx);
            if(damage >= 30f) {
                int n = (int)((damage - 25f) / 18f) + 1;
                if(n > 3) n = 3;
                for(int i = 0; i < n; i++) Fx(damageFx);
            }
        }

        if(health > MAX_HP) health = MAX_HP;
    }

    public void Kill() {
        Kill(true);
    }
    public void Kill(bool deathEffect) {
        state = STATE.STOP;
        nextState = STATE.STOP;
        health = 0f;
        courage = 0f;
        stateTime = 0f;

        if(deathEffect){
            Fx(deathFx);
            showPlayer(false);
        }
        else {
            Fx(damageFx);
        }
        if(animator != null) animator.Kill(deathEffect);
        //TODO game over sequence
    }

    //TODO pool fx
    public void Fx(GameObject fx) {
        Fx(fx, transform.position, transform.rotation);
    }

    public void Fx(GameObject fx, Vector3 position, Quaternion rotation) {
        Instantiate(fx, position, rotation);
    }

    public void showPlayer(bool show) {
        costume.SetActive(show);
    }

    //input region (TODO replace with a dedicated input controller)
    public bool input_SpaceDown() {
        return Input.GetKeyDown(KeyCode.Space);
    }
    public bool input_Space() {
        return Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.Space);
    }
}
