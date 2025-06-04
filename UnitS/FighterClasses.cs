
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FighterClasses : MonoBehaviour
{
    
    //list of the predefined images for the classes
    public List<Sprite> classDefaultImageList;
    public List<Sprite> promotedClassDefaultImageList;
    public List<TMP_Dropdown.OptionData> DropDownList;/* 
    public List<string> promotedClassesNameList = new List<string>()
    { "Duelist", "Demolisher", "Painbringer", "Hellebardier", "Sharpshooter", "", "Mage", "Priest", "Whitcher", "Monk",
    "Swordmaster", "Spartan", "Master Shooter", "Wizard", "", "", "", "", "", "",
    "Warrior", "Wildling"}; */
    private string classDescription;
    public TMP_Dropdown DropdownButton;
    public GameObject PromotionDropdownMenu;
    public Image PromotionClassImage;
    private int playerChoice =-1;
    private int sumOfavailableClasses;
    public List<int> PromotionClassBoniList;

/*  0- hero
    1- swordsman
    2- thug
    3- brawler
    4- guard
    5- soldier
    6- hunter
    7- magicians apprentice 
    

*/
/*  Tier1 - experts - 1 point in weapon specialisation +2 weaponskillpoints
    1- duelist  +mAcc/mDef
    2- demolisher   +pdmg
    3- Painbringer  +armorPierce
    4- hellebardier      +spearpush
    5- Sharpshooter +pdmg
    
    7- mage         +mdmg(2magicflow) 
    8- priest       +hp
    9- whitcher     +sta
    10- monk        +acc(1wisdom)
    
    Tier2 - veterans
    11- swordmaster     +mAcc/mDef      -3points in specialisation
    12- spartan
    13- MasterShooter   +pdmg+sta       -3points in specialisation
    13- wizard          +mdmg           -2staff, 2magicflow, 2wisdom, 2elementalist

    Tier1 - auras
    21- warrior     HP      -2melee, 2def    
    22- wildling    sta     -2melee, dual wielding
    23- Destroyer pdmg    -2melee, two-handed
    24- captain     acc/eva -1ranged,1melee,1def
    25- demonhunter mind-res     -2ranged, 2magic
    26- paladin     move-res     -2melee, 2magic
    27- savior      HP+
    28- sorcerer    Sta+
    29- sage        acc+     -2staff, 3wisdom

    Tier2 - strong auras
    31- warlord     HP+      -2melee, specialisation, 4def
    32- berserker   sta+     -2melee, 3dualwielding, 1def
    33- Landsknecht   pdmg+    -2melee, 3two-handed, 1def
    34- general     acc/eva+ -2ranged, 2melee, 2def, quickdrawskill */

    private static FighterClasses _instance;
    public static FighterClasses Instance { get { return _instance; } }
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
    // Start is called before the first frame update
    void Start()
    {

        classDefaultImageList.Add(Resources.Load<Sprite>("Faces/hero"));
        classDefaultImageList.Add(Resources.Load<Sprite>("Img/AvatarIconsMegapack/CharacterIcons/Characters_WithBackground/Human_32_scout"));
        classDefaultImageList.Add(Resources.Load<Sprite>("Img/AvatarIconsMegapack/CharacterIcons/Characters_WithBackground/Human_28_thug"));
        classDefaultImageList.Add(Resources.Load<Sprite>("Img/AvatarIconsMegapack/CharacterIcons/Characters_WithBackground/Human_33_warrior"));
        classDefaultImageList.Add(Resources.Load<Sprite>("Img/AvatarIconsMegapack/CharacterIcons/Characters_WithBackground/Guard"));
        classDefaultImageList.Add(Resources.Load<Sprite>("Img/AvatarIconsMegapack/CharacterIcons/Characters_WithBackground/Human_01_archer"));
        classDefaultImageList.Add(Resources.Load<Sprite>("Img/AvatarIconsMegapack/CharacterIcons/Characters_WithBackground/Human_04 1"));
        classDefaultImageList.Add(Resources.Load<Sprite>("Img/AvatarIconsMegapack/CharacterIcons/Characters_WithBackground/Magicians_Apprentice"));
        
    }

    public Sprite GetClassImage(int whichClass)
    {
        return classDefaultImageList[whichClass];
    }    
    public Sprite GetPromotedClassImage(int whichClass)
    {
        return promotedClassDefaultImageList[whichClass];
    }

    public string GetClassDescription(int whichClass)
    {
        //string classDescription;
        switch(whichClass)
        {
            case 0:
                classDescription = "<color=#DAA520>Hero</color> \nThe hero is not limited in his attribute distribution nor his available skills!";
            break;
            case 1:
                classDescription = "<color=#DAA520>Swordsman</color> \nFocused on melee accuracy and evasion, swordsman tend to be very agile fighters!";
            break;
            case 2:
                classDescription = "<color=#DAA520>Thug</color> \nFocused on physical damage and melee accuracy, thugs try to crush their opponents with an axe!";
            break;
            case 3:
                classDescription = "<color=#DAA520>Brawler</color> \nFocused on physical damage and hit points, brawlers can stun their opponents with maces!";
            break;
            case 4:
                classDescription = "<color=#DAA520>Guard</color> \nFocused on evasion, guards are excelling in surviving fights for a long time with their spears!";
            break;
            case 5:
                classDescription = "<color=#DAA520>Soldier</color> \nFocused on physical damage and ranged accuracy, soldiers can be pretty lethal with their crossbows!";
            break;
            case 6:
                classDescription = "<color=#DAA520>Hunter</color> \nFocused on stamina and ranged accuracy, hunters are very athletic and can deal consistent heavy damage from afar!";
            break;
            case 7:
                classDescription = "<color=#DAA520>Apprentice</color> \nFocused on magic damage and ranged accuracy, apprentices can support the troop in various ways, given they are experienced enough!";
            break;

        }
        return classDescription;
    }


    public string GetPromotedClassDescription(int whichClass)
    {
        switch(whichClass)
        {
            case 0:
                classDescription = "<color=#DAA520>Promotion</color> \nIf your character has access to it, he can choose an advanced class here!";
            break;
            case 1:
                classDescription = "<color=#DAA520>Duelist</color> \nAspiring to win noble tournaments, duelists have improved melee accuracy and evasion ("+PromotionClassBoniList[1]+")!";
            break;
            case 2:
                classDescription = "<color=#DAA520>Demolisher</color> \nFocusing completely on destruction, the demolisher excels in physical damage ("+PromotionClassBoniList[2]+")!";
            break;
            case 3:
                classDescription = "<color=#DAA520>Painbringer</color> \nBruteforcing through every wall the Painbringer has increased armor piercing attacks ("+PromotionClassBoniList[3]+")!";
            break;
            case 4:
                classDescription = "<color=#DAA520>Hellebardier</color> \nThis character became an expert at keeping melee enemies at distance increasing melee evasion ("+PromotionClassBoniList[4]+")!";
            break;
            case 5:
                classDescription = "<color=#DAA520>Sharpshooter</color> \nAs an expert at long range attacks the sharpshooter has increased ranged Accuracy ("+PromotionClassBoniList[5]+")!";
            break;
            case 7:
                classDescription = "<color=#DAA520>Mage</color> \nKnowing the magic essence the mage deals even more magical damage ("+PromotionClassBoniList[7]+")!";
            break;
            case 8:
                classDescription = "<color=#DAA520>Priest</color> \nGaining strength from supporting his allies the priest has improved hit points ("+PromotionClassBoniList[8]+")!";
            break;
            case 9:
                classDescription = "<color=#DAA520>Whitcher</color> \nWeakening his enemies gives the whitcher a second wave, improving his stamina ("+PromotionClassBoniList[9]+")!";
            break;   
            case 10:
                classDescription = "<color=#DAA520>Monk</color> \nWaiting for the right opportunity the monk gains increased accuracy ("+PromotionClassBoniList[10]+")!";
            break; 
            //Tier 2 - veterans
            case 11:
                classDescription = "<color=#DAA520>Swordmaster</color> \nDancing over the battlefield like a feather, the swordmaster is the ultimate melee weapon. Increased melee accuracy and evasion ("+PromotionClassBoniList[11]+")!";
            break;     
            case 12:
                classDescription = "<color=#DAA520>Spartan</color> \nHolding the line, the spartan gets even more effective with shields ("+PromotionClassBoniList[12]+"%)!";
            break;  
            case 13:
                classDescription = "<color=#DAA520>Master Shooter</color> \nMastering the bow, this character has become a persistent ranged damage machine (Sta+"+PromotionClassBoniList[13]*10+", Str"+PromotionClassBoniList[13]+")!";
            break;  
            case 14:
                classDescription = "<color=#DAA520>Wizard</color> \nThe Wizard is the ultimate machine of mass destruction, having no penalty to area damage!";
            break;  
            //Tier 1 - Auras           
            case 21:
                classDescription = "<color=#DAA520>Warrior</color> \nGetting used to battle, the warrior has his companions' backs! Hp-Aura ("+PromotionClassBoniList[21]+")!";
            break;           
            case 22:
                classDescription = "<color=#DAA520>Wildling</color> \nCharging into battle, the wildling inspires his companions to keep on fighting! Sta-Aura ("+PromotionClassBoniList[22]+")!";
            break;           
            case 23:
                classDescription = "<color=#DAA520>Destroyer</color> \nThis character shows his companions what destruction should look like! Strength-Aura ("+PromotionClassBoniList[23]+")!";
            break;          
            case 24:
                classDescription = "<color=#DAA520>Captain</color> \nKnowing the battlefield, the captain advices his allies into good formation! Evasion-Aura ("+PromotionClassBoniList[24]+")!";
            break;            
            case 25:
                classDescription = "<color=#DAA520>Demon Hunter</color> \nThis character learned to anticipate his enemy! Status-Resistance-Aura ("+PromotionClassBoniList[25]+")!";
            break;           
            case 26:
                classDescription = "<color=#DAA520>Paladin</color> \nNever Waver! Slow-Resistance-Aura ("+PromotionClassBoniList[26]+")!";
            break;          
            case 27:
                classDescription = "<color=#DAA520>Savior</color> \nOne-for all and all for one! Hp-Aura ("+PromotionClassBoniList[27]+")!";
            break;     
            case 28:
                classDescription = "<color=#DAA520>Sorcerer</color> \nDon't ease up on the attack! Sta-Aura ("+PromotionClassBoniList[28]+")!";
            break;     
            case 29:
                classDescription = "<color=#DAA520>Sage</color> \nThis character emits such patience, that his companion can feel it as well! Acc-Aura ("+PromotionClassBoniList[29]+")!";
            break;   
            //Tier 2 Auras  
            case 31:
                classDescription = "<color=#DAA520>Warlord</color> \nBattle-hardened, this character emits an aura of strengths that his allies don't want to dissapoint! HP-Aura ("+PromotionClassBoniList[31]+")!";
            break; 
            case 32:
                classDescription = "<color=#DAA520>Berserker</color> \nThis battle machine, never stops attacking! Sta-Aura ("+PromotionClassBoniList[32]+")!";
            break; 
            case 33:
                classDescription = "<color=#DAA520>Landsknecht</color> \nA mercenary that doesn't only look intimidating, but also knows his craft! Strength-Aura ("+PromotionClassBoniList[33]+")!";
            break; 
            case 34:
                classDescription = "<color=#DAA520>General</color> \nKnowledge of all crafts of battle makes this character a great supervisor in the midst of the battlefield! Evasion-Aura ("+PromotionClassBoniList[34]+")!";
            break; 

        }
        return classDescription;
    }

    public void GetPromotedClassBoni(Fighter stats, int whichClass, int onOff)
    {
        bool auraStacking = false;
        stats.promotionClassList[whichClass] += onOff;
        switch(whichClass)
        {
            case 0:
            break;
            //duelist
            case 1:
                stats.mAcc += PromotionClassBoniList[1]*onOff;
                stats.mEva += PromotionClassBoniList[1]*onOff;
            break;
            //demolisher
            case 2:
                stats.pDmg += PromotionClassBoniList[2]*onOff;
            break;
            //painbringer
            case 3:
                stats.armorPierce += PromotionClassBoniList[3]*onOff;
            break;
            //pikemen
            case 4:
                stats.mEva += PromotionClassBoniList[4]*onOff;
            break;
            case 5:
            //sharpshooter
                stats.rAcc += PromotionClassBoniList[5]*onOff;
            break;
            case 7:
            //mage
                stats.mDmg += PromotionClassBoniList[7]*onOff;
            break;
            case 8:
            //priest
                stats.maxHP += PromotionClassBoniList[8]*onOff;
                stats.currentHP += PromotionClassBoniList[8]*onOff;
            break;
            case 9:
            //whitcher
                stats.maxStamina += PromotionClassBoniList[9]*onOff;
                stats.currentStamina += PromotionClassBoniList[9]*onOff;
            break;   
            case 10:
            //monk
                stats.mAcc+= PromotionClassBoniList[10]*onOff;
                stats.rAcc+= PromotionClassBoniList[10]*onOff;
            break; 
            //Tier 2 - veterans
            case 11:
            //swordmaster
                stats.mAcc+= PromotionClassBoniList[11]*onOff;
                stats.mEva+= PromotionClassBoniList[11]*onOff;
            break;       
            case 12:
            //spartan
                //shield bonuses in weapon equip script stats.promotionClassList[whichClass] += onOff;
                stats.promotionClassList[whichClass] -= onOff;
                if(stats.skill4Active)
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], -1);
                }
                else
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], -1);
                }
                stats.promotionClassList[whichClass] += onOff;          
                if(stats.skill4Active)
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], 1);
                }
                else
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], 1);
                }
            break;  
            case 13:
            //master shooter
                stats.pDmg += PromotionClassBoniList[13]*onOff;
                stats.maxStamina += PromotionClassBoniList[13]*10*onOff;
                stats.currentStamina += PromotionClassBoniList[13]*10*onOff;
            break;  
            case 14:
            //wizard
                //stats.mDmg += 40*onOff;
                //area dmg bonus in weapon equip script
            break;  
            //Tier 1 - Auras           
            case 21:
            //warrior                
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.maxHP += PromotionClassBoniList[21]*onOff;
                        unitStats.currentHP +=PromotionClassBoniList[21]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break;           
            case 22:
            //wildling             
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.maxStamina += PromotionClassBoniList[22]*onOff;
                        unitStats.currentStamina += PromotionClassBoniList[22]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break;           
            case 23:
            //destroyer         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.pDmg += PromotionClassBoniList[23]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break;          
            case 24:
            //captain         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.mEva += PromotionClassBoniList[24]*onOff;
                        unitStats.rEva += PromotionClassBoniList[24]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break;            
            case 25:
            //demonhunter         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.mindResistance += PromotionClassBoniList[25]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break;           
            case 26:
            //paladin         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.moveResistance += PromotionClassBoniList[25]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break;          
            case 27:
            //saviour         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.maxHP += PromotionClassBoniList[27]*onOff;
                        unitStats.currentHP += PromotionClassBoniList[27]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break;     
            case 28:
            //sorcerer         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.maxStamina += PromotionClassBoniList[28]*onOff;
                        unitStats.currentStamina += PromotionClassBoniList[28]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break;     
            case 29:
            //sage         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.mAcc += PromotionClassBoniList[29]*onOff;
                        unitStats.rAcc += PromotionClassBoniList[29]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break;   
            //Tier 2 Auras  
            case 31:
            //warlord         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.maxHP += PromotionClassBoniList[31]*onOff;
                        unitStats.currentHP += PromotionClassBoniList[31]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break; 
            case 32:
            //berserker         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.maxStamina += PromotionClassBoniList[32]*onOff;
                        unitStats.currentStamina += PromotionClassBoniList[32]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break; 
            case 33:
            //landsknecht         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.pDmg += PromotionClassBoniList[33]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break; 
            case 34:
            //general         
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    Fighter unitStats = unit.GetComponent<Fighter>();
                    if(unitStats != stats && whichClass == unitStats.promotedFighterClass)
                    {
                        auraStacking = true;
                    }
                }
                if(auraStacking)
                {
                    StartCoroutine(SkillSystem.Instance.AuraStackingWarning());
                }
                else
                {
                    foreach(GameObject unit in UnitSelections.Instance.unitList)
                    {
                        Fighter unitStats = unit.GetComponent<Fighter>();
                        unitStats.mEva += PromotionClassBoniList[34]*onOff;
                        unitStats.rEva += PromotionClassBoniList[34]*onOff;
                    }
                    //stats.ChangeAura(onOff);
                }
            break; 

        }
    }

    public void OpenPromotionChoice()
    {
        Fighter unitStats = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();
/*         int sum = 0;
        for(int i=1; i<unitStats.promotionClassList.Count; i++)
        {
            //if class available add its index to possible choices
            if(unitStats.promotionClassList[i]>0)
            {
                sum++;
            }
        } */
        if(sumOfavailableClasses>0)
        {
            PromotionDropdownMenu.SetActive(true);
            //PromotionClassImage.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void CancelPromotionChoice()
    {
        PromotionDropdownMenu.SetActive(false);
    }
    public void DropdownPromotionChoice()
    {
        playerChoice = DropdownButton.value;
        
        PromotionDropdownMenu.SetActive(false);
        SkillSystem.Instance.InitializeSkillScreen();
    }
    public void GetDropDownPromotion(Fighter stats)
    {
        DropdownButton.GetComponent<TMP_Dropdown>().ClearOptions();
        Fighter unitStats = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();

/*         //for the first time choice
        if(unitStats.promotedFighterClass==0)
        {  */
            DropdownButton.GetComponent<TMP_Dropdown>().options.Add(item: new TMP_Dropdown.OptionData(text: "Choose", image: null));
/*         } */
        //initiate a list of the possible choices
        List<int> dropdownChoicesList = new List<int>();
        //remove old class boni
        sumOfavailableClasses = 0;
        for(int i=1; i<unitStats.promotionClassList.Count; i++)
        {
            //if class available add its index to possible choices
            if(unitStats.promotionClassList[i]>0)
            {
                sumOfavailableClasses++;
                dropdownChoicesList.Add(i);
                DropdownButton.GetComponent<TMP_Dropdown>().options.Add(item: new TMP_Dropdown.OptionData(text: "", image: promotedClassDefaultImageList[i]));
                //Debug.Log(i);
                //remove previous class choice
                if(unitStats.promotionClassList[i]==2)
                {
                    if(playerChoice >-1)
                    {
                        GetPromotedClassBoni(unitStats, i, -1);
                    }
                }
            }
        }

        Debug.Log(playerChoice);
        //get the new promoted class from the unit class list at the place of the index of the available choices index
        if(playerChoice >-1)
        {
/*             if(unitStats.promotedFighterClass==0)
            { */
                GetPromotedClassBoni(unitStats, dropdownChoicesList[playerChoice-1], 1);
                unitStats.promotedFighterClass = dropdownChoicesList[playerChoice-1];
                //DropdownButton.SetValueWithoutNotify(playerChoice-1);
                DropdownButton.SetValueWithoutNotify(0);
/*             }
            else
            {
                GetPromotedClassBoni(unitStats, dropdownChoicesList[playerChoice], 1);
                unitStats.promotedFighterClass = dropdownChoicesList[playerChoice];
                DropdownButton.SetValueWithoutNotify(playerChoice);
            } */
        }

        //for available class, but no choice yet, change text from none to choose
        if(sumOfavailableClasses>0)
        {
            if(unitStats.promotedFighterClass==0)
            {
                PromotionClassImage.GetComponentInChildren<TMP_Text>().text = "Choose";
                PromotionClassImage.color = new Color32(0,0,0,255);
            }    
            else
            {               
                PromotionClassImage.GetComponentInChildren<TMP_Text>().text = "";
                PromotionClassImage.color = new Color32(255,255,255,255);
            }        
        }
        else
        {
            PromotionClassImage.GetComponentInChildren<TMP_Text>().text = "None";
            PromotionClassImage.color = new Color32(0,0,0,255);
        }
        
        //set button image
        //DropdownButton.GetComponent<Image>().sprite = GetPromotedClassImage(unitStats.promotedFighterClass);
        DropdownButton.RefreshShownValue();

        PromotionClassImage.sprite = DropdownButton.GetComponent<Image>().sprite;

        playerChoice = -1;
    }

    public void CheckForPromotions(Fighter stats)
    {
        int meleeRating = Math.Max(stats.swordSkill,0)+Math.Max(stats.axeSkill,0)+Math.Max(stats.maceSkill,0)+Math.Max(stats.spearSkill,0);
        int rangedRating = Math.Max(stats.bowSkill,0)+Math.Max(stats.crossbowSkill,0);
        int defenseRating = Math.Max(stats.shieldSkill,0)+Math.Max(stats.defenderSkill,0)+Math.Max(stats.acrobatSkill,0)+Math.Max(stats.armorhabituationSkill,0)+Math.Max(stats.armormasterSkill,0)+Math.Max(stats.shieldWallSkill,0);
        int magicRating = Math.Max(stats.magicFlowSkill,0)+Math.Max(stats.wisdomSkill,0)+Math.Max(stats.bufferSkill,0)+Math.Max(stats.debufferSkill,0)+Math.Max(stats.elementalistSkill,0)+Math.Max(stats.magicResistanceSkill,0)+Math.Max(stats.magicDiffusionSkill,0);
        Debug.Log(meleeRating);
        Debug.Log(rangedRating);
        Debug.Log(defenseRating);
        Debug.Log(magicRating);

        //duelist
        if(stats.promotionClassList[1]==0)
        {
            if(stats.swordSkill>1 && stats.sworddancerSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[1]++;
            }
        }
        //demolisher
        if(stats.promotionClassList[2]==0)
        {
            if(stats.axeSkill>1 && stats.armorbreakerSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[2]++;
            }
        }
        //painbringer
        if(stats.promotionClassList[3]==0)
        {
            if(stats.maceSkill>1 && stats.basherSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[3]++;
            }
        }
        //hellebard
        if(stats.promotionClassList[4]==0)
        {
            if(stats.spearSkill>1 && stats.holdTheLineSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[4]++;
            }
        }
        //sharpshooter
        if(stats.promotionClassList[5]==0)
        {
            if(rangedRating>1 && stats.hawkeyeSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[5]++;
            }
        }
        //mage
        if(stats.promotionClassList[7]==0)
        {
            if(stats.staffSkill>1&& stats.elementalistSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[7]++;
            }
        }
        //priest
        if(stats.promotionClassList[8]==0)
        {
            if(stats.staffSkill>1&& stats.bufferSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[8]++;
            }
        }        
        //whitcher
        if(stats.promotionClassList[9]==0)
        {
            if(stats.staffSkill>1&& stats.debufferSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[9]++;
            }
        }
        //monk
        if(stats.promotionClassList[10]==0)
        {
            if(stats.staffSkill>1&& stats.wisdomSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[10]++;
            }
        }
        //tier 2 specialisations
        //swordmaster
        if(stats.promotionClassList[11]==0)
        {
            if(stats.swordSkill>3 && stats.sworddancerSkill>2)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[11]++;
            }
        }
        //spartan
        if(stats.promotionClassList[12]==0)
        {
            if(stats.promotionClassList[4]>0 && stats.shieldWallSkill>1 && stats.armorhabituationSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[12]++;
            }
        }
        //mastershooter
        if(stats.promotionClassList[13]==0)
        {
            if(stats.bowSkill>3 && stats.quickshotSkill>2)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[13]++;
            }
        }
        //wizard
        if(stats.promotionClassList[14]==0)
        {
            if(stats.promotionClassList[7]>0 && stats.magicDiffusionSkill>2)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[14]++;
            }
        }

        //tier 1 auras
        //warrior
        if(stats.promotionClassList[21]==0)
        {
            if(meleeRating>1 && defenseRating>1)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[21]++;
            }
        }
        //wildling
        if(stats.promotionClassList[22]==0)
        {
            if(meleeRating>1 && stats.dualwieldingSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[22]++;
            }
        }
        //destroyer
        if(stats.promotionClassList[23]==0)
        {
            if(meleeRating>1 && stats.twohandedSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[23]++;
            }
        }
        //captain
        if(stats.promotionClassList[24]==0)
        {
            if(meleeRating>1 && stats.oneHandedSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[24]++;
            }
        }
        //demonhunter
        if(stats.promotionClassList[25]==0)
        {
            if(rangedRating>1 && magicRating>1)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[25]++;
            }
        }
        //paladin
        if(stats.promotionClassList[26]==0)
        {
            if(meleeRating>1 && magicRating>1)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[26]++;
            }
        }
        //saviour
        if(stats.promotionClassList[27]==0)
        {
            if(stats.staffSkill>1 && stats.bufferSkill>2)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[27]++;
            }
        }
        //sorcerer
        if(stats.promotionClassList[28]==0)
        {
            if(stats.staffSkill>1 && stats.debufferSkill>2)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[28]++;
            }
        }
        //sage
        if(stats.promotionClassList[29]==0)
        {
            if(stats.staffSkill>1 && stats.wisdomSkill>2)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[29]++;
            }
        }

        //tier 2 auras
        //warlord
        if(stats.promotionClassList[31]==0)
        {
            if(meleeRating>1 && (stats.sworddancerSkill>0 || stats.armorbreakerSkill>0 || stats.basherSkill>0 || stats.holdTheLineSkill>0) && defenseRating>3)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[31]++;
            }
        }
        //berserker
        if(stats.promotionClassList[32]==0)
        {
            if(meleeRating>1 && stats.dualwieldingSkill>2 && stats.whirlWindSkill >0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[32]++;
            }
        }
        //Landsknecht
        if(stats.promotionClassList[33]==0)
        {
            if(meleeRating>1 && stats.twohandedSkill>2 && stats.stormTrooperSkill >0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[33]++;
            }
        }
        //general
        if(stats.promotionClassList[34]==0)
        {
            if(meleeRating>1 && defenseRating>1 && stats.oneHandedSkill>0 && stats.conterSkill>0)
            {
                StartCoroutine(SkillSystem.Instance.NewPromotionClassAvailable());
                stats.promotionClassList[34]++;
            }
        }
    }
    /*  Tier1 - experts - 1 point in weapon specialisation +2 weaponskillpoints
    1- duelist  +mAcc/mDef
    2- demolisher   +pdmg
    3- Painbringer  +armorPierce
    4- hellebardier      +spearpush
    5- Sharpshooter +pdmg
    
    7- mage         +mdmg(2magicflow) 
    8- priest       +hp
    9- whitcher     +sta
    10- monk        +acc(1wisdom)
    
    Tier2 - veterans
    11- swordmaster     +mAcc/mDef      -3points in specialisation
    12- spartan
    13- MasterShooter   +pdmg+sta       -3points in specialisation
    13- wizard          +mdmg           -2staff, 2magicflow, 2wisdom, 2elementalist

    Tier1 - auras
    21- warrior     HP      -2melee, 2def    
    22- wildling    sta     -2melee, dual wielding
    23- Destroyer pdmg    -2melee, two-handed
    24- captain     acc/eva -1ranged,1melee,1def
    25- demonhunter mind-res     -2ranged, 2magic
    26- paladin     move-res     -2melee, 2magic
    27- savior      HP+
    28- sorcerer    Sta+
    29- sage        acc+     -2staff, 3wisdom

    Tier2 - strong auras
    31- warlord     HP+      -2melee, specialisation, 4def
    32- berserker   sta+     -2melee, 3dualwielding, 1def
    33- Landsknecht   pdmg+    -2melee, 3two-handed, 1def
    34- general     acc/eva+ -2ranged, 2melee, 2def, quickdrawskill */
}
