using System.Collections;
using System.Collections.Generic;
using SoftKitty.MasterCharacterCreator;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class WeaponStatsData
{
/*     //public GameObject prefab;
    public string prefab;
    public WeaponController weaponController;
    public WeaponController offhandWeaponController;
    public string itemName;
    public EquipmentAppearance myItemAppearance;
    //public Texture myItemIcon;
    public string myItemIcon;
    public OutfitSlots mySlot; //indicates the slot for entity
    public int itemSlotType; //indicates the slot for stat system
    //public AudioClip attackSound; 
    public string attackSound; 
    //public GameObject projectile;
    public string projectile;
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
    public bool offhand;
    public int pDmg;
    public int mDmg;
    public float speed;


    public WeaponStatsData(WeaponStats item)
    {
        prefab = item.prefab !=null ? GetAssetPath(item.prefab) : string.Empty;
        myItemIcon = item.myItemIcon !=null ? GetAssetPath(item.myItemIcon) : string.Empty;
        attackSound = item.attackSound !=null ? GetAssetPath(item.attackSound) : string.Empty;
        projectile = item.projectile !=null ? GetAssetPath(item.projectile) : string.Empty;
        weaponController = item.weaponController;
        offhandWeaponController = item.offhandWeaponController;
        itemName = item.itemName;
        myItemAppearance = item.myItemAppearance;
        mySlot = item.mySlot; //indicates the slot for entity
        itemSlotType = item.itemSlotType; //indicates the slot for stat system

        health = item.health;
        stamina = item.stamina;
        armor = item.armor;
        maxArmor = item.maxArmor;
        mAcc = item.mAcc;
        mEva = item.mEva;
        rAcc = item.rAcc;
        rEva = item.rEva;
        range = item.range;
        weaponType = item.weaponType;
        dmgType = item.dmgType;
        dmg = item.dmg;
        armorBypass = item.armorBypass;
        armorPierce = item.armorPierce;
        attackCost = item.attackCost;
        attackRate = item.attackRate;
        dmgOverTime = item.dmgOverTime;
        slowAttack = item.slowAttack;
        bashAttack = item.bashAttack;
        pushAttack = item.pushAttack;
        blindingAttack = item.blindingAttack;
        weakeningAttack = item.weakeningAttack;
        value = item.value;
        offhand = item.offhand;
        pDmg = item.pDmg;
        mDmg = item.mDmg;
        speed = item.speed;
    }

    public WeaponStats ToWeaponStats()
    {    
        WeaponStats item = ScriptableObject.CreateInstance<WeaponStats>();
        #if UNITY_EDITOR
            item.prefab = !string.IsNullOrEmpty(prefab) ? UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefab) : null;
            item.attackSound = !string.IsNullOrEmpty(attackSound) ? UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(attackSound) : null;
            item.projectile = !string.IsNullOrEmpty(projectile) ? UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(projectile) : null;
            item.attackSound = !string.IsNullOrEmpty(projectile) ? UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(projectile) : null; 
            item.projectile = !string.IsNullOrEmpty(projectile) ? UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(projectile) : null;
        #else
            if (!string.IsNullOrEmpty(prefabPath))
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(prefab);
                item.prefab = await handle.Task;
            }
            if (!string.IsNullOrEmpty(texturePath))
            {
                var handle = Addressables.LoadAssetAsync<Texture>(texture);
                item.myItemIcon = await handle.Task;
            }
            if (!string.IsNullOrEmpty(audioPath))
            {
                var handle = Addressables.LoadAssetAsync<AudioClip>(audio);
                item.attackSound = await handle.Task;
            }
        #endif

        item.itemName = itemName;
        item.myItemAppearance = myItemAppearance;
        item.weaponController = weaponController;
        item.offhandWeaponController = offhandWeaponController;
        item.mySlot = mySlot; //indicates the slot for entity
        item.itemSlotType = itemSlotType; //indicates the slot for stat system

        item.health = health;
        item.stamina = stamina;
        item.armor = armor;
        item.maxArmor = maxArmor;
        item.mAcc = mAcc;
        item.mEva = mEva;
        item.rAcc = rAcc;
        item.rEva = rEva;
        item.range = range;
        item.weaponType = weaponType;
        item.dmgType = dmgType;
        item.dmg = dmg;
        item.armorBypass = armorBypass;
        item.armorPierce = armorPierce;
        item.attackCost = attackCost;
        item.attackRate = attackRate;
        item.dmgOverTime = dmgOverTime;
        item.slowAttack = slowAttack;
        item.bashAttack = bashAttack;
        item.pushAttack = pushAttack;
        item.blindingAttack = blindingAttack;
        item.weakeningAttack = weakeningAttack;
        item.value = value;
        item.offhand = offhand;
        item.pDmg = pDmg;
        item.mDmg = mDmg;

        return item;
    }



    private string GetAssetPath(Object asset)
    {
        return asset ? UnityEditor.AssetDatabase.GetAssetPath(asset) : string.Empty;
    }
 */
}