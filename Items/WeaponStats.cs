using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftKitty.MasterCharacterCreator;
using System;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public  class WeaponStats : ScriptableObject
{
/*     public enum DmgType 
    {
        Physical,
        Arcane,
        Fire
    } */
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
    public bool offhand =false;
    public int pDmg;
    public int mDmg;
    public float speed;


}
