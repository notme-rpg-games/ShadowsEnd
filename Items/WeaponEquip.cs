using System;
using System.Collections;
using System.Collections.Generic;
using SoftKitty.MasterCharacterCreator;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

/* namespace SoftKitty.MasterCharacterCreator
{ */
public class WeaponEquip : MonoBehaviour
{
    //skill multipliers
    public int swordBypassSkill;
    public float swordSpeedSkill;
    public float axeDmgSkill;
    public int axeArmorDestructionSkill;
    public float maceDmgSkill;
    public int maceBashSkill;
    public float spearEvasionSkill;
    //public int spearPushSkill;
    public int crossbowScatterShot;
    public int crossbowStun;
    public float bowQuickshot;
    public int shieldBashSkill;
    //public int shieldHunkerDownSkill;
    public float twoHandedDmgSkill;
    public int crossbowBypassSkill;
    public int bowBypassSkill;
    public float crossbowAimSkill;
    public float bowAimSkill;
    public float magicDmgSkill;
    public int magicAoeRadiusSkill;
    public float magicAoeDmgReductionSkill;

    private static WeaponEquip _instance;
    public static WeaponEquip Instance { get { return _instance; } }
    private Animator myAnim;
    private WeaponController myWeaponController;
    private CharacterEntity myCharacterEntity;
    public RuntimeAnimatorController fistAnim;
    public RuntimeAnimatorController spearAnim;
    public RuntimeAnimatorController swordShieldAnim;
    public RuntimeAnimatorController spearShieldAnim;
    public RuntimeAnimatorController bowAnim;
    public RuntimeAnimatorController mageAnim;
    public RuntimeAnimatorController twoHAnim;
    public RuntimeAnimatorController crossbowAnim;
    public RuntimeAnimatorController dualWieldingAnim;
    //public int weaponType;
/*  weaponType
    0 fist
    1 sword
    2 axe
    3 mace
    4 spear
    5 fist + buckler
    6 sword+buckler
    7 axe+buckler
    8 mace+buckler
    9 spear+buckler
    10 fist + shield
    11 sword + long shield
    12 axe + long shield
    13 mace + long shield
    14 spear + long shield
    15 2*sword
    16 2*axe
    17 2*mace
    18 2*spear
    21 2-H sword
    22 2-Axe
    23 2-Mace
    24 2-H Spear
    25 crossbow
    26 bow
    30 arcane magic (all kind of buffs, but weaker)
    31 fire magic (dmg/heal over time)
    (41 lighting-bash)
    32 water (slow/sped up)
    33 earth (bash)
    (42 ice stronger debuff, but weaker dmg)
    34 wind (pushattack)
    (44 gas-dmg over time)
    (44 lava - dmg over time)
    35 light
    36 shadow  */
    public bool initiation;
    
    void Awake()
    {
        //if an instance of this already exists and it isn't this one
        if(_instance != null && _instance != this)
        {
            //destroy this instance
            Destroy(this.gameObject);
        }
        else
        {
            //make this the instance
            _instance = this;
        }
    }



