using UnityEngine;
using static PlayerControl;

/** Items change the gameplay subtly or significantly. Only one item can be active per state, each. Items are not to be confused with Upgrades or Accessories, bought at shops.
 * Special vanilla items are: shield and gravity belt
 */
public abstract class Item {
    public enum TYPE {
        SHIELD, //reserved for the shield only
        AIR, //activates when pressing shift midair
        GROUND,
        WEAPON, //usually has ammo
        BOOST, //always active, has a set time before disappearing
        STACKABLE //multiple items (even the same ones) can be acquired simultaneously
    }

    public virtual void Reset() {

    }

    public virtual void Start(PlayerControl pcon) {

    }

    /** Always called, whether it is active or not, unlike Update. Runs before Update.
     * @return whether this item should be removed; NEVER CALL removeItem!
     */
    public virtual bool UpdateAlways(PlayerControl pcon) {
        return false;
    }

    /** @return whether the item was used; further items will not be used
     */
    public virtual bool Update(PlayerControl pcon) {
        return false;
    }

    public virtual bool HandleDamage(PlayerControl pcon, float damage, GameObject? source) {
        return true;
    }

    public virtual void BuildTable(GameObject table) {

    }

    public abstract TYPE Type();
}
