
using SoftKitty.MasterCharacterCreator;
using UnityEngine;

public class ItemStats : MonoBehaviour
{
    public GameObject prefab;
    public WeaponController weaponController;
    public WeaponController offhandWeaponController;
    public string itemName;
    public EquipmentAppearance myItemAppearance;
    public Texture myItemIcon;
    public OutfitSlots mySlot; //indicates the slot for entity
    public int itemSlotType; //indicates the slot for stat system
    public AudioClip attackSound; 
    public GameObject projectile;
    public int health;
    public int stamina;
    public int armor;
    public int maxArmor;
    public int mAcc;
    public int mEva;
    public int rAcc;
    public int rEva;
    public int range;
    public int weaponType;
    public int dmgType;
    public int dmg;
    public int armorBypass;
    public int armorPierce;
    public int attackCost;
    public float attackRate;
    public int dmgOverTime;
    public int slowAttack;
    public int bashAttack;
    public int pushAttack;
    public int blindingAttack;
    public int weakeningAttack;
    public int value;

}
