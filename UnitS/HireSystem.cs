using System.Collections;
using System.Collections.Generic;
using SoftKitty.MasterCharacterCreator;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HireSystem : MonoBehaviour
{
    public List<WeaponStats> startingEquipment;
    //list of the predefined images for the classes
    //public List<Sprite> classDefaultImageList;
    
    //the available characters
    public List<string> nameList;
    public List<int> classList;
    public List<Sprite> classImageList;
    public List<int> levelList;
    public List<int> potentialList;
    public List<int> costList;
    public List<string> appearanceList;
    public List<GameObject> nameObjectList;
    public List<GameObject> appearanceObjectList;
    public List<GameObject> classObjectList;
    public List<GameObject> levelObjectList;
    public List<GameObject> potentialObjectList;
    public List<GameObject> costObjectList;
    public List<GameObject> selectedMarkerList;
    public List<GameObject> unitNameObjectList;
    public List<GameObject> unitAppearanceObjectList;
    public List<GameObject> unitClassObjectList;
    public List<GameObject> unitLevelObjectList;
    public List<GameObject> unitPotentialObjectList;
    public List<GameObject> unitPromotionObjectList;
    public List<GameObject> selectedUnitMarkerList;
    private List<string> unitNameList = new List<string>();
    private List<string> unitAppearanceList = new List<string>();
    private List<int> unitClassList = new List<int>();
    private List<int> unitPromotionList = new List<int>();
    private List<int> unitLevelList = new List<int>();
    private List<int> unitPotentialList = new List<int>();
    private int hirePage=0;
    private int hirePageLength=9;
    private int rosterPage=0;
    private int rosterPageLength=9;
    private int mercenarySlotIndex= 0;
    private int unitSlotIndex;
    public GameObject HoverOverImage;
    public GameObject ApplyChangesButtons;
    public GameObject FireButtons;
    public GameObject BlockImage;
    public TMP_Text HoverOverText;
    public TMP_Text RosterSizeText;
    public WeaponStats InitialArmor;
    public WeaponStats InitialPants;
    public WeaponStats InitialBoots;
    private bool disableButtons;
    public List<GameObject> pageButtonFramesList;
    public bool tutorialTips = true;
/*  0- hero
    1- swordsman
    2- thug
    3- brawler
    4- guard
    5- soldier
    6- hunter
    7- magicians apprentice 
    
    11-captain
    12-general
    13- swordmaster
    14- warrior
    15- warlord
    16- barbarion
    17- berserker
    18- spartan
    19- sniper
    20- demon hunter
    21- paladin
    22- demon crusher
    31- wizard
    32- priest
    33- whitcher*/


    
    private static HireSystem _instance;
    public static HireSystem Instance { get { return _instance; } }
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
    public void Start()
    {
        levelList.Clear();
        potentialList.Clear();
        costList.Clear();
        for(int i=0; i<nameList.Count;i++)
        {
            levelList.Add(Random.Range(1,10));
            potentialList.Add(Random.Range(3,12));
            //costList.Add(100+levelList[i]*potentialList[i]*10+20+4+4);
            costList.Add(30*levelList[i]+10*potentialList[i]*potentialList[i]+20+4+4);
            //add additional weapon cost
            switch(classList[i])
            {
                case 1:
                    costList[i]+=20;
                break;
                case 2:
                    costList[i]+=5;
                break;
                case 5:
                    costList[i]+=30;
                break;
                case 6:
                    costList[i]+=20;
                break;
                case 7:
                    costList[i]+=55;
                break;
            }
        }

    }

    public void InitializeHireSystem()
    {

        RosterSizeText.text = "("+UnitSelections.Instance.unitList.Count+"/"+GameManager.Instance.maxTroopStrength+")";

        //get unit data for class image conversion later
        unitNameList.Clear();
        unitAppearanceList.Clear();
        unitClassList.Clear();
        unitPromotionList.Clear();
        unitLevelList.Clear();
        unitPotentialList.Clear();
        for(int i=0; i<UnitSelections.Instance.unitList.Count;i++)
        {
            Fighter stats = UnitSelections.Instance.unitList[i].GetComponent<Fighter>();
            unitNameList.Add(stats.unitName);
            unitAppearanceList.Add(stats.appearance);
            unitClassList.Add(stats.fighterClass);
            unitPromotionList.Add(stats.promotedFighterClass);
            unitLevelList.Add(stats.level);
            unitPotentialList.Add(stats.potential);
        }
        

        //show mercenary data
        int buttonIndex = 0;
        for(int i=0+hirePage*hirePageLength; i<hirePageLength+hirePage*hirePageLength; i++)
        {
            if(i>=nameList.Count)
            {
                nameObjectList[buttonIndex].gameObject.SetActive(false);
                appearanceObjectList[buttonIndex].gameObject.SetActive(false);
                classObjectList[buttonIndex].gameObject.SetActive(false);
                levelObjectList[buttonIndex].gameObject.SetActive(false);
                potentialObjectList[buttonIndex].gameObject.SetActive(false);
                costObjectList[buttonIndex].gameObject.SetActive(false);
            }
            else
            {
                nameObjectList[buttonIndex].gameObject.SetActive(true);
                appearanceObjectList[buttonIndex].gameObject.SetActive(true);
                classObjectList[buttonIndex].gameObject.SetActive(true);
                levelObjectList[buttonIndex].gameObject.SetActive(true);
                potentialObjectList[buttonIndex].gameObject.SetActive(true);
                costObjectList[buttonIndex].gameObject.SetActive(true);

                nameObjectList[buttonIndex].GetComponent<TMP_Text>().text = nameList[i];
                appearanceObjectList[buttonIndex].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Faces/face_"+appearanceList[i]);
                classObjectList[buttonIndex].GetComponent<UnityEngine.UI.Image>().sprite = FighterClasses.Instance.GetClassImage(classList[i]);
                //classObjectList[buttonIndex].GetComponent<UnityEngine.UI.Image>().sprite = classDefaultImageList[classList[i]];
                levelObjectList[buttonIndex].GetComponent<TMP_Text>().text = ""+levelList[i];
                potentialObjectList[buttonIndex].GetComponent<TMP_Text>().text = ""+potentialList[i];
                costObjectList[buttonIndex].GetComponent<TMP_Text>().text = ""+costList[i];
            }
            buttonIndex++;
        }

        //show unit data
        buttonIndex = 0;
        for(int i=0+rosterPage*rosterPageLength; i<rosterPageLength+rosterPage*rosterPageLength; i++)
        {
            if(unitNameList.Count<i+1)
            {
                unitNameObjectList[buttonIndex].gameObject.SetActive(false);
                unitAppearanceObjectList[buttonIndex].gameObject.SetActive(false);
                unitClassObjectList[buttonIndex].gameObject.SetActive(false);
                unitLevelObjectList[buttonIndex].gameObject.SetActive(false);
                unitPotentialObjectList[buttonIndex].gameObject.SetActive(false);
                unitPromotionObjectList[buttonIndex].gameObject.SetActive(false);
            }
            else
            {
                unitNameObjectList[buttonIndex].gameObject.SetActive(true);
                unitAppearanceObjectList[buttonIndex].gameObject.SetActive(true);
                unitClassObjectList[buttonIndex].gameObject.SetActive(true);
                unitLevelObjectList[buttonIndex].gameObject.SetActive(true);
                unitPotentialObjectList[buttonIndex].gameObject.SetActive(true);
                
                unitNameObjectList[buttonIndex].GetComponent<TMP_Text>().text = unitNameList[i];
                unitClassObjectList[buttonIndex].GetComponent<UnityEngine.UI.Image>().sprite = FighterClasses.Instance.GetClassImage(unitClassList[i]);
                unitAppearanceObjectList[buttonIndex].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Faces/face_"+unitAppearanceList[i]);
                unitLevelObjectList[buttonIndex].GetComponent<TMP_Text>().text = ""+unitLevelList[i];
                unitPotentialObjectList[buttonIndex].GetComponent<TMP_Text>().text = ""+unitPotentialList[i];
                if(unitPromotionList[i]!=0)
                {
                    unitPromotionObjectList[buttonIndex].gameObject.SetActive(true);
                    unitPromotionObjectList[buttonIndex].GetComponent<UnityEngine.UI.Image>().sprite = FighterClasses.Instance.promotedClassDefaultImageList[unitPromotionList[i]];
                }
                else
                {
                    unitPromotionObjectList[buttonIndex].gameObject.SetActive(false);
                }
            }
            buttonIndex++;
        }        

        //activate buttons
        UnitSelections.Instance.EnableUnitSelections();
        disableButtons = false;

        if(tutorialTips)
        {
            GameManager.Instance.tipIndex= 1;
            GameManager.Instance.Tutorial();
            DisableInput();
        }
    }

    public void FireUnit(int buttonIndex)
    {
        if(!disableButtons)
        {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.ClickSound, GameManager.Instance.sfxVolume);
        //BlockImage.SetActive(true);
        unitSlotIndex = buttonIndex + hirePage*hirePageLength;
        selectedUnitMarkerList[buttonIndex].SetActive(true);
        FireButtons.SetActive(true);
        Debug.Log(unitSlotIndex);
        DisableInput();
        }
        
    }

    public void CancelFire()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.ClickDenySound, GameManager.Instance.sfxVolume);
        foreach(GameObject selected in selectedUnitMarkerList)
        {
            selected.SetActive(false);
        }
        //BlockImage.SetActive(false);
        FireButtons.SetActive(false);
        InitializeHireSystem();
    }
    public void ConfirmedFireUnit()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.ClickSound, GameManager.Instance.sfxVolume);
        //remove items for new appearance initiation
        Unit unitStats = UnitSelections.Instance.unitList[unitSlotIndex].GetComponent<Unit>();
        CharacterEntity myEntity = UnitSelections.Instance.unitList[unitSlotIndex].GetComponent<CharacterEntity>();
        if(unitStats.skill4Active)
        {
            WeaponEquip.Instance.EquipWeapon(unitStats, unitStats.itemList[2], unitStats.itemList[3], -1);
        }
        else
        {
            WeaponEquip.Instance.EquipWeapon(unitStats, unitStats.itemList[0], unitStats.itemList[1], -1);
        }
        for(int i=4; i<9; i++)
        {
            if(unitStats.itemList[i]!=null)
            {
                myEntity.Unequip(unitStats.itemList[i].mySlot);
            }
        }
        //for the new unit initiation
        //unitStats.startItemsLoaded = false;
        /* unitStats.enabled = false; */

        //remove from unitselections
        UnitSelections.Instance.unitList[unitSlotIndex].gameObject.SetActive(false);
        UnitSelections.Instance.unitList.Remove(UnitSelections.Instance.unitList[unitSlotIndex]);
        
        //close menu
        //BlockImage.SetActive(false);
        FireButtons.SetActive(false);
        foreach(GameObject selected in selectedUnitMarkerList)
        {
            selected.SetActive(false);
        }
        InitializeHireSystem();
    }
    public void RecruitUnit(int buttonIndex)
    {
        
        if(!disableButtons)
        {
            DisableInput();
            mercenarySlotIndex = buttonIndex + hirePage*hirePageLength;
            if(costList[mercenarySlotIndex]>GameManager.Instance.gold)
            {
                Debug.Log("Recruit no money");
                StartCoroutine(InventorySystem.Instance.NoMoney());
                InitializeHireSystem();
            }
            else if(UnitSelections.Instance.unitList.Count>=GameManager.Instance.maxTroopStrength)
            {
                Debug.Log("Recruit no prestige");
                StartCoroutine(InventorySystem.Instance.NotEnoughPrestige());
                InitializeHireSystem();
            }
            else
            {
                Debug.Log("Recruit changes start");
                //BlockImage.SetActive(true);
                selectedMarkerList[buttonIndex].SetActive(true);
                ApplyChangesButtons.SetActive(true);
            }
        }
    }

    public void CancelRecruit()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.ClickDenySound, GameManager.Instance.sfxVolume);
        foreach(GameObject selected in selectedMarkerList)
        {
            selected.SetActive(false);
        }
        //BlockImage.SetActive(false);
        ApplyChangesButtons.SetActive(false);
        InitializeHireSystem();
    }
    public void InitiateUnit()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(InventorySystem.Instance.SellSound, GameManager.Instance.sfxVolume);
        //pay
        GameManager.Instance.ChangeGold(-costList[mercenarySlotIndex]);
        //remove Auras for reappling onto new unit as well later 
        foreach(GameObject unit in UnitSelections.Instance.unitList)
        {
            Unit unitStats = unit.GetComponent<Unit>();
            FighterClasses.Instance.GetPromotedClassBoni(unitStats,unitStats.promotedFighterClass,-1);
        }

        //close menus
        //BlockImage.SetActive(false);
        ApplyChangesButtons.SetActive(false);

        foreach(GameObject selected in selectedMarkerList)
        {
            selected.SetActive(false);
        }


        //find first inactive unit, activate it, send it to unitlist and write mercenary stats into it
        for(int i=0; i<UnitSelections.Instance.inActiveUnitList.Count; i++)
        {
            if(!UnitSelections.Instance.inActiveUnitList[i].activeSelf)
            {
                UnitSelections.Instance.inActiveUnitList[i].SetActive(true);
                UnitSelections.Instance.unitList.Add(UnitSelections.Instance.inActiveUnitList[i]);
                //initiate map position
                UnitSelections.Instance.unitInitialPositionList.Add(UnitSelections.Instance.inActiveUnitList[i].transform.position);


                Unit stats = UnitSelections.Instance.inActiveUnitList[i].GetComponent<Unit>();
                stats.startItemsLoaded = true;

                stats.unitName = nameList[mercenarySlotIndex];
                //the image list is limited to the number of slots -> use %
                stats.unitImage = appearanceObjectList[mercenarySlotIndex%hirePageLength].GetComponent<UnityEngine.UI.Image>().sprite;     //softkitty image?
                stats.appearance = appearanceList[mercenarySlotIndex];
                
                CharacterEntity myEntity = UnitSelections.Instance.unitList[i].GetComponent<CharacterEntity>();
                myEntity.LoadFromResourceFile("MasterCharacterCreator/CustomBlueprints/Characters/"+appearanceList[mercenarySlotIndex]);
                appearanceList.RemoveAt(mercenarySlotIndex);
                stats.maxHP = 100;
                stats.currentHP= stats.maxHP;
                stats.maxStamina = 100;
                stats.currentStamina = stats.maxStamina;
                stats.pDmg=10;
                stats.mDmg=10;
                stats.mAcc=10;
                stats.mEva=10;
                stats.rAcc=10;
                stats.rEva=10;
                stats.armor=0;
                stats.armorPierce=0;
                stats.armorBypass=0;
                stats.armorDestruction=0;
                stats.speed = GameManager.Instance.defaultSpeed;
                stats.skill1Active = false;
                stats.skill2Active = false;
                stats.skill3Active = false;
                stats.skill4Active = false;
                stats.aggroRange = GameManager.Instance.aggroRange;

                //equip new armor
                stats.itemList[1]=null;
                stats.itemList[2]=null;
                stats.itemList[3]=null;
                stats.itemList[4]=null;
                stats.itemList[5]= Instantiate(startingEquipment[5]);
                DontDestroyOnLoad(stats.itemList[5]);
                stats.itemList[6]=null;
                stats.itemList[7]= Instantiate(startingEquipment[7]);
                DontDestroyOnLoad(stats.itemList[7]);
                stats.itemList[8]= Instantiate(startingEquipment[8]);
                DontDestroyOnLoad(stats.itemList[8]);
                
                //skillsystemstats
                stats.currentXP = 0;
                stats.skillPoints = 0;
                stats.attributePoints = 0;
                stats.promotedFighterClass = 0;
                for(int j=0; j<stats.promotionClassList.Count; j++)
                {
                    stats.promotionClassList[j] = 0;
                }

                for(int j=0; j<8;j++)
                {
                    stats.resistanceList[j] = 0;
                }
                //skills
                //weapons
                stats.swordSkill = 0;
                stats.axeSkill = 0;
                stats.maceSkill = 0;
                stats.spearSkill = 0;
                stats.crossbowSkill = 0;
                stats.bowSkill = 0;
                stats.shieldSkill = 0;
                stats.staffSkill = 0;
                //utility
                stats.quickdrawSkill = 0;
                stats.dualwieldingSkill = 0;
                stats.twohandedSkill = 0;
                stats.acrobatSkill = 0;
                stats.defenderSkill = 0;
                stats.athleteSkill = 0;
                stats.armorhabituationSkill = 0;
                stats.armormasterSkill = 0;
                stats.magicFlowSkill = 0;
                //specialisations
                stats.sworddancerSkill = 0;
                stats.armorbreakerSkill = 0;
                stats.basherSkill = 0;
                stats.holdTheLineSkill = 0;
                stats.hawkeyeSkill = 0;
                stats.quickshotSkill = 0;
                stats.bufferSkill = 0;
                stats.debufferSkill = 0;
                stats.wisdomSkill = 0;
                //veteranskills
                stats.specialistSkill = 0;
                stats.lionheartSkill = 0;
                stats.marathonerSkill = 0;
                stats.unstoppableSkill = 0;
                stats.survivalistSkill = 0;
                stats.elementalistSkill = 0;
                stats.elementalistSkillElement = 0;
                stats.magicResistanceSkill = 0;

                //apply new skill values and lvlup
                stats.potential = potentialList[mercenarySlotIndex];
                stats.fighterClass = classList[mercenarySlotIndex];
                //stats.level= levelList[mercenarySlotIndex];
                stats.level = 1;
                for(int j=1; j<levelList[mercenarySlotIndex]; j++)
                {
                    StartCoroutine(stats.LevelUP(false));
                }


                switch(classList[mercenarySlotIndex])
                {
                    case 1:
                        stats.itemList[0] = Instantiate(startingEquipment[11]);
                        DontDestroyOnLoad(stats.itemList[0]);
                        //tier 1
                        stats.swordSkill = 1;
                        stats.axeSkill = 0;
                        stats.maceSkill = 0;
                        stats.spearSkill = 0;
                        stats.crossbowSkill = 0;
                        stats.bowSkill = 0;
                        stats.shieldSkill = 0;
                        stats.staffSkill = -1;
                        //tier 2
                        stats.quickdrawSkill = 0;
                        stats.oneHandedSkill = 0;
                        stats.dualwieldingSkill = 0;
                        stats.twohandedSkill = 0;
                        stats.acrobatSkill = 0;
                        stats.defenderSkill = 0;
                        stats.athleteSkill = 0;
                        stats.armorhabituationSkill = 0;
                        stats.armormasterSkill = 0;
                        stats.magicFlowSkill = 0;
                        //tier 3
                        stats.sworddancerSkill = 0;
                        stats.armorbreakerSkill = -1;
                        stats.basherSkill = -1;
                        stats.holdTheLineSkill = -1;
                        stats.hawkeyeSkill = -1;
                        stats.quickdrawSkill = -1;
                        stats.quickshotSkill = -1;
                        stats.bufferSkill = -1;
                        stats.debufferSkill = 0;
                        stats.wisdomSkill = -1;
                        //tier 4
                        //tier 5
                        stats.specialistSkill = 0;
                        stats.lionheartSkill = 0;
                        stats.marathonerSkill = 0;
                        stats.unstoppableSkill = 0;
                        stats.survivalistSkill = 0;
                        stats.elementalistSkill = -1;
                        stats.magicResistanceSkill = 0;

                    break;
                    case 2:
                        stats.itemList[0] = Instantiate(startingEquipment[12]);
                        DontDestroyOnLoad(stats.itemList[0]);
                        //tier 1
                        stats.swordSkill = 0;
                        stats.axeSkill = 1;
                        stats.maceSkill = 0;
                        stats.spearSkill = 0;
                        stats.crossbowSkill = 0;
                        stats.bowSkill = 0;
                        stats.shieldSkill = 0;
                        stats.staffSkill = -1;
                        //tier 2
                        stats.quickdrawSkill = 0;
                        stats.oneHandedSkill = 0;
                        stats.dualwieldingSkill = 0;
                        stats.twohandedSkill = 0;
                        stats.acrobatSkill = 0;
                        stats.defenderSkill = 0;
                        stats.athleteSkill = 0;
                        stats.armorhabituationSkill = 0;
                        stats.armormasterSkill = 0;
                        stats.magicFlowSkill = 0;
                        //tier 3
                        stats.sworddancerSkill = -1;
                        stats.armorbreakerSkill = 0;
                        stats.basherSkill = -1;
                        stats.holdTheLineSkill = -1;
                        stats.hawkeyeSkill = -1;
                        stats.quickdrawSkill = -1;
                        stats.quickshotSkill = -1;
                        stats.bufferSkill = -1;
                        stats.debufferSkill = 0;
                        stats.wisdomSkill = -1;
                        //tier 4
                        //tier 5
                        stats.specialistSkill = 0;
                        stats.lionheartSkill = 0;
                        stats.marathonerSkill = 0;
                        stats.unstoppableSkill = 0;
                        stats.survivalistSkill = 0;
                        stats.elementalistSkill = -1;
                        stats.magicResistanceSkill = 0;
                    break;
                    case 3:
                        stats.itemList[0] = Instantiate(startingEquipment[13]);
                        DontDestroyOnLoad(stats.itemList[0]);
                        //tier 1
                        stats.swordSkill = 0;
                        stats.axeSkill = 0;
                        stats.maceSkill = 1;
                        stats.spearSkill = 0;
                        stats.crossbowSkill = 0;
                        stats.bowSkill = 0;
                        stats.shieldSkill = 0;
                        stats.staffSkill = -1;
                        //tier 2
                        stats.quickdrawSkill = 0;
                        stats.oneHandedSkill = 0;
                        stats.dualwieldingSkill = 0;
                        stats.twohandedSkill = 0;
                        stats.acrobatSkill = 0;
                        stats.defenderSkill = 0;
                        stats.athleteSkill = 0;
                        stats.armorhabituationSkill = 0;
                        stats.armormasterSkill = 0;
                        stats.magicFlowSkill = 0;
                        //tier 3
                        stats.sworddancerSkill = -1;
                        stats.armorbreakerSkill = -1;
                        stats.basherSkill = 0;
                        stats.holdTheLineSkill = -1;
                        stats.hawkeyeSkill = -1;
                        stats.quickdrawSkill = -1;
                        stats.quickshotSkill = -1;
                        stats.bufferSkill = -1;
                        stats.debufferSkill = 0;
                        stats.wisdomSkill = -1;
                        //tier 4
                        //tier 5
                        stats.specialistSkill = 0;
                        stats.lionheartSkill = 0;
                        stats.marathonerSkill = 0;
                        stats.unstoppableSkill = 0;
                        stats.survivalistSkill = 0;
                        stats.elementalistSkill = -1;
                        stats.magicResistanceSkill = 0;
                    break;
                    case 4:
                        stats.itemList[0] = Instantiate(startingEquipment[14]);
                        DontDestroyOnLoad(stats.itemList[0]);
                        //tier 1
                        stats.swordSkill = 0;
                        stats.axeSkill = 0;
                        stats.maceSkill = 0;
                        stats.spearSkill = 1;
                        stats.crossbowSkill = 0;
                        stats.bowSkill = 0;
                        stats.shieldSkill = 0;
                        stats.staffSkill = -1;
                        //tier 2
                        stats.quickdrawSkill = 0;
                        stats.oneHandedSkill = 0;
                        stats.dualwieldingSkill = 0;
                        stats.twohandedSkill = 0;
                        stats.acrobatSkill = 0;
                        stats.defenderSkill = 0;
                        stats.athleteSkill = 0;
                        stats.armorhabituationSkill = 0;
                        stats.armormasterSkill = 0;
                        stats.magicFlowSkill = 0;
                        //tier 3
                        stats.sworddancerSkill = -1;
                        stats.armorbreakerSkill = -1;
                        stats.basherSkill = -1;
                        stats.holdTheLineSkill = 0;
                        stats.hawkeyeSkill = -1;
                        stats.quickdrawSkill = -1;
                        stats.quickshotSkill = -1;
                        stats.bufferSkill = -1;
                        stats.debufferSkill = 0;
                        stats.wisdomSkill = -1;
                        //tier 4
                        //tier 5
                        stats.specialistSkill = 0;
                        stats.lionheartSkill = 0;
                        stats.marathonerSkill = 0;
                        stats.unstoppableSkill = 0;
                        stats.survivalistSkill = 0;
                        stats.elementalistSkill = -1;
                        stats.magicResistanceSkill = 0;
                    break;
                    case 5:
                        stats.itemList[0] = Instantiate(startingEquipment[15]);
                        DontDestroyOnLoad(stats.itemList[0]);
                        //tier 1
                        stats.swordSkill = 0;
                        stats.axeSkill = 0;
                        stats.maceSkill = 0;
                        stats.spearSkill = 0;
                        stats.crossbowSkill = 1;
                        stats.bowSkill = 0;
                        stats.shieldSkill = 0;
                        stats.staffSkill = -1;
                        //tier 2
                        stats.quickdrawSkill = 0;
                        stats.oneHandedSkill = 0;
                        stats.dualwieldingSkill = 0;
                        stats.twohandedSkill = 0;
                        stats.acrobatSkill = -1;
                        stats.defenderSkill = -1;
                        stats.athleteSkill = -1;
                        stats.armorhabituationSkill = 0;
                        stats.armormasterSkill = 0;
                        stats.magicFlowSkill = 0;
                        //tier 3
                        stats.sworddancerSkill = -1;
                        stats.armorbreakerSkill = -1;
                        stats.basherSkill = -1;
                        stats.holdTheLineSkill = -1;
                        stats.hawkeyeSkill = 0;
                        stats.quickshotSkill = 0;
                        stats.wisdomSkill = -1;
                        stats.bufferSkill = -1;
                        stats.debufferSkill = 0;
                        //tier 4
                        //tier 5
                        stats.specialistSkill = 0;
                        stats.lionheartSkill = 0;
                        stats.marathonerSkill = 0;
                        stats.unstoppableSkill = 0;
                        stats.survivalistSkill = 0;
                        stats.elementalistSkill = -1;
                        stats.magicResistanceSkill = 0;
                    break;
                    case 6:
                        stats.itemList[0] = Instantiate(startingEquipment[16]);
                        DontDestroyOnLoad(stats.itemList[0]);
                        //tier 1
                        stats.swordSkill = 0;
                        stats.axeSkill = 0;
                        stats.maceSkill = 0;
                        stats.spearSkill = 0;
                        stats.crossbowSkill = 0;
                        stats.bowSkill = 1;
                        stats.shieldSkill = 0;
                        stats.staffSkill = -1;
                        //tier 2
                        stats.quickdrawSkill = 0;
                        stats.oneHandedSkill = 0;
                        stats.dualwieldingSkill = 0;
                        stats.twohandedSkill = 0;
                        stats.acrobatSkill = 0;
                        stats.defenderSkill = -1;
                        stats.athleteSkill = 0;
                        stats.armorhabituationSkill = -1;
                        stats.armormasterSkill = -1;
                        stats.magicFlowSkill = 0;
                        //tier 3
                        stats.sworddancerSkill = -1;
                        stats.armorbreakerSkill = -1;
                        stats.basherSkill = -1;
                        stats.holdTheLineSkill = -1;
                        stats.hawkeyeSkill = 0;
                        stats.quickshotSkill = 0;
                        stats.bufferSkill = -1;
                        stats.debufferSkill = 0;
                        stats.wisdomSkill = -1;
                        //tier 4
                        //tier 5
                        stats.specialistSkill = 0;
                        stats.lionheartSkill = 0;
                        stats.marathonerSkill = 0;
                        stats.unstoppableSkill = 0;
                        stats.survivalistSkill = 0;
                        stats.elementalistSkill = -1;
                        stats.magicResistanceSkill = 0;
                    break;
                    case 7:
                        stats.itemList[0] = Instantiate(startingEquipment[17]);
                        DontDestroyOnLoad(stats.itemList[0]);
                        //tier 1
                        stats.swordSkill = 0;
                        stats.axeSkill = 0;
                        stats.maceSkill = 0;
                        stats.spearSkill = 0;
                        stats.crossbowSkill = 0;
                        stats.bowSkill = 0;
                        stats.shieldSkill = -1;
                        stats.staffSkill = 1;
                        //tier 2
                        stats.quickdrawSkill = 0;
                        stats.oneHandedSkill = -1;
                        stats.dualwieldingSkill = -1;
                        stats.twohandedSkill = -1;
                        stats.acrobatSkill = 0;
                        stats.defenderSkill = -1;
                        stats.athleteSkill = 0;
                        stats.armorhabituationSkill = -1;
                        stats.armormasterSkill = -1;
                        stats.magicFlowSkill = 0;
                        //tier 3
                        stats.sworddancerSkill = -1;
                        stats.armorbreakerSkill = -1;
                        stats.basherSkill = -1;
                        stats.holdTheLineSkill = -1;
                        stats.hawkeyeSkill = -1;
                        stats.quickdrawSkill = -1;
                        stats.quickshotSkill = -1;
                        stats.bufferSkill = 0;
                        stats.debufferSkill = 0;
                        stats.wisdomSkill = 0;
                        //tier 4
                        //tier 5
                        stats.specialistSkill = 0;
                        stats.lionheartSkill = 0;
                        stats.marathonerSkill = 0;
                        stats.unstoppableSkill = 0;
                        stats.survivalistSkill = 0;
                        stats.elementalistSkill = 0;
                        stats.magicResistanceSkill = 0;
                    break;
                }


                //increase i over max so loop is stopped and no more units getting overwritten
                i=19;

                stats.LoadItems();
            }
        }
        classList.RemoveAt(mercenarySlotIndex);
        nameList.RemoveAt(mercenarySlotIndex);
        potentialList.RemoveAt(mercenarySlotIndex);
        levelList.RemoveAt(mercenarySlotIndex);
        costList.RemoveAt(mercenarySlotIndex);


        //reactivate auras
        foreach(GameObject unit in UnitSelections.Instance.unitList)
        {
            Unit unitStats = unit.GetComponent<Unit>();
            FighterClasses.Instance.GetPromotedClassBoni(unitStats,unitStats.promotedFighterClass, 1);
        }

        InitializeHireSystem();
    }
           
    
    public void EquipArmor(Fighter stats, CharacterEntity myEntity, WeaponStats weaponStats, int unequip)
    {
        //visual equip
        myEntity.Equip(weaponStats.myItemAppearance);
        //stats equip see equiparmor stats in inventorysys
        stats.maxHP += weaponStats.health * unequip;
        stats.currentHP += weaponStats.health * unequip;
        stats.maxStamina += Mathf.RoundToInt(weaponStats.stamina*(1-stats.armorhabituationSkill*0.2f))* unequip;          //apply armorhabituationskill
        stats.currentStamina += Mathf.RoundToInt(weaponStats.stamina*(1-stats.armorhabituationSkill*0.2f))* unequip;          //apply armorhabituationskill
        stats.armor += Mathf.RoundToInt(weaponStats.armor*(1+stats.armormasterSkill*0.2f))* unequip;                    //apply armormasterskill
        stats.mAcc += weaponStats.mAcc * unequip;
        stats.mEva += weaponStats.mEva * unequip;
        stats.rAcc += weaponStats.rAcc * unequip;
        stats.rEva += weaponStats.rEva * unequip;
        stats.hpBar.maxValue = stats.maxHP;
        stats.hpBar.value = stats.currentHP;
        stats.staminaBar.maxValue = stats.maxStamina;

        
        //UpdateUnitStats();
    }    
    public void HoverEnter(int index)
    {
        HoverOverImage.SetActive(true);
        Vector3 ImageDisposition = new (0, -140,0);
        HoverOverImage.transform.position = Input.mousePosition + ImageDisposition;

        switch (index)
        {
            //titles
            case 0:
                HoverOverText.text = "<color=#DAA520>Hire</color>  \nHire mercenaries by clicking on their names!";
            break;
            case 1:
                HoverOverText.text = "<color=#DAA520>Name</color> \nRepresents the mercenaries name!";
            break;
            case 2:
                HoverOverText.text = "<color=#DAA520>Class</color> \nThe class determines the units attribute affinities, their learnable skills and starting weapon!";
            break;
            case 3:
                HoverOverText.text = "<color=#DAA520>Level</color> \nThis stat shows the mercenaries current level!";
            break;
            case 4:
                HoverOverText.text = "<color=#DAA520>Potential</color> \nThe potential determines how many skill points (not attribute points) a mercenary can accumulate!";
            break;
            case 5:
                HoverOverText.text = "<color=#DAA520>Cost</color> \nShows how much you have to pay to hire the mercenary!";
            break;
            //roster
            case 7:
                HoverOverText.text = "<color=#DAA520>Current Roster</color> \nShows your own mercenaries! Click on their names to remove them!";
            break;
            case 8:
                HoverOverText.text = "<color=#DAA520>Roster Size</color> \nThe maximum roster size depends on your prestige!";
            break;
            case 9:
                HoverOverText.text = "<color=#DAA520>Promotion</color> \nThis indicates if the character has evolved to another class already!";
            break;

            default:
                //get the button index for the class and remove 11 to get to the correspondent place in the item list
                if(index<20)
                {
                    int whichClass = classList[index-10+hirePage*hirePageLength];
                    HoverOverText.text = FighterClasses.Instance.GetClassDescription(whichClass);

                }
                else if(index<30)
                {
                    int whichUnitClass = unitClassList[index-20+rosterPage*rosterPageLength];
                    HoverOverText.text = FighterClasses.Instance.GetClassDescription(whichUnitClass);
                }
                else
                {
                    int whichPromotedClass = unitPromotionList[index-30+rosterPage*rosterPageLength];
                    HoverOverText.text = FighterClasses.Instance.GetPromotedClassDescription(whichPromotedClass);
                }
            break;
            
   
        }
    }

    public void HoverOverExit()
    {
        HoverOverImage.SetActive(false);
    }

    public void SetPage(int page)
    {
        if(page==0)
        {
            pageButtonFramesList[0].SetActive(true);
            pageButtonFramesList[1].SetActive(false);
        }
        else
        {
            pageButtonFramesList[0].SetActive(false);
            pageButtonFramesList[1].SetActive(true);
        }
        if(!disableButtons)
        {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        hirePage = page;
        InitializeHireSystem();
        }
    }    
    public void SetRosterPage(int page)
    {
        //2nd two bottoms in the list for pagebutton frames
        if(page==0)
        {
            pageButtonFramesList[2].SetActive(true);
            pageButtonFramesList[3].SetActive(false);
        }
        else
        {
            pageButtonFramesList[2].SetActive(false);
            pageButtonFramesList[3].SetActive(true);
        }
        if(!disableButtons)
        {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        rosterPage = page;
        InitializeHireSystem();
        }
    }    


    
    public void DisableInput()
    {
        UnitSelections.Instance.DisableUnitSelections();
        disableButtons = true;
        HoverOverExit();
    }
}
