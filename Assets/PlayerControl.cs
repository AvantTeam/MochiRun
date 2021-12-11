using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public const float PLAYER_RADIUS = 0.5f;

    public static float JUMP_MAX = 0f; //y velocity of the jump. To set it using blocks instead, use setJumpHeight().
    public static float SPEED_MAX = 6f;
    public static float ACCEL = 10f;
    public static float MAX_HP = 200f;
    public static float HP_LOSS = 4f; //per second
    public static int COURAGES = 1;
    public static float COURAGE_SLOT = 1.5f; //max courage = SLOT * COURAGES
    public static float FLOAT_YVEL = -0.7f;
    public static float JUMP_RELEASE_REDUCE = 0.5f;

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
    private bool collided = false;
    private bool jumpReleased = false;

    Rigidbody2D rigid;
    CameraController cameraControl;
    public GameObject burstFx, courageStartFx, deathFx;
    // Start is called before the first frame update
    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        cameraControl = GameObject.Find("Main Camera").GetComponent<CameraController>();
        Reset();
    }

    // Update is called once per frame
    void Update() {
        Vector2 vel = rigid.velocity;
        stateTime += Time.deltaTime;
        checkLanded();
        checkDeath();

        //AUTO STATE CHANGE
        if(nextState == STATE.NONE) {
            switch(state) {
                case STATE.RUN:
                if(!landed) {
                    stateAir();
                }
                else {
                    if(input_SpaceDown()) nextState = STATE.JUMP;
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
                    if(!usedCourage) Instantiate(courageStartFx, transform.position, transform.rotation);
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

        if(state != STATE.STOP) {
            vel.x += ACCEL * Time.deltaTime;
            if(Mathf.Abs(vel.x) > SPEED_MAX) vel.x = SPEED_MAX * Mathf.Sign(vel.x);
        }
        else {
            vel.x *= 0.7f; //abrupt stop when dead
        }
        //UPDATE (per state)
        switch(state) {
            case STATE.RUN:
                
            break;
            case STATE.JUMP:
                if(input_Space() || jumpReleased || vel.y <= 0f) break; //nothing to do here
                vel.y *= JUMP_RELEASE_REDUCE; //early jump key up; reduce jump height
                jumpReleased = true;
            break;
            case STATE.FLOAT:
                if(courage > 0f) {
                    if(input_Space()) {
                        if(vel.y < FLOAT_YVEL) vel.y = FLOAT_YVEL;
                        courage -= Time.deltaTime;
                        courageTime += Time.deltaTime;
                    }
                    else{
                        if(courageTime > 0f && courageTime < 0.15f) {
                            //fast tap
                            courage = (Mathf.Ceil(courage / COURAGE_SLOT) - 1f) * COURAGE_SLOT; //use one full slot
                            courageBurst();
                        }
                        courageTime = 0f;
                    }
                }
            break;
        }
        
        rigid.velocity = vel;
    }

    public void Reset() {
        state = STATE.NONE;
        nextState = STATE.RUN;
        setJumpHeight(2.5f);
        showPlayer(true);

        health = MAX_HP;
        usedCourage = false;
        courage = COURAGES * COURAGE_SLOT;
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
        if(collider == null) {
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
        else if(courage > 0f && input_Space() && rigid.velocity.y <= -1.0f && state != STATE.FLOAT) nextState = STATE.FLOAT;
    }

    private void courageBurst() {
        Instantiate(burstFx, transform.position, transform.rotation);
    }

    public void Kill() {
        Kill(true);
    }
    public void Kill(bool deathEffect) {
        state = STATE.STOP;
        nextState = STATE.STOP;
        health = 0f;
        stateTime = 0f;

        if(deathEffect){
            Instantiate(deathFx, transform.position, transform.rotation);
            showPlayer(false);
        }
        //TODO
    }

    public void showPlayer(bool show) {
        GetComponent<Renderer>().enabled = show;
    }

    //input region (TODO replace with a dedicated input controller)
    public bool input_SpaceDown() {
        return Input.GetKeyDown(KeyCode.Space);
    }
    public bool input_Space() {
        return Input.GetKey(KeyCode.Space);
    }
}