    public void EquipWeapon(Fighter stats, WeaponStats weaponStats, WeaponStats offhand, int unequip)
    {
/*         Debug.Log(weaponStats);
        Debug.Log(offhand); */
        //remove active skills to get base stats
        if(stats.skill1Active)
        {
            stats.ToggleSkill1();
        }
        if(stats.skill2Active)
        {
            stats.ToggleSkill2();
        }
        if(stats.skill3Active)
        {
            stats.ToggleSkill3();
        }
        //eliminate range otherwise next weapon still uses range
        stats.projectilePrefab = null;
/*         if(string.IsNullOrEmpty(weapon))
        {
            weapon = "Fists";
        } */
        if(weaponStats==null)
        {
            weaponStats = Resources.Load<WeaponStats>("Equipment/Weapon/Fists");
            weaponStats.armor = 9999;
        }
        
        myWeaponController = weaponStats.weaponController;
        //myWeaponController = Resources.Load<GameObject>("Equipment/Weapon/"+weaponStats.name).GetComponent<WeaponController>();
        
        //WeaponStats weaponStats = Resources.Load("Equipment/Weapon/"+weapon).GetComponent<WeaponStats>();

        myCharacterEntity = stats.GetComponent<CharacterEntity>();
        if(unequip==1)
        {
            myCharacterEntity.EquipWeapon(myWeaponController, WeaponState.Hold);
        }
        else
        {
            if(weaponStats.weaponType >20)
            {
            myCharacterEntity.UnequipWeapon(WeaponType.TwoHanded);
            }
            else
            {
            myCharacterEntity.UnequipWeapon(WeaponType.RightHand);
            }
        }

        stats.currentWeaponType = weaponStats.weaponType;
        stats.maxStamina += weaponStats.stamina*unequip;
        stats.currentStamina += weaponStats.stamina*unequip;
        stats.mAcc += weaponStats.mAcc*unequip;
        stats.mEva += weaponStats.mEva*unequip;
        stats.rAcc += weaponStats.rAcc*unequip;
        stats.rEva += weaponStats.rEva*unequip;
        stats.range = weaponStats.range*unequip;
        if(weaponStats.projectile != null)
        {
            stats.projectilePrefab = weaponStats.projectile;
        }
        stats.dmgType = weaponStats.dmgType;

        //apply magicflow damage if magic damage
        if(stats.dmgType >0)
        {
            stats.dmg = Mathf.RoundToInt((stats.mDmg + weaponStats.dmg)*(1+SkillSystem.Instance.magicFlowEffect/100f*stats.magicFlowSkill))*unequip;
        }
        else
        {
            stats.dmg = stats.pDmg + weaponStats.dmg*unequip;
        }
        stats.baseAttackCost = weaponStats.attackCost*unequip;
        stats.armorBypass = weaponStats.armorBypass*unequip;
        stats.armorPierce = weaponStats.armorPierce*unequip;
        stats.attackRate = weaponStats.attackRate*unequip - stats.attackRateDebuffValue;

        stats.dmgOverTime += weaponStats.dmgOverTime*unequip;
        stats.slowAttack += weaponStats.slowAttack*unequip;
        stats.bashAttack += weaponStats.bashAttack*unequip;
        stats.pushAttack += weaponStats.pushAttack*unequip;
        stats.blindingAttack += weaponStats.blindingAttack*unequip;
        stats.weakeningAttack += weaponStats.weakeningAttack*unequip;

        //check if offhand is free for one-handed skills
        bool offhandFree = false;
        stats.isCountering = false;
        if(offhand == null)
        {
            offhandFree = true;
        } 
        ApplyWeaponSkill(stats, weaponStats, unequip, offhandFree); 
        
        stats.attackCost = stats.baseAttackCost;
        stats.attackSound = weaponStats.attackSound;
        

        stats.dmg2= 0;
        stats.usingShield = false;

        //load offhand if not 2H weapon equipped
        if(offhand!=null && stats.currentWeaponType<20)
        {
            myWeaponController = offhand.offhandWeaponController;
            //myWeaponController = Resources.Load<GameObject>("Equipment/Weapon/"+offhand.name).GetComponent<WeaponController>();
/*             myCharacterEntity = stats.GetComponent<CharacterEntity>();
            if(unequip==1)
            {
                myCharacterEntity.EquipWeapon(myWeaponController, WeaponState.Hold);
            }
            else
            {
                myCharacterEntity.EquipWeapon(myWeaponController, WeaponState.Carry);
            } */        
            //weaponStats = offhand;
            if(unequip==1)
            {
                //marks the 1H weapon as offhand
                if(offhand.weaponType <5)
                {
                    if(stats.skill4Active)
                    {
                        stats.itemList[3].itemSlotType =4;
                    }
                    else
                    {
                        stats.itemList[1].itemSlotType =4;
                    }
                }
                myCharacterEntity.EquipWeapon(myWeaponController, WeaponState.Hold);
            }
            else
            {
                //marks the 1H weapon as offhand
                if(offhand.weaponType <5)
                {
                    if(stats.skill4Active)
                    {
                        stats.itemList[3].itemSlotType =1;
                    }
                    else
                    {
                        stats.itemList[1].itemSlotType =1;
                    }
                }
                myCharacterEntity.UnequipWeapon(WeaponType.LeftHand);
            }


            if(offhand.weaponType <5) //if second weapon
            {
                //without mainhand equipped add current weapontype
                if(stats.currentWeaponType ==0)
                {
                    stats.dmg = 0;
                    stats.currentWeaponType+= offhand.weaponType;
                }
                stats.currentWeaponType += 14;
                float dualwieldingEffect = SkillSystem.Instance.dualWieldingEffect/100f;
                if(offhand.dmgType >0)
                {
                    //add damage depending on dualwielding and magic flow
                    stats.dmg2 = Mathf.RoundToInt((stats.mDmg + offhand.dmg)*(0.7f+0.1f*stats.dualwieldingSkill)*(1+SkillSystem.Instance.magicFlowEffect/100f*stats.magicFlowSkill))*unequip;
                }
                else
                {
                    stats.dmg2 = Mathf.RoundToInt((stats.pDmg + offhand.dmg)*(0.7f+0.1f*stats.dualwieldingSkill))*unequip;
                }
                stats.baseAttackCost += offhand.attackCost*unequip;
                stats.armorBypass = (stats.armorBypass + offhand.armorBypass*unequip)/2;
                stats.armorPierce = (stats.armorPierce + offhand.armorPierce*unequip)/2;

                stats.maxStamina += offhand.stamina*unequip;
                stats.currentStamina += offhand.stamina*unequip;
                stats.mAcc -= weaponStats.mAcc*unequip;
                stats.mEva -= weaponStats.mEva*unequip;
                stats.rAcc -= weaponStats.rAcc*unequip;
                stats.rEva -= weaponStats.rEva*unequip;
                stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*(0.5f+dualwieldingEffect*stats.dualwieldingSkill))*unequip;
                stats.mEva += Mathf.RoundToInt(weaponStats.mEva*(0.5f+dualwieldingEffect*stats.dualwieldingSkill))*unequip;
                stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*(0.5f+dualwieldingEffect*stats.dualwieldingSkill))*unequip;
                stats.rEva += Mathf.RoundToInt(weaponStats.rEva*(0.5f+dualwieldingEffect*stats.dualwieldingSkill))*unequip;
                stats.mAcc += Mathf.RoundToInt(offhand.mAcc*(0.5f+dualwieldingEffect*stats.dualwieldingSkill))*unequip;
                stats.mEva += Mathf.RoundToInt(offhand.mEva*(0.5f+dualwieldingEffect*stats.dualwieldingSkill))*unequip;
                stats.rAcc += Mathf.RoundToInt(offhand.rAcc*(0.5f+dualwieldingEffect*stats.dualwieldingSkill))*unequip;
                stats.rEva += Mathf.RoundToInt(offhand.rEva*(0.5f+dualwieldingEffect*stats.dualwieldingSkill))*unequip;

                stats.dmgType2 = offhand.dmgType;

                if(stats.whirlWindSkill > 0)
                {
                    stats.mAcc += Mathf.RoundToInt(stats.speed*SkillSystem.Instance.whirlWindEffect*stats.whirlWindSkill)*unequip;
                    stats.mEva += Mathf.RoundToInt(stats.speed*SkillSystem.Instance.whirlWindEffect*stats.whirlWindSkill)*unequip;
                    stats.rEva += Mathf.RoundToInt(stats.speed*SkillSystem.Instance.whirlWindEffect*stats.whirlWindSkill)*unequip;
                }

                //stats.range = weaponStats.range*unequip; //here apply range 

                    
                    //not double applying 
                    //ApplyWeaponSkill(stats, weaponStats, unequip); 

/*                 if((stats.attackRate - stats.attackRateDebuffValue) < (offhand.attackRate - stats.attackRateDebuffValue)) //choose the slower attack rate of the two
                {
                    stats.attackRate = offhand.attackRate  - stats.attackRateDebuffValue;  
                }    */

            }
            else if(offhand.weaponType == 5) //if buckler
            {
                stats.usingShield = true;
                stats.currentWeaponType += 5;
                stats.maxStamina += offhand.stamina*unequip;
                stats.currentStamina += offhand.stamina*unequip;
                //don't apply range for shields
                ApplyWeaponSkill(stats, offhand, unequip, false); 
            }
            else //great shield
            {
                stats.usingShield = true;
                stats.currentWeaponType += 10;
                stats.maxStamina += offhand.stamina*unequip;
                stats.currentStamina += offhand.stamina*unequip;
                //don't apply range for shields
                ApplyWeaponSkill(stats, offhand, unequip, false); 
            }       
            
