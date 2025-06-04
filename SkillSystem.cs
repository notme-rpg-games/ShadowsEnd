using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSystem : MonoBehaviour
{
    public int swordEffect;
    public int axeEffect;
    public int maceEffect;
    public int spearEffect;
    public int crossbowEffect;
    public int bowEffect;
    public int shieldEffect;
    public int staffEffect; 

    public int oneHandedEffect;
    public int dualWieldingEffect;
    public int twoHandedEffect;
    public int shieldWallEffect;
    public int hawkeyeEffect;
    public int athleteEffect;
    public int armorHabituationEffect;
    public int wisdomEffect;
    public int elementalistEffect;

    public int swordDancerEffect;
    public int armorBreakerEffect;
    public int BasherEffect;
    public int holdTheLineEffect;
    public int tinkerEffect;
    public int quickshotEffect;
    public int armorMasterEffect;
    public int debufferEffect;
    public int bufferEffect;

    public int conterEffect;
    public int whirlWindEffect;
    public int stormtrooperEffect;
    public int lionheartEffect;
    public int acrobatEffect;
    public int defenderEffect;
    public int magicDiffusionEffect;
    public int magicFlowEffect;
    public int magicResistanceEffect;

    public int swordMax;
    public int axeMax;
    public int maceMax;
    public int spearMax;
    public int crossbowMax;
    public int bowMax;
    public int shieldMax;
    public int staffMax; 

    public int oneHandedMax;
    public int dualWieldingMax;
    public int twoHandedMax;
    public int shieldWallMax;
    public int hawkeyeMax;
    public int athleteMax;
    public int armorHabituationMax;
    public int wisdomMax;
    public int elementalistMax;

    public int swordDancerMax;
    public int armorBreakerMax;
    public int BasherMax;
    public int holdTheLineMax;
    public int tinkerMax;
    public int quickshotMax;
    public int armorMasterMax;
    public int debufferMax;
    public int bufferMax;

    public int conterMax;
    public int whirlWindMax;
    public int stormtrooperMax;
    public int lionheartMax;
    public int acrobatMax;
    public int defenderMax;
    public int magicDiffusionMax;
    public int magicFlowMax;
    public int magicResistanceMax;
    private List<bool> SkillIndexMaxedList = new List<bool>();

    private int Tier2LevelAccess = 4;
    private int Tier3LevelAccess = 6;
    private int Tier4LevelAccess = 8;
    private int Tier5LevelAccess = 12;
    public List<int> SkillEffectList;
    public List<int> SkillMaxLevelList;
    public GameObject HoverOverImage;
    public TMP_Text HoverOverText;
    private UnityEngine.Vector3 ImageDisposition;
    public List<GameObject> SkillObjectList;
    public List<Slider> SkillBarList;
    public List<Image> SkillImage;
    public TMP_Text skillPointsText;
    public GameObject acceptChangesCanvas;
    public GameObject WarningObject;
    public bool acceptChanges;
    public bool inputDone;
    public List<int> maxSkillValues;
    public List<int> unitSkillValues;
    public AudioClip warningSound;
    public AudioClip skillUpSound;
    public AudioClip newPromotionClassSound;

    //unitstats
    public List<TMP_Text> unitStatsList; 
    //skillsystem
    public GameObject statbuttons;
    public TMP_Dropdown DropdownButton;

    private Fighter stats;
    private int playerChoice;
    private Color32 colorSkillUpgraded = new Color32(0,200,0,150);
    private Color32 colorSkillNotavailable = new Color32(0,0,0,200);
    private Color32 colorTransparent = new Color32(0,0,0,0);
    public GameObject promotionClassHelpWindow;
    public List<TMP_Text> promotionExplanationListObject;
    private int promotionExplanationListPage;
    public List<GameObject> promotionExplanationPageList;
    public bool tutorialTips = true;
    private static SkillSystem _instance;
    public static SkillSystem Instance { get { return _instance; } }
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

    void Start()
    {
        for(int i=0; i<promotionExplanationListObject.Count; i++)
        {
            if(promotionExplanationListObject[i]!=null)
            {
                promotionExplanationListObject[i].text = FighterClasses.Instance.GetPromotedClassDescription(i);
                promotionExplanationListObject[i].GetComponentInChildren<Image>().sprite = FighterClasses.Instance.GetPromotedClassImage(i);
            }
        }
    }

    public void InitializeSkillScreen()
    {        
        //generate bool list to check if skills are maxed
        for(int i=0; i<60; i++)
        {
            SkillIndexMaxedList.Add(false);
        }

        for(int i=0; i<SkillObjectList.Count; i++)
        {
            if(SkillObjectList[i]!=null)
            {
                SkillObjectList[i].GetComponent<EventTrigger>().enabled = true;
            }
        }

        stats = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();
        FighterClasses.Instance.GetDropDownPromotion(stats);
        UpdateUnitStats();
        UpdateUnitSkills();

        //make tier skills available
        if(stats.level>=Tier5LevelAccess)
        {
            SkillImage[20].gameObject.SetActive(false);
            SkillImage[30].gameObject.SetActive(false);
            SkillImage[40].gameObject.SetActive(false);
            SkillImage[50].gameObject.SetActive(false);
        }
        else if(stats.level>=Tier4LevelAccess)
        {
            SkillImage[20].gameObject.SetActive(false);
            SkillImage[30].gameObject.SetActive(false);
            SkillImage[40].gameObject.SetActive(false);
            SkillImage[50].gameObject.SetActive(true);
        }
        else if(stats.level>=Tier3LevelAccess)
        {
            SkillImage[20].gameObject.SetActive(false);
            SkillImage[30].gameObject.SetActive(false);
            SkillImage[40].gameObject.SetActive(true);
            SkillImage[50].gameObject.SetActive(true);
        }
        else if(stats.level>=Tier2LevelAccess)
        {
            SkillImage[20].gameObject.SetActive(false);
            SkillImage[30].gameObject.SetActive(true);
            SkillImage[40].gameObject.SetActive(true);
            SkillImage[50].gameObject.SetActive(true);
        }
        else
        {
            SkillImage[20].gameObject.SetActive(true);
            SkillImage[30].gameObject.SetActive(true);
            SkillImage[40].gameObject.SetActive(true);
            SkillImage[50].gameObject.SetActive(true);
        }    
        
        if(tutorialTips)
        {
            GameManager.Instance.tipIndex= 1;
            GameManager.Instance.Tutorial();
            DisableInput();
        }
    }

    public void IncreaseSkill1(int index)
    {
        if(stats.skillPoints>0)
        {
            if(index > 50 && stats.level<Tier5LevelAccess || index >30 && stats.level<Tier3LevelAccess || index >20 && stats.level<Tier2LevelAccess)
            {
                StartCoroutine(LevelTooLow());
            }
            else if(SkillMaxedCheck(index))
            { 
                StartCoroutine(SkillAlreadyMaxed());
            } 
            else if(index ==29 && stats.elementalistSkill==0)      //elementalist
            {
                StartCoroutine(ChooseElement(index));
            }
            else
            {
                StartCoroutine(IncreaseSkill2(index));
            }
        }
        else
        {
            StartCoroutine(NoSkillPointsAvailable());
        }
    }
    public IEnumerator IncreaseSkill2(int index)
    {
        DisableInput();
        acceptChangesCanvas.SetActive(true);
        inputDone = false;
        while (!inputDone)
        {
            yield return null;
        }

        if(acceptChanges)
        {
            IncreaseSkill3(index);
        }

        acceptChangesCanvas.SetActive(false);
    }

    
    public IEnumerator ChooseElement(int index)
    {
        DisableInput();
        playerChoice = -1;                  //reset choice
        acceptChangesCanvas.SetActive(true);
        DropdownButton.gameObject.SetActive(true);
        inputDone = false;
        while (!inputDone)
        {
            yield return null;
        }

        if(acceptChanges && playerChoice!=-1)
        {
            IncreaseSkill3(index);
        }

        acceptChangesCanvas.SetActive(false);
        DropdownButton.gameObject.SetActive(false);
    }

    public void DropDownValue()
    {
        playerChoice = DropdownButton.value;
        Debug.Log("choice" +playerChoice);
    }

    public void Accept()
    {
        acceptChanges = true;
        inputDone = true;
        UnitSelections.Instance.EnableUnitSelections();
    }    
    public void Deny()
    {
        acceptChanges = false;
        inputDone = true;
        UnitSelections.Instance.EnableUnitSelections();
        InitializeSkillScreen();
    }
    public void IncreaseSkill3(int index)
    {
        stats.skillPoints--;
        //unequip weapon to reapply the correct values after reequip
        if(stats.skill4Active)
        {
            WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], -1);
        }
        else
        {
            WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], -1);
        }

        //add skill point
        switch(index)
        {
            case 11:
                stats.swordSkill += 1;
            break;
            case 12:
                stats.axeSkill += 1;
            break;
            case 13:
                stats.maceSkill += 1;
            break;
            case 14:
                stats.spearSkill += 1;
            break;
            case 15:
                stats.crossbowSkill += 1;
            break;
            case 16:
                stats.bowSkill += 1;
            break;
            case 17:
                stats.shieldSkill += 1;
            break;
            case 18:
                stats.staffSkill += 1;
            break;

            case 21:
                stats.oneHandedSkill += 1;
            break;
            case 22:
                stats.dualwieldingSkill += 1;
            break;
            case 23:
                stats.twohandedSkill += 1;
            break;
            case 24:
                stats.shieldWallSkill += 1;
            break;
            case 25:
                stats.hawkeyeSkill += 1;
            break;
            case 26:
                stats.athleteSkill += 1;
                stats.speed += 0.5f;
                stats.rEva += athleteEffect;
                if(!GameManager.Instance.cityMap)
                {
                    stats.myAgent.speed += 0.5f;
                }
            break;
            case 27:
                InventorySystem.Instance.EquipAllArmorStats(-1);
                stats.armorhabituationSkill += 1;
                InventorySystem.Instance.EquipAllArmorStats(1);
            break;
            case 28:
                stats.wisdomSkill += 1;
            break;
            case 29:
                if(stats.elementalistSkill ==0)
                {
                    stats.elementalistSkillElement = playerChoice;
                }
                stats.elementalistSkill += 1;
            break;
            //tier 3 - weaponspecialist skills
            case 31:
                stats.sworddancerSkill += 1;
            break;
            case 32:
                stats.armorbreakerSkill += 1;
            break;
            case 33:
                stats.basherSkill += 1;
            break;
            case 34:
                stats.holdTheLineSkill += 1;
            break;
            case 35:
                stats.tinkerSkill += 1;
            break;
            case 36:
                stats.quickshotSkill += 1;
            break;
            case 37:       
                InventorySystem.Instance.EquipAllArmorStats(-1);
                stats.armormasterSkill += 1;
                InventorySystem.Instance.EquipAllArmorStats(1);
            break;
            case 38:
                stats.debufferSkill += 1;
            break;
            case 39:
                stats.bufferSkill += 1;
            break;
            //crafts
            case 51:
                stats.conterSkill += 1;
            break;
            case 52:
                stats.whirlWindSkill ++;
            break;
            case 53:
                stats.stormTrooperSkill++;
            break;
            case 54:
                stats.lionheartSkill++;
            break;
            case 55:
                stats.acrobatSkill++;
            break;
            case 56:
                stats.defenderSkill++;
            break;
            case 57:
                stats.magicDiffusionSkill++;
            break;
            case 58:
                stats.magicFlowSkill++;
            break;
            case 59:
                stats.magicResistanceSkill++;
            break;


        }

        //re-equip weapons
        if(stats.skill4Active)
        {
            WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], 1);
        }
        else
        {
            WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], 1);
        }

        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(skillUpSound, GameManager.Instance.sfxVolume);
        FighterClasses.Instance.CheckForPromotions(stats);
        InitializeSkillScreen();
    }

    public bool SkillMaxedCheck(int index)
    {
        bool maxed = false;
        //add skill point
        switch(index)
        {
            case 11:
                if(stats.swordSkill == swordMax)
                {
                    maxed = true;
                }
            break;
            case 12:
                if(stats.axeSkill == axeMax)
                {
                    maxed = true;
                }
            break;
            case 13:
                if(stats.maceSkill == maceMax)
                {
                    maxed = true;
                }
            break;
            case 14:
                if(stats.spearSkill == spearMax)
                {
                    maxed = true;
                }
            break;
            case 15:
                if(stats.crossbowSkill == crossbowMax)
                {
                    maxed = true;
                }
            break;
            case 16:
                if(stats.bowSkill == bowMax)
                {
                    maxed = true;
                }
            break;
            case 17:
                if(stats.shieldSkill == shieldMax)
                {
                    maxed = true;
                }
            break;
            case 18:
                if(stats.staffSkill == staffMax)
                {
                    maxed = true;
                }
            break;

            case 21:
                if(stats.oneHandedSkill == oneHandedMax)
                {
                    maxed = true;
                }
            break;
            case 22:
                if(stats.dualwieldingSkill == dualWieldingMax)
                {
                    maxed = true;
                }
            break;
            case 23:
                if(stats.twohandedSkill == twoHandedMax)
                {
                    maxed = true;
                }
            break;
            case 24:
                if(stats.shieldWallSkill == shieldWallMax)
                {
                    maxed = true;
                }
            break;
            case 25:
                if(stats.hawkeyeSkill == hawkeyeMax)
                {
                    maxed = true;
                }
            break;
            case 26:
                if(stats.athleteSkill == athleteMax)
                {
                    maxed = true;
                }
            break;
            case 27:
                if(stats.armorhabituationSkill == armorHabituationMax)
                {
                    maxed = true;
                }
            break;
            case 28:
                if(stats.wisdomSkill == wisdomMax)
                {
                    maxed = true;
                }
            break;
            case 29:
                if(stats.elementalistSkill == elementalistMax)
                {
                    maxed = true;
                }
            break;
            //tier 3 - weaponspecialist skills
            case 31:
                if(stats.sworddancerSkill == swordDancerMax)
                {
                    maxed = true;
                }
            break;
            case 32:
                if(stats.armorbreakerSkill == armorBreakerMax)
                {
                    maxed = true;
                }
            break;
            case 33:
                if(stats.basherSkill == BasherMax)
                {
                    maxed = true;
                }
            break;
            case 34:
                if(stats.holdTheLineSkill == holdTheLineMax)
                {
                    maxed = true;
                }
            break;
            case 35:
                if(stats.tinkerSkill == tinkerMax)
                {
                    maxed = true;
                }
            break;
            case 36:
                if(stats.quickshotSkill == quickshotMax)
                {
                    maxed = true;
                }
            break;
            case 37:       
                if(stats.armormasterSkill == armorMasterMax)
                {
                    maxed = true;
                }
            break;
            case 38:
                if(stats.debufferSkill == debufferMax)
                {
                    maxed = true;
                }
            break;
            case 39:
                if(stats.bufferSkill == bufferMax)
                {
                    maxed = true;
                }
            break;
            //crafts
            case 51:
                if(stats.conterSkill == conterMax)
                {
                    maxed = true;
                }
            break;
            case 52:
                if(stats.whirlWindSkill == whirlWindMax)
                {
                    maxed = true;
                }
            break;
            case 53:
                if(stats.stormTrooperSkill == stormtrooperMax)
                {
                    maxed = true;
                }
            break;
            case 54:
                if(stats.lionheartSkill == lionheartMax)
                {
                    maxed = true;
                }
            break;
            case 55:
                if(stats.acrobatSkill == acrobatMax)
                {
                    maxed = true;
                }
            break;
            case 56:
                if(stats.defenderSkill == defenderMax)
                {
                    maxed = true;
                }
            break;
            case 57:
                if(stats.magicDiffusionSkill == magicDiffusionMax)
                {
                    maxed = true;
                }
            break;
            case 58:
                if(stats.magicFlowSkill == magicFlowMax)
                {
                    maxed = true;
                }
            break;
            case 59:
                if(stats.magicResistanceSkill == magicResistanceMax)
                {
                    maxed = true;
                }
            break;

        }

        return maxed;
    }

    //update the skill indicators
    void UpdateUnitSkills()
    {
        skillPointsText.text = "("+stats.skillPoints+")";
        //tier 1 weapon skills
        if(stats.swordSkill == -1)
        {
            SkillObjectList[1].SetActive(false);
        }
        else
        {
            SkillObjectList[1].SetActive(true);
            SkillBarList[1].value = stats.swordSkill;
            SkillBarList[1].maxValue = swordMax;
            
            if(stats.swordSkill == swordMax)
            {
                SkillIndexMaxedList[11] = true;
            }
        }

        if(stats.axeSkill == -1)
        {
            SkillObjectList[2].SetActive(false);
        }
        else
        {
            SkillObjectList[2].SetActive(true);
            SkillBarList[2].value = stats.axeSkill;
            SkillBarList[2].maxValue = axeMax;
            
            if(stats.axeSkill == axeMax)
            {
                SkillIndexMaxedList[12] = true;
            }
        }

        if(stats.maceSkill == -1)
        {
            SkillObjectList[3].SetActive(false);
        }
        else
        {
            SkillObjectList[3].SetActive(true);
            SkillBarList[3].value = stats.maceSkill;
            SkillBarList[3].maxValue = maceMax;

            if(stats.maceSkill == maceMax)
            {
                SkillIndexMaxedList[13] = true;
            }
        }

        if(stats.spearSkill == -1)
        {
            SkillObjectList[4].SetActive(false);
        }
        else
        {
            SkillObjectList[4].SetActive(true);
            SkillBarList[4].value = stats.spearSkill;
            SkillBarList[4].maxValue = spearMax;
            
            if(stats.spearSkill == spearMax)
            {
                SkillIndexMaxedList[14] = true;
            }
        }

        
        if(stats.crossbowSkill == -1)
        {
            SkillObjectList[5].SetActive(false);
        }
        else
        {
            SkillObjectList[5].SetActive(true);
            SkillBarList[5].value = stats.crossbowSkill;
            SkillBarList[5].maxValue = crossbowMax;
            
            if(stats.crossbowSkill == crossbowMax)
            {
                SkillIndexMaxedList[15] = true;
            }
        }
        
        if(stats.bowSkill == -1)
        {
            SkillObjectList[6].SetActive(false);
        }
        else
        {
            SkillObjectList[6].SetActive(true);
            SkillBarList[6].value = stats.bowSkill;
            SkillBarList[6].maxValue = bowMax;
            
            if(stats.bowSkill == bowMax)
            {
                SkillIndexMaxedList[16] = true;
            }
        }
        
        if(stats.shieldSkill == -1)
        {
            SkillObjectList[7].SetActive(false);
        }
        else
        {
            SkillObjectList[7].SetActive(true);
            SkillBarList[7].value = stats.shieldSkill;
            SkillBarList[7].maxValue = shieldMax;
            
            if(stats.shieldSkill== shieldMax)
            {
                SkillIndexMaxedList[17] = true;
            }
        }
        
        if(stats.staffSkill == -1)
        {
            SkillObjectList[8].SetActive(false);
        }
        else
        {
            SkillObjectList[8].SetActive(true);
            SkillBarList[8].value = stats.staffSkill;
            SkillBarList[8].maxValue = staffMax;
            
            if(stats.staffSkill == staffMax)
            {
                SkillIndexMaxedList[18] = true;
            }
        }
        
        //tier2 - utility weapon skills
/*         if(stats.quickdrawSkill == -1)
        {
            SkillObjectList[11].SetActive(false);
        }
        else
        {
            SkillObjectList[11].SetActive(true);
            SkillBarList[11].value = stats.quickdrawSkill;
            SkillBarList[11].maxValue = 1;
        } */
        if(stats.oneHandedSkill == -1)
        {
            SkillObjectList[11].SetActive(false);
        }
        else
        {
            SkillObjectList[11].SetActive(true);
            SkillBarList[11].value = stats.oneHandedSkill;
            SkillBarList[11].maxValue = oneHandedMax;
            
            if(stats.oneHandedSkill == oneHandedMax)
            {
                SkillIndexMaxedList[21] = true;
            }
        } 
                
        if(stats.dualwieldingSkill == -1)
        {
            SkillObjectList[12].SetActive(false);
        }
        else
        {
            SkillObjectList[12].SetActive(true);
            SkillBarList[12].value = stats.dualwieldingSkill;
            SkillBarList[12].maxValue = dualWieldingMax;
            
            if(stats.dualwieldingSkill == dualWieldingMax)
            {
                SkillIndexMaxedList[22] = true;
            }
        }     

        if(stats.twohandedSkill == -1)
        {
            SkillObjectList[13].SetActive(false);
        }
        else
        {
            SkillObjectList[13].SetActive(true);
            SkillBarList[13].value = stats.twohandedSkill;
            SkillBarList[13].maxValue = twoHandedMax;
            
            if(stats.twohandedSkill == twoHandedMax)
            {
                SkillIndexMaxedList[23] = true;
            }
        }         
/*         if(stats.acrobatSkill == -1)
        {
            SkillObjectList[14].SetActive(false);
        }
        else
        {
            SkillObjectList[14].SetActive(true);
            SkillBarList[14].value = stats.acrobatSkill;
            SkillBarList[14].maxValue = acrobatMax;
        }      
        if(stats.defenderSkill == -1)
        {
            SkillObjectList[15].SetActive(false);
        }
        else
        {
            SkillObjectList[15].SetActive(true);
            SkillBarList[15].value = stats.defenderSkill;
            SkillBarList[15].maxValue = defenderMax;
        }      */   
        if(stats.shieldWallSkill == -1)
        {
            SkillObjectList[14].SetActive(false);
        }
        else
        {
            SkillObjectList[14].SetActive(true);
            SkillBarList[14].value = stats.shieldWallSkill;
            SkillBarList[14].maxValue = shieldWallMax;
            
            if(stats.shieldWallSkill == shieldWallMax)
            {
                SkillIndexMaxedList[24] = true;
            }
        }      

        if(stats.hawkeyeSkill == -1)
        {
            SkillObjectList[15].SetActive(false);
        }
        else
        {
            SkillObjectList[15].SetActive(true);
            SkillBarList[15].value = stats.hawkeyeSkill;
            SkillBarList[15].maxValue = hawkeyeMax;
            
            if(stats.hawkeyeSkill == hawkeyeMax)
            {
                SkillIndexMaxedList[25] = true;
            }
        }   

        if(stats.athleteSkill == -1)
        {
            SkillObjectList[16].SetActive(false);
        }
        else
        {
            SkillObjectList[16].SetActive(true);
            SkillBarList[16].value = stats.athleteSkill;
            SkillBarList[16].maxValue = athleteMax;
            
            if(stats.oneHandedSkill == oneHandedMax)
            {
                SkillIndexMaxedList[26] = true;
            }
        }         

        if(stats.armorhabituationSkill == -1)
        {
            SkillObjectList[17].SetActive(false);
        }
        else
        {
            SkillObjectList[17].SetActive(true);
            SkillBarList[17].value = stats.armorhabituationSkill;
            SkillBarList[17].maxValue = armorHabituationMax;
            
            if(stats.armorhabituationSkill == armorHabituationMax)
            {
                SkillIndexMaxedList[27] = true;
            }
        }     
        if(stats.wisdomSkill == -1)
        {
            SkillObjectList[18].SetActive(false);
        }
        else
        {
            SkillObjectList[18].SetActive(true);
            SkillBarList[18].value = stats.wisdomSkill;
            SkillBarList[18].maxValue = wisdomMax;
            
            if(stats.wisdomSkill == wisdomMax)
            {
                SkillIndexMaxedList[28] = true;
            }
        }     
        if(stats.elementalistSkill == -1)
        {
            SkillObjectList[19].SetActive(false);
        }
        else
        {
            SkillObjectList[19].SetActive(true);
            SkillBarList[19].value = stats.elementalistSkill;
            SkillBarList[19].maxValue = elementalistMax;
            
            if(stats.elementalistSkill == elementalistMax)
            {
                SkillIndexMaxedList[29] = true;
            }
        }     

        //tier3 weapon specialisations
        if(stats.sworddancerSkill == -1)
        {
            SkillObjectList[21].SetActive(false);
        }
        else
        {
            SkillObjectList[21].SetActive(true);
            SkillBarList[21].value = stats.sworddancerSkill;
            SkillBarList[21].maxValue = swordDancerMax;
            
            if(stats.sworddancerSkill == swordDancerMax)
            {
                SkillIndexMaxedList[31] = true;
            }
        }     
        if(stats.armorbreakerSkill == -1)
        {
            SkillObjectList[22].SetActive(false);
        }
        else
        {
            SkillObjectList[22].SetActive(true);
            SkillBarList[22].value = stats.armorbreakerSkill;
            SkillBarList[22].maxValue = armorBreakerMax;
            
            if(stats.armorbreakerSkill == armorBreakerMax)
            {
                SkillIndexMaxedList[32] = true;
            }
        }     
        if(stats.basherSkill == -1)
        {
            SkillObjectList[23].SetActive(false);
        }
        else
        {
            SkillObjectList[23].SetActive(true);
            SkillBarList[23].value = stats.basherSkill;
            SkillBarList[23].maxValue = BasherMax;
            
            if(stats.basherSkill == BasherMax)
            {
                SkillIndexMaxedList[33] = true;
            }
        }     
        if(stats.holdTheLineSkill == -1)
        {
            SkillObjectList[24].SetActive(false);
        }
        else
        {
            SkillObjectList[24].SetActive(true);
            SkillBarList[24].value = stats.holdTheLineSkill;
            SkillBarList[24].maxValue = holdTheLineMax;
            
            if(stats.holdTheLineSkill == holdTheLineMax)
            {
                SkillIndexMaxedList[34] = true;
            }
        }     
        if(stats.tinkerSkill == -1)
        {
            SkillObjectList[25].SetActive(false);
        }
        else
        {
            SkillObjectList[25].SetActive(true);
            SkillBarList[25].value = stats.tinkerSkill;
            SkillBarList[25].maxValue = tinkerMax;
            
            if(stats.tinkerSkill == tinkerMax)
            {
                SkillIndexMaxedList[35] = true;
            }
        }     
        if(stats.quickshotSkill == -1)
        {
            SkillObjectList[26].SetActive(false);
        }
        else
        {
            SkillObjectList[26].SetActive(true);
            SkillBarList[26].value = stats.quickshotSkill;
            SkillBarList[26].maxValue = quickshotMax;
            
            if(stats.quickshotSkill == quickshotMax)
            {
                SkillIndexMaxedList[36] = true;
            }
        }     
        if(stats.armormasterSkill == -1)
        {
            SkillObjectList[27].SetActive(false);
        }
        else
        {
            SkillObjectList[27].SetActive(true);
            SkillBarList[27].value = stats.armormasterSkill;
            SkillBarList[27].maxValue = armorMasterMax;
            
            if(stats.armormasterSkill == armorMasterMax)
            {
                SkillIndexMaxedList[37] = true;
            }
        }     
        if(stats.debufferSkill == -1)
        {
            SkillObjectList[28].SetActive(false);
        }
        else
        {
            SkillObjectList[28].SetActive(true);
            SkillBarList[28].value = stats.debufferSkill;
            SkillBarList[28].maxValue = debufferMax;
            
            if(stats.debufferSkill == debufferMax)
            {
                SkillIndexMaxedList[38] = true;
            }
        }     
        if(stats.bufferSkill == -1)
        {
            SkillObjectList[29].SetActive(false);
        }
        else
        {
            SkillObjectList[29].SetActive(true);
            SkillBarList[29].value = stats.bufferSkill;
            SkillBarList[29].maxValue = bufferMax;
            
            if(stats.bufferSkill == bufferMax)
            {
                SkillIndexMaxedList[39] = true;
            }
        }     

        //tier 4 - craft 

        //tier 5 - utility
/*         if(stats.specialistSkill == -1)
        {
            SkillObjectList[41].SetActive(false);
        }
        else
        {
            SkillObjectList[41].SetActive(true);
            SkillBarList[41].value = stats.specialistSkill;
            SkillBarList[41].maxValue = 3;
        }   */        
        if(stats.conterSkill == -1)
        {
            SkillObjectList[41].SetActive(false);
        }
        else
        {
            SkillObjectList[41].SetActive(true);
            SkillBarList[41].value = stats.conterSkill;
            SkillBarList[41].maxValue = conterMax;
            
            if(stats.conterSkill == conterMax)
            {
                SkillIndexMaxedList[51] = true;
            }
        }     
        if(stats.whirlWindSkill == -1)
        {
            SkillObjectList[42].SetActive(false);
        }
        else
        {
            SkillObjectList[42].SetActive(true);
            SkillBarList[42].value = stats.whirlWindSkill ;
            SkillBarList[42].maxValue = whirlWindMax;
            
            if(stats.whirlWindSkill == whirlWindMax)
            {
                SkillIndexMaxedList[52] = true;
            }
        }     
        if(stats.stormTrooperSkill == -1)
        {
            SkillObjectList[43].SetActive(false);
        }
        else
        {
            SkillObjectList[43].SetActive(true);
            SkillBarList[43].value = stats.stormTrooperSkill;
            SkillBarList[43].maxValue = stormtrooperMax;
            
            if(stats.stormTrooperSkill == stormtrooperMax)
            {
                SkillIndexMaxedList[53] = true;
            }
        }     
        if(stats.lionheartSkill == -1)
        {
            SkillObjectList[44].SetActive(false);
        }
        else
        {
            SkillObjectList[44].SetActive(true);
            SkillBarList[44].value = stats.lionheartSkill;
            SkillBarList[44].maxValue = lionheartMax;
            
            if(stats.lionheartSkill == lionheartMax)
            {
                SkillIndexMaxedList[54] = true;
            }
        }     
        if(stats.acrobatSkill == -1)
        {
            SkillObjectList[45].SetActive(false);
        }
        else
        {
            SkillObjectList[45].SetActive(true);
            SkillBarList[45].value = stats.acrobatSkill;
            SkillBarList[45].maxValue = acrobatMax;
            
            if(stats.acrobatSkill == acrobatMax)
            {
                SkillIndexMaxedList[55] = true;
            }
        }     
        if(stats.defenderSkill == -1)
        {
            SkillObjectList[46].SetActive(false);
        }
        else
        {
            SkillObjectList[46].SetActive(true);
            SkillBarList[46].value = stats.defenderSkill;
            SkillBarList[46].maxValue = defenderMax;
            
            if(stats.defenderSkill == defenderMax)
            {
                SkillIndexMaxedList[56] = true;
            }
        }   
        if(stats.magicDiffusionSkill == -1)
        {
            SkillObjectList[47].SetActive(false);
        }
        else
        {
            SkillObjectList[47].SetActive(true);
            SkillBarList[47].value = stats.magicDiffusionSkill;
            SkillBarList[47].maxValue = magicDiffusionMax;
            
            if(stats.magicDiffusionSkill == magicDiffusionMax)
            {
                SkillIndexMaxedList[57] = true;
            }
        }     
        if(stats.magicFlowSkill == -1)
        {
            SkillObjectList[48].SetActive(false);
        }
        else
        {
            SkillObjectList[48].SetActive(true);
            SkillBarList[48].value = stats.magicFlowSkill;
            SkillBarList[48].maxValue = magicFlowMax;
            
            if(stats.magicFlowSkill == magicFlowMax)
            {
                SkillIndexMaxedList[58] = true;
            }
        }        
        if(stats.magicResistanceSkill == -1)
        {
            SkillObjectList[49].SetActive(false);
        }
        else
        {
            SkillObjectList[49].SetActive(true);
            SkillBarList[49].value = stats.magicResistanceSkill;
            SkillBarList[49].maxValue = magicResistanceMax;
            
            if(stats.magicResistanceSkill == magicResistanceMax)
            {
                SkillIndexMaxedList[59] = true;
            }
        }     
        SkillImage[61].sprite = FighterClasses.Instance.GetClassImage(stats.fighterClass);
        
        SkillImage[62].sprite = FighterClasses.Instance.GetPromotedClassImage(stats.promotedFighterClass);
        
    }

    public void HoverOverSkill(int index)
    {
        HoverOverImage.SetActive(true);
/*         if(Input.mousePosition.y >500)
        { */
            ImageDisposition = new (-100,-200,0);
/*         }
        else
        {
            ImageDisposition = new (0,+200,0);
        } */
        switch (index)
        {
            //tier1
            case 10:
                HoverOverText.text = "<color=#DAA520>Weaponskills</color>  \nBasic Skills";
            break;
            case 11:
                HoverOverText.text = "<color=#DAA520>Swordskill("+stats.swordSkill +"/"+swordMax+")</color> \nIncrease melee accuracy and evasion with swords by "+swordEffect;
            break;
            case 12:
                HoverOverText.text = "<color=#DAA520>Axeskill("+stats.axeSkill +"/"+axeMax+")</color> \nIncrease melee accuracy and evasion with axes by "+axeEffect;
            break;
            case 13:
                HoverOverText.text = "<color=#DAA520>Maceskill("+stats.maceSkill +"/"+maceMax+")</color> \nIncrease melee accuracy and evasion with maces by "+maceEffect;
            break;
            case 14:
                HoverOverText.text = "<color=#DAA520>Pikeskill("+stats.spearSkill +"/"+spearMax+")</color> \nIncrease melee accuracy and evasion with spears by "+spearEffect;
            break;
            case 15:
                HoverOverText.text = "<color=#DAA520>Crossbowskill("+stats.crossbowSkill +"/"+crossbowMax+")</color> \nIncrease ranged accuracy with crossbows by "+crossbowEffect;
            break;
            case 16:
                HoverOverText.text = "<color=#DAA520>Bowskill("+stats.bowSkill +"/"+bowMax+")</color> \nIncrease ranged accuracy with bows by "+bowEffect;
            break;
            case 17:
                HoverOverText.text = "<color=#DAA520>Shieldskill("+stats.shieldSkill +"/"+shieldMax+")</color> \nIncrease shield evasion bonus by "+shieldEffect+"%";
            break;
            case 18:
                HoverOverText.text = "<color=#DAA520>Staffskill("+stats.staffSkill +"/"+staffMax+")</color> \nIncrease ranged accuracy and melee evasion with staffs by "+staffEffect;
            break;

            //tier2
            case 20:
                HoverOverText.text = "<color=#DAA520>Utility skills</color>  \nAvailable at level "+Tier2LevelAccess;
            break;
            case 21:
                HoverOverText.text = "<color=#DAA520>Specialist("+stats.oneHandedSkill +"/"+oneHandedMax+")</color> \nWith a one-handed weapon and the offhand free, increase all weapon stats by "+oneHandedEffect+"%";
            break;
            case 22:
                HoverOverText.text = "<color=#DAA520>Dual Wielding("+stats.dualwieldingSkill +"/"+dualWieldingMax+")</color> \nIncreases the efficiency with an offhand weapon by "+dualWieldingEffect+"%";
            break;
            case 23:
                HoverOverText.text = "<color=#DAA520>Two-Handed("+stats.twohandedSkill +"/"+twoHandedMax+")</color> \nIncreases all weapon stats when wielding a two-handed melee weapon by "+twoHandedEffect+"%";
            break;
            case 24:
                HoverOverText.text = "<color=#DAA520>Shieldwall("+stats.shieldWallSkill +"/"+shieldWallMax+"</color> \nReduces the damage a shield takes by "+shieldWallEffect+"%";
            break;
            case 25:
                HoverOverText.text = "<color=#DAA520>Hawkeye("+stats.hawkeyeSkill +"/"+hawkeyeMax+"</color> \nIncreases attack range with bows and crossbows by "+hawkeyeEffect+"%";
            break;
            case 26:
                HoverOverText.text = "<color=#DAA520>Athlete("+stats.athleteSkill +"/"+athleteMax+")</color> \nIncrease movement speed by 25% and evasion by "+athleteEffect +"%";
            break;
            case 27:
                HoverOverText.text = "<color=#DAA520>Armor Habituation("+stats.armorhabituationSkill +"/"+armorHabituationMax+")</color> \nReduce the stamina cost for wearing armor by "+armorHabituationEffect+"%";
            break;
            case 28:
                HoverOverText.text = "<color=#DAA520>Wisdom("+stats.wisdomSkill +"/"+wisdomMax+")</color> \nDecreases the base attack cost when using staffs by "+wisdomEffect+"%";
            break;
            case 29:
                if(stats.elementalistSkill>0)
                {
                switch(stats.elementalistSkillElement)
                {
                    case 0:
                        HoverOverText.text = "<color=#DAA520>Elementalist("+stats.elementalistSkill +"/"+elementalistMax+")</color> \nIncrease damage with Arcane Magic by ("+elementalistEffect+"%)";
                    break;
                    case 1:
                        HoverOverText.text = "<color=#DAA520>Elementalist("+stats.elementalistSkill +"/"+elementalistMax+")</color> \nIncrease damage with Fire Magic by ("+elementalistEffect+"%)";
                    break;
                    case 2:
                        HoverOverText.text = "<color=#DAA520>Elementalist("+stats.elementalistSkill +"/"+elementalistMax+")</color> \nIncrease damage with Water Magic by ("+elementalistEffect+"%)";
                    break;
                    case 3:
                        HoverOverText.text = "<color=#DAA520>Elementalist("+stats.elementalistSkill +"/"+elementalistMax+")</color> \nIncrease damage with Earth Magic by ("+elementalistEffect+"%)";
                    break;
                    case 4:
                        HoverOverText.text = "<color=#DAA520>Elementalist("+stats.elementalistSkill +"/"+elementalistMax+")</color> \nIncrease damage with Air Magic by ("+elementalistEffect+"%)";
                    break;
                    case 5:
                        HoverOverText.text = "<color=#DAA520>Elementalist("+stats.elementalistSkill +"/"+elementalistMax+")</color> \nIncrease damage with Light Magic by ("+elementalistEffect+"%)";
                    break;
                    case 6:
                        HoverOverText.text = "<color=#DAA520>Elementalist("+stats.elementalistSkill +"/"+elementalistMax+")</color> \nIncrease damage with Shadow Magic by ("+elementalistEffect+"%)";
                    break;
                }
                }
                else
                {
                    HoverOverText.text = "<color=#DAA520>Elementalist("+stats.elementalistSkill +"/"+elementalistMax+")</color> \nChoose a magic element that deals increased damage ("+elementalistEffect+"%)";
                }
            break;

            //tier3
            case 30:
                HoverOverText.text = "<color=#DAA520>Weapon Specialist</color> \nAvailable at level "+Tier3LevelAccess;
            break;
            case 31:
                HoverOverText.text = "<color=#DAA520>Sword-Dancer("+stats.sworddancerSkill +"/"+swordDancerMax+")</color> \nIncrease attack speed with swords by "+swordDancerEffect+"%";
            break;      
            case 32:
                HoverOverText.text = "<color=#DAA520>Armor-Breaker("+stats.armorbreakerSkill +"/"+armorBreakerMax+")</color> \nIncrease armor destruction with axes by "+armorBreakerEffect+"%";
            break; 
            case 33:
                HoverOverText.text = "<color=#DAA520>Basher("+stats.basherSkill +"/"+BasherMax+")</color> \nMaces gain a chance to bash on base attack of "+BasherEffect+"%";
            break;    
            case 34:
                HoverOverText.text = "<color=#DAA520>Hold the Line("+stats.holdTheLineSkill +"/"+holdTheLineMax+")</color> \nWhen using reach advantage multiply all melee evasion by "+ holdTheLineEffect+" instead of only adding the weapon bonus";
            break;          
            case 35:
                HoverOverText.text = "<color=#DAA520>Tinker("+stats.tinkerSkill +"/"+tinkerMax+")</color> \nIncrease armor pierce with crossbows by "+tinkerEffect+"%";
            break;   
            case 36:
                HoverOverText.text = "<color=#DAA520>Quickshot("+stats.quickshotSkill +"/"+quickshotMax+")</color> \nIncrease attack speed with bows by "+quickshotEffect+"%";
            break;
            case 37:
                HoverOverText.text = "<color=#DAA520>Armor Master("+stats.armormasterSkill +"/"+armorMasterMax+")</color> \nIncrease armor value by "+armorMasterEffect+"%";
            break;   
            case 38:
                HoverOverText.text = "<color=#DAA520>Debuff-Specialist("+stats.debufferSkill +"/"+debufferMax+")</color> \nIncrease magical debuff effects by "+debufferEffect+"%";
            break;      
            case 39:
                HoverOverText.text = "<color=#DAA520>Buff-Specialist("+stats.bufferSkill+"/"+bufferMax+")</color> \nIncrease magical buff effects by "+bufferEffect+"%";
            break;   
            
            //tier4
            case 40:
                HoverOverText.text = "<color=#DAA520>Professions</color> \nAvailable at level "+Tier4LevelAccess;
            break;

            //tier5
            case 50:
                HoverOverText.text = "<color=#DAA520>Veteran Skills</color> \nAvailable at level "+Tier5LevelAccess;
            break;
            case 51:
                HoverOverText.text = "<color=#DAA520>Counter Stance("+stats.conterSkill +"/"+conterMax+")</color> \nGet a Chance to retaliate with a melee attack, if a one-handed melee weapon with the offhand free is equipped ("+conterEffect+"%)";
            break;  
            case 52:
                HoverOverText.text = "<color=#DAA520>Whirlwind("+stats.whirlWindSkill +"/"+whirlWindMax+")</color> \nIncrease all evasion and melee accuracy by "+conterEffect+" times your speed (base Speed 2) when dual wielding";
            break; 
            case 53:
                HoverOverText.text = "<color=#DAA520>StormTrooper("+stats.conterSkill +"/"+stormtrooperMax+")</color> \nYour strength counts twice to your physical melee damage with two-handed weapons";
            break;     
            case 54:
                HoverOverText.text = "<color=#DAA520>Lion Heart("+stats.lionheartSkill +"/"+lionheartMax+")</color> \nOnce per fight revive with 25% hp after being knocked down";
            break; 
            case 55:
                HoverOverText.text = "<color=#DAA520>Acrobat("+stats.acrobatSkill+"/"+acrobatMax+")</color> \nDefending attacks without a shield costs less stamina ("+acrobatEffect+"%)";
            break;           
            case 56:
                HoverOverText.text = "<color=#DAA520>Vanguard("+stats.defenderSkill+"/"+defenderMax+")</color> \nReduces the stamina cost when blocking with a shield ("+defenderEffect+"%)";
            break;   
            case 57:
                HoverOverText.text = "<color=#DAA520>Magic Diffusion("+stats.magicDiffusionSkill+"/"+magicDiffusionMax+")</color> \nIncreases the effect radius of all magic attacks by 1m";
            break;   
            case 58:
                HoverOverText.text = "<color=#DAA520>Magic Flow("+stats.magicFlowSkill +"/"+magicFlowMax+")</color> \nIncreases the damage inflicted with magic weapons by "+magicFlowEffect+"%";
                
            break;     
            case 59:
                HoverOverText.text = "<color=#DAA520>Magic Resistance("+stats.magicResistanceSkill +"/"+magicResistanceMax+")</color> \nReduces the received damage from all magic by "+magicResistanceEffect+"%";
            break;

            //classes       
            case 61:
                ImageDisposition = new (0,180,0);
                HoverOverText.text = FighterClasses.Instance.GetClassDescription(stats.fighterClass);
            break;        
            case 62:
                ImageDisposition = new (0,180,0);
                HoverOverText.text = FighterClasses.Instance.GetPromotedClassDescription(stats.promotedFighterClass);
            break;        
            case 63:
                ImageDisposition = new (0,180,0);
                HoverOverText.text = "<color=#DAA520>Promotion Class Help</color> \nClick here to get a list with detailed descriptions of all the requirements and benefits of class boni!";
            break;  
        }
        
        HoverOverImage.transform.position = Input.mousePosition + ImageDisposition;
    }

    public void HoverOverExit()
    {
        HoverOverImage.SetActive(false);
    }

    public void IncreaseStats(int index)
    {
        Fighter stats = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();
        stats.attributePoints--;

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

        switch(index)
        {
            case 1:
                stats.maxHP += 10;
                stats.currentHP += 10;
            break;
            case 2:
                stats.maxStamina += 10;
                stats.currentStamina += 10;
            break;
            case 3:
                stats.pDmg += 1;
                //apply damage
                if(stats.dmgType==0)
                {
                    stats.dmg++;
                }
                if(stats.dmg2 != 0 && stats.dmgType2==0)
                {
                    stats.dmg2++;
                }
            break;
            case 4:
                stats.mDmg += 1;
                //apply damage
                if(stats.dmgType!=0)
                {
                    stats.dmg++;
                }
                if(stats.dmg2 != 0 && stats.dmgType2!=0)
                {
                    stats.dmg2++;
                }
            break;
            case 5:
                stats.mAcc += 1;
            break;
            case 6:
                stats.mEva += 1;
            break;
            case 7:
                stats.rAcc += 1;
            break;
            case 8:
                stats.rEva += 1;
            break;
        }
        UpdateUnitStats();
    }

    public void UpdateUnitStats()
    {
        Fighter stats = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].GetComponent<Fighter>();

        statbuttons.SetActive(true);

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
        //added equip stat
        unitStatsList[19].text = ""+stats.range;

        if(stats.attributePoints == 0)
        {
            statbuttons.SetActive(false);
        }
    }
    IEnumerator NoSkillPointsAvailable()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(warningSound, GameManager.Instance.sfxVolume);
        WarningObject.GetComponentInChildren<TMP_Text>().text = "No skill points available!"; 
        WarningObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        WarningObject.SetActive(false);
    }
    public IEnumerator AuraStackingWarning()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(warningSound, GameManager.Instance.sfxVolume);
        WarningObject.SetActive(true);
        WarningObject.GetComponentInChildren<TMP_Text>().text = "Auras do not stack!"; 
        yield return new WaitForSecondsRealtime(1.5f);
        WarningObject.SetActive(false);
    }

    public IEnumerator NewPromotionClassAvailable()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(newPromotionClassSound, GameManager.Instance.sfxVolume);
        WarningObject.SetActive(true);
        WarningObject.GetComponentInChildren<TMP_Text>().text = "You got access to a new class Promotion!"; 
        yield return new WaitForSecondsRealtime(1.5f);
        WarningObject.SetActive(false);
    }    
    
    public IEnumerator LevelTooLow()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.WarningSound, GameManager.Instance.sfxVolume);
        WarningObject.SetActive(true);
        WarningObject.GetComponentInChildren<TMP_Text>().text = "The character needs a higher level too get access to this skill tier!"; 
        yield return new WaitForSecondsRealtime(3f);
        WarningObject.SetActive(false);
    }

    public IEnumerator SkillAlreadyMaxed()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.WarningSound, GameManager.Instance.sfxVolume);
        WarningObject.SetActive(true);
        WarningObject.GetComponentInChildren<TMP_Text>().text = "The character has already perfected this skill!"; 
        yield return new WaitForSecondsRealtime(2f);
        WarningObject.SetActive(false);
    }

    public void OpenPromotionClassHelpWindow()
    {
        DisableInput();
        promotionClassHelpWindow.SetActive(true);
        SetPromotionClassHelpWindowPage(promotionExplanationListPage);
    }
    public void ClosePromotionClassHelpWindow()
    {
        InitializeSkillScreen();
        UnitSelections.Instance.EnableUnitSelections();
        promotionClassHelpWindow.SetActive(false);
    }

    public void SetPromotionClassHelpWindowPage(int buttonIndex)
    {
        promotionExplanationListPage = buttonIndex;
        switch(promotionExplanationListPage)
        {
            case 0: 
                promotionExplanationPageList[0].SetActive(true);
                promotionExplanationPageList[1].SetActive(false);
                promotionExplanationPageList[2].SetActive(false);
                promotionExplanationPageList[3].SetActive(false);
                promotionExplanationPageList[4].SetActive(false);
                promotionExplanationPageList[5].SetActive(false);
            break;
            case 1: 
                promotionExplanationPageList[0].SetActive(false);
                promotionExplanationPageList[1].SetActive(true);
                promotionExplanationPageList[2].SetActive(false);
                promotionExplanationPageList[3].SetActive(false);
                promotionExplanationPageList[4].SetActive(false);
                promotionExplanationPageList[5].SetActive(false);
            break;
            case 2: 
                promotionExplanationPageList[0].SetActive(false);
                promotionExplanationPageList[1].SetActive(false);
                promotionExplanationPageList[2].SetActive(true);
                promotionExplanationPageList[3].SetActive(false);
                promotionExplanationPageList[4].SetActive(false);
                promotionExplanationPageList[5].SetActive(false);
            break;
            case 3: 
                promotionExplanationPageList[0].SetActive(false);
                promotionExplanationPageList[1].SetActive(false);
                promotionExplanationPageList[2].SetActive(false);
                promotionExplanationPageList[3].SetActive(true);
                promotionExplanationPageList[4].SetActive(false);
                promotionExplanationPageList[5].SetActive(false);
            break;
            case 4: 
                promotionExplanationPageList[0].SetActive(false);
                promotionExplanationPageList[1].SetActive(false);
                promotionExplanationPageList[2].SetActive(false);
                promotionExplanationPageList[3].SetActive(false);
                promotionExplanationPageList[4].SetActive(true);
                promotionExplanationPageList[5].SetActive(false);
            break;
            case 5: 
                promotionExplanationPageList[0].SetActive(false);
                promotionExplanationPageList[1].SetActive(false);
                promotionExplanationPageList[2].SetActive(false);
                promotionExplanationPageList[3].SetActive(false);
                promotionExplanationPageList[4].SetActive(false);
                promotionExplanationPageList[5].SetActive(true);
            break;
        }
    }
    
    public void DisableInput()
    {
        UnitSelections.Instance.DisableUnitSelections();
        HoverOverExit();
        for(int i=0; i<SkillObjectList.Count; i++)
        {
            if(SkillObjectList[i]!=null)
            {
                SkillObjectList[i].GetComponent<EventTrigger>().enabled = false;
            }
        }
    }

}
