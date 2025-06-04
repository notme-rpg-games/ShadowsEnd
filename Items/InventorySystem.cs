using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoftKitty.MasterCharacterCreator;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class InventorySystem : MonoBehaviour
{
    public List<WeaponStats> itemObjectList;
    private int inventoryPage =0;
    private int inventoryPageLength = 30;
    public Texture emptySlotTexture;
    //public Texture unavailableSlotTexture;
    public AudioClip EquipSound;
    public AudioClip UnequipSound;
    public AudioClip warningSound;
    public AudioClip SellSound;
    public AudioClip repairSound;
    //public AudioSource camAudio;
    public GameObject WarningCanvas;
    public TMP_Text WarningText;
    public int inventoryCapacity;
    public List<TMP_Text> InspectItemTextList;
    public List<Image> InspectItemImageList;
    public RawImage InspectItemIcon;
    public GameObject InspectItemWindow;
    public string inspectSkillString1{get; private set;}
    public string inspectSkillString2{get; private set;}
    public string inspectSkillString3{get; private set;}
    
/*  0 empty
    1 weapon 
    2 offhand
    3 both
    4 offhand 1H weapon
    5 weapon2 
    6 offhand2
    7 both2
    8 offhand 1H weapon2
    11 helmet
    12 armor
    13 gauntlet
    14 pants
    15 boots
    */
    public List<WeaponStats> itemStatsList;
    //private EquipmentAppearance myItemAppearance;
    public List<GameObject> equipButtons;
    public List<GameObject> unitequipButtons;
    public List<WeaponStats> unitItemStatsList;
    
    //stats - attributes
    public List<TMP_Text> unitStatsList; 

    public GameObject RightClickOptionsCanvas;
    public GameObject RightClickOptionsButtons;
    public GameObject DropButton;
    public GameObject RepairButton;
    public GameObject OffhandButton;
    public Sprite normalButtonImage;
    public Sprite disabledButtonImage;
    public int option;
    public bool inputDone;
    private bool foundEmptySlot;

    public List<TMP_Text> hoverStatsList;
    public GameObject hoverStatsImage;
    public Image itemTypeImage;
    public Image dmgTypeImage;
    public List<TMP_Text> shopHoverStatsList;
    public GameObject shopHoverStatsImage;
    public Image shopItemTypeImage;
    public Image shopDmgTypeImage;
    public List<Sprite> dmgTypeImageList;
    public List<Sprite> itemTypeImageList;
    private Fighter stats;
    private WeaponStats saveWeaponStats;
    public List<Texture> unitequipButtonSprites;
    public List<TMP_Text> shopUnitStatsList;
    public List<TMP_Text> shopItemDifferencesList;
    public GameObject shopUnitStats;
    //private Fighter unit;
    public GameObject WeaponSetFrame1;
    public GameObject WeaponSetFrame2;
    public List<GameObject> pageButtonFramesList;
    public bool tutorialTips = true;

    private static InventorySystem _instance;
    public static InventorySystem Instance { get { return _instance; } }
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


    public void InitializeInventory()
    {
        LoadInventoryPage(inventoryPage);
        LoadUnitPage();
        if(UnitSelections.Instance.shopScreen)
        {
            //updates shopstats
            UpdateUnitStats();
        }
        else
        {
            //allows to increase attributes
            SkillSystem.Instance.UpdateUnitStats();
        }
                
        //dont open if initialized from shop        
        if(tutorialTips && UnitSelections.Instance.inventoryScreen)
        {
            GameManager.Instance.tipIndex= 1;
            GameManager.Instance.Tutorial();
            DisableInput();
        }
        else
        {
            EnableInput();
        }
        
    }

    void LoadUnitPage()
    {
        unitItemStatsList.Clear();

        stats = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();

        unitItemStatsList.AddRange(stats.itemList);

        for(int i=0; i<9; i++)
        if(unitItemStatsList[i]==null || string.IsNullOrEmpty(unitItemStatsList[i].itemName))
        {
            unitequipButtons[i].GetComponent<RawImage>().texture = unitequipButtonSprites[i];
            unitequipButtons[i].GetComponent<RawImage>().color = new Color32(255,255,255,100);
            unitequipButtons[i].GetComponent<EventTrigger>().enabled = false;
        }
        else
        {
            unitequipButtons[i].GetComponent<RawImage>().texture = unitItemStatsList[i].myItemIcon;
            unitequipButtons[i].GetComponent<RawImage>().color = new Color32(255,255,255,255);
            unitequipButtons[i].GetComponent<EventTrigger>().enabled = true;
        }

    }
    public void LoadInventoryPage(int page)
    {
        foreach(GameObject frame in pageButtonFramesList)
        {
            frame.SetActive(false);
        }
        pageButtonFramesList[page].SetActive(true);
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        inventoryPage = page;
        int buttonIndex = 0; //only 30 buttons always go from 0 - 29

        for(int i=0+page*inventoryPageLength; i<page*inventoryPageLength+inventoryPageLength; i++)
        {
            RawImage buttonImage = equipButtons[buttonIndex].GetComponentInChildren<RawImage>();
            //Debug.Log("initialize inventroy" +i);
            if(i>=inventoryCapacity)
            {
                //buttonImage.texture = unavailableSlotTexture;
                equipButtons[buttonIndex].SetActive(false);
            }
            else
            {
                equipButtons[buttonIndex].SetActive(true);

                
                if(itemStatsList[i] == null)
                {
                    //buttonImage.texture = emptySlotTexture;
                    buttonImage.color = new Color32(255,255,255,0);
                }
                else
                {
                    buttonImage.color = new Color32(255,255,255,255);
                    buttonImage.texture = itemStatsList[i].myItemIcon;
                }
            }
            buttonIndex++;
        }

    }

    //initiates equipping an inventory item to unit slot
    public void EquipItemAppearance(int buttonIndex)
    {
        int inventorySlotIndex = buttonIndex +inventoryPage*inventoryPageLength;
        if(itemStatsList[inventorySlotIndex] != null)
        {
            if(itemStatsList[inventorySlotIndex].stamina*(-1+Math.Max(stats.armorhabituationSkill,0)*0.2f) > stats.currentStamina)
            {
                StartCoroutine(NotEnoughStamina());
            }
            else
            {
                saveWeaponStats = itemStatsList[inventorySlotIndex];
                CheckUnitSlot(inventorySlotIndex);
            }
        }
    }

    //Checks if there is an empty inventory slot and if so it writes the unititem stats into that slot else it returns a warning
    void CheckUnitSlot(int inventorySlotIndex)
    {
        foundEmptySlot = true;
        bool dualwieldingCheck = true;
        //check which unit slot to add to
        int unitSlotIndex = 0;
        switch(saveWeaponStats.itemSlotType)
        {
            case 1:
                if(stats.skill4Active)
                {
                    unitSlotIndex = 2;
                }
                else
                {
                    unitSlotIndex = 0;
                }
            break;
            case 2:
                if(stats.skill4Active)
                {
                    //check if two-handed equipped
                    if(unitItemStatsList[2]==null || unitItemStatsList[2].itemSlotType==3)
                    {
                        UnequipUnitSlot(2);
                    }
                    unitSlotIndex = 3;
                }
                else
                {
                    //check if two-handed equipped
                    
                    if(unitItemStatsList[0]==null || unitItemStatsList[0].itemSlotType==3)
                    {
                        UnequipUnitSlot(0);
                    }
                    unitSlotIndex = 1;
                }
            break;
            //unequip offhand if two-handed weapon shall be equipped
            case 3:
                if(stats.skill4Active)
                {
                    unitSlotIndex = 2;
                    UnequipUnitSlot(3);
                }
                else
                {
                    unitSlotIndex = 0;
                    UnequipUnitSlot(1);
                }
            break;
            case 4:
                if(stats.skill4Active)
                {
                    //check if same weapon type
                    if(unitItemStatsList[0]!=null)
                    {
                        if(unitItemStatsList[2].weaponType==saveWeaponStats.weaponType)
                        {
                            unitSlotIndex = 3;
                        }
                        else
                        {
                            dualwieldingCheck = false;
                        }
                    }
                    else
                    {
                        unitSlotIndex = 3;
                    }
                }
                else
                {
                    //check if same weapon type
                    if(unitItemStatsList[0]!=null)
                    {
                        if(unitItemStatsList[0].weaponType==saveWeaponStats.weaponType)
                        {
                            unitSlotIndex = 1;
                        }
                        else
                        {
                            dualwieldingCheck = false;
                        }
                    }
                    else
                    {
                        unitSlotIndex = 1;
                    }
                }
            break;
                /*     11 helmet
                    12 armor
                    13 gauntlet
                    14 pants
                    15 boots */
            case 11:
                unitSlotIndex = 4;
            break;
            case 12:
                unitSlotIndex = 5;
            break;
            case 13:
                unitSlotIndex = 6;
            break;
            case 14:
                unitSlotIndex = 7;
            break;
            case 15:
                unitSlotIndex = 8;
            break;
        }
        
        if(!dualwieldingCheck)
        {
            StartCoroutine(DualWieldingWarning(inventorySlotIndex));
        }           
        else if(!foundEmptySlot)
        {
            StartCoroutine(NoEmptySlot());
        }
        else
        {
            //empty inventory slot
            ClearSlot(inventorySlotIndex);
            UnequipUnitSlot(unitSlotIndex);
            EquipUnitSlot(unitSlotIndex,inventorySlotIndex);
            InitializeInventory();
        }    
    }

    public void EquipUnitSlot(int unitSlotIndex, int inventorySlotIndex)
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(EquipSound, GameManager.Instance.sfxVolume);
        Debug.Log("equip");
        unitItemStatsList[unitSlotIndex] = saveWeaponStats;
        if(saveWeaponStats.itemSlotType>10)
        {
                CharacterEntity myEntity = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<CharacterEntity>();
                myEntity.Equip(saveWeaponStats.myItemAppearance);
                EquipItemStats(saveWeaponStats, stats, 1);   
                //AddItemStats(1);  
                /*     11 helmet
                    12 armor
                    13 gauntlet
                    14 pants
                    15 boots */
                    switch(unitItemStatsList[unitSlotIndex].itemSlotType)
                    {
                        case 11:
                            //stats.equippedHelmet = "Helmet/"+unitItemStatsList[unitSlotIndex].itemName;
                            stats.itemList[4]=saveWeaponStats;
                        break;
                        case 12:
                            //stats.equippedArmor = "Armor/"+unitItemStatsList[unitSlotIndex].itemName;
                            stats.itemList[5]=saveWeaponStats;
                        break;
                        case 13:
                            //stats.equippedGloves = "Gloves/"+unitItemStatsList[unitSlotIndex].itemName;
                            stats.itemList[6]=saveWeaponStats;
                        break;
                        case 14:
                            //stats.equippedPants = "Pants/"+unitItemStatsList[unitSlotIndex].itemName;
                            stats.itemList[7]=saveWeaponStats;
                        break;
                        case 15:
                            //stats.equippedBoots = "Boots/"+unitItemStatsList[unitSlotIndex].itemName;
                            stats.itemList[8]=saveWeaponStats;
                        break;
                    } 
        }
        else
        {
            EquipWeapon(saveWeaponStats);
        }

    }
    //writes the item from the equipped slot into the first free inventory slot
    public void UnequipUnitSlot(int unitSlotIndex)
    {
        //check for empty inventory slot and unequip slot
        foundEmptySlot = false;
        for(int i=0; i<inventoryCapacity && !foundEmptySlot; i++)
        {
            if(itemStatsList[i]==null)
            {
                foundEmptySlot = true;
                //empty unitslot on fighter
                if(unitItemStatsList[unitSlotIndex] != null)
                {
                if(unitItemStatsList[unitSlotIndex].itemSlotType >10)
                {
                /*     11 helmet
                    12 armor
                    13 gauntlet
                    14 pants
                    15 boots */
                    switch(unitItemStatsList[unitSlotIndex].itemSlotType)
                    {
                        case 11:
                            stats.itemList[4]=null;
                        break;
                        case 12:
                            stats.itemList[5]=null;
                        break;
                        case 13:
                            stats.itemList[6]=null;
                        break;
                        case 14:
                            stats.itemList[7]=null;
                        break;
                        case 15:
                            stats.itemList[8]=null;
                        break;
                    }
                    CharacterEntity myEntity = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<CharacterEntity>();
                    myEntity.Unequip(unitItemStatsList[unitSlotIndex].mySlot);
                    EquipItemStats(unitItemStatsList[unitSlotIndex], stats, -1);   
                }
                else
                {
                    UnequipWeapon(unitSlotIndex);
                }
                //write unititemstats in inventory slot i
                itemStatsList[i] = unitItemStatsList[unitSlotIndex];
                unitItemStatsList[unitSlotIndex] = null;
                GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(UnequipSound, GameManager.Instance.sfxVolume);
                }
            }
        }
        //let player no there is no empty slot
        if(!foundEmptySlot)
        {
            StartCoroutine(NoEmptySlot());
        }
        else
        {  
            InitializeInventory();
        }
    }

    

    public void ItemRightClickOptions(int buttonIndex)
    {
        if(buttonIndex >199)
        {
            //=shopitem just show inspect window
            InspectItem(buttonIndex);
        }
        else
        {
            int inventorySlotIndex;
            //unit item
            if(buttonIndex >99)
            {
                inventorySlotIndex = buttonIndex;
            }
            //inventory item
            else
            {
                inventorySlotIndex = buttonIndex+inventoryPage*inventoryPageLength;
            }

            RightClickOptionsCanvas.SetActive(true);
            RightClickOptionsButtons.transform.position = Input.mousePosition;



            if(UnitSelections.Instance.shopScreen)
            {
                int sellValue = itemStatsList[inventorySlotIndex].value/2*itemStatsList[inventorySlotIndex].armor/itemStatsList[inventorySlotIndex].maxArmor;
                int repairCost = itemStatsList[inventorySlotIndex].value/2*(itemStatsList[inventorySlotIndex].maxArmor-itemStatsList[inventorySlotIndex].armor)/itemStatsList[inventorySlotIndex].maxArmor;
                DropButton.GetComponentInChildren<TMP_Text>().text = "Sell ("+sellValue+")";
                RepairButton.GetComponent<Button>().enabled = true;
                RepairButton.GetComponent<Image>().sprite = normalButtonImage;
                RepairButton.GetComponentInChildren<TMP_Text>().text = "Repair ("+repairCost+")";
            }
            else
            {
                RepairButton.GetComponent<Button>().enabled = false;
                RepairButton.GetComponent<Image>().sprite = disabledButtonImage;
                DropButton.GetComponentInChildren<TMP_Text>().text = "Drop Item";
            }
            DropButton.GetComponent<Button>().enabled = true;
            DropButton.GetComponent<Image>().sprite = normalButtonImage;
            
            //only show offhand for weapon type 1
            if(itemStatsList[inventorySlotIndex].itemSlotType==1)
            {
                OffhandButton.GetComponent<Button>().enabled = true;
                OffhandButton.GetComponent<Image>().sprite = normalButtonImage;
            }
            else
            {
                OffhandButton.GetComponent<Button>().enabled = false;
                OffhandButton.GetComponent<Image>().sprite = disabledButtonImage;
            }

            StartCoroutine(ItemRightClickOptions2(inventorySlotIndex));
        }
    }

    public IEnumerator ItemRightClickOptions2(int inventorySlotIndex)
    {
        inputDone = false;
        while (!inputDone)
        {
            yield return null;
        }

        if(option==1)
        {
            //sell or drop
            if(UnitSelections.Instance.shopScreen)
            {
                SellItem(inventorySlotIndex);
            }
            else
            {
                ClearSlot(inventorySlotIndex);
            }
        }
        else if(option==2)
        {
            //itemStatsList[inventorySlotIndex] = Resources.Load<GameObject>("Equipment/Weapon/"+itemStatsList[inventorySlotIndex].prefab.name+"-offhand").GetComponent<WeaponStats>();
            itemStatsList[inventorySlotIndex].itemSlotType=4;
            EquipItemAppearance(inventorySlotIndex-inventoryPage*inventoryPageLength);
        }
        else if(option==3)
        {
            RepairItem(inventorySlotIndex);
        }
        else if(option==4)
        {
            InspectItem(inventorySlotIndex);
        }

        RightClickOptionsCanvas.SetActive(false);
    }

    public void ChooseOption(int buttonIndex)
    {
        option = buttonIndex;
        inputDone = true;
    }    
    
    
    public void SellItem(int inventorySlotIndex)
    {
        int gold = itemStatsList[inventorySlotIndex].value/2*itemStatsList[inventorySlotIndex].armor/itemStatsList[inventorySlotIndex].maxArmor;
        GameManager.Instance.ChangeGold(gold);
        ShopSystem.Instance.itemStatsList.Add(itemStatsList[inventorySlotIndex]);
        GameManager.Instance.GetComponent<AudioSource>() .PlayOneShot(SellSound, GameManager.Instance.sfxVolume);
        itemStatsList[inventorySlotIndex]=null;
        ShopSystem.Instance.InitializeShop();
    }

    void RepairItem(int inventorySlotIndex)
    {
        int repairCost = itemStatsList[inventorySlotIndex].value/2*(itemStatsList[inventorySlotIndex].maxArmor-itemStatsList[inventorySlotIndex].armor)/itemStatsList[inventorySlotIndex].maxArmor;
        if(repairCost < GameManager.Instance.gold)
        {
            GameManager.Instance.ChangeGold(-repairCost);
            //if no need to add repaired armor to character as long as only inventory items can be repaired
            itemStatsList[inventorySlotIndex].armor = itemStatsList[inventorySlotIndex].maxArmor;
            GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(repairSound, GameManager.Instance.sfxVolume);
        }
        else
        {
            StartCoroutine(NoMoney());
        }
    }
    void ClearSlot(int inventorySlotIndex)
    {
        //itemNameList[inventorySlotIndex] = "";
        //itemSlotType[inventorySlotIndex] = 0;
        //itemCharacterIndex[inventorySlotIndex] = 0;
        itemStatsList[inventorySlotIndex] = null;
        LoadInventoryPage(inventoryPage);
    }


    public void EquipAllArmorStats(int unequip)
    {
        //this function is called from outside of the inventorysystem
        Fighter stats = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();
            for(int i=4; i<9; i++)
            {
                if(stats.itemList[i]!=null)
                {
                    //Fighter unit =  UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();
                    stats.maxHP += stats.itemList[i].health * unequip;
                    stats.currentHP += stats.itemList[i].health * unequip;
                    stats.currentStamina += Mathf.RoundToInt(stats.itemList[i].stamina*(1-stats.armorhabituationSkill*SkillSystem.Instance.SkillEffectList[17]/100f))* unequip;  
                    stats.maxStamina += Mathf.RoundToInt(stats.itemList[i].stamina*(1-stats.armorhabituationSkill*SkillSystem.Instance.SkillEffectList[17]/100f))* unequip;          //apply armorhabituationskill
                    stats.armor += Mathf.RoundToInt(stats.itemList[i].armor*(1+stats.armormasterSkill*SkillSystem.Instance.SkillEffectList[18]/100f))* unequip;                    //apply armormasterskill
                    stats.mAcc += stats.itemList[i].mAcc * unequip;
                    stats.mEva += stats.itemList[i].mEva * unequip;
                    stats.rAcc += stats.itemList[i].rAcc * unequip;
                    stats.rEva += stats.itemList[i].rEva * unequip;
                    stats.hpBar.maxValue = stats.maxHP;
                    stats.hpBar.value = stats.currentHP;
                    stats.staminaBar.maxValue = stats.maxStamina;
                    stats.speed += stats.itemList[i].speed*unequip;
                    stats.pDmg += stats.itemList[i].pDmg*unequip;
                    stats.mDmg += stats.itemList[i].mDmg*unequip;

                }
            }
    }

    public void EquipItemStats(WeaponStats weaponStats,Fighter stats, int unequip)
    {
        //Fighter unit =  UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();
        stats.maxHP += weaponStats.health * unequip;
        stats.currentHP += weaponStats.health * unequip;
        stats.currentStamina += Mathf.RoundToInt(weaponStats.stamina*(1-stats.armorhabituationSkill*SkillSystem.Instance.armorHabituationEffect/100f))* unequip; 
        stats.maxStamina += Mathf.RoundToInt(weaponStats.stamina*(1-stats.armorhabituationSkill*SkillSystem.Instance.armorHabituationEffect/100f))* unequip;          //apply armorhabituationskill
        stats.armor += Mathf.RoundToInt(weaponStats.armor*(1+stats.armormasterSkill*SkillSystem.Instance.armorMasterEffect/100f))* unequip;                    //apply armormasterskill
        stats.mAcc += weaponStats.mAcc * unequip;
        stats.mEva += weaponStats.mEva * unequip;
        stats.rAcc += weaponStats.rAcc * unequip;
        stats.rEva += weaponStats.rEva * unequip;
        stats.hpBar.maxValue = stats.maxHP;
        stats.hpBar.value = stats.currentHP;
        stats.staminaBar.maxValue = stats.maxStamina;
        stats.speed += weaponStats.speed*unequip;
        stats.pDmg += weaponStats.pDmg*unequip;
        stats.mDmg += weaponStats.mDmg*unequip;
    }    

    private void UnequipWeapon(int unitSlotIndex) //cases -> different weapon types, which one currently equipped?
    {

            //make sure the slottype doesnt get changed during weaponswap
            int slotType = unitItemStatsList[unitSlotIndex].itemSlotType;
            if(unitSlotIndex<2)
            {
                //don't change current equip if this weapon set is not active
                if(!stats.skill4Active)
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], -1);
                }

                //change the correspondent weapon slot
                switch(slotType)
                {
                    case 1: //one-handed main
                        stats.itemList[0] = null;
                    break;
                    case 2: //offhand
                        stats.itemList[1] = null;
                    break;
                    case 3: //2-handed    
                        stats.itemList[0] = null;
                        stats.itemList[1] = null;
                    break;
                    case 4: //one-handed offhand
                        stats.itemList[1] = null;
                        /* string itemName = unitItemStatsList[unitSlotIndex].prefab.name.Remove(unitItemStatsList[unitSlotIndex].prefab.name.Length - 8);
                        unitItemStatsList[unitSlotIndex] = Resources.Load<GameObject>("Equipment/Weapon/"+itemName).GetComponent<WeaponStats>(); */
                    break;
                }
                //don't change current equip if this weapon set is not active
                if(!stats.skill4Active)
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], 1);
                }
            }      
            else if(unitSlotIndex>1)
            {
                //don't change current equip if this weapon set is not active
                if(stats.skill4Active)
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], -1);
                }

                //change the correspondent weapon slot
                switch(slotType)
                {
                    case 1: //one-handed main
                        stats.itemList[2] = null;
                    break;
                    case 2: //offhand
                        stats.itemList[3] = null;
                    break;
                    case 3: //2-handed    
                        stats.itemList[2] = null;
                        stats.itemList[3] = null;
                    break;
                    case 4: //one-handed offhand
                        stats.itemList[3] = null;
/*                         string itemName = unitItemStatsList[unitSlotIndex].prefab.name.Remove(unitItemStatsList[unitSlotIndex].prefab.name.Length - 8);
                        unitItemStatsList[unitSlotIndex] = Resources.Load<GameObject>("Equipment/Weapon/"+itemName).GetComponent<WeaponStats>(); */
                    break;
                }
                //don't change current equip if this weapon set is not active
                if(stats.skill4Active)
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], 1);
                }
            }
    }

    private void EquipWeapon(WeaponStats weaponStats) //cases -> different weapon types, which one currently equipped?
    {

        Debug.Log("Inv-EquipW"+weaponStats.itemSlotType);
        //Fighter unit = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();
        
            if(!stats.skill4Active)
            {
                WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], -1);
                //change the correspondent weapon slot
                switch(weaponStats.itemSlotType)
                {
                    case 1: //one-handed main
                        stats.itemList[0] = weaponStats;
                    break;
                    case 2: //offhand
                        stats.itemList[1] = weaponStats;
                    break;
                    case 3: //2-handed    
                        stats.itemList[0] = weaponStats;
                        stats.itemList[1] = null;
                    break;
                    case 4: //one-handed offhand
                        stats.itemList[1] = weaponStats;
                    break;
                }
                WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], 1);
            }      
            else
            {
                WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], -1);

                //change the correspondent weapon slot
                switch(weaponStats.itemSlotType)
                {
                    case 1: //one-handed main
                        stats.itemList[2] = weaponStats;
                    break;
                    case 2: //offhand
                        stats.itemList[3] = weaponStats;
                    break;
                    case 3: //2-handed    
                        stats.itemList[2] = weaponStats;
                        stats.itemList[3] = null;
                    break;
                    case 4: //one-handed offhand
                        stats.itemList[3] = weaponStats;
                    break;
                }

                WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], 1);
            }             
            
    }

    public void UpdateUnitStats()
    {
        //get fighter again for statscanvas refresh
        Fighter stats = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();
        //now gets only called if shopscreen anyways
/*         if(UnitSelections.Instance.shopScreen)
        { */
            shopUnitStats.SetActive(true);
            //attributes
            shopUnitStatsList[0].text = ""+stats.currentHP +"/" + stats.maxHP; 
            shopUnitStatsList[1].text = ""+stats.currentStamina +"/" + stats.maxStamina; 
            //shopUnitStatsList[2].text = ""+stats.pDmg; 
            //shopUnitStatsList[3].text = ""+stats.mDmg; 
            shopUnitStatsList[2].text = ""+stats.mAcc; 
            shopUnitStatsList[3].text = ""+stats.mEva; 
            shopUnitStatsList[4].text = ""+stats.rAcc; 
            shopUnitStatsList[5].text = ""+stats.rEva;
/*         }
        else
        {
            shopUnitStats.SetActive(false);
            unitStatsList[0].text = "("+stats.attributePoints+")"; 
            //attributes
            unitStatsList[1].text = ""+stats.currentHP +"/" + stats.maxHP; 
            unitStatsList[2].text = ""+stats.currentStamina +"/" + stats.maxStamina; 
            unitStatsList[3].text = ""+stats.pDmg; 
            unitStatsList[4].text = ""+stats.mDmg; 
            unitStatsList[5].text = ""+stats.mAcc; 
            unitStatsList[6].text = ""+stats.mEva; 
            unitStatsList[7].text = ""+stats.rAcc; 
            unitStatsList[8].text = ""+stats.rEva;
            //equip
            if(stats.dmg2 !=0)
            {
                unitStatsList[9].text = ""+stats.dmg +"+" +stats.dmg2; 
            } 
            else
            {
                unitStatsList[9].text = ""+stats.dmg; 
            }
            unitStatsList[10].text = ""+stats.armor; 
            unitStatsList[11].text = ""+stats.armorBypass; 
            unitStatsList[12].text = ""+stats.armorPierce;
            unitStatsList[13].text = ""+stats.attackRate;
            unitStatsList[14].text = ""+stats.attackCost;
            //secondary stats
            unitStatsList[15].text = ""+stats.moveResistance;
            unitStatsList[16].text = ""+stats.mindResistance;
            unitStatsList[17].text = ""+stats.resistanceList[1];
            unitStatsList[18].text = ""+stats.speed;

        } */

        if(stats.skill4Active)
        {
            WeaponSetFrame1.GetComponent<Image>().color = new Color32(192,192,192,255);
            WeaponSetFrame1.transform.GetChild(0).GetComponent<Image>().color = new Color32(192,192,192,255);
            WeaponSetFrame2.GetComponent<Image>().color = new Color32(218,165,32,255);
            WeaponSetFrame2.transform.GetChild(0).GetComponent<Image>().color = new Color32(218,165,32,255);
        }
        else
        {
            WeaponSetFrame1.GetComponent<Image>().color = new Color32(218,165,32,255);
            WeaponSetFrame1.transform.GetChild(0).GetComponent<Image>().color = new Color32(218,165,32,255);
            WeaponSetFrame2.GetComponent<Image>().color = new Color32(192,192,192,255);
            WeaponSetFrame2.transform.GetChild(0).GetComponent<Image>().color = new Color32(192,192,192,255);
        }
    }

    public void WeaponSetButton(int buttonIndex)
    {
        if(stats.skill4Active && buttonIndex == 1)
        {
            UnitSelections.Instance.ToggleSkill4();
        }        
        if(!stats.skill4Active && buttonIndex == 2)
        {
            UnitSelections.Instance.ToggleSkill4();
        }
        
    }
    
    public IEnumerator NoEmptySlot()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(warningSound, GameManager.Instance.sfxVolume);
        WarningCanvas.SetActive(true);
        WarningText.text = "No empty inventory slot available!"; 
        yield return new WaitForSecondsRealtime(2f);
        WarningCanvas.SetActive(false);
    }    
    public IEnumerator NoMoney()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(warningSound, GameManager.Instance.sfxVolume);
        WarningCanvas.SetActive(true);
        WarningText.text = "Not enough gold!"; 
        yield return new WaitForSecondsRealtime(2f);
        WarningCanvas.SetActive(false);
    }    
    public IEnumerator NotEnoughPrestige()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(warningSound, GameManager.Instance.sfxVolume);
        WarningCanvas.SetActive(true);
        WarningText.text = "You need more prestige to lead more people!"; 
        yield return new WaitForSecondsRealtime(2f);
        WarningCanvas.SetActive(false);
    }  
    IEnumerator DualWieldingWarning(int inventorySlotIndex)
    {
        //string itemName = itemStatsList[inventorySlotIndex].prefab.name.Remove(itemStatsList[inventorySlotIndex].prefab.name.Length - 8);
        //itemStatsList[inventorySlotIndex] = Resources.Load<GameObject>("Equipment/Weapon/"+itemName).GetComponent<WeaponStats>();
        itemStatsList[inventorySlotIndex].itemSlotType = 1;
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(warningSound, GameManager.Instance.sfxVolume);
        WarningCanvas.SetActive(true);
        WarningText.text = "Need two weapons of the same type for dual wielding!"; 
        yield return new WaitForSecondsRealtime(2f);
        WarningCanvas.SetActive(false);
    }    
    public IEnumerator NotEnoughStamina()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(warningSound, GameManager.Instance.sfxVolume);
        WarningCanvas.SetActive(true);
        WarningText.text = "Not enough Stamina!"; 
        yield return new WaitForSecondsRealtime(2f);
        WarningCanvas.SetActive(false);
    }  

    public void HoverEnter(int buttonIndex)
    {
        
        shopHoverStatsImage.SetActive(false);
        //shop item
        if(buttonIndex>199)
        {
            int inventorySlotIndex = buttonIndex -200 +ShopSystem.Instance.shopPageLength*ShopSystem.Instance.shopPage;

            shopHoverStatsImage.SetActive(true);
            if(ShopSystem.Instance.shownItemStatsList[inventorySlotIndex]!=null)
            {
                int unitItemSlot = 0;
                hoverStatsImage.transform.position = Input.mousePosition + new Vector3(-300,-250,0);
                //determine the item type to compare to equipped item
                switch(ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].itemSlotType)
                {
                    case 1:
                        if(stats.skill4Active)
                        {
                            unitItemSlot = 2;
                        }
                        else
                        {
                            unitItemSlot = 0;
                        }
                    break;
                    case 2:
                        if(stats.skill4Active)
                        {
                            unitItemSlot = 3;
                        }
                        else
                        {
                            unitItemSlot = 1;
                        }
                    break;
                    case 3:
                        if(stats.skill4Active)
                        {
                            unitItemSlot = 2;
                        }
                        else
                        {
                            unitItemSlot = 0;
                        }
                    break;
                    case 4:
                        if(stats.skill4Active)
                        {
                            unitItemSlot = 2;
                        }
                        else
                        {
                            unitItemSlot = 0;
                        }
                    break;
                    case 11:
                        unitItemSlot = 4;
                    break;
                    case 12:
                        unitItemSlot = 5;
                    break;
                    case 13:
                        unitItemSlot = 6;
                    break;
                    case 14:
                        unitItemSlot = 7;
                    break;
                    case 15:
                        unitItemSlot = 8;
                    break;
                }                
                //if item type equipped show stats
                if(unitItemStatsList[unitItemSlot]!=null)
                {
/*                     hoverStatsImage.SetActive(true);
                    if(unitItemStatsList[unitItemSlot].itemSlotType==4)
                    {
                        itemTypeImage.sprite = itemTypeImageList[1];
                    }
                    else
                    {
                        itemTypeImage.sprite = itemTypeImageList[unitItemStatsList[unitItemSlot].itemSlotType];
                    }
                    hoverStatsList[0].text = unitItemStatsList[unitItemSlot].itemName;
                    hoverStatsList[1].text = ""+unitItemStatsList[unitItemSlot].dmg; */
                    //for dmgtype only image
                    int difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].dmg -unitItemStatsList[unitItemSlot].dmg;    
                    if(difference >0)
                    {     
                        shopItemDifferencesList[1].text = "+"+difference; 
                        shopItemDifferencesList[1].color = Color.green;
                    }
                    else
                    {     
                        shopItemDifferencesList[1].text = ""+difference; 
                        shopItemDifferencesList[1].color = Color.red;
                    }
                    //dmgTypeImage.sprite = dmgTypeImageList[unitItemStatsList[unitItemSlot].dmgType];
                    switch(unitItemStatsList[unitItemSlot].dmgType)
                    {
                        case 0: 
                            shopItemDifferencesList[2].text = "Phys";
                        break;
                        case 1: 
                            shopItemDifferencesList[2].text = "Arcane";
                        break;
                        case 2: 
                            shopItemDifferencesList[2].text = "Fire";
                        break;
                        case 3: 
                            shopItemDifferencesList[2].text = "Water";
                        break;
                        case 4: 
                            shopItemDifferencesList[2].text = "Earth";
                        break;
                        case 5: 
                            shopItemDifferencesList[2].text = "Air";
                        break;
                        case 6: 
                            shopItemDifferencesList[2].text = "Light";
                        break;
                        case 7: 
                            shopItemDifferencesList[2].text = "Shadow";
                        break;
                    }
                    //hoverStatsList[3].text = ""+unitItemStatsList[inventorySlotIndex].maxArmor;
                    shopItemDifferencesList[4].text = ""+unitItemStatsList[unitItemSlot].armor+"/"+unitItemStatsList[unitItemSlot].maxArmor;
                    
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].health -unitItemStatsList[unitItemSlot].health;   
                    if(difference >0)
                    {
                        shopItemDifferencesList[5].text = "+"+difference; 
                        shopItemDifferencesList[5].color = Color.green;
                    }
                    else
                    {
                        shopItemDifferencesList[5].text = ""+difference; 
                        shopItemDifferencesList[5].color = Color.red;
                    }
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].stamina -unitItemStatsList[unitItemSlot].stamina;   
                    if(difference >0)
                    {
                        shopItemDifferencesList[6].text = "+"+difference; 
                        shopItemDifferencesList[6].color = Color.green;
                    }
                    else
                    {
                        shopItemDifferencesList[6].text = ""+difference; 
                        shopItemDifferencesList[6].color = Color.red;
                    }
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].mAcc -unitItemStatsList[unitItemSlot].mAcc;  
                    if(difference >0)
                    {
                        shopItemDifferencesList[7].text = "+"+difference; 
                        shopItemDifferencesList[7].color = Color.green;
                    }
                    else
                    {
                        shopItemDifferencesList[7].text = ""+difference; 
                        shopItemDifferencesList[7].color = Color.red;
                    }
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].mEva -unitItemStatsList[unitItemSlot].mEva;   
                    if(difference >0)
                    {
                        shopItemDifferencesList[8].text = "+"+difference; 
                        shopItemDifferencesList[8].color = Color.green;
                    }
                    else
                    {
                        shopItemDifferencesList[8].text = ""+difference; 
                        shopItemDifferencesList[8].color = Color.red;
                    }
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].rAcc -unitItemStatsList[unitItemSlot].rAcc;   
                    if(difference >0)
                    {
                        shopItemDifferencesList[9].text = "+"+difference; 
                        shopItemDifferencesList[9].color = Color.green;
                    }
                    else
                    {
                        shopItemDifferencesList[9].text = ""+difference; 
                        shopItemDifferencesList[9].color = Color.red;
                    }
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].rEva -unitItemStatsList[unitItemSlot].rEva;   
                    if(difference >0)
                    {
                        shopItemDifferencesList[10].text = "+"+difference; 
                        shopItemDifferencesList[10].color = Color.green;
                    }
                    else
                    {
                        shopItemDifferencesList[10].text = ""+difference; 
                        shopItemDifferencesList[10].color = Color.red;
                    }
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].armorBypass -unitItemStatsList[unitItemSlot].armorBypass;   
                    if(difference >0)
                    {
                        shopItemDifferencesList[11].text = "+"+difference; 
                        shopItemDifferencesList[11].color = Color.green;
                    }
                    else
                    {
                        shopItemDifferencesList[11].text = ""+difference; 
                        shopItemDifferencesList[11].color = Color.red;
                    }
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].armorPierce -unitItemStatsList[unitItemSlot].armorPierce;  
                    if(difference >0)
                    { 
                        shopItemDifferencesList[12].text = "+"+difference; 
                        shopItemDifferencesList[12].color = Color.green;
                    }
                    else
                    { 
                        shopItemDifferencesList[12].text = ""+difference; 
                        shopItemDifferencesList[12].color = Color.red;
                    }
                    float floatDifference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].attackRate -unitItemStatsList[unitItemSlot].attackRate;   
                    if(floatDifference >0)
                    {
                        shopItemDifferencesList[13].text = $"+{floatDifference:F2}"; 
                        shopItemDifferencesList[13].color = Color.green;
                    }
                    else
                    {
                        shopItemDifferencesList[13].text = $"{floatDifference:F2}"; 
                        shopItemDifferencesList[13].color = Color.red;
                    }
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].mDmg -unitItemStatsList[unitItemSlot].mDmg; 
                    if(difference >0)
                    {  
                        shopItemDifferencesList[15].text = "+"+difference; 
                        shopItemDifferencesList[15].color = Color.green;
                    }
                    else
                    {  
                        shopItemDifferencesList[15].text = ""+difference; 
                        shopItemDifferencesList[15].color = Color.red;
                    }
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].pDmg -unitItemStatsList[unitItemSlot].pDmg; 
                    if(difference >0)
                    {  
                        shopItemDifferencesList[16].text = "+"+difference; 
                        shopItemDifferencesList[16].color = Color.green;
                    }
                    else
                    {  
                        shopItemDifferencesList[16].text = ""+difference; 
                        shopItemDifferencesList[16].color = Color.red;
                    }
                    floatDifference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].speed -unitItemStatsList[unitItemSlot].speed;  
                    if(floatDifference >0)
                    { 
                        shopItemDifferencesList[17].text = $"+{floatDifference:F2}"; 
                        shopItemDifferencesList[17].color = Color.green;
                    }
                    else
                    { 
                        shopItemDifferencesList[17].text = $"{floatDifference:F2}"; 
                        shopItemDifferencesList[17].color = Color.red;
                    }
                    difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].range - unitItemStatsList[unitItemSlot].range;  
                    if(difference >0)
                    { 
                        shopItemDifferencesList[18].text = "+"+difference; 
                        shopItemDifferencesList[18].color = Color.green;
                    }
                    else
                    { 
                        shopItemDifferencesList[18].text = ""+difference; 
                        shopItemDifferencesList[18].color = Color.red;
                    }
                    //value depending on durability - no difference just values
                    if(unitItemStatsList[unitItemSlot].maxArmor>unitItemStatsList[unitItemSlot].armor)
                    {
                        difference = Mathf.RoundToInt(unitItemStatsList[unitItemSlot].value*unitItemStatsList[unitItemSlot].armor/unitItemStatsList[unitItemSlot].maxArmor);
                    }
                    else
                    {
                        difference = unitItemStatsList[unitItemSlot].value;
                    }   
                    //difference = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].value - difference;   
                    shopItemDifferencesList[14].text = ""+difference; 

                    
                }
                else
                {
                    for(int i=0; i<shopItemDifferencesList.Count; i++)
                    {
                        if(shopItemDifferencesList[i] != null)
                        {
                            shopItemDifferencesList[i].color = new Color32(0,0,0,0);
                        }
                    }
                }




                shopHoverStatsImage.transform.position = Input.mousePosition + new Vector3(350,100,0);

                shopItemTypeImage.sprite = itemTypeImageList[ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].itemSlotType];
                
                
                shopHoverStatsList[0].text = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].itemName;
                shopHoverStatsList[1].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].dmg;
                //for dmgtype only image
                shopDmgTypeImage.sprite = dmgTypeImageList[ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].dmgType];
                switch(ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].dmgType)
                {
                    case 0: 
                        shopHoverStatsList[2].text = "Phys";
                    break;
                    case 1: 
                        shopHoverStatsList[2].text = "Arcane";
                    break;
                    case 2: 
                        shopHoverStatsList[2].text = "Fire";
                    break;
                    case 3: 
                        shopHoverStatsList[2].text = "Water";
                    break;
                    case 4: 
                        shopHoverStatsList[2].text = "Earth";
                    break;
                    case 5: 
                        shopHoverStatsList[2].text = "Air";
                    break;
                    case 6: 
                        shopHoverStatsList[2].text = "Light";
                    break;
                    case 7: 
                        shopHoverStatsList[2].text = "Shadow";
                    break;
                }
                //hoverStatsList[3].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].maxArmor;
                shopHoverStatsList[4].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].armor+"/"+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].maxArmor;
                shopHoverStatsList[5].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].stamina;
                shopHoverStatsList[6].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].health;
                shopHoverStatsList[7].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].mAcc;
                shopHoverStatsList[8].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].mEva;
                shopHoverStatsList[9].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].rAcc;
                shopHoverStatsList[10].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].rEva;
                shopHoverStatsList[11].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].armorBypass;
                shopHoverStatsList[12].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].armorPierce;
                shopHoverStatsList[13].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].attackRate;
                shopHoverStatsList[15].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].mDmg;
                shopHoverStatsList[16].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].pDmg;
                shopHoverStatsList[17].text = "+"+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].speed;
                shopHoverStatsList[18].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].range;
                //shop item full armor thats why no calculation for value
                shopHoverStatsList[14].text = ""+ShopSystem.Instance.shownItemStatsList[inventorySlotIndex].value;
                

            }
        }
        //unit item
        else if(buttonIndex>99)
        {
            int inventorySlotIndex = buttonIndex -100;
            if(unitItemStatsList[inventorySlotIndex]!=null)
            {
                hoverStatsImage.SetActive(true);
                if(unitItemStatsList[inventorySlotIndex].itemSlotType==4)
                {
                    itemTypeImage.sprite = itemTypeImageList[1];
                }
                else
                {
                    itemTypeImage.sprite = itemTypeImageList[unitItemStatsList[inventorySlotIndex].itemSlotType];
                }
                hoverStatsImage.transform.position = Input.mousePosition + new Vector3(-300,-150,0);
                hoverStatsList[0].text = unitItemStatsList[inventorySlotIndex].itemName;
                hoverStatsList[1].text = ""+unitItemStatsList[inventorySlotIndex].dmg;
                //for dmgtype only image
                dmgTypeImage.sprite = dmgTypeImageList[unitItemStatsList[inventorySlotIndex].dmgType];
                switch(unitItemStatsList[inventorySlotIndex].dmgType)
                {
                    case 0: 
                        hoverStatsList[2].text = "Phys";
                    break;
                    case 1: 
                        hoverStatsList[2].text = "Arcane";
                    break;
                    case 2: 
                        hoverStatsList[2].text = "Fire";
                    break;
                    case 3: 
                        hoverStatsList[2].text = "Water";
                    break;
                    case 4: 
                        hoverStatsList[2].text = "Earth";
                    break;
                    case 5: 
                        hoverStatsList[2].text = "Air";
                    break;
                    case 6: 
                        hoverStatsList[2].text = "Light";
                    break;
                    case 7: 
                        hoverStatsList[2].text = "Shadow";
                    break;
                }
                //hoverStatsList[3].text = ""+unitItemStatsList[inventorySlotIndex].maxArmor;
                hoverStatsList[4].text = ""+unitItemStatsList[inventorySlotIndex].armor+"/"+unitItemStatsList[inventorySlotIndex].maxArmor;
                hoverStatsList[5].text = ""+unitItemStatsList[inventorySlotIndex].stamina;
                hoverStatsList[6].text = ""+unitItemStatsList[inventorySlotIndex].health;
                hoverStatsList[7].text = ""+unitItemStatsList[inventorySlotIndex].mAcc;
                hoverStatsList[8].text = ""+unitItemStatsList[inventorySlotIndex].mEva;
                hoverStatsList[9].text = ""+unitItemStatsList[inventorySlotIndex].rAcc;
                hoverStatsList[10].text = ""+unitItemStatsList[inventorySlotIndex].rEva;
                hoverStatsList[11].text = ""+unitItemStatsList[inventorySlotIndex].armorBypass;
                hoverStatsList[12].text = ""+unitItemStatsList[inventorySlotIndex].armorPierce;
                hoverStatsList[13].text = ""+unitItemStatsList[inventorySlotIndex].attackRate;
                hoverStatsList[15].text = ""+unitItemStatsList[inventorySlotIndex].mDmg;
                hoverStatsList[16].text = ""+unitItemStatsList[inventorySlotIndex].pDmg;
                hoverStatsList[17].text = "+"+unitItemStatsList[inventorySlotIndex].speed;
                hoverStatsList[18].text = ""+unitItemStatsList[inventorySlotIndex].range;
                if(unitItemStatsList[inventorySlotIndex].maxArmor>unitItemStatsList[inventorySlotIndex].armor)
                {
                    hoverStatsList[14].text = ""+Mathf.RoundToInt(unitItemStatsList[inventorySlotIndex].value*unitItemStatsList[inventorySlotIndex].armor/unitItemStatsList[inventorySlotIndex].maxArmor);
                }
                else
                {
                    hoverStatsList[14].text = ""+unitItemStatsList[inventorySlotIndex].value;
                }
            }
        }
        //inventory item
        else
        {
            int inventorySlotIndex = buttonIndex + inventoryPage*inventoryPageLength;
            
            if(itemStatsList[inventorySlotIndex]!=null)
            {
                hoverStatsImage.SetActive(true);
                itemTypeImage.sprite = itemTypeImageList[itemStatsList[inventorySlotIndex].itemSlotType];
                hoverStatsImage.transform.position = Input.mousePosition + new Vector3(-300,100,0);
                hoverStatsList[0].text = itemStatsList[inventorySlotIndex].itemName;
                hoverStatsList[1].text = ""+itemStatsList[inventorySlotIndex].dmg;
                //for dmgtype only image
                dmgTypeImage.sprite = dmgTypeImageList[itemStatsList[inventorySlotIndex].dmgType];
                switch(itemStatsList[inventorySlotIndex].dmgType)
                {
                    case 0: 
                        hoverStatsList[2].text = "Phys";
                    break;
                    case 1: 
                        hoverStatsList[2].text = "Arcane";
                    break;
                    case 2: 
                        hoverStatsList[2].text = "Fire";
                    break;
                    case 3: 
                        hoverStatsList[2].text = "Water";
                    break;
                    case 4: 
                        hoverStatsList[2].text = "Earth";
                    break;
                    case 5: 
                        hoverStatsList[2].text = "Air";
                    break;
                    case 6: 
                        hoverStatsList[2].text = "Light";
                    break;
                    case 7: 
                        hoverStatsList[2].text = "Shadow";
                    break;
                }
                //hoverStatsList[3].text = ""+itemStatsList[inventorySlotIndex].maxArmor;
                hoverStatsList[4].text = ""+itemStatsList[inventorySlotIndex].armor+"/"+itemStatsList[inventorySlotIndex].maxArmor;
                hoverStatsList[5].text = ""+itemStatsList[inventorySlotIndex].stamina;
                hoverStatsList[6].text = ""+itemStatsList[inventorySlotIndex].health;
                hoverStatsList[7].text = ""+itemStatsList[inventorySlotIndex].mAcc;
                hoverStatsList[8].text = ""+itemStatsList[inventorySlotIndex].mEva;
                hoverStatsList[9].text = ""+itemStatsList[inventorySlotIndex].rAcc;
                hoverStatsList[10].text = ""+itemStatsList[inventorySlotIndex].rEva;
                hoverStatsList[11].text = ""+itemStatsList[inventorySlotIndex].armorBypass;
                hoverStatsList[12].text = ""+itemStatsList[inventorySlotIndex].armorPierce;
                hoverStatsList[13].text = ""+itemStatsList[inventorySlotIndex].attackRate;
                hoverStatsList[15].text = ""+itemStatsList[inventorySlotIndex].mDmg;
                hoverStatsList[16].text = ""+itemStatsList[inventorySlotIndex].pDmg;
                hoverStatsList[17].text = "+"+itemStatsList[inventorySlotIndex].speed;
                hoverStatsList[18].text = ""+itemStatsList[inventorySlotIndex].range;
                if(itemStatsList[inventorySlotIndex].maxArmor>itemStatsList[inventorySlotIndex].armor)
                {
                    hoverStatsList[14].text = ""+Mathf.RoundToInt(itemStatsList[inventorySlotIndex].value*itemStatsList[inventorySlotIndex].armor/itemStatsList[inventorySlotIndex].maxArmor);
                }
                else
                {
                    hoverStatsList[14].text = ""+itemStatsList[inventorySlotIndex].value;
                }
            }
        }
    }
    public void HoverExit()
    {
        hoverStatsImage.SetActive(false);
        shopHoverStatsImage.SetActive(false);
    }

    public void InspectItem(int inventorySlotIndex)
    {
        InspectItemWindow.SetActive(true);
        WeaponStats item;
        if(inventorySlotIndex>199)
        {
            item = ShopSystem.Instance.shownItemStatsList[inventorySlotIndex-200+ShopSystem.Instance.shopPage*ShopSystem.Instance.shopPageLength];
            //base parameters
        }
        else
        {
            item = itemStatsList[inventorySlotIndex+inventoryPage*inventoryPageLength];
        }
        
            InspectItemIcon.texture = item.myItemIcon;
            InspectItemTextList[0].text = item.itemName;
            InspectItemTextList[1].text = item.dmg.ToString(); 
            InspectItemImageList[0].sprite = dmgTypeImageList[item.dmgType];
            switch(item.dmgType)
            {
                case 0: 
                    InspectItemTextList[2].text = "Phys";
                break;
                case 1: 
                    InspectItemTextList[2].text = "Arcane";
                break;
                case 2: 
                    InspectItemTextList[2].text = "Fire";
                break;
                case 3: 
                    InspectItemTextList[2].text = "Water";
                break;
                case 4: 
                    InspectItemTextList[2].text = "Earth";
                break;
                case 5: 
                    InspectItemTextList[2].text = "Air";
                break;
                case 6: 
                    InspectItemTextList[2].text = "Light";
                break;
                case 7: 
                    InspectItemTextList[2].text = "Shadow";
                break;
            }
            InspectItemTextList[3].text = ""+item.armor +"/" +item.maxArmor;
            InspectItemTextList[4].text = item.health.ToString(); 
            InspectItemTextList[5].text = item.stamina.ToString(); 
            InspectItemTextList[6].text = item.mDmg.ToString(); 
            InspectItemTextList[7].text = item.pDmg.ToString(); 
            InspectItemTextList[8].text = item.mAcc.ToString(); 
            InspectItemTextList[9].text = item.mEva.ToString(); 
            InspectItemTextList[10].text = item.rAcc.ToString(); 
            InspectItemTextList[11].text = item.rEva.ToString(); 
            InspectItemTextList[12].text = item.armorBypass.ToString(); 
            InspectItemTextList[13].text = item.armorPierce.ToString(); 
            InspectItemTextList[14].text = item.attackRate.ToString("F2"); 
            InspectItemTextList[15].text = item.range.ToString(); 
            InspectItemTextList[16].text = item.attackCost.ToString(); 
            InspectItemTextList[17].text = item.speed.ToString("F2"); 
            if(item.maxArmor>item.armor)
            {
                InspectItemTextList[18].text = ""+Mathf.RoundToInt(item.value*item.armor/item.maxArmor);
            }
            else
            {
                InspectItemTextList[18].text = ""+item.value;
            }

            //additional parameters
            switch(item.itemSlotType)
            {
                case 1:
                case 2:
                    InspectItemImageList[2].gameObject.SetActive(true);
                    InspectItemTextList[21].text = "One-Handed";
                    InspectItemImageList[1].sprite = itemTypeImageList[1];
                    switch(item.weaponType)
                    {
                        case 1: 
                            InspectItemImageList[2].sprite = itemTypeImageList[21];
                            InspectItemImageList[3].gameObject.SetActive(true);
                            InspectItemTextList[22].text = "Sword";
                        break;
                        case 2: 
                            InspectItemImageList[2].sprite = itemTypeImageList[22];
                            InspectItemTextList[22].text = "Axe";
                        break;
                        case 3: 
                            InspectItemImageList[2].sprite = itemTypeImageList[23];
                            InspectItemTextList[22].text = "Mace";
                        break;
                        case 4: 
                            InspectItemImageList[2].sprite = itemTypeImageList[24];
                            InspectItemTextList[22].text = "Spear";
                        break;
                        case 5: 
                            InspectItemImageList[1].sprite = itemTypeImageList[2];
                            InspectItemImageList[2].sprite = itemTypeImageList[27];
                            InspectItemTextList[22].text = "Buckler";
                        break;
                        case 10: 
                            InspectItemImageList[1].sprite = itemTypeImageList[2];
                            InspectItemImageList[2].sprite = itemTypeImageList[28];
                            InspectItemTextList[22].text = "Shield";
                        break;
                    }
                break;
                case 3:
                    InspectItemImageList[2].gameObject.SetActive(true);
                    InspectItemTextList[21].text = "Two-Handed";
                    InspectItemImageList[1].sprite = itemTypeImageList[3];
                    switch(item.weaponType)
                    {
                        case 21: 
                            InspectItemImageList[2].sprite = itemTypeImageList[21];
                            InspectItemTextList[22].text = "Sword";
                        break;
                        case 22: 
                            InspectItemImageList[2].sprite = itemTypeImageList[22];
                            InspectItemTextList[22].text = "Axe";
                        break;
                        case 23: 
                            InspectItemImageList[2].sprite = itemTypeImageList[23];
                            InspectItemTextList[22].text = "Mace";
                        break;
                        case 24: 
                            InspectItemImageList[2].sprite = itemTypeImageList[24];
                            InspectItemTextList[22].text = "Spear/Pike";
                        break;
                        case 25: 
                            InspectItemImageList[2].sprite = itemTypeImageList[25];
                            InspectItemTextList[22].text = "Crossbow";
                        break;
                        case 26: 
                            InspectItemImageList[2].sprite = itemTypeImageList[26];
                            InspectItemTextList[22].text = "Bow";
                        break;
                        default: 
                            InspectItemImageList[2].sprite = itemTypeImageList[29];
                            InspectItemTextList[22].text = "Magic Staff";
                        break;
                    }
                break;
                case 11:
                    InspectItemImageList[2].gameObject.SetActive(false);
                    InspectItemTextList[21].text = "Helmet";
                    InspectItemTextList[22].text = "";
                    InspectItemImageList[1].sprite = itemTypeImageList[11];
                break;
                case 12:
                    InspectItemImageList[2].gameObject.SetActive(false);
                    InspectItemTextList[21].text = "Body Armor";
                    InspectItemTextList[22].text = "";
                    InspectItemImageList[1].sprite = itemTypeImageList[12];
                break;
                case 13:
                    InspectItemImageList[2].gameObject.SetActive(false);
                    InspectItemTextList[21].text = "Gloves";
                    InspectItemTextList[22].text = "";
                    InspectItemImageList[1].sprite = itemTypeImageList[13];
                break;
                case 14:
                    InspectItemImageList[2].gameObject.SetActive(false);
                    InspectItemTextList[21].text = "Pants";
                    InspectItemTextList[22].text = "";
                    InspectItemImageList[1].sprite = itemTypeImageList[14];
                break;
                case 15:
                    InspectItemImageList[2].gameObject.SetActive(false);
                    InspectItemTextList[21].text = "Boots";
                    InspectItemTextList[22].text = "";
                    InspectItemImageList[1].sprite = itemTypeImageList[15];
                break;
            }

            InspectItemTextList[24].text = item.dmgOverTime.ToString();
            InspectItemTextList[25].text = item.slowAttack.ToString();
            InspectItemTextList[26].text = item.bashAttack.ToString();
            InspectItemTextList[27].text = item.pushAttack.ToString();
            InspectItemTextList[28].text = item.blindingAttack.ToString();
            InspectItemTextList[29].text = item.weakeningAttack.ToString();

            //get item skill images and explanations
            UnitSelections.Instance.GetSkillImageForOtherScripts(item.weaponType, InspectItemImageList[3], InspectItemImageList[4], InspectItemImageList[5]);
    }

    public void CloseInspectItemWindow()
    {
        InspectItemWindow.SetActive(false);
    }
    

    public void DisableInput()
    {
        UnitSelections.Instance.DisableUnitSelections();
        HoverExit();
        for(int i=0; i<equipButtons.Count; i++)
        {
            equipButtons[i].GetComponent<Button>().enabled = false;  
        }
    }
    public void EnableInput()
    {
        for(int i=0; i<equipButtons.Count; i++)
        {
            equipButtons[i].GetComponent<Button>().enabled = true;  
        }
    }
}