            stats.attackCost = stats.baseAttackCost;
        }




        myAnim = stats.GetComponent<Animator>();
        switch (stats.currentWeaponType)
        {
        case 0: //fists
            myAnim.runtimeAnimatorController = fistAnim;
            stats.skill3available = false;
            break;
        case 1: //sword
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = false;
            break;
        case 2: //axe
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = false;
            break;
        case 3: //mace
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = false;
            break;
        case 4: //spear
            myAnim.runtimeAnimatorController = spearAnim;
            stats.skill3available = false;
            break;

        case 5: //fist + buckler
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = true;
            break;
        case 6: //sword + buckler
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = true;
            break;
        case 7: //sword + buckler
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = true;
            break;
        case 8: //sword + buckler
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = true;
            break;
        case 9: //spear + buckler
            myAnim.runtimeAnimatorController = spearShieldAnim;
            stats.skill3available = true;
            break;

        case 10: //fist + shield
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = true;
            break;
        case 11: //sword + shield
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = true;
            break;
        case 12: //sword + shield
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = true;
            break;
        case 13: //sword + shield
            myAnim.runtimeAnimatorController = swordShieldAnim;
            stats.skill3available = true;
            break;
        case 14: //spear + shield
            myAnim.runtimeAnimatorController = spearShieldAnim;
            stats.skill3available = true;
            break;
            
        case 15: //sword x2
            myAnim.runtimeAnimatorController = dualWieldingAnim;
            stats.skill3available = false;
            break;
        case 16: //axe x2
            myAnim.runtimeAnimatorController = dualWieldingAnim;
            stats.skill3available = false;
            break;
        case 17: //mace x2
            myAnim.runtimeAnimatorController = dualWieldingAnim;
            stats.skill3available = false;
            break;
        case 18: //spear x2
            myAnim.runtimeAnimatorController = dualWieldingAnim;
            stats.skill3available = false;
            break;
        
        case 21: //2h sword
            myAnim.runtimeAnimatorController = twoHAnim;
            stats.skill3available = true;
            break;
        case 22: //2h Axe
            myAnim.runtimeAnimatorController = twoHAnim;
            stats.skill3available = true;
            break;
        case 23: //2h Mace
            myAnim.runtimeAnimatorController = twoHAnim;
            stats.skill3available = true;
            break;
        case 24: //spear
            myAnim.runtimeAnimatorController = spearAnim;
            stats.skill3available = true;
            break;
        
        case 25: //crossbow
            myAnim.runtimeAnimatorController = crossbowAnim;
            stats.skill3available = true;
            break;
        case 26: //bow
            myAnim.runtimeAnimatorController = bowAnim;
            stats.skill3available = false;
            break;

        case 30: //arcane
            myAnim.runtimeAnimatorController = mageAnim;
            stats.skill3available = true;
            break;
        case 31: //fire
            myAnim.runtimeAnimatorController = mageAnim;
            stats.skill3available = true;
            stats.dmgOverTime += 5 * unequip;
            break;
        case 32: //water
            myAnim.runtimeAnimatorController = mageAnim;
            stats.skill3available = true;
            break;
        case 33: //earth
            myAnim.runtimeAnimatorController = mageAnim;
            stats.skill3available = true;
            break;
        case 34: //air
            myAnim.runtimeAnimatorController = mageAnim;
            stats.skill3available = true;
            break;
            
        case 35: //light
            myAnim.runtimeAnimatorController = mageAnim;
            stats.skill3available = true;
            break;
        
        case 36: //shadow
            myAnim.runtimeAnimatorController = mageAnim;
            stats.skill3available = true;
            break;
        }

        //apply elementalist-skill dmg to both weapons if dmg type fits
        if(stats.elementalistSkill >0)
        {
            if(stats.dmgType == stats.elementalistSkillElement+1)
            {
                stats.dmg += Mathf.RoundToInt(stats.dmg*SkillSystem.Instance.elementalistEffect*stats.elementalistSkill/100f);
            }            
            if(stats.dmgType2 == stats.elementalistSkillElement+1)
            {
                stats.dmg2 += Mathf.RoundToInt(stats.dmg*SkillSystem.Instance.elementalistEffect*stats.elementalistSkill/100f);
            }
        } 
    }

    private void ApplyWeaponSkill(Fighter stats, WeaponStats weaponStats, int unequip, bool noOffhand)
    {
        switch(weaponStats.weaponType)
        {
            //one-handed
            case 1:
                if(stats.swordSkill==-1) //remove weapon stats
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.mAcc += SkillSystem.Instance.swordEffect*stats.swordSkill*unequip;
                    stats.mEva += SkillSystem.Instance.swordEffect*stats.swordSkill*unequip;

                    if(stats.sworddancerSkill>0)            //reduce speed by 20% for each
                    {
                        if(unequip==1)
                        {
                            float sworddancerEffect = 1-(SkillSystem.Instance.swordDancerEffect/100f);
                            stats.attackRate *= Mathf.Pow(sworddancerEffect, stats.sworddancerSkill);
                        }
/*                         else                                //necessary?
                        {
                            stats.attackRate /= Mathf.Pow(0.8f, stats.sworddancerSkill);
                        } */
                    }
                if(noOffhand && stats.oneHandedSkill >0)
                {
                    if(stats.conterSkill >0)
                    {
                        stats.isCountering = true;
                    }
                    stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.mEva += Mathf.RoundToInt(weaponStats.mEva*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.rEva += Mathf.RoundToInt(weaponStats.rEva*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.dmg += Mathf.RoundToInt(weaponStats.dmg*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                }
                }
            break;
            case 2:
                if(stats.axeSkill==-1) // remove weapon stats
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.mAcc += SkillSystem.Instance.axeEffect*stats.axeSkill*unequip;
                    stats.mEva += SkillSystem.Instance.axeEffect*stats.axeSkill*unequip;

                    if(stats.armorbreakerSkill==1)        //only 1 or 0
                    {
                        stats.armorDestruction += SkillSystem.Instance.armorBreakerEffect*unequip;
                    }
                }
                if(noOffhand && stats.oneHandedSkill >0)
                {
                    if(stats.conterSkill >0)
                    {
                        stats.isCountering = true;
                    }
                    stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.mEva += Mathf.RoundToInt(weaponStats.mEva*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.rEva += Mathf.RoundToInt(weaponStats.rEva*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.dmg += Mathf.RoundToInt(weaponStats.dmg*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                }
            break;
            case 3:
                if(stats.maceSkill==-1) //remove weapon stats
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.mAcc += SkillSystem.Instance.maceEffect*stats.maceSkill*unequip;
                    stats.mEva += SkillSystem.Instance.maceEffect*stats.maceSkill*unequip;

                    if(stats.basherSkill ==1)        //only 1 or 0
                    {    
                        stats.bashAttack += SkillSystem.Instance.BasherEffect*unequip;
                    }
                }
                if(noOffhand && stats.oneHandedSkill >0)
                {
                    if(stats.conterSkill >0)
                    {
                        stats.isCountering = true;
                    }
                    stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.mEva += Mathf.RoundToInt(weaponStats.mEva*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.rEva += Mathf.RoundToInt(weaponStats.rEva*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.dmg += Mathf.RoundToInt(weaponStats.dmg*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                }
            break;
            case 4:
                if(stats.spearSkill==-1) 
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.isCountering = true;
                    stats.mAcc += SkillSystem.Instance.spearEffect*stats.spearSkill*unequip;
                    stats.mEva += SkillSystem.Instance.spearEffect*stats.spearSkill*unequip;
                }
                stats.reachAdvantageBonus = 2*weaponStats.mEva;

                if(noOffhand && stats.oneHandedSkill >0)
                {
                    if(stats.conterSkill >0)
                    {
                        stats.isCountering = true;
                    }
                    stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.mEva += Mathf.RoundToInt(weaponStats.mEva*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.rEva += Mathf.RoundToInt(weaponStats.rEva*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                    stats.dmg += Mathf.RoundToInt(weaponStats.dmg*stats.oneHandedSkill*SkillSystem.Instance.oneHandedEffect/100f*unequip);
                }
            break;
            //shields
            case 5:
                if(stats.shieldSkill==-1) //if skill == 0 remove weapon stats
                {
                    stats.mAcc -= weaponStats.mAcc/2*unequip;
                    stats.mEva -= weaponStats.mEva/2*unequip;
                    stats.rAcc -= weaponStats.rAcc/2*unequip;
                    stats.rEva -= weaponStats.rEva/2*unequip;
                }
                else
                {
                    float shieldskill = SkillSystem.Instance.shieldEffect/100f;
                    stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*(1+stats.shieldSkill*shieldskill))*unequip;
                    stats.mEva += Mathf.RoundToInt(weaponStats.mEva*(1+stats.shieldSkill*shieldskill))*unequip;
                    stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*(1+stats.shieldSkill*shieldskill))*unequip;
                    stats.rEva += Mathf.RoundToInt(weaponStats.rEva*(1+stats.shieldSkill*shieldskill))*unequip;
                    if(stats.promotionClassList[12]==2)
                    {
                        stats.mEva += Mathf.RoundToInt(weaponStats.mEva*FighterClasses.Instance.PromotionClassBoniList[12]/100f)*unequip;
                        stats.rEva += Mathf.RoundToInt(weaponStats.rEva*FighterClasses.Instance.PromotionClassBoniList[12]/100f)*unequip;
                    }
                }
            break;
            case 10:
                if(stats.shieldSkill==-1) //if skill == 0 remove weapon stats
                {
                    stats.mAcc -= weaponStats.mAcc/2*unequip;
                    stats.mEva -= weaponStats.mEva/2*unequip;
                    stats.rAcc -= weaponStats.rAcc/2*unequip;
                    stats.rEva -= weaponStats.rEva/2*unequip;
                    stats.maxStamina += weaponStats.stamina/2*unequip;
                    stats.hunkerdownBonus = weaponStats.rEva/2;
                }
                else
                {
                    float shieldskill = SkillSystem.Instance.shieldEffect/100f;
                    stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*(1+stats.shieldSkill*shieldskill))*unequip;
                    stats.mEva += Mathf.RoundToInt(weaponStats.mEva*(1+stats.shieldSkill*shieldskill))*unequip;
                    stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*(1+stats.shieldSkill*shieldskill))*unequip;
                    stats.rEva += Mathf.RoundToInt(weaponStats.rEva*(1+stats.shieldSkill*shieldskill))*unequip;

                    stats.hunkerdownBonus = Mathf.RoundToInt(weaponStats.rEva*(1+stats.shieldSkill*shieldskill))*unequip;
                    //spartan class ability
                    if(stats.promotionClassList[12]==2)
                    {
                        stats.mEva += Mathf.RoundToInt(weaponStats.mEva*FighterClasses.Instance.PromotionClassBoniList[12]/100f)*unequip;
                        stats.rEva += Mathf.RoundToInt(weaponStats.rEva*FighterClasses.Instance.PromotionClassBoniList[12]/100f)*unequip;
                        stats.hunkerdownBonus += Mathf.RoundToInt(weaponStats.rEva*FighterClasses.Instance.PromotionClassBoniList[12]/100f)*unequip;
                    }
                }
            break;
            //two-handed
            case 21:
                if(stats.swordSkill==-1) //sword skill
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.mAcc += SkillSystem.Instance.swordEffect*stats.swordSkill*unequip;
                    stats.mEva += SkillSystem.Instance.swordEffect*stats.swordSkill*unequip;
                }          
                      
                if(stats.twohandedSkill==-1) 
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    float twohandedSkill = SkillSystem.Instance.twoHandedEffect/100f;
                    stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.mEva += Mathf.RoundToInt(weaponStats.mEva*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.rEva += Mathf.RoundToInt(weaponStats.rEva*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.dmg += Mathf.RoundToInt(stats.pDmg*stats.twohandedSkill*twohandedSkill)*unequip;
                    
                    //apply char strength a second time for stormtrooper skill if physical damage
                    if(stats.stormTrooperSkill>0 && weaponStats.dmgType==0)
                    {
                        stats.dmg += stats.pDmg;
                    }
                }
            break;
            case 22:
                if(stats.axeSkill==-1) // remove weapon stats
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.mAcc += SkillSystem.Instance.axeEffect*stats.axeSkill*unequip;
                    stats.mEva += SkillSystem.Instance.axeEffect*stats.axeSkill*unequip;
                }          
                      
                if(stats.twohandedSkill==-1) 
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    float twohandedSkill = SkillSystem.Instance.twoHandedEffect/100f;
                    stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.mEva += Mathf.RoundToInt(weaponStats.mEva*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.rEva += Mathf.RoundToInt(weaponStats.rEva*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.dmg += Mathf.RoundToInt(stats.pDmg*stats.twohandedSkill*twohandedSkill)*unequip;
                    
                    //apply char strength a second time for stormtrooper skill if physical damage
                    if(stats.stormTrooperSkill>0 && weaponStats.dmgType==0)
                    {
                        stats.dmg += stats.pDmg;
                    }
                }
                if(stats.armorbreakerSkill==1)        //only 1 or 0
                {
                    stats.armorDestruction += 100*unequip;
                }
            break;
            case 23:
                if(stats.maceSkill==-1) //remove weapon stats
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.mAcc += SkillSystem.Instance.maceEffect*stats.maceSkill*unequip;
                    stats.mEva += SkillSystem.Instance.maceEffect*stats.maceSkill*unequip;
                }          
                      
                if(stats.twohandedSkill==-1) 
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    float twohandedSkill = SkillSystem.Instance.twoHandedEffect/100f;
                    stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.mEva += Mathf.RoundToInt(weaponStats.mEva*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.rEva += Mathf.RoundToInt(weaponStats.rEva*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.dmg += Mathf.RoundToInt(stats.pDmg*stats.twohandedSkill*twohandedSkill)*unequip;
                    //apply char strength a second time for stormtrooper skill if physical damage
                    if(stats.stormTrooperSkill>0 && weaponStats.dmgType==0)
                    {
                        stats.dmg += stats.pDmg;
                    }
                }
            break;
            case 24:
                if(stats.spearSkill==-1) 
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.mAcc += SkillSystem.Instance.spearEffect*stats.spearSkill*unequip;
                    stats.mEva += SkillSystem.Instance.spearEffect*stats.spearSkill*unequip;
                }          
                      
                if(stats.twohandedSkill==-1) 
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    float twohandedSkill = SkillSystem.Instance.twoHandedEffect/100f;
                    stats.mAcc += Mathf.RoundToInt(weaponStats.mAcc*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.mEva += Mathf.RoundToInt(weaponStats.mEva*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.rAcc += Mathf.RoundToInt(weaponStats.rAcc*stats.twohandedSkill*twohandedSkill)*unequip;
                    stats.rEva += Mathf.RoundToInt(weaponStats.rEva*stats.twohandedSkill*twohandedSkill)*unequip;
                    if(weaponStats.dmgType==0)
                    {
                        stats.dmg += Mathf.RoundToInt(stats.pDmg*stats.twohandedSkill*twohandedSkill)*unequip;
                    }
                    //apply char strength a second time for stormtrooper skill if physical damage
                    if(stats.stormTrooperSkill>0 && weaponStats.dmgType==0)
                    {
                        stats.dmg += stats.pDmg;
                    }
                }
            break;
            case 25:
                if(stats.crossbowSkill==-1) 
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.rAcc += SkillSystem.Instance.crossbowEffect*stats.crossbowSkill*unequip;

                    if(stats.hawkeyeSkill>0)           
                    {
                        stats.range += Mathf.RoundToInt(stats.range*stats.hawkeyeSkill*SkillSystem.Instance.hawkeyeEffect/100f)*unequip; //make sure it is rounded with smallest error
        
                    }
                }

                if(stats.tinkerSkill >0)
                {
                    stats.armorPierce += SkillSystem.Instance.tinkerEffect;
                }
            break;
            case 26:
                if(stats.bowSkill==-1) 
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.rAcc += SkillSystem.Instance.bowEffect*stats.bowSkill*unequip;

                    if(stats.hawkeyeSkill>0)           
                    {
                        if(unequip==1)//only changed when equipped
                        {
                            stats.range += Mathf.RoundToInt(stats.range*stats.hawkeyeSkill*SkillSystem.Instance.hawkeyeEffect/100f); //make sure it is rounded with smallest error
                        }
                    }

                    if(stats.quickshotSkill>0)
                    {
                        if(unequip==1)
                        {
                            float quickshot = (100-SkillSystem.Instance.quickshotEffect)/100f;
                            stats.attackRate *= Mathf.Pow(quickshot, stats.quickshotSkill);
                        }
                    }
                }
            break;
            case 30:
            case 31:
            case 32:
            case 33:
            case 34:
            case 35:
            case 36:
                if(stats.staffSkill==-1) 
                {
                    stats.mAcc -= weaponStats.mAcc*unequip;
                    stats.mEva -= weaponStats.mEva*unequip;
                    stats.rAcc -= weaponStats.rAcc*unequip;
                    stats.rEva -= weaponStats.rEva*unequip;
                    stats.baseAttackCost *=2;
                }
                else
                {
                    stats.rAcc += SkillSystem.Instance.staffEffect*stats.staffSkill*unequip;
                    stats.mEva += SkillSystem.Instance.staffEffect*stats.staffSkill*unequip;

                    if(stats.wisdomSkill >0)
                    {
                        if(unequip==1)
                        {
                            float wisdom = SkillSystem.Instance.wisdomEffect/100f;
                            stats.baseAttackCost = Mathf.RoundToInt(stats.baseAttackCost*(1-wisdom*stats.wisdomSkill)); //reduce base attack by 20% for each wisdom point
                        }
                    }
                    //apply aoe effect if skill
                    if(stats.magicDiffusionSkill >0)
                    {
                        stats.aoeAttack += SkillSystem.Instance.magicDiffusionEffect*stats.magicDiffusionSkill*unequip;
                    }
                }
            break;
        }
    }
        //if weapon lvl 0 set bonus to 0 and double attack cost
        //don't forget to requip weapon every time a weapon skill is leveled in the skill system.
    public void ActivateSkill(Fighter stats, int weaponType, int whichSkill, int onOff)    //get the weapon type, the skill 1,2 or 3 to toggle and if it shall be on or off
    {
        switch (weaponType)
        {
        case 1: //sword
            switch(whichSkill)
            {
                case 1:
                    stats.armorBypass += swordBypassSkill* onOff; //sword armorbypass
                break;
                case 2:
                    stats.attackRate /= Mathf.Pow(swordSpeedSkill, onOff);
                break;

            }
            break;
        case 2: //axe
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg * Mathf.Pow(axeDmgSkill, onOff));
                break;
                case 2:
                    stats.armorDestruction += axeArmorDestructionSkill* onOff; //increase attack speed by decreasing the attack interval
                break;
            }
            break;
        case 3: //mace 
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg * Mathf.Pow(maceDmgSkill, onOff));
                break;
                case 2:
                    stats.bashAttack += maceBashSkill* onOff; //increase attack speed by decreasing the attack interval
                break;
            }
            break;

        case 4: //spear
            switch(whichSkill)
            {
                case 1:
                    if(stats.holdTheLineSkill==1)
                    {    
                        stats.mEva = Mathf.RoundToInt(stats.mEva*Mathf.Pow(spearEvasionSkill, onOff));
                    }
                    else
                    {
                        stats.mEva += stats.reachAdvantageBonus;
                    }
                break;
                case 2:
                    stats.pushAttack += stats.range*onOff;
                break;

            }
            break;

        case 5: //fists + buckler
            switch(whichSkill)
            {
                case 1:
                break;
                case 2:
                break;
                case 3: 
                    stats.bashAttack += shieldBashSkill* onOff; //increase attack speed by decreasing the attack interval
                break;

            }
        break;
        case 6: //sword + buckler
            switch(whichSkill)
            {
                case 1:
                    stats.armorBypass += swordBypassSkill* onOff; //sword armorbypass
                break;
                case 2:
                    stats.attackRate /= Mathf.Pow(swordSpeedSkill, onOff);
                break;
                case 3: 
                    stats.bashAttack += shieldBashSkill* onOff; //increase attack speed by decreasing the attack interval
                break;

            }
            break;
        case 7: //axe + buckler
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg * Mathf.Pow(axeDmgSkill, onOff));
                break;
                case 2:
                    stats.armorDestruction += axeArmorDestructionSkill* onOff; //increase attack speed by decreasing the attack interval
                break;
                case 3: 
                    stats.bashAttack += shieldBashSkill* onOff; //increase attack speed by decreasing the attack interval
                break;

            }
            break;
        case 8: //mace + buckler
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg * Mathf.Pow(maceDmgSkill, onOff));
                break;
                case 2:
                    stats.bashAttack += shieldBashSkill* onOff; //increase attack speed by decreasing the attack interval
                break;
                case 3: 
                    stats.bashAttack += shieldBashSkill* onOff; //increase attack speed by decreasing the attack interval
                break;

            }
            break;

        case 9: //spear + buckler
            switch(whichSkill)
            {
                case 1:
                    if(stats.holdTheLineSkill==1)
                    {    
                        stats.mEva = Mathf.RoundToInt(stats.mEva*Mathf.Pow(spearEvasionSkill, onOff));
                    }
                    else
                    {
                        stats.mEva += stats.reachAdvantageBonus;
                    }
                break;
                case 2:
                    stats.pushAttack += stats.range*onOff;
                break;
                case 3: 
                    stats.bashAttack += shieldBashSkill* onOff; //increase attack speed by decreasing the attack interval
                break;

            }
            break;

        case 10: //fists + shield
            switch(whichSkill)
            {
                case 3: 
                    stats.mEva += stats.hunkerdownBonus * onOff;
                    stats.rEva += stats.hunkerdownBonus * onOff;
                    if(GameManager.Instance.cityMap)
                    {
                        stats.speed /= Mathf.Pow(2,onOff);
                    }
                    else
                    {
                        stats.speed /= Mathf.Pow(2,onOff);
                        stats.myAgent.speed /= Mathf.Pow(2,onOff);
                    }
                break;

            }
            break;
        case 11: //sword + shield
            switch(whichSkill)
            {
                case 1:
                    stats.armorBypass += swordBypassSkill* onOff; //sword armorbypass
                break;
                case 2:
                    stats.attackRate /= Mathf.Pow(swordSpeedSkill, onOff);
                break;
                case 3: 
                    stats.mEva += stats.hunkerdownBonus * onOff;
                    stats.rEva += stats.hunkerdownBonus * onOff;
                    if(GameManager.Instance.cityMap)
                    {
                        stats.speed /= Mathf.Pow(2,onOff);
                    }
                    else
                    {
                        stats.speed /= Mathf.Pow(2,onOff);
                        stats.myAgent.speed /= Mathf.Pow(2,onOff);
                    }
                break;

            }
            break;
        case 12: //axe + shield
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg * Mathf.Pow(axeDmgSkill, onOff));
                break;
                case 2:
                    stats.armorDestruction += axeArmorDestructionSkill* onOff; //increase attack speed by decreasing the attack interval
                break;
                case 3: 
                    stats.mEva += stats.hunkerdownBonus * onOff;
                    stats.rEva += stats.hunkerdownBonus * onOff;
                    if(GameManager.Instance.cityMap)
                    {
                        stats.speed /= Mathf.Pow(2,onOff);
                    }
                    else
                    {
                        stats.speed /= Mathf.Pow(2,onOff);
                        stats.myAgent.speed /= Mathf.Pow(2,onOff);
                    }
                break;

            }
            break;
        case 13: //mace + shield
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg * Mathf.Pow(maceDmgSkill, onOff));
                break;
                case 2:
                    stats.bashAttack += maceBashSkill* onOff; //increase attack speed by decreasing the attack interval
                break;
                case 3: 
                    stats.mEva += stats.hunkerdownBonus * onOff;
                    stats.rEva += stats.hunkerdownBonus * onOff;
                    if(GameManager.Instance.cityMap)
                    {
                        stats.speed /= Mathf.Pow(2,onOff);
                    }
                    else
                    {
                        stats.speed /= Mathf.Pow(2,onOff);
                        stats.myAgent.speed /= Mathf.Pow(2,onOff);
                    }
                break;

            }
            break;

        case 14: //spear + shield
            switch(whichSkill)
            {
                case 1:
                    if(stats.holdTheLineSkill==1)
                    {    
                        stats.mEva = Mathf.RoundToInt(stats.mEva*Mathf.Pow(spearEvasionSkill, onOff));
                    }
                    else
                    {
                        stats.mEva += stats.reachAdvantageBonus;
                    }
                break;
                case 2:
                    stats.pushAttack += stats.range*onOff;
                break;
                case 3: 
                    stats.mEva += stats.hunkerdownBonus * onOff;
                    stats.rEva += stats.hunkerdownBonus * onOff;
                    if(GameManager.Instance.cityMap)
                    {
                        stats.speed /= Mathf.Pow(2,onOff);
                    }
                    else
                    {
                        stats.speed /= Mathf.Pow(2,onOff);
                        stats.myAgent.speed /= Mathf.Pow(2,onOff);
                    }
                break;

            }
            break;
        case 15: //swordx2
            switch(whichSkill)
            {
                case 1:
                    stats.armorBypass += swordBypassSkill* onOff; //sword armorbypass
                break;
                case 2:
                    stats.attackRate /= Mathf.Pow(swordSpeedSkill, onOff);
                break;

            }
            break;
        case 16: //axe x2
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg * Mathf.Pow(axeDmgSkill, onOff));
                break;
                case 2:
                    stats.armorDestruction += axeArmorDestructionSkill* onOff; //increase attack speed by decreasing the attack interval
                break;
            }
            break;
        case 17: //mace x2
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg * Mathf.Pow(maceDmgSkill, onOff));
                break;
                case 2:
                    stats.bashAttack += maceBashSkill* onOff; //increase attack speed by decreasing the attack interval
                break;
            }
            break;

        case 18: //spearx2
            switch(whichSkill)
            {
                case 1:
                    if(stats.holdTheLineSkill==1)
                    {    
                        stats.mEva = Mathf.RoundToInt(stats.mEva*Mathf.Pow(spearEvasionSkill, onOff));
                    }
                    else
                    {
                        stats.mEva += stats.reachAdvantageBonus;
                    }
                break;
                case 2:
                    stats.pushAttack += stats.range*onOff;
                break;

            }
            break;
        
        case 21: //2h sword
            switch(whichSkill)
            {
                case 1:                    
                    stats.dmg = Mathf.RoundToInt(stats.dmg*Mathf.Pow(twoHandedDmgSkill, onOff));
                break;
                case 2:
                    stats.attackRate /= Mathf.Pow(swordSpeedSkill, onOff);
                break;
                case 3:
                    stats.aoeAttack += stats.range *onOff;
                break;

            }
            break; 

        case 22: //2h axe
            switch(whichSkill)
            {
                case 1:                    
                    stats.dmg = Mathf.RoundToInt(stats.dmg*Mathf.Pow(twoHandedDmgSkill, onOff));
                break;
                case 2:
                    stats.armorDestruction += axeArmorDestructionSkill*onOff;
                break;
                case 3:
                    stats.aoeAttack += stats.range *onOff;
                break;

            }
            break;

        case 23: //2h mace
            switch(whichSkill)
            {
                case 1:                    
                    stats.dmg = Mathf.RoundToInt(stats.dmg*Mathf.Pow(twoHandedDmgSkill, onOff));
                break;
                case 2:
                    stats.bashAttack += maceBashSkill*onOff;
                break;
                case 3:
                    stats.aoeAttack += stats.range *onOff;
                break;
            }
            break;
            

        case 24: //2h spear
            switch(whichSkill)
            {
                case 1:
                    if(stats.holdTheLineSkill==1)
                    {    
                        stats.mEva = Mathf.RoundToInt(stats.mEva*Mathf.Pow(spearEvasionSkill, onOff));
                    }
                    else
                    {
                        stats.mEva += stats.reachAdvantageBonus;
                    }
                break;
                case 2:
                    stats.pushAttack += stats.range*onOff;
                break;
                case 3:
                    stats.aoeAttack += stats.range *onOff;
                break;

            }
            break;
        
        
        case 25: //crossbow
            switch(whichSkill)
            {
                case 1:
                    stats.rAcc = Mathf.RoundToInt(stats.rAcc*Mathf.Pow(crossbowAimSkill, onOff)); 
                break;
                case 2:
                //scattershot increase acc, increase radius, reduce range
                    stats.rAcc = Mathf.RoundToInt(stats.rAcc*Mathf.Pow(2, onOff)); 
                    stats.aoeAttack += Mathf.RoundToInt(Mathf.Pow(crossbowScatterShot, onOff)); 
                    stats.range = Mathf.RoundToInt(stats.range*Mathf.Pow(2, -onOff)); 
                break;
                case 3:
                //stunshot add stun, reduce range
                    stats.bashAttack += crossbowStun*onOff;
                    stats.range = Mathf.RoundToInt(stats.range*Mathf.Pow(2, -onOff)); 
                break;
            }
            break;
        case 26: //bow
            switch(whichSkill)
            {
                case 1:
                    stats.rAcc = Mathf.RoundToInt(stats.rAcc*Mathf.Pow(bowAimSkill, onOff)); 
                break;
                case 2:
                //quickshot
                    stats.attackRate /= Mathf.Pow(bowQuickshot, onOff); 
                break;
            }
            break;
        
        case 30: //arcane
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg*Mathf.Pow(magicDmgSkill, onOff));
                break;
                case 2:
                    stats.aoeAttack += magicAoeRadiusSkill*onOff;
                    //wizard class
                    if(stats.promotionClassList[14]!=2)
                    {
                        stats.dmg = Mathf.RoundToInt(stats.dmg/Mathf.Pow(magicAoeDmgReductionSkill, onOff));
                    }
                break;
            }
            break;
        
        case 31: //fire
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg*Mathf.Pow(magicDmgSkill, onOff));
                break;
                case 2:
                    stats.aoeAttack += magicAoeRadiusSkill*onOff;
                    //wizard class
                    if(stats.promotionClassList[14]!=2)
                    {
                        stats.dmg = Mathf.RoundToInt(stats.dmg/Mathf.Pow(magicAoeDmgReductionSkill, onOff));
                    }
                break;
                case 3:
                    stats.target = null;
                    if(onOff==1)
                    {
                        stats.supportAttack = true;
                        stats.attackCost /=2;
                    }
                    else
                    {
                        stats.supportAttack = false;
                        stats.attackCost *=2;
                    }
                break;
            }
            break;
        case 32: //water
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg*Mathf.Pow(magicDmgSkill, onOff));
                break;
                case 2:
                    stats.aoeAttack += magicAoeRadiusSkill*onOff;
                    //wizard class
                    if(stats.promotionClassList[14]!=2)
                    {
                        stats.dmg = Mathf.RoundToInt(stats.dmg/Mathf.Pow(magicAoeDmgReductionSkill, onOff));
                    }
                break;
                case 3:
                    stats.target = null;
                    if(onOff==1)
                    {
                        stats.supportAttack = true;
                        stats.attackCost /=2;
                    }
                    else
                    {
                        stats.supportAttack = false;
                        stats.attackCost *=2;
                    }
                break;
            }
            break;
        case 33: //earth
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg*Mathf.Pow(magicDmgSkill, onOff));
                break;
                case 2:
                    stats.aoeAttack += magicAoeRadiusSkill*onOff;
                    //wizard class
                    if(stats.promotionClassList[14]!=2)
                    {
                        stats.dmg = Mathf.RoundToInt(stats.dmg/Mathf.Pow(magicAoeDmgReductionSkill, onOff));
                    }
                break;
                case 3:
                    stats.target = null;
                    if(onOff==1)
                    {
                        stats.supportAttack = true;
                        stats.attackCost /=2;
                    }
                    else
                    {
                        stats.supportAttack = false;
                        stats.attackCost *=2;
                    }
                break;
            }
            break;
        case 34: //air
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg*Mathf.Pow(magicDmgSkill, onOff));
                break;
                case 2:
                    stats.aoeAttack += magicAoeRadiusSkill*onOff;
                    //wizard class
                    if(stats.promotionClassList[14]!=2)
                    {
                        stats.dmg = Mathf.RoundToInt(stats.dmg/Mathf.Pow(magicAoeDmgReductionSkill, onOff));
                    }
                break;
                case 3:
                    stats.target = null;
                    if(onOff==1)
                    {
                        stats.supportAttack = true;
                        stats.attackCost /=2;
                    }
                    else
                    {
                        stats.supportAttack = false;
                        stats.attackCost *=2;
                    }
                break;
            }
            break;
        case 35: //light
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg*Mathf.Pow(magicDmgSkill, onOff));
                break;
                case 2:
                    stats.aoeAttack += magicAoeRadiusSkill*onOff;
                    //wizard class
                    if(stats.promotionClassList[14]!=2)
                    {
                        stats.dmg = Mathf.RoundToInt(stats.dmg/Mathf.Pow(magicAoeDmgReductionSkill, onOff));
                    }
                break;
                case 3:
                    stats.target = null;
                    if(onOff==1)
                    {
                        stats.supportAttack = true;
                        stats.attackCost /=2;
                    }
                    else
                    {
                        stats.supportAttack = false;
                        stats.attackCost *=2;
                    }
                break;
            }
            break;        
        case 36: //shadow
            switch(whichSkill)
            {
                case 1:
                    stats.dmg = Mathf.RoundToInt(stats.dmg*Mathf.Pow(magicDmgSkill, onOff));
                break;
                case 2:
                    stats.aoeAttack += magicAoeRadiusSkill*onOff;
                    //wizard class
                    if(stats.promotionClassList[14]!=2)
                    {
                        stats.dmg = Mathf.RoundToInt(stats.dmg/Mathf.Pow(magicAoeDmgReductionSkill, onOff));
                    }
                break;
                case 3:
                    stats.target = null;
                    if(onOff==1)
                    {
                        stats.supportAttack = true;
                        stats.attackCost /=2;
                    }
                    else
                    {
                        stats.supportAttack = false;
                        stats.attackCost *=2;
                    }
                break;
            }
            break;
        }
    }
}
/* } */ 
