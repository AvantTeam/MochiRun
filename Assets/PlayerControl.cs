using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public const float PLAYER_RADIUS = 0.45f;

    public static float JUMP_MAX = 0f; //y velocity of the jump. To set it using blocks instead, use setJumpHeight().
    public static float SPEED_MAX = 4f;
    public static float INVIN_TIME = 0.75f; //invincibility seconds after taking any damage
    public static float MAX_HP = 100f;
    public static float HP_LOSS = 4f; //per second
    //public static int COURAGES = 2;
    //public static float COURAGE_SLOT = 1f; //max courage = SLOT * COURAGES
    public static float MAX_COURAGE = 1f;
    public static float C_REGEN = 0.4f;
    public const float FLOAT_YVEL = -0.7f;
    private const float JUMP_RELEASE_REDUCE = 0.75f;
    private const float JUMP_GRACE = 0.3f; //the motive to "press jump" lasts for this long; i.e. press jump up to X seconds before touching the ground to immediately jump after
    private const float ACCEL = 10f;
    public const float STUN_TIME = 0.65f;

    public enum STATE {
        NONE = -1,
        RUN = 0, //running on the ground, or just fell after no jump
        JUMP,
        FLOAT, //using courage glide
        STUNNED, //ran into a wall
        STOP,
        CUTSCENE,
        NUM
    };

    public STATE state = STATE.NONE;
    public STATE nextState = STATE.NONE;
    public float stateTime = 0f; //time after a state change
    public float health = 0f;
    public float courage = 0f;
    //public float courageTime = 0f;
    public bool dead = false;
    public int coins = 0;

    public List<Item> items = new List<Item>();
    public ShieldItem shield = null; //stored for easy lookup
    public byte updateFxFlag = 0;

    public bool landed = false, collided = false;
    public bool usedCourage = false, gliding = false;
    private bool jumpReleased = false;
    private float jumpPressTimer = JUMP_GRACE + 0.1f;
    public float invincibility = 0f;
    private Collider2D[] colresult = new Collider2D[25]; //attempting to get all overlapping colliders will get 20 max
    private RaycastHit2D[] colsingle = new RaycastHit2D[1];
    private ContactFilter2D triggerContactFilter, floorContactFilter;
    private Vector2 externalForce = Vector2.zero, overrideForce = Vector2.zero, floorVector = new Vector2(-0.04f, -0.1f), wallVector = new Vector2(1f, 0.3f);
    private bool overrideForceX, overrideForceY;

    Rigidbody2D rigid;
    Collider2D collider2d;
    CameraController cameraControl;
    public PlayerAnimator animator;
    public InputHandler input;
    public GameObject itemsTable;
    public GameObject costume, burstFx, courageStartFx, deathFx, damageFx, courageFailFx, bumpWallFx;

    private void Awake() {
        input = Vars.mobile ? new MobileInputHandler(GameObject.Find("Main Camera").GetComponent<Camera>()) : new InputHandler(GameObject.Find("Main Camera").GetComponent<Camera>());
    }

    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
        cameraControl = GameObject.Find("Main Camera").GetComponent<CameraController>();
        triggerContactFilter = new ContactFilter2D();
        triggerContactFilter.useTriggers = true;
        floorContactFilter = new ContactFilter2D();
        floorContactFilter.useTriggers = false;

        reset();
        CurtainUpdater cu = Instantiate(Vars.main.prefabs.curtain, Vector3.zero, Quaternion.identity).GetComponent<CurtainUpdater>();
        cu.blackout = !LevelLoader.main.wasCurtainScene;
        cu.Set(true);
        cu.Hide(1f);
    }

    void Update() {
        Vector2 vel = rigid.velocity;
        updateFxFlag = 0;
        bool inputJump = input.Jump();
        stateTime += Time.deltaTime;
        checkLanded();
        checkWallFront(); //collided is trun when mochi is running fast and there is a wall in front
        checkDeath();
        bool tglide = false;

        if(input.JumpDown()) {
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
                            //jump
                            jumpPressTimer = JUMP_GRACE + 0.1f;
                            nextState = STATE.JUMP;
                            vel.x = SPEED_MAX;
                        }
                        else if(collided && vel.x < 0.1f * SPEED_MAX) {//wall in front, and stuck
                            //bump back
                            nextState = STATE.STUNNED;
                            vel.x = -1.1f * SPEED_MAX;
                            vel.y = 3f;
                        }
                    }
                    break;
                case STATE.FLOAT:
                case STATE.JUMP:
                    stateAir();
                    break;
                case STATE.STUNNED:
                    if(stateTime > STUN_TIME && Mathf.Abs(vel.x) < 1f && landed && animator.stunEnded) nextState = STATE.RUN;
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
                    //courageTime = 0f;
                    break;
                case STATE.RUN:
                    usedCourage = false;
                    //courage = COURAGES * COURAGE_SLOT;
                    break;
                case STATE.STUNNED:
                    Fx(bumpWallFx);
                    break;
            }
            stateTime = 0f;
        }

        if(state != STATE.STOP && state != STATE.STUNNED && (state == STATE.RUN || vel.x > SPEED_MAX * 0.1f)) { //keep speed constantly big except when you try slamming onto a wall
            if(state == STATE.RUN && landed && Mathf.Abs(externalForce.x) < 0.1f) vel.x = SPEED_MAX;
            else if(Mathf.Abs(vel.x) > SPEED_MAX) vel.x = SPEED_MAX * Mathf.Sign(vel.x); //todo fix this mess
            else vel.x += ACCEL * Time.deltaTime;
        }
        else if(state == STATE.STOP){
            vel.x *= 0.7f; //abrupt stop when dead
        }

        //update all items
        int itemn = items.Count;
        if(itemn > 0) {
            bool rem = false;
            for(int i = itemn - 1; i >= 0; i--) {
                if(items[i].UpdateAlways(this)) {
                    //remove this item
                    items.RemoveAt(i);
                    rem = true;
                }
            }
            if(rem) {
                rebuildItemUI();
                itemn = items.Count;
            }

            //only update the latest active item
            for(int i = itemn - 1; i >= 0; i--) {
                if(items[i].Update(this)) break;
            }
        }

        //UPDATE (per state)
        switch(state) {
            case STATE.RUN:
                if(courage < MAX_COURAGE) courage = Mathf.Min(MAX_COURAGE, courage + C_REGEN * Time.deltaTime);
            break;
            case STATE.JUMP:
                if(courage < MAX_COURAGE) courage = Mathf.Min(MAX_COURAGE, courage + C_REGEN * Time.deltaTime);
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
                        if(vel.y <= FLOAT_YVEL + 0.1f) {
                            //glide if you are falling down
                            tglide = true;
                            if(vel.y < FLOAT_YVEL) vel.y = FLOAT_YVEL;
                            courage -= Time.deltaTime;
                        }
                        //courageTime += Time.deltaTime; //even if you are jumping up, you can still use the burst
                    }
                    /*
                    else{
                        if(courageTime < 0.2f && courageTime > 0f) {
                            //fast tap
                            courage = (Mathf.Ceil(courage / COURAGE_SLOT) - 1f) * COURAGE_SLOT; //use one full slot
                            courageBurst();
                        }
                        courageTime = 0f;
                    }*/
                }
                break;
        }
        gliding = tglide;
        
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

    public void AddItem(Item item) {
        //if the same item type already exists, remove it first
        Item.TYPE type = item.Type();
        if(type == Item.TYPE.STACKABLE) return; //nope
        if(type == Item.TYPE.SHIELD) shield = null;
        items.RemoveAll(i => i.Type() == type);

        //shields go in front
        if(item is ShieldItem si){
            items.Insert(0, item);
            shield = si;
        }
        else items.Add(item);

        item.Reset();
        rebuildItemUI();
        item.Start(this);
    }

    public void removeItem(Item.TYPE type) {
        if(type == Item.TYPE.STACKABLE) return; //nope
        if(type == Item.TYPE.SHIELD) shield = null;
        items.RemoveAll(i => i.Type() == type);
        rebuildItemUI();
    }

    public void removeItem(Item item) {
        if(item.Type() == Item.TYPE.SHIELD) shield = null;
        items.Remove(item);
        rebuildItemUI();
    }

    public void clearItems(bool addVanilla) {
        shield = null;
        items.Clear();
        //todo add vanilla items, list stored in Level
        rebuildItemUI();
    }

    private void rebuildItemUI() {
        UI.ClearChildren(itemsTable);
        foreach(Item i in items) {
            i.BuildTable(itemsTable);
        }
    }

    //note: Reset is reserved
    public void reset() {
        state = STATE.NONE;
        nextState = STATE.RUN;
        jumpPressTimer = JUMP_GRACE + 0.1f;
        //setJumpHeight(2f);
        showPlayer(true);

        dead = false;
        health = MAX_HP;
        usedCourage = false;
        courage = MAX_COURAGE;
        gliding = false;
        invincibility = 0f;
        externalForce = Vector2.zero;
        overrideForce = Vector2.zero;
        overrideForceX = overrideForceY = false;
        coins = 0;
        clearItems(true);
        shield = null;
        if(animator != null) animator.reset();
    }

    public void Apply(Level level) {
        Physics2D.gravity = level.gravity;
        setJumpHeight(level.jumpHeight);
        MAX_HP = level.maxHealth;
        if(MAX_HP < 1) MAX_HP = 1;
        HP_LOSS = level.healthLoss;
        SPEED_MAX = level.maxSpeed;
        MAX_COURAGE = level.courage;
        if(cameraControl == null) cameraControl = GameObject.Find("Main Camera").GetComponent<CameraController>();
        cameraControl.targetZoom = cameraControl.zoom = level.zoom;
        reset();
    }

    public void setJumpHeight(float h) {
        JUMP_MAX = heightToVel(h);
    }

    public float heightToVel(float h) {
        Debug.Log(Physics2D.gravity.ToString());
        return Mathf.Sqrt(Mathf.Abs(2f * Physics2D.gravity.y * h)) * Mathf.Sign(-Physics2D.gravity.y * h);//v = sqrt(2gh)
    }

    private void checkLanded() {
        landed = false;
        //Vector2 s = new Vector2(transform.position.x, transform.position.y - 0.2f);
        //Vector2 e = s + Vector2.down * 0.6f;
        //int i = Physics2D.OverlapCircle(s, 0.4f, floorContactFilter, colsingle);
        int i = collider2d.Cast(floorVector, floorContactFilter, colsingle, 0.12f);

        if(i < 1) {
            return;
        }
        Collider2D collider = colsingle[0].collider;
        //Debug.Log(collider);
        if(collider.gameObject.CompareTag("Floor")) {
            cameraControl.targetFloorY = collider.gameObject.transform.localScale.y / 2f + collider.gameObject.transform.position.y + PLAYER_RADIUS;
        }

        //there is a floor under me
        if(state == STATE.JUMP && stateTime < Time.deltaTime * 3f) {
            return; //don't check for ground right after a jump so the player *can* jump
        }
        landed = true;
    }

    private void checkWallFront() {
        collided = false;

        if(rigid.velocity.x < 0f) return;

        int i = collider2d.Cast(wallVector, floorContactFilter, colsingle, 0.12f);

        if(i < 1) {
            return;
        }
        collided = true;
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

        float passiveLoss = HP_LOSS * Time.deltaTime;
        if((landed && state != STATE.STUNNED) || health > passiveLoss) health -= passiveLoss; //don't kill due to natural loss when mochi is airborne

        if(health <= 0) {
            //died because running was no longer an available option
            Kill(false);
        }
    }

    private void stateAir() {
        if(landed) nextState = STATE.RUN;
        else if(courage > 0f && input.JumpDown() && (jumpReleased || rigid.velocity.y <= 0f) && state != STATE.FLOAT) nextState = STATE.FLOAT;
    }

    public void SnapShield() {
        if(animator != null) animator.SnapShield();
        if(shield != null) shield.shieldCooldown = 0f;
    }

    //unused
    private void courageBurst() {
        bool playfx = true;
        
        int n = collider2d.OverlapCollider(triggerContactFilter, colresult);
        for(int i = 0; i < n; i++) {
            GameObject o = colresult[i].gameObject;
            if(o.CompareTag("CourageTrigger")) {
                BlockUpdater build = o.GetComponent<BlockUpdater>();
                if(build != null){
                    build.Couraged(this);
                    playfx = false;
                }
            }
        }
        if(animator != null) animator.CourageBurst();
        if(playfx) Fx(courageFailFx);
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
        if(damage < 0.5f) {
            //heal effect?
            health -= damage;
        }
        else if(damage > 1f) { //status effects (dot damage, <1) are not prevented by invincibility
            if(invincibility > 0.1f) return;

            //check items for damage negation
            for(int i = items.Count - 1; i >= 0; i--) {
                if(!items[i].HandleDamage(this, damage, source)) return;
            }

            health -= damage;
            //invincibility = INVIN_TIME;

            Fx(damageFx);
            if(animator != null) animator.Damage();
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
        if(dead) return;
        dead = true;
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

        if(LevelLoader.IsEditor()) {
            //if editor, skip the death cutscene sequence entirely
            Invoke("End", 1f);
            return;
        }

        Invoke("DropCurtains", 1f);
        Invoke("End", 2.5f);
    }

    public void DropCurtains() {
        CurtainUpdater cu = Instantiate(Vars.main.prefabs.curtain, Vector3.zero, Quaternion.identity).GetComponent<CurtainUpdater>();
        cu.blackout = !LevelLoader.main.wasCurtainScene;
        cu.Show(1.4f);
    }

    public void End() {
        Time.timeScale = 1f;//reset time scale
        LevelLoader.End();
    }

    public void Quit() {
        Time.timeScale = 1f;
        LevelLoader.Quit();
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
}
