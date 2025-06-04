using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//thanks to "SpawnCampGames" - Multiple Unit Selection in Unity || RTS, Diablo, City Builder Type Selection

public class UnitSelections : MonoBehaviour
{
    public GameObject UI;
    public GameObject ClickSystem;
    public GameObject SkillSystemCanvas;
    public GameObject shopCanvas;
    public GameObject InventorySystemCanvas;
    public GameObject hireSystemCanvas;
    public GameObject unitStatsCanvas;
    public GameObject ApplyChangesCanvas;
    public TMP_Text ApplyChangesText;
    private bool acceptChanges;
    private bool inputDone;
    private UnityEngine.Vector3 camPosition;
    private UnityEngine.Vector3 hoverImageDispostion = UnityEngine.Vector3.zero;
/*     public EquipmentAppearance[] myEquipment;
    public bool equipped; */

    public List<GameObject> inActiveUnitList = new List<GameObject>(); //put all possible units here to put them in order for unitList
    public List<GameObject> unitList = new List<GameObject>(); //to check which units can be shift- and drag-clicked
    public List<GameObject> unitsSelected = new List<GameObject>(); //collect the selected units

    public List<GameObject> troop1 = new List<GameObject>();
    private List<GameObject> troop2 = new List<GameObject>();
    private List<GameObject> troop3 = new List<GameObject>();

    public List<GameObject> enemiesList = new List<GameObject>(); //list of enemies for the player characters to aggro check
    private GameObject enemySelected; //to check if an enemy is selected
    public List<UnityEngine.Vector3> unitInitialPositionList;
    public List<GameObject> UIButtonsList;

    public int selectedUnitnumber = 0;
    public bool gameOver;
    public bool fightWon;
    public GameObject menuCanvas;
    public GameObject menuWon;
    public GameObject menuPause;
    public GameObject menuLost;
    public GameObject menuLostVolume;
    public bool pause;
    public bool inventoryScreen;
    public bool shopScreen;
    public bool skillScreen;
    public bool hireScreen;
    public bool optionScreen;

    //playerUI stats
    public TMP_Text unitName;
    public Image unitImage;
    public Image unitPromotionClassImage;
    public TMP_Text unitHP;
    public TMP_Text unitStamina;
    public TMP_Text unitArmor;
    public TMP_Text unitDmg;
    public TMP_Text unitAttackCost;
    public Slider xpBar;
    public Slider hpBar;
    public Slider staminaBar;
    public Toggle skillToggle1;
    public Toggle skillToggle2;
    public Toggle skillToggle3;
    public Toggle commandToggle4;
    public Toggle commandToggle5;
    public Toggle commandToggle6;
    public Toggle commandToggle7;
    private string skillString1;
    private string skillString2;
    private string skillString3;
    private string skillString4;
    private string skillString5;
    private string skillString6;
    private string skillString7;
    public GameObject HoverOverImage;
    public TMP_Text HoverOverText;
    public Text skillLabel1;
    public Text skillLabel2;
    public Text skillLabel3;
    public GameObject skillIndicator1;
    public GameObject skillIndicator2;
    public GameObject skillIndicator3;
    public GameObject skillIndicator4;
    public GameObject commandIndicator5;
    public GameObject commandIndicator6;
    public GameObject commandIndicator7;
    public GameObject UIStats;
    public GameObject UISkills;
    public GameObject UICommands;
    private Sprite AimImage;
    private Sprite ArmorBypassImage;
    private Sprite ShieldBashImage;
    private Sprite SwordSpeedImage;
    private Sprite ReachAdvantageImage;
    private Sprite MagicFocusImage;
    private Sprite MagicAoeImage;
    private Sprite DmgImage;
    private Sprite MeleeAoeImage;
    private Sprite ArmorDestructionImage;
    private Sprite MaceBashImage;
    private Sprite SpearWallImage;
    private Sprite CrossbowScattershotImage;
    private Sprite QuickshotImage;
    private Sprite HunkerDownImage;
    private Sprite MagicSupportImage;
    private Sprite HealingFlamesSupportImage;
    private Sprite RefreshingLiquidsSupportImage;
    private Sprite ResistingStoneSupportImage;
    private Sprite HastedWindSupportImage;
    private Sprite IlluminatingLightSupportImage;
    private Sprite StrengtheningShadowSupportImage;
    private Sprite skillImageQ;
    private Sprite skillImageW;
    private Sprite skillImageE;
    public Color defaultTitleTextColor = new Color32(218,165,32,255);
    public Color defaultTextColor = new Color32(192,192,192,255);
    public bool disableInput;
    public bool enemySpawnFinished;
    private string skillExplain1;
    private string skillExplain2;
    private string skillExplain3;
    


    private static UnitSelections _instance;
    public static UnitSelections Instance { get { return _instance; } }

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
            DontDestroyOnLoad(gameObject);
        }
 
        AimImage = Resources.Load<Sprite>("Img/SkillsIcons/Misc/aim");
        ArmorBypassImage = Resources.Load<Sprite>("Img/SkillsIcons/Auras/Skill_BreakMediumArmor");
        SwordSpeedImage = Resources.Load<Sprite>("Img/SkillsIcons/SwordSpeed");
        MagicFocusImage = Resources.Load<Sprite>("Img/Blink/Arcanist1");
        MagicAoeImage = Resources.Load<Sprite>("Img/CaptainCatSparrow/Barbarian_1");
        DmgImage = Resources.Load<Sprite>("Img/Dmg");
        MeleeAoeImage = Resources.Load<Sprite>("Img/SkillsIcons/Auras/Skill_AxeSplash");
        ArmorDestructionImage = Resources.Load<Sprite>("Img/SkillsIcons/ArmorBreak");
        MaceBashImage = Resources.Load<Sprite>("Img/SkillsIcons/Bash");
        SpearWallImage = Resources.Load<Sprite>("Img/SkillsIcons/SpearWall");
        ReachAdvantageImage = Resources.Load<Sprite>("Img/SkillsIcons/Auras/Skill_SpearAttack");
        HunkerDownImage = Resources.Load<Sprite>("Img/SkillsIcons/Warriorskill_24_Block");
        ShieldBashImage = Resources.Load<Sprite>("Img/SkillsIcons/Warriorskill_25");
        CrossbowScattershotImage = Resources.Load<Sprite>("Img/ProfessionIcons/ProfessionAndCraftIcons/Engineering/Engineering_20_copper_balls");
        QuickshotImage = Resources.Load<Sprite>("Img/SahilGandhi/UI_Skill_Icon_Arrow_Barrage");

        MagicSupportImage = Resources.Load<Sprite>("Img/SahilGandhi/UI_Skill_Icon_Heal");

        HealingFlamesSupportImage = Resources.Load<Sprite>("Img/SkillsIcons/Misc/healing_wave");
        RefreshingLiquidsSupportImage = Resources.Load<Sprite>("Img/SkillsIcons/Auras/Aura_Water");
        ResistingStoneSupportImage = Resources.Load<Sprite>("Img/SkillsIcons/Shamanskill_35_earthtotme");
        HastedWindSupportImage = Resources.Load<Sprite>("Img/SkillsIcons/Shamanskill_45_wind");
        IlluminatingLightSupportImage = Resources.Load<Sprite>("Img/SkillsIcons/Auras/Skill_Light");
        StrengtheningShadowSupportImage = Resources.Load<Sprite>("Img/SkillsIcons/Misc/shadow_nobg");

        skillImageQ = Resources.Load<Sprite>("Img/Default_Skill_Q");
        skillImageW = Resources.Load<Sprite>("Img/Default_Skill_W");
        skillImageE = Resources.Load<Sprite>("Img/Default_Skill_E");

        skillString1 = "<color=#DAA520>Skill 1(Q)</color>  \nActivate Attack modificator 1 for the selected units!";
        skillString2 = "<color=#DAA520>Skill 2(W)</color>  \nActivate Attack modificator 2 for the selected units!";
        skillString3 = "<color=#DAA520>Skill 3(E)</color>  \nActivate Attack modificator 3 for the selected units!";
        skillString4 = "<color=#DAA520>Weapon Swap(R)</color>  \nSwap weapons for the selected unit(s)!";
        skillString5 = "<color=#DAA520>Attack(A)</color>  \nThe selected unit(s) always attack!";
        skillString6 = "<color=#DAA520>Stop(S)</color>  \nThe selected unit(s) never attack!";
        skillString7 = "<color=#DAA520>Hold (D)</color>  \nThe selected unit(s) only attack if something enters their attack range!";

    }

    void Start()
    {
        Debug.Log("unitselections start");
        GameManager.Instance.GetComponent<AudioSource>().volume = GameManager.Instance.bgmVolume;
        unitList.Clear();
        unitsSelected.Clear();
        if(GameManager.Instance.cityMap)
        {
            //allows the player to control units with mouse
            ClickSystem.SetActive(false);
            UIButtonsList[1].SetActive(true);
            UIButtonsList[2].SetActive(true);
            UIButtonsList[3].SetActive(true);
            UIButtonsList[4].SetActive(true);
            UIButtonsList[5].SetActive(false);
            UIButtonsList[10].SetActive(false);
            
        }
        else
        {
            UIButtonsList[1].SetActive(false);
            UIButtonsList[2].SetActive(false);
            UIButtonsList[3].SetActive(false);
            UIButtonsList[4].SetActive(false);
            UIButtonsList[5].SetActive(true);
            UIButtonsList[10].SetActive(true);
            ClickSystem.SetActive(true);
        }
        
        foreach(GameObject unit in inActiveUnitList)
        {
            if(unit.activeSelf)
            {
                unit.GetComponent<Fighter>().Setup();
                unitList.Add(unit);
                if(!GameManager.Instance.cityMap)
                {
                    ShiftClickSelect(unit);
                }
            }
        }

        
         if(GameManager.Instance.cityMap)
        {
            if(unitInitialPositionList.Count ==0)
            {
                for(int i=0; i<unitList.Count; i++)
                { 
                    unitInitialPositionList.Add(unitList[i].transform.position);
                }
            }
        }


       
        
    }




    //player UI Update
    void Update()
    {
    if(!disableInput)
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CallOptions();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PauseGame();
        } 
/*         if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            CheatBonusStatsMainChar();
        }  
        if(Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            CheatCode();
        }  */

        if(GameManager.Instance.cityMap && !pause)
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                CallInventorySystem();
            } 
            if(Input.GetKeyDown(KeyCode.M))
            {
                CallShopSystem();
            } 
            if(Input.GetKeyDown(KeyCode.H))
            {
                CallHireSystem();
            } 
            if(Input.GetKeyDown(KeyCode.B))
            {
                CallLeaveCity1();
            } 
        }
        
        if(Input.GetKeyDown(KeyCode.L))
        {
            CallSkillSystem();
        }    
        
        if(!GameManager.Instance.cityMap && !pause && !gameOver)
        {
            if(unitList.Count == 0)
            {
                StartCoroutine(GameOverScreen());
            }
            if(enemiesList.Count == 0)
            {
                StartCoroutine(LeaveArena());
            }
        }
        


        //select next unit
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextUnit();
        }
        if(Camera.main.GetComponent<CameraController>().isActiveAndEnabled && Input.GetKeyDown(KeyCode.C))
        {
            CallCenterUnit();
        } 

        if(unitsSelected.Count > 0)
        {
            UISkills.SetActive(true);
            UICommands.SetActive(true);

            if(unitsSelected.Count == 1)
            {
                UIStats.SetActive(true);

                selectedUnitnumber = unitList.IndexOf(unitsSelected[0]);
                Fighter stats = unitsSelected[0].GetComponent<Fighter>();
                unitName.SetText(stats.unitName);
                unitImage.gameObject.SetActive(true);
                unitImage.sprite = stats.unitImage;
                unitHP.SetText(stats.currentHP +"/" + stats.maxHP);
                unitStamina.SetText(stats.currentStamina +"/" + stats.maxStamina);
                unitArmor.SetText("" +stats.armor);
                if(stats.dmg2 !=0)
                {
                    unitDmg.SetText("" +stats.dmg +" + " +stats.dmg2);
                }
                else
                {
                    unitDmg.SetText("" +stats.dmg);
                }
                unitAttackCost.SetText(""+stats.attackCost);
                if(stats.acrobatSkill>stats.maxStamina)
                {
                    unitAttackCost.color = Color.red;
                }
                else
                {
                    unitAttackCost.color = defaultTextColor;
                }
                xpBar.gameObject.SetActive(true);
                hpBar.gameObject.SetActive(true);
                staminaBar.gameObject.SetActive(true);
                xpBar.maxValue = stats.xpToNextLevel;
                xpBar.value = Mathf.RoundToInt(stats.currentXP);
                xpBar.GetComponentInChildren<TMP_Text>().text = "Lvl "+stats.level +" (" +stats.currentXP +"/" +stats.xpToNextLevel +")";
                hpBar.maxValue = stats.maxHP;
                hpBar.value = stats.currentHP;
                staminaBar.maxValue = stats.maxStamina;
                staminaBar.value = stats.currentStamina;

                if(stats.promotedFighterClass==0)
                {
                    unitPromotionClassImage.gameObject.SetActive(false);
                }
                else
                {
                    unitPromotionClassImage.gameObject.SetActive(true);
                    unitPromotionClassImage.sprite = FighterClasses.Instance.GetPromotedClassImage(stats.promotedFighterClass);
                }

                    if (stats.skill1Active)
                    {
                        skillIndicator1.SetActive(true);
                    }
                    else
                    {
                        skillIndicator1.SetActive(false);
                    }
                    if (stats.skill2Active)
                    {
                        skillIndicator2.SetActive(true);
                    }
                    else
                    {
                        skillIndicator2.SetActive(false);
                    }
                    if (stats.skill3Active)
                    {
                        skillIndicator3.SetActive(true);
                    }
                    else
                    {
                        skillIndicator3.SetActive(false);
                    }
                    if (stats.skill4Active)
                    {
                        skillIndicator4.SetActive(true);
                    }
                    else
                    {
                        skillIndicator4.SetActive(false);
                    }
                    if (stats.commandActive5)
                    {
                        commandIndicator5.SetActive(true);
                    }
                    else
                    {
                        commandIndicator5.SetActive(false);
                    }
                    if (stats.commandActive6)
                    {
                        commandIndicator6.SetActive(true);
                    }
                    else
                    {
                        commandIndicator6.SetActive(false);
                    }
                    if (stats.commandActive7)
                    {
                        commandIndicator7.SetActive(true);
                    }
                    else
                    {
                        commandIndicator7.SetActive(false);
                    }

                GetSkillImage(stats.currentWeaponType, stats); 
            }
            else
            {
                UIStats.SetActive(false);

                skillString1 = "<color=#DAA520>Skill 1(Q)</color>  \nActivate Attack modificator 1 for the selected units!";
                skillString2 = "<color=#DAA520>Skill 2(W)</color>  \nActivate Attack modificator 2 for the selected units!";
                skillString3 = "<color=#DAA520>Skill 3(E)</color>  \nActivate Attack modificator 3 for the selected units!";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = skillImageQ;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = skillImageW;
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = skillImageE;
                skillIndicator1.SetActive(false);
                skillIndicator2.SetActive(false);
                skillIndicator3.SetActive(false);
                skillIndicator4.SetActive(false);
                commandIndicator5.SetActive(false);
                commandIndicator6.SetActive(false);
                commandIndicator7.SetActive(false);
            }
        } 
        else if (enemySelected != null)
        {
            xpBar.gameObject.SetActive(false);
            UISkills.SetActive(false);
            UICommands.SetActive(false);
            UIStats.SetActive(true);

            Fighter stats = enemySelected.GetComponent<Fighter>();
            unitPromotionClassImage.gameObject.SetActive(false);
            unitName.SetText(stats.unitName);
            unitImage.gameObject.SetActive(true);
            unitImage.sprite = stats.unitImage;
            unitHP.SetText(stats.currentHP +"/" + stats.maxHP);
            unitStamina.SetText(stats.currentStamina +"/" + stats.maxStamina);
            unitArmor.SetText("" +stats.armor);
            unitDmg.SetText("" +stats.dmg);
            hpBar.maxValue = stats.maxHP;
            hpBar.value = stats.currentHP;
            staminaBar.maxValue = stats.maxStamina;
            staminaBar.value = stats.currentStamina;

        } 
        else
        {
            xpBar.gameObject.SetActive(false);
            UISkills.SetActive(false);
            UICommands.SetActive(false);
            UIStats.SetActive(false);
        }


        //Hotkeys 
        //skills
        if(unitsSelected.Count > 0 && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleSkill1();
        }        
        if(unitsSelected.Count > 0 && Input.GetKeyDown(KeyCode.W))
        {
            ToggleSkill2();
        }        
        if(unitsSelected.Count > 0 && Input.GetKeyDown(KeyCode.E) && skillToggle3.gameObject.activeInHierarchy)
        {
            ToggleSkill3();
        }
                
        if(unitsSelected.Count > 0 && Input.GetKeyDown(KeyCode.R))
        {
            ToggleSkill4();
        }    
        if(unitsSelected.Count > 0 && Input.GetKeyDown(KeyCode.A))
        {
            AttackCommand();
        }   
        if(unitsSelected.Count > 0 && Input.GetKeyDown(KeyCode.S))
        {
            StopCommand();
        }   
        if(unitsSelected.Count > 0 && Input.GetKeyDown(KeyCode.D))
        {
            HoldCommand();
        }


        //troop selecting
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(Input.GetKey(KeyCode.LeftAlt))
            {
            troop1.Clear();
            troop1.AddRange(unitsSelected);
            }
            else
            {
            SelectTroop1();
            }
        } 
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(Input.GetKey(KeyCode.LeftAlt))
            {
                troop2.Clear();
                troop2.AddRange(unitsSelected);
            }
            else
            {
                SelectTroop2();
            }
        } 
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            if(Input.GetKey(KeyCode.LeftAlt)) //set group
            {
                troop3.Clear();
                troop3.AddRange(unitsSelected);
            }
            else                            //select group
            {
                SelectTroop3();
            }
        } 
    }
    }

    public void DisableUnitSelections()
    {
        HoverOverExit();
        disableInput = true;
        UI.SetActive(false);
    }    
    public void EnableUnitSelections()
    {
        HoverOverExit();
        disableInput = false;
        UI.SetActive(true);
    }

    public void CheatCode()
    {
        Unit stats = unitList[0].GetComponent<Unit>();
        StartCoroutine(stats.LevelUP(true));

    }
    public void CheatBonusStatsMainChar()
    {
        Unit stats = unitList[0].GetComponent<Unit>();
        stats.maxHP += 100;
        stats.currentHP +=100;
        stats.maxStamina += 100;
        stats.currentStamina += 100;
        stats.mAcc +=10;
        stats.rAcc += 10;
        stats.mEva += 10;
        stats.rEva += 10;
        stats.pDmg += 10;
        stats.mDmg += 10;
        stats.dmg += 10;
    }

    public IEnumerator GameOverScreen()
    {
            GameManager.Instance.GetComponent<AudioSource>().clip = GameManager.Instance.GameOverSound;  
            GameManager.Instance.GetComponent<AudioSource>().Play(); 
            gameOver = true;
            menuCanvas.SetActive(true);
            menuLost.SetActive(true);
            menuLostVolume.SetActive(true);
            yield return new WaitForSecondsRealtime(4);
            menuCanvas.SetActive(false);
            menuLost.SetActive(false);
            if(!optionScreen)
            {
                //CallOptions();
                GameManager.Instance.ShowScoreboard();
            }
    }

    public void CloseGameOverScreen()
    {
            GameManager.Instance.GetComponent<AudioSource>().clip = GameManager.Instance.CityBGM;   
            GameManager.Instance.GetComponent<AudioSource>().Play(); 
            gameOver = false;
            menuCanvas.SetActive(false);
            menuLost.SetActive(false);
            menuLostVolume.SetActive(false);
            if(optionScreen)
            {
                CallOptions();
            }
            Start();
    }

    public void HoverEnter(int index)
    {
        HoverOverImage.SetActive(true);
        //skills
        switch(index)
        {
            case 1:
                HoverOverText.text = skillString1;
                hoverImageDispostion = new UnityEngine.Vector3(50, 250, 0);
            break;
            case 2:
                HoverOverText.text = skillString2;
                hoverImageDispostion = new UnityEngine.Vector3(50, 250, 0);
            break;
            case 3:
                HoverOverText.text = skillString3;
                hoverImageDispostion = new UnityEngine.Vector3(50, 250, 0);
            break;
            case 4:
                HoverOverText.text = skillString4;
                hoverImageDispostion = new UnityEngine.Vector3(50, 250, 0);
            break;
            case 5:
                HoverOverText.text = skillString5;
                hoverImageDispostion = new UnityEngine.Vector3(50, 250, 0);
            break;
            case 6:
                HoverOverText.text = skillString6;
                hoverImageDispostion = new UnityEngine.Vector3(50, 250, 0);
            break;
            case 7:
                HoverOverText.text = skillString7;
                hoverImageDispostion = new UnityEngine.Vector3(50, 250, 0);
            break;
            case 8:
                HoverOverText.text = FighterClasses.Instance.GetPromotedClassDescription(unitList[selectedUnitnumber].GetComponent<Fighter>().promotedFighterClass);
                hoverImageDispostion = new UnityEngine.Vector3(50, 250, 0);
            break;
            
            //menu left
            case 11:
                HoverOverText.text = "<color=#DAA520>(L)eveling</color>  \nLevel up your units attributes and skills!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 250, 0);
            break;
            case 12:
                HoverOverText.text = "<color=#DAA520>(I)nventory</color>  \nEquip your units in regard to their skills suited to the needs of your roster and your enemies!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 250, 0);
            break;
            case 13:
                HoverOverText.text = "<color=#DAA520>(H)ire</color>  \nTake a look at your roster and available mercenaries to hire!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 250, 0);
            break;
            case 14:
                HoverOverText.text = "<color=#DAA520>(M)arket</color>  \nBuy and sell new equipment!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 250, 0);
            break;
            case 15:
                hoverImageDispostion = new UnityEngine.Vector3(250, 250, 0);
                switch(GameManager.Instance.arenaLevel)
                {
                    case 0:
                        HoverOverText.text = "<color=#DAA520>(B)attle</color> \nPrepare your character and defeat a thug!";
                    break;
                    case 1:
                        HoverOverText.text = "<color=#DAA520>(B)attle</color> \nDefeat inexperienced Warriors! They use simple melee weapons, shields and armor!";
                    break;
                    case 2:
                        HoverOverText.text = "<color=#DAA520>(B)attle</color> \nDefeat Butzemans! A Butze is the weakest type of demon using simple melee attacks! But like all demons they have a high resistance to physical and shadow attacks, while being weak against light damage!";
                    break;
                    case 3:
                        HoverOverText.text = "<color=#DAA520>(B)attle</color> \nDefeat monster beetles! They are fast and have high armor, but are weak to elemental damage!";
                    break;
                    case 4:
                        HoverOverText.text = "<color=#DAA520>(B)attle</color> \nDefeat Mercenaries! This is a mixed troop of veteran warriors!";
                    break;
                    case 5:
                        HoverOverText.text = "<color=#DAA520>(B)attle</color> \n\nDefeat the Dragon! The Dragon is a strong creature that can fly and uses physical and fire attacks. It has increased resistance to fire!";
                    break;
                    default:
                        HoverOverText.text = "<color=#DAA520>(B)attle</color> \n\nDefeat random enemies! Their number and strength increases with each level!";
                    break;

                }
            break;
            //menu right
            case 10:
                HoverOverText.text = "<color=#DAA520>Pause(Space)</color>  \nPause the Game!";
                hoverImageDispostion = new UnityEngine.Vector3(-250, 250, 0);
            break;
            case 16:
                HoverOverText.text = "<color=#DAA520>(C)enter on Unit</color>  \nCenters the currently selected unit into view!";
                hoverImageDispostion = new UnityEngine.Vector3(-250, 250, 0);
            break;
            case 17:
                HoverOverText.text = "<color=#DAA520>Next(Tab)</color>  \nSelects the next available unit!";
                hoverImageDispostion = new UnityEngine.Vector3(-250, 250, 0);
            break;
            case 18:
                HoverOverText.text = "<color=#DAA520>Prestige</color>  \nHigher prestige allows to hire more mercenaries and talk to more traders and persons of interest!";
                hoverImageDispostion = new UnityEngine.Vector3(-250, 250, 0);
            break;
            case 19:
                HoverOverText.text = "<color=#DAA520>Gold</color>  \nYour current wallet! Used to pay mercenaries and buy goods!";
                hoverImageDispostion = new UnityEngine.Vector3(-250, 250, 0);
            break;
            case 20:
                HoverOverText.text = "<color=#DAA520>Options(ESC)</color>  \nOpens the game options and pauses the game!";
                hoverImageDispostion = new UnityEngine.Vector3(-250, 250, 0);
            break;
            //menu middle
            case 21:
                HoverOverText.text = "<color=#DAA520>Damage</color>  \nThe total damage with all modificators inflicted with a single attack!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 250, 0);
            break;
            case 22:
                HoverOverText.text = "<color=#DAA520>Armor</color>  \nReduces incoming damage that is not bypassing or piercing armor!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 250, 0);
            break;
            case 23:
                HoverOverText.text = "<color=#DAA520>Attack Cost</color>  \nThe character will be unable to attack with less stamina with the currently (in-)active skills!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 250, 0);
            break;

            //statscanvas
            case 101:
                HoverOverText.text = "<color=#DAA520>Hit Points</color>  \nThe amount of damage a character can endure before collapsing!";
                hoverImageDispostion = new UnityEngine.Vector3(250, -50, 0);
            break;
            case 102:
                HoverOverText.text = "<color=#DAA520>Stamina</color>  \nEach action costs stamina! Without stamina a character is unable to attack or defend!";
                hoverImageDispostion = new UnityEngine.Vector3(250, -50, 0);
            break;
            case 103:
                HoverOverText.text = "<color=#DAA520>Strength</color>  \nThis value is added to the weapon damage when using physical attacks!";
                hoverImageDispostion = new UnityEngine.Vector3(250, -50, 0);
            break;
            case 104:
                HoverOverText.text = "<color=#DAA520>Intelligence</color>  \nThis value is added to the weapon damage when using magical attacks!";
                hoverImageDispostion = new UnityEngine.Vector3(250, -50, 0);
            break;
            case 105:
                HoverOverText.text = "<color=#DAA520>Melee Accuracy</color>  \nThe characters base accuracy with melee attacks!";
                hoverImageDispostion = new UnityEngine.Vector3(250, -50, 0);
            break;
            case 106:
                HoverOverText.text = "<color=#DAA520>Melee Evasion</color>  \nThe characters base evasion against melee attacks!";
                hoverImageDispostion = new UnityEngine.Vector3(250, -50, 0);
            break;
            case 107:
                HoverOverText.text = "<color=#DAA520>Ranged Accuracy</color>  \nThe characters base accuracy with ranged attacks!";
                hoverImageDispostion = new UnityEngine.Vector3(250, -50, 0);
            break;
            case 108:
                HoverOverText.text = "<color=#DAA520>Ranged Evasion</color>  \nThe characters base evasion against ranged attacks!";
                hoverImageDispostion = new UnityEngine.Vector3(250, -50, 0);
            break;
            case 111:
                HoverOverText.text = "<color=#DAA520>Damage</color>  \nThe total damage with all modificators inflicted with a single attack!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 112:
                HoverOverText.text = "<color=#DAA520>Armor</color>  \nReduces incoming damage that is not bypassing or piercing armor!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 113:
                HoverOverText.text = "<color=#DAA520>Armor Bypass</color>  \nPercentage of damage that ignores the enemies armor!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 114:
                HoverOverText.text = "<color=#DAA520>Armor Pierce</color>  \nPercentage of damage that ignores and destroys the enemies armor!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 115:
                HoverOverText.text = "<color=#DAA520>Attack Rate</color>  \nThe time in seconds that passes until the next attack gets executed!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 116:
                HoverOverText.text = "<color=#DAA520>Attack Cost</color>  \nThe character will be unable to attack with less stamina with the currently (in-)active skills!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 117:
                HoverOverText.text = "<color=#DAA520>Attack Range</color>  \nThe maximum distance in meter a fighter can attack it's target from!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 121:
                HoverOverText.text = "<color=#DAA520>Slow Resistance</color>  \nReduces the duration of push, slow and stun effects by x percent!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 122:
                HoverOverText.text = "<color=#DAA520>Status Resistance</color>  \nReduces the duration of damage over time, weaking and blinding effects by x percent!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 123:
                HoverOverText.text = "<color=#DAA520>Move Speed</color>  \nThe characters' movement speed on the battle map!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 124:
                HoverOverText.text = "<color=#DAA520>Value</color>  \nThe item value scales proportionally to its durability. If you sell it you get half the value!";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 131:
                HoverOverText.text = "<color=#DAA520>Burning</color>  \nAttacks deal additional burn damage over time. The value determines the duration in seconds.";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 132:
                HoverOverText.text = "<color=#DAA520>Slow</color>  \nAttacks slow the targets attack and movement speed for its values duration in seconds.";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 133:
                HoverOverText.text = "<color=#DAA520>Bash</color>  \nThe value determines the chance to bash a target. A bashed target can neither attack, move nor defend for a short duration.";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 134:
                HoverOverText.text = "<color=#DAA520>Push</color>  \nThe value determines how many meters a target gets pushed back when hit.";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 135:
                HoverOverText.text = "<color=#DAA520>Blinding</color>  \nDepending on your damage the target looses evasion and accuracy for the values duration in seconds.";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 136:
                HoverOverText.text = "<color=#DAA520>Weakening</color>  \nDepending on your damage the target looses max and current HP and Stamina for the values duration in seconds, but regains them afterwards.";
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 141:
                HoverOverText.text = skillExplain1;
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 142:
                HoverOverText.text = skillExplain2;
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;
            case 143:
                HoverOverText.text = skillExplain3;
                hoverImageDispostion = new UnityEngine.Vector3(250, +100, 0);
            break;

            //shop
            case 200:
                HoverOverText.text = "<color=#DAA520>All</color>  \nShow all offers!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 200, 0);
            break;
            case 201:
                HoverOverText.text = "<color=#DAA520>One-Handed</color>  \nShow one-handed weapons!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 200, 0);
            break;
            case 202:
                HoverOverText.text = "<color=#DAA520>Two-Handed</color>  \nShow two-handed weapons!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 200, 0);
            break;
            case 203:
                HoverOverText.text = "<color=#DAA520>Armor</color>  \nShow available armor!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 200, 0);
            break;
            case 204:
                HoverOverText.text = "<color=#DAA520>Consumables</color>  \nShow consumable items!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 200, 0);
            break;
            case 205:
                HoverOverText.text = "<color=#DAA520>Miscellaneous</color>  \nShow trading items and resources!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 200, 0);
            break;
            case 208:
                HoverOverText.text = "<color=#DAA520>Repair All</color>  \nRepair all items for <color=red>"+ShopSystem.Instance.repairCost+"</color> gold!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 200, 0);
            break;
            case 209:
                HoverOverText.text = "<color=#DAA520>Refresh Shop</color>  \nRefresh all shop items for a small fee of <color=red>"+ShopSystem.Instance.shopRefreshCost+"</color> gold!";
                hoverImageDispostion = new UnityEngine.Vector3(250, 200, 0);
            break;
        }
        HoverOverImage.transform.position = Input.mousePosition + hoverImageDispostion;
    }

    public void HoverOverExit()
    {
        HoverOverImage.SetActive(false);
    }

    public void SelectNextUnit()
    { 
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SelectSound, GameManager.Instance.sfxVolume);     
            //if in shop in city move unit back to its start place
            if(shopScreen)
            {
                if(GameManager.Instance.cityMap)
                {
                    unitList[selectedUnitnumber].transform.position = unitInitialPositionList[selectedUnitnumber];
                }
            }
            //get the next unit in list and if list at end jump back to first
            selectedUnitnumber += 1;
            if (selectedUnitnumber >= unitList.Count)
            {
                selectedUnitnumber = 0;
            }
            DeselectAll();
            unitsSelected.Add(unitList[selectedUnitnumber]);
            
            unitsSelected[0].transform.GetChild(0).gameObject.SetActive(true);
            
            //re-initialize menu screens
            if(inventoryScreen)
            {
                InventorySystem.Instance.InitializeInventory();
            }
            else if(skillScreen)
            {
                SkillSystem.Instance.InitializeSkillScreen();
            }
            else if(hireScreen)
            {
                HireSystem.Instance.InitializeHireSystem();
            }            
            //if in shop in city move unit to shopping place
            else if(shopScreen)
            {
                ShopSystem.Instance.InitializeShop();
                if(GameManager.Instance.cityMap)
                {
                    unitList[selectedUnitnumber].transform.position = new UnityEngine.Vector3(50,0,0);
                }
            }

            //if orbit camera is on reset target position
            if(Camera.main.GetComponent<OrbitCamera>().enabled)
            {
                Camera.main.GetComponent<OrbitCamera>().targetPos = unitList[selectedUnitnumber].transform.position + new UnityEngine.Vector3(0,1.5f,0);
            }
    }

    public void PauseGame()
    {

        if(Time.timeScale ==1)
        {
            GameManager.Instance.pauseCounter++;
            //Debug.Log(GameManager.Instance.pauseCounter);

            menuCanvas.SetActive(true);
            menuPause.SetActive(true);
            pause = true;
            Time.timeScale = 0;
        }
        else
        {
            menuCanvas.SetActive(false);
            menuPause.SetActive(false);
            pause = false;
            Time.timeScale = 1;
        }        
    }
    public void CallCenterUnit()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SelectSound, GameManager.Instance.sfxVolume);
        Camera.main.GetComponent<CameraController>().CenterCameraOnUnit();
    }

    public IEnumerator LeaveArena()
    {
        GameManager.Instance.fightTime += Time.time;
        Debug.Log(GameManager.Instance.fightTime);

        gameOver = true;
        menuCanvas.SetActive(true);
        menuWon.SetActive(true);
        GameManager.Instance.cityMap = true;

        GameManager.Instance.ChangeGold(Mathf.RoundToInt(Mathf.Pow(GameManager.Instance.arenaLevel,1)*500+500));
        GameManager.Instance.ChangePrestige(Mathf.RoundToInt(Mathf.Pow(GameManager.Instance.arenaLevel,1)*500+200));
        if(GameManager.Instance.arenaLevel==0)
        {
            GameManager.Instance.maxTroopStrength++;
        }
        else if(GameManager.Instance.arenaLevel==3)
        {
            GameManager.Instance.maxTroopStrength++;
        }
        else if(GameManager.Instance.arenaLevel==5)
        {
            GameManager.Instance.maxTroopStrength++;
        }
        else if(GameManager.Instance.arenaLevel==9)
        {
            GameManager.Instance.maxTroopStrength++;
        }
        GameManager.Instance.arenaLevel ++;

        ShopSystem.Instance.RefreshShop();
        GameManager.Instance.GetComponent<AudioSource>().clip = GameManager.Instance.CityBGM;  
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.VictorySound, GameManager.Instance.sfxVolume);
        yield return new WaitForSeconds(1);
        menuCanvas.SetActive(false);
        menuWon.SetActive(false);
        //place units back in tavern
        unitList.Clear();
        foreach(GameObject unit in inActiveUnitList)
        {
            if(unit.activeSelf)
            {
                unitList.Add(unit);
            }
        }
        for(int i=0; i<unitList.Count; i++)
        {
            Unit unitStats = unitList[i].GetComponent<Unit>();
            unitStats.GetComponent<Animator>().SetFloat("Speed", 0);
            unitStats.rb.velocity = Vector3.zero; 
            unitList[i].transform.position = unitInitialPositionList[i];
            unitStats.myAgent.Warp(unitInitialPositionList[i]);
            unitStats.myAgent.isStopped= true;
            unitStats.myAgent.enabled = false;
            unitStats.target = null;
            unitStats.initiation = false;
            if(unitStats.currentHP <= 0)
            {
                unitStats.GetComponent<Animator>().ResetTrigger("Death");
                unitStats.GetComponent<Animator>().SetTrigger("Revive");
            } 
            unitStats.currentHP = unitStats.maxHP;
        } 
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(2);

        ClickSelect(unitList[selectedUnitnumber]); 
        Start();
        SceneManager.LoadScene("Tavern");

        gameOver = false;
        
        if(GameManager.Instance.arenaLevel>5)
        {
            GameManager.Instance.ShowScoreboard();
        }
        
        yield return new WaitForSecondsRealtime(GameManager.Instance.VictorySound.length-4);
        GameManager.Instance.GetComponent<AudioSource>().Play();  
    }

    public void CallLeaveCity1()
    {        
        if(skillScreen)
        {
            CallSkillSystem();
        } 
        else if(inventoryScreen)
        {
            CallInventorySystem();
        }
        else if(hireScreen)
        {
            CallHireSystem();
        }
        else if(shopScreen)
        {
            CallShopSystem();
        }
        DisableUnitSelections();
        ApplyChangesText.text = "<color=#DAA520>Start Battle?</color>  \nAre you sure you are prepared your squad (hiring, equipment, attributes, skills)?";
        StartCoroutine(CallLeaveCity2());
    }
    public IEnumerator CallLeaveCity2()
    {
        ApplyChangesCanvas.SetActive(true);
        inputDone = false;
        while (!inputDone)
        {
            yield return null;
        }

        if(acceptChanges)
        {
            StartCoroutine(LeaveCity());
        }

        ApplyChangesCanvas.SetActive(false);
        EnableUnitSelections();
    }

    public void ApplyChanges()
    {
        inputDone = true;
        acceptChanges = true;
    }    
    public void DenyChanges()
    {
        inputDone = true;
        acceptChanges = false;
    }
    public IEnumerator LeaveCity()
    {
        pause = false;
        GameManager.Instance.cityMap = false;
        PauseGame();

        GameManager.Instance.pauseCounter--;
        Debug.Log(GameManager.Instance.pauseCounter);

        for(int i=0; i<unitList.Count; i++)
        {
            unitList[i].GetComponent<NavMeshAgent>().enabled = true;
            unitList[i].transform.position = new UnityEngine.Vector3(-22,0,-2*i);
            unitList[i].GetComponent<NavMeshAgent>().Warp(new UnityEngine.Vector3(-22,0,-2*i));
        }
        yield return new WaitForEndOfFrame();
        GameManager.Instance.GetComponent<AudioSource>().clip = GameManager.Instance.BattleBGM;  
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.BattleSound, GameManager.Instance.sfxVolume);
        if(GameManager.Instance.arenaLevel<6)
        {
            SceneManager.LoadScene("Arena"+GameManager.Instance.arenaLevel);
        }
        else
        {
            enemySpawnFinished = false;
            SceneManager.LoadScene("Arena6");
            while(!enemySpawnFinished)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        //refresh the unit colliders/rbs for clickselect
        Physics.SyncTransforms();
        
        yield return new WaitForSecondsRealtime(1);
        while(enemiesList.Count==0)
        {
            yield return new WaitForEndOfFrame();
        }
        Start();
        
        foreach(GameObject enemy in enemiesList)
        {
            int multiplier = 2;
            int dragonMultiplier = 5;
            if(enemy.name == "Dragon")
            {
                Fighter enemyStats = enemy.GetComponent<Fighter>();
                enemyStats.maxHP += GameManager.Instance.arenaLevel*multiplier *10 *dragonMultiplier;
                enemyStats.currentHP += GameManager.Instance.arenaLevel*multiplier *10 *dragonMultiplier;
                enemyStats.maxStamina += GameManager.Instance.arenaLevel*multiplier *10 *dragonMultiplier;
                enemyStats.currentStamina += GameManager.Instance.arenaLevel*multiplier *10 *dragonMultiplier;
                enemyStats.mAcc += GameManager.Instance.arenaLevel*multiplier *dragonMultiplier;
                enemyStats.rAcc += GameManager.Instance.arenaLevel*multiplier *dragonMultiplier;
                enemyStats.mEva += GameManager.Instance.arenaLevel*multiplier *dragonMultiplier;
                enemyStats.rEva += GameManager.Instance.arenaLevel*multiplier *dragonMultiplier;
                enemyStats.dmg += GameManager.Instance.arenaLevel*multiplier *dragonMultiplier;
                enemyStats.armor += GameManager.Instance.arenaLevel*multiplier *dragonMultiplier;
            }
            else
            {
                Fighter enemyStats = enemy.GetComponent<Fighter>();
                enemyStats.maxHP += GameManager.Instance.arenaLevel*multiplier *10;
                enemyStats.currentHP += GameManager.Instance.arenaLevel*multiplier *10;
                enemyStats.maxStamina += GameManager.Instance.arenaLevel*multiplier *10;
                enemyStats.currentStamina += GameManager.Instance.arenaLevel*multiplier *10;
                enemyStats.mAcc += GameManager.Instance.arenaLevel*multiplier;
                enemyStats.rAcc += GameManager.Instance.arenaLevel*multiplier;
                enemyStats.mEva += GameManager.Instance.arenaLevel*multiplier;
                enemyStats.rEva += GameManager.Instance.arenaLevel*multiplier;
                enemyStats.dmg += GameManager.Instance.arenaLevel*multiplier;
                enemyStats.armor += GameManager.Instance.arenaLevel*multiplier;
            }
        }

        yield return new WaitForSecondsRealtime(GameManager.Instance.BattleSound.length-2);
        
        GameManager.Instance.GetComponent<AudioSource>().Play();  

    }

    public void CallInventorySystem()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        if(skillScreen)
        {
            CallSkillSystem();
        }
        else if(shopScreen)
        {
            CallShopSystem();
        }
        else if(hireScreen)
        {
            CallHireSystem();
        }
        if(!inventoryScreen)
        {
            ClickSelect(unitList[selectedUnitnumber]);
            inventoryScreen = true;
            InventorySystemCanvas.SetActive(true);
            unitStatsCanvas.gameObject.SetActive(true);
            InventorySystem.Instance.InitializeInventory();
            if(!GameManager.Instance.cityMap)
            {
                ChangeToOrbitCamera();
            }
        }
        else
        {
            inventoryScreen = false;
            InventorySystemCanvas.SetActive(false);
            InventorySystem.Instance.HoverExit();
            unitStatsCanvas.gameObject.SetActive(false);
            if(!GameManager.Instance.cityMap)
            {
                ChangeToBirdviewCamera();
            }
        }
    }
    public void CallHireSystem()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        if(skillScreen)
        {
            CallSkillSystem();
        }
        else if(shopScreen)
        {
            CallShopSystem();
        }
        else if(inventoryScreen)
        {
            CallInventorySystem();
        }
        if(!hireScreen)
        {
            DeselectAll();
            hireScreen = true;
            hireSystemCanvas.SetActive(true);
            HireSystem.Instance.InitializeHireSystem();
            ChangeToOrbitCamera();
/*             camPosition = Camera.main.transform.position;
            Camera.main.GetComponent<CameraController>().enabled = false;
            Camera.main.GetComponent<OrbitCamera>().enabled = true;
            Camera.main.transform.position = unitList[selectedUnitnumber].transform.position + new UnityEngine.Vector3(0,1,-5); //move camera to unit
            Camera.main.transform.LookAt(unitList[selectedUnitnumber].transform.position + new UnityEngine.Vector3(0,1,0)); //look at body (not feet) of character */
        }
        else
        {
            hireScreen = false;
            hireSystemCanvas.SetActive(false);
            HireSystem.Instance.HoverOverExit();
            if(!GameManager.Instance.cityMap)
            {
                ChangeToBirdviewCamera();
            }
/*             Camera.main.transform.position = camPosition;
            Camera.main.transform.eulerAngles = new UnityEngine.Vector3(45,0,0);
            Camera.main.GetComponent<CameraController>().enabled = true;
            Camera.main.GetComponent<OrbitCamera>().enabled = false; */
        }
    }
    public void CallShopSystem()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        if(skillScreen)
        {
            CallSkillSystem();
        } else if(inventoryScreen)
        {
            CallInventorySystem();
        }
        else if(hireScreen)
        {
            CallHireSystem();
        }
        if(!shopScreen)
        {
            ClickSelect(unitList[selectedUnitnumber]);
            shopScreen = true;
            InventorySystemCanvas.SetActive(true);
            shopCanvas.gameObject.SetActive(true);
            ShopSystem.Instance.InitializeShop();
            //move current unit to market
            if(GameManager.Instance.cityMap)
            {
                unitList[selectedUnitnumber].transform.position = new UnityEngine.Vector3(50,0,0);
            }
                ChangeToOrbitCamera();
            
        }
        else
        {
            shopScreen = false;
            InventorySystemCanvas.SetActive(false);
            shopCanvas.gameObject.SetActive(false);
            InventorySystem.Instance.HoverExit();
            if(GameManager.Instance.cityMap)
            {
                unitList[selectedUnitnumber].transform.position = unitInitialPositionList[selectedUnitnumber];
                ChangeToOrbitCamera();
            }
            else
            {
                ChangeToBirdviewCamera();
            }
        }
    }

    public void CallSkillSystem()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        if(shopScreen)
        {
            CallShopSystem();
        } 
        else if(inventoryScreen)
        {
            CallInventorySystem();
        }
        else if(hireScreen)
        {
            CallHireSystem();
        }

        if(!skillScreen)
        {
            ClickSelect(unitList[selectedUnitnumber]);
            skillScreen = true;
            SkillSystemCanvas.SetActive(true);
            unitStatsCanvas.gameObject.SetActive(true);
            SkillSystem.Instance.InitializeSkillScreen();
            if(!GameManager.Instance.cityMap)
            {
                ChangeToOrbitCamera();
            }
        }
        else
        {
            skillScreen = false;
            SkillSystem.Instance.HoverOverExit();
            SkillSystemCanvas.SetActive(false);
            if(!GameManager.Instance.cityMap)
            {
                ChangeToBirdviewCamera();
            }
            SkillSystem.Instance.statbuttons.SetActive(false);
            unitStatsCanvas.gameObject.SetActive(false);
        }
    }

    public void CallOptions()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        if(shopScreen)
        {
            CallShopSystem();
        } 
        else if(inventoryScreen)
        {
            CallInventorySystem();
        }
        else if(hireScreen)
        {
            CallHireSystem();
        }
        else if(skillScreen)
        {
            CallSkillSystem();
        }
        if(optionScreen)
        {
            //GameManager.Instance.CloseOptionsMenu();
            EnableUnitSelections();
            Time.timeScale = 1;
            optionScreen = false;
        }
        else
        {
            Time.timeScale = 0;
            optionScreen = true;
            GameManager.Instance.InitializeOptionsMenu();
            DisableUnitSelections();
        }

    }

    public void ChangeToOrbitCamera()
    {
        camPosition = Camera.main.transform.position;
        Camera.main.GetComponent<CameraController>().enabled = false;
        Camera.main.GetComponent<OrbitCamera>().enabled = true;
        if(hireScreen)
        {
            Camera.main.GetComponent<OrbitCamera>().targetPos = new UnityEngine.Vector3(0,1.5f,0);
        }
        else if(shopScreen)
        {
            Camera.main.GetComponent<OrbitCamera>().targetPos = new UnityEngine.Vector3(50,1.5f,0);            
        }
        else
        {
            Camera.main.GetComponent<OrbitCamera>().targetPos = unitList[selectedUnitnumber].transform.position + new UnityEngine.Vector3(0,1.5f,0);
        }
        Camera.main.transform.LookAt(Camera.main.GetComponent<OrbitCamera>().targetPos); //look at body (not feet) of character
    }
    public void ChangeToBirdviewCamera()
    {
        Camera.main.transform.position = camPosition;
        Camera.main.transform.eulerAngles = new UnityEngine.Vector3(45,0,0);
        Camera.main.GetComponent<CameraController>().enabled = true;
        Camera.main.GetComponent<OrbitCamera>().enabled = false;
    }
    public void SelectCurrentUnit()
    {
        //get the current unit
        DeselectAll();
        unitsSelected.Add(unitList[selectedUnitnumber]);
        unitsSelected[0].transform.GetChild(0).gameObject.SetActive(true);
    }

    public void SelectTroop1()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SelectSound, GameManager.Instance.sfxVolume);
        foreach (var unit in unitsSelected)
        {
            unit.transform.GetChild(0).gameObject.SetActive(false);
        }

        unitsSelected.Clear();

        foreach(GameObject unit in troop1)
        {
            unitsSelected.Add(unit);
            unit.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public void SelectTroop2()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SelectSound, GameManager.Instance.sfxVolume);
        foreach (var unit in unitsSelected)
        {
            unit.transform.GetChild(0).gameObject.SetActive(false);
        }

        unitsSelected.Clear();

        foreach(GameObject unit in troop2)
        {
            unitsSelected.Add(unit);
            unit.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public void SelectTroop3()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SelectSound, GameManager.Instance.sfxVolume);
        foreach (var unit in unitsSelected)
        {
            unit.transform.GetChild(0).gameObject.SetActive(false);
        }

        unitsSelected.Clear();

        foreach(GameObject unit in troop3)
        {
            unitsSelected.Add(unit);
            unit.transform.GetChild(0).gameObject.SetActive(true);
        }
    }


    public void ClickSelect(GameObject unitToAdd)
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SelectSound, GameManager.Instance.sfxVolume);
        DeselectAll();
        DeselectEnemy();
        unitsSelected.Add(unitToAdd);
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
        //unitToAdd.GetComponent<UnitMovement>().enabled = true;
    }
    public void ShiftClickSelect(GameObject unitToAdd)
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SelectSound, GameManager.Instance.sfxVolume);
            DeselectEnemy();

            if (!unitsSelected.Contains(unitToAdd))
            {
                unitsSelected.Add(unitToAdd);
                //unitToAdd.GetComponent<UnitMovement>().enabled = true;
                unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            { 
                //unitToAdd.GetComponent<UnitMovement>().enabled = false;
                unitToAdd.transform.GetChild(0).gameObject.SetActive(false);
                unitsSelected.Remove(unitToAdd);
            }
    }
    public void DragSelect(GameObject unitToAdd)
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SelectSound, GameManager.Instance.sfxVolume);        
        DeselectEnemy();
        //check if unit already selected and if unit is no enemy
        if (!unitsSelected.Contains(unitToAdd))
        {
            //unitToAdd.GetComponent<UnitMovement>().enabled = true;
            unitsSelected.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
        }
        
    }
    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            //unit.GetComponent<UnitMovement>().enabled = false;
            unit.transform.GetChild(0).gameObject.SetActive(false);
        }
        unitsSelected.Clear();
    }

    
/*     public void Deselect(GameObject unitToDeselect)
    {
        
    } */

    public void ClickSelectEnemy(GameObject enemy)
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SelectSound, GameManager.Instance.sfxVolume);
        DeselectAll();
        enemySelected = enemy;
        enemy.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void DeselectEnemy()
    {
        //UICanvas.SetActive(false);
        if (enemySelected != null)
        {
            enemySelected.transform.GetChild(0).gameObject.SetActive(false);
            enemySelected = null;

        }
    }

    public void AttackCommand()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SkillSound, GameManager.Instance.sfxVolume);
        foreach (GameObject unit in unitsSelected)
        {
            Fighter fighterStats = unit.GetComponent<Fighter>();
            fighterStats.AttackCommand();
        }
    }
    public void StopCommand()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SkillSound, GameManager.Instance.sfxVolume);
        foreach (GameObject unit in unitsSelected)
        {
            Fighter fighterStats = unit.GetComponent<Fighter>();
            fighterStats.StopCommand();
        }
    }
    public void HoldCommand()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SkillSound, GameManager.Instance.sfxVolume);
        foreach (GameObject unit in unitsSelected)
        {
            Fighter fighterStats = unit.GetComponent<Fighter>();
            fighterStats.HoldCommand();
        }
    }

    public void ToggleSkill1()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SkillSound, GameManager.Instance.sfxVolume);
        foreach (GameObject unit in unitsSelected)
        {
            Fighter fighterStats = unit.GetComponent<Fighter>();
            fighterStats.ToggleSkill1();
            if(inventoryScreen || skillScreen)
            {
                InventorySystem.Instance.UpdateUnitStats();
            }
        }
    }    
    public void ToggleSkill2()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SkillSound, GameManager.Instance.sfxVolume);
        foreach (GameObject unit in unitsSelected)
        {
            Fighter fighterStats = unit.GetComponent<Fighter>();
            fighterStats.ToggleSkill2();
            if(inventoryScreen || skillScreen)
            {
                InventorySystem.Instance.UpdateUnitStats();
            }
        }
    }
    public void ToggleSkill3()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SkillSound, GameManager.Instance.sfxVolume);
        foreach (GameObject unit in unitsSelected)
        {
            Fighter fighterStats = unit.GetComponent<Fighter>();
            fighterStats.ToggleSkill3();
            if(inventoryScreen || skillScreen)
            {
                InventorySystem.Instance.UpdateUnitStats();
            }
        }
    }
    public void ToggleSkill4()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.SkillSound, GameManager.Instance.sfxVolume);
        foreach (GameObject unit in unitsSelected)
        {
            Fighter fighterStats = unit.GetComponent<Fighter>();
            if((skillScreen || inventoryScreen) && !shopScreen)     //only debuff when not in menu
            {
                fighterStats.ToggleSkill4(true);
                if(skillScreen)
                {
                    SkillSystem.Instance.UpdateUnitStats();
                }
                if(inventoryScreen)
                {
                    InventorySystem.Instance.UpdateUnitStats();
                }
            }
            else
            {
                fighterStats.ToggleSkill4(false);
            }
        }
    }




    public void GetSkillImage(int weaponType, Fighter stats)
    {
        switch(weaponType)
        {
            case 0: //unarmed
                skillToggle1.gameObject.SetActive(false);
                skillToggle2.gameObject.SetActive(false);
                skillToggle3.gameObject.SetActive(false);
            break;
            case 1: //sword
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = ArmorBypassImage;
                skillString1 = "<color=#DAA520>Armorbypass(Q)</color>  \nIncrease Armorbypass by " +WeaponEquip.Instance.swordBypassSkill;
                skillString2 = "<color=#DAA520>Sword-Speed(W)</color>  \nMultiplies attack speed by " +WeaponEquip.Instance.swordSpeedSkill;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = SwordSpeedImage;
                skillToggle3.gameObject.SetActive(false);
            break;
            case 2: //axe
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillString1 = "<color=#DAA520>Hard Hit(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.axeDmgSkill;
                skillString2 = "<color=#DAA520>Armor Destroyer(W)</color>  \nIncrease armor destruction by " +WeaponEquip.Instance.axeArmorDestructionSkill;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = ArmorDestructionImage;
                skillToggle3.gameObject.SetActive(false);
            break;
            case 3: //mace
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Hard Hit(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.maceDmgSkill;
                skillString2 = "<color=#DAA520>Basher(W)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.maceBashSkill;
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MaceBashImage;
                skillToggle3.gameObject.SetActive(false);
            break;
            case 4: //spear
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Reach Advantage(Q)</color>  \nMultiply melee evasion  by " +WeaponEquip.Instance.spearEvasionSkill;
                skillString2 = "<color=#DAA520>Spear-Wall(W)</color>  \nPush the enemy back out of your attack range";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = ReachAdvantageImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = SpearWallImage;
                skillToggle3.gameObject.SetActive(false);
            break;

            case 5: //unarmed+ buckler
                skillToggle1.gameObject.SetActive(false);
                skillToggle2.gameObject.SetActive(false);
                skillToggle3.gameObject.SetActive(true);
                skillString3 = "<color=#DAA520>Shield-Bash(E)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.shieldBashSkill;
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = ShieldBashImage;
            break;
            case 6: //sword + buckler
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Armorbypass(Q)</color>  \nIncrease Armorbypass by " +WeaponEquip.Instance.swordBypassSkill;
                skillString2 = "<color=#DAA520>Sword-Speed(W)</color>  \nMultiplies attack speed by " +WeaponEquip.Instance.swordSpeedSkill;
                skillString3 = "<color=#DAA520>Shield-Bash(E)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.shieldBashSkill;
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = ArmorBypassImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = SwordSpeedImage;      
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = ShieldBashImage;
            break;
            case 7: //axe + buckler
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Hard Hit(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.axeDmgSkill;
                skillString2 = "<color=#DAA520>Armor Destroyer(W)</color>  \nIncrease armor destruction by " +WeaponEquip.Instance.axeArmorDestructionSkill;
                skillString3 = "<color=#DAA520>Shield-Bash(E)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.shieldBashSkill;
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = ArmorDestructionImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = ShieldBashImage;
            break;
            case 8: //mace + buckler
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Hard Hit(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.maceDmgSkill;
                skillString2 = "<color=#DAA520>Basher(W)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.maceBashSkill;
                skillString3 = "<color=#DAA520>Shield-Bash(E)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.shieldBashSkill;
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MaceBashImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = ShieldBashImage;
            break;
            case 9: //spear + buckler
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Reach Advantage(Q)</color>  \nMultiply melee evasion  by " +WeaponEquip.Instance.spearEvasionSkill;
                skillString2 = "<color=#DAA520>Spear-Wall(W)</color>  \nPush the enemy back out of your attack range";
                skillString3 = "<color=#DAA520>Shield-Bash(E)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.shieldBashSkill;
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = ReachAdvantageImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = SpearWallImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = ShieldBashImage;
            break;

            case 10: //unarmed+ shield
                skillToggle1.gameObject.SetActive(false);
                skillToggle2.gameObject.SetActive(false);
                skillToggle3.gameObject.SetActive(true);
                skillString3 = "<color=#DAA520>Hunker-Down(E)</color>  \nIncrease your Block-Chance and reduce movement speed";
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = HunkerDownImage;
            break;
            case 11: //sword + shield
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Armorbypass(Q)</color>  \nIncrease Armorbypass by " +WeaponEquip.Instance.swordBypassSkill;
                skillString2 = "<color=#DAA520>Sword-Speed(W)</color>  \nMultiplies attack speed by " +WeaponEquip.Instance.swordSpeedSkill;
                skillString3 = "<color=#DAA520>Hunker-Down(E)</color>  \nIncrease your Block-Chance and reduce movement speed";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = ArmorBypassImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = SwordSpeedImage;       
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = HunkerDownImage;
            break;

            case 12: //axe + shield
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Hard Hit(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.axeDmgSkill;
                skillString2 = "<color=#DAA520>Armor Destroyer(W)</color>  \nIncrease armor destruction by " +WeaponEquip.Instance.axeArmorDestructionSkill;
                skillString3 = "<color=#DAA520>Hunker-Down(E)</color>  \nIncrease your Block-Chance and reduce movement speed";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = ArmorDestructionImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = HunkerDownImage;
            break;
            case 13: //mace + shield
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Hard Hit(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.maceDmgSkill;
                skillString2 = "<color=#DAA520>Basher(W)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.maceBashSkill;
                skillString3 = "<color=#DAA520>Hunker-Down(E)</color>  \nIncrease your Block-Chance and reduce movement speed";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MaceBashImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = HunkerDownImage;
            break;
            case 14: //spear + shield
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Reach Advantage(Q)</color>  \nMultiply melee evasion  by " +WeaponEquip.Instance.spearEvasionSkill;
                skillString2 = "<color=#DAA520>Spear-Wall(W)</color>  \nPush the enemy back out of your attack range";
                skillString3 = "<color=#DAA520>Hunker-Down(E)</color>  \nIncrease your Block-Chance and reduce movement speed";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = ReachAdvantageImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = SpearWallImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = HunkerDownImage;
            break;

            case 15: //sword x2
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Armorbypass(Q)</color>  \nIncrease Armorbypass by " +WeaponEquip.Instance.swordBypassSkill;
                skillString2 = "<color=#DAA520>Sword-Speed(W)</color>  \nMultiplies attack speed by " +WeaponEquip.Instance.swordSpeedSkill;
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = ArmorBypassImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = SwordSpeedImage;
                skillToggle3.gameObject.SetActive(false);
            break;
            case 16: //axe x2
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Hard Hit(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.axeDmgSkill;
                skillString2 = "<color=#DAA520>Armor Destroyer(W)</color>  \nIncrease armor destruction by " +WeaponEquip.Instance.axeArmorDestructionSkill;
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = ArmorDestructionImage;
                skillToggle3.gameObject.SetActive(false);
            break;
            case 17: //mace x2
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Hard Hit(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.maceDmgSkill;
                skillString2 = "<color=#DAA520>Basher(W)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.maceBashSkill;
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MaceBashImage;
                skillToggle3.gameObject.SetActive(false);
            break;
            case 18: //spear x2
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Reach Advantage(Q)</color>  \nMultiply melee evasion  by " +WeaponEquip.Instance.spearEvasionSkill;
                skillString2 = "<color=#DAA520>Spear-Wall(W)</color>  \nPush the enemy back out of your attack range";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = ReachAdvantageImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = SpearWallImage;
                skillToggle3.gameObject.SetActive(false);
            break;

            case 21: //2h sword
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Fatal Blow(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.twoHandedDmgSkill;
                skillString2 = "<color=#DAA520>Sword-Speed(W)</color>  \nMultiply attack speed by " +WeaponEquip.Instance.swordSpeedSkill;
                skillString3 = "<color=#DAA520>Round-Swing(E)</color>  \nAttack in a 180 degree arc in front of you!";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = SwordSpeedImage;
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = MeleeAoeImage;       
                skillToggle3.gameObject.SetActive(true);
            break;            
            case 22: //2h axe
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Fatal Blow(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.twoHandedDmgSkill;
                skillString2 = "<color=#DAA520>Armor Destroyer(W)</color>  \nIncrease armor destruction by" +WeaponEquip.Instance.axeArmorDestructionSkill;
                skillString3 = "<color=#DAA520>Round-Swing(E)</color>  \nAttack in a 180 degree arc in front of you!";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = ArmorDestructionImage;
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = MeleeAoeImage;      
                skillToggle3.gameObject.SetActive(true);
            break;
            case 23: //2h mace
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Fatal Blow(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.twoHandedDmgSkill;
                skillString2 = "<color=#DAA520>Basher(W)</color>  \nIncrease your chance to stun the enemy by" +WeaponEquip.Instance.maceBashSkill;
                skillString3 = "<color=#DAA520>Round-Swing(E)</color>  \nAttack in a 180 degree arc in front of you!";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MaceBashImage;  
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = MeleeAoeImage;  
                skillToggle3.gameObject.SetActive(true);
            break;
            case 24: //2h spear
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Fatal Blow(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.twoHandedDmgSkill;
                skillString2 = "<color=#DAA520>Pike-Wall(W)</color>  \nPush the enemy back out of your attack range";
                skillString3 = "<color=#DAA520>Round-Swing(E)</color>  \nAttack in a 180 degree arc in front of you!";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = SpearWallImage;
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = MeleeAoeImage;    
                skillToggle3.gameObject.SetActive(true);
            break;

            case 25: //crossbow
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Aim(Q)</color>  \nMultiply accuracy by " +WeaponEquip.Instance.crossbowAimSkill;
                skillString2 = "<color=#DAA520>Scattershot(W)</color>  \nHalf your range for a big aoe (" +WeaponEquip.Instance.crossbowScatterShot+ "m) attack with increased accuracy";
                skillString3 = "<color=#DAA520>Hammershot(E)</color>  \nHalf your range to get a chance to stun the enemy (" +WeaponEquip.Instance.crossbowStun + "%)";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = AimImage;     
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = CrossbowScattershotImage;    
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = MaceBashImage;
                skillToggle3.gameObject.SetActive(true);
            break;
            case 26: //bow
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Aim(Q)</color>  \nMultiply accuracy by " +WeaponEquip.Instance.bowAimSkill;
                skillString2 = "<color=#DAA520>Quickshot(W)</color>  \nMultiply attackspeed by " +WeaponEquip.Instance.bowQuickshot;
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = AimImage;     
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = QuickshotImage;
                skillToggle3.gameObject.SetActive(false);
            break;

            case 30: //arcane
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillString2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;       
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                skillToggle3.gameObject.SetActive(false);
            break;
            case 31: //fire
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillString2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillString3 = "<color=#DAA520>Healing Flames(E)</color>  \nReverse the debuff effect and heal a companion over time (no stamina cost increase)";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;      
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = HealingFlamesSupportImage;
            break;
            case 32: //water
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillString2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillString3 = "<color=#DAA520>Refreshing Liquid(E)</color>  \nReverse the debuff effect and increase attack and movement speed of a companion (no stamina cost increase)";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;            
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = RefreshingLiquidsSupportImage;
            break;
            case 33: //earth
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillString2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillString3 = "<color=#DAA520>Resistent Earth(E)</color>  \nInstead of stunning an opponent increase armor for a companion (no stamina cost increase)";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;              
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = ResistingStoneSupportImage;
            break;
            case 34: //air
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillString2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillString3 = "<color=#DAA520>Hasted Winds(E)</color>  \nReverse the debuff effect and increase stamina and movement speed of a companion (no stamina cost increase)";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;              
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = HastedWindSupportImage;
            break;
            case 35: //light
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillString2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillString3 = "<color=#DAA520>Illuminating Light(E)</color>  \nReverse the debuff effect and increase accuracy and evasion of a companion (no stamina cost increase)";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;           
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = IlluminatingLightSupportImage;
            break;
            case 36: //shadow
                skillToggle1.gameObject.SetActive(true);
                skillToggle2.gameObject.SetActive(true);
                skillString1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillString2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillString3 = "<color=#DAA520>Strenghtening Shadows(E)</color>  \nReverse the debuff effect and increase stamina and HP of a companion (no stamina cost increase)";
                skillToggle1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;         
                skillToggle2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                skillToggle3.gameObject.SetActive(true);
                skillToggle3.gameObject.GetComponentInChildren<Image>().sprite = StrengtheningShadowSupportImage;
            break;
        }
    }


    public void GetSkillImageForOtherScripts(int weaponType, Image img1, Image img2, Image img3)
    {
        switch(weaponType)
        {
            case 0: //armor
                img1.gameObject.SetActive(false);
                img2.gameObject.SetActive(false);
                img3.gameObject.SetActive(false);
            break;
            case 1: //sword
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                img1.gameObject.GetComponentInChildren<Image>().sprite = ArmorBypassImage;
                skillExplain1 = "<color=#DAA520>Armorbypass(Q)</color>  \nIncrease Armorbypass by " +WeaponEquip.Instance.swordBypassSkill;
                skillExplain2 = "<color=#DAA520>Sword-Speed(W)</color>  \nMultiplies attack speed by " +WeaponEquip.Instance.swordSpeedSkill;
                img2.gameObject.GetComponentInChildren<Image>().sprite = SwordSpeedImage;
                img3.gameObject.SetActive(false);
            break;
            case 2: //axe
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                img1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                skillExplain1 = "<color=#DAA520>Hard Hit(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.axeDmgSkill;
                skillExplain2 = "<color=#DAA520>Armor Destroyer(W)</color>  \nIncrease armor destruction by " +WeaponEquip.Instance.axeArmorDestructionSkill;
                img2.gameObject.GetComponentInChildren<Image>().sprite = ArmorDestructionImage;
                img3.gameObject.SetActive(false);
            break;
            case 3: //mace
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Hard Hit(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.maceDmgSkill;
                skillExplain2 = "<color=#DAA520>Basher(W)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.maceBashSkill;
                img1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                img2.gameObject.GetComponentInChildren<Image>().sprite = MaceBashImage;
                img3.gameObject.SetActive(false);
            break;
            case 4: //spear
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Reach Advantage(Q)</color>  \nMultiply melee evasion  by " +WeaponEquip.Instance.spearEvasionSkill;
                skillExplain2 = "<color=#DAA520>Spear-Wall(W)</color>  \nPush the enemy back out of your attack range";
                img1.gameObject.GetComponentInChildren<Image>().sprite = ReachAdvantageImage;
                img2.gameObject.GetComponentInChildren<Image>().sprite = SpearWallImage;
                img3.gameObject.SetActive(false);
            break;

            case 5: //buckler
                img1.gameObject.SetActive(false);
                img2.gameObject.SetActive(false);
                img3.gameObject.SetActive(true);
                img3.gameObject.GetComponentInChildren<Image>().sprite = ShieldBashImage;
                skillExplain3 = "<color=#DAA520>Shield-Bash(E)</color>  \nIncrease your chance to stun the enemy by " +WeaponEquip.Instance.shieldBashSkill;
            break;
            case 10: //shield
                img1.gameObject.SetActive(false);
                img2.gameObject.SetActive(false);
                img3.gameObject.SetActive(true);
                skillExplain3 = "<color=#DAA520>Hunker-Down(E)</color>  \nIncrease your Block-Chance and reduce movement speed";
                img3.gameObject.GetComponentInChildren<Image>().sprite = HunkerDownImage;
            break;

            case 21: //2h sword
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Fatal Blow(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.twoHandedDmgSkill;
                skillExplain2 = "<color=#DAA520>Sword-Speed(W)</color>  \nMultiply attack speed by " +WeaponEquip.Instance.swordSpeedSkill;
                skillExplain3 = "<color=#DAA520>Round-Swing(E)</color>  \nAttack in a 180 degree arc in front of you!";
                img1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                img2.gameObject.GetComponentInChildren<Image>().sprite = SwordSpeedImage;
                img3.gameObject.GetComponentInChildren<Image>().sprite = MeleeAoeImage;       
                img3.gameObject.SetActive(true);
            break;            
            case 22: //2h axe
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Fatal Blow(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.twoHandedDmgSkill;
                skillExplain2 = "<color=#DAA520>Armor Destroyer(W)</color>  \nIncrease armor destruction by" +WeaponEquip.Instance.axeArmorDestructionSkill;
                skillExplain3 = "<color=#DAA520>Round-Swing(E)</color>  \nAttack in a 180 degree arc in front of you!";
                img1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                img2.gameObject.GetComponentInChildren<Image>().sprite = ArmorDestructionImage;
                img3.gameObject.GetComponentInChildren<Image>().sprite = MeleeAoeImage;      
                img3.gameObject.SetActive(true);
            break;
            case 23: //2h mace
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Fatal Blow(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.twoHandedDmgSkill;
                skillExplain2 = "<color=#DAA520>Basher(W)</color>  \nIncrease your chance to stun the enemy by" +WeaponEquip.Instance.maceBashSkill;
                skillExplain3 = "<color=#DAA520>Round-Swing(E)</color>  \nAttack in a 180 degree arc in front of you!";
                img1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                img2.gameObject.GetComponentInChildren<Image>().sprite = MaceBashImage;  
                img3.gameObject.GetComponentInChildren<Image>().sprite = MeleeAoeImage;  
                img3.gameObject.SetActive(true);
            break;
            case 24: //2h spear
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Fatal Blow(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.twoHandedDmgSkill;
                skillExplain2 = "<color=#DAA520>Pike-Wall(W)</color>  \nPush the enemy back out of your attack range";
                skillExplain3 = "<color=#DAA520>Round-Swing(E)</color>  \nAttack in a 180 degree arc in front of you!";
                img1.gameObject.GetComponentInChildren<Image>().sprite = DmgImage;
                img2.gameObject.GetComponentInChildren<Image>().sprite = SpearWallImage;
                img3.gameObject.GetComponentInChildren<Image>().sprite = MeleeAoeImage;    
                img3.gameObject.SetActive(true);
            break;

            case 25: //crossbow
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Aim(Q)</color>  \nMultiply accuracy by " +WeaponEquip.Instance.crossbowAimSkill;
                skillExplain2 = "<color=#DAA520>Scattershot(W)</color>  \nHalf your range for a big aoe (" +WeaponEquip.Instance.crossbowScatterShot+ "m) attack with increased accuracy";
                skillExplain2 = "<color=#DAA520>Hammershot(E)</color>  \nHalf your range to get a chance to stun the enemy (" +WeaponEquip.Instance.crossbowStun + "%)";
                img1.gameObject.GetComponentInChildren<Image>().sprite = AimImage;     
                img2.gameObject.GetComponentInChildren<Image>().sprite = CrossbowScattershotImage;    
                img3.gameObject.GetComponentInChildren<Image>().sprite = MaceBashImage;
                img3.gameObject.SetActive(true);
            break;
            case 26: //bow
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Aim(Q)</color>  \nMultiply accuracy by " +WeaponEquip.Instance.bowAimSkill;
                skillExplain2 = "<color=#DAA520>Quickshot(W)</color>  \nMultiply attackspeed by " +WeaponEquip.Instance.bowQuickshot;
                img1.gameObject.GetComponentInChildren<Image>().sprite = AimImage;     
                img2.gameObject.GetComponentInChildren<Image>().sprite = QuickshotImage;
                img3.gameObject.SetActive(false);
            break;

            case 30: //arcane
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillExplain2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                img1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;       
                img2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                img3.gameObject.SetActive(false);
            break;
            case 31: //fire
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillExplain2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillExplain3 = "<color=#DAA520>Healing Flames(E)</color>  \nReverse the debuff effect and heal a companion over time (no stamina cost increase)";
                img1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;      
                img2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                img3.gameObject.SetActive(true);
                img3.gameObject.GetComponentInChildren<Image>().sprite = HealingFlamesSupportImage;
            break;
            case 32: //water
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillExplain2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillExplain3 = "<color=#DAA520>Refreshing Liquid(E)</color>  \nReverse the debuff effect and increase attack and movement speed of a companion (no stamina cost increase)";
                img1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;            
                img2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                img3.gameObject.SetActive(true);
                img3.gameObject.GetComponentInChildren<Image>().sprite = RefreshingLiquidsSupportImage;
            break;
            case 33: //earth
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillExplain2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillExplain3 = "<color=#DAA520>Resistent Earth(E)</color>  \nInstead of stunning an opponent increase armor for a companion (no stamina cost increase)";
                img1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;              
                img2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                img3.gameObject.SetActive(true);
                img3.gameObject.GetComponentInChildren<Image>().sprite = ResistingStoneSupportImage;
            break;
            case 34: //air
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillExplain2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillExplain3 = "<color=#DAA520>Hasted Winds(E)</color>  \nReverse the debuff effect and increase stamina and movement speed of a companion (no stamina cost increase)";
                img1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;              
                img2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                img3.gameObject.SetActive(true);
                img3.gameObject.GetComponentInChildren<Image>().sprite = HastedWindSupportImage;
            break;
            case 35: //light
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillExplain2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillExplain3 = "<color=#DAA520>Illuminating Light(E)</color>  \nReverse the debuff effect and increase accuracy and evasion of a companion (no stamina cost increase)";
                img1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;           
                img2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                img3.gameObject.SetActive(true);
                img3.gameObject.GetComponentInChildren<Image>().sprite = IlluminatingLightSupportImage;
            break;
            case 36: //shadow
                img1.gameObject.SetActive(true);
                img2.gameObject.SetActive(true);
                skillExplain1 = "<color=#DAA520>Focus(Q)</color>  \nMultiply damage by " +WeaponEquip.Instance.magicDmgSkill;
                skillExplain2 = "<color=#DAA520>Magic Diffusion(W)</color>  \nAttack an whole area (circle with 3m radius) with doubled accuracy, but halfed damage";
                skillExplain3 = "<color=#DAA520>Strenghtening Shadows(E)</color>  \nReverse the debuff effect and increase stamina and HP of a companion (no stamina cost increase)";
                img1.gameObject.GetComponentInChildren<Image>().sprite = MagicFocusImage;         
                img2.gameObject.GetComponentInChildren<Image>().sprite = MagicAoeImage;
                img3.gameObject.SetActive(true);
                img3.gameObject.GetComponentInChildren<Image>().sprite = StrengtheningShadowSupportImage;
            break;
        }
        
    }

    public void ChangetargetSize(Fighter stats, float newSize)
    {
        foreach(GameObject unit in unitList)
        {
            Fighter attackerStats = unit.GetComponent<Fighter>();
            Debug.Log(stats);
            Debug.Log(attackerStats);
            if(attackerStats.targetStats == stats)
            {
                attackerStats.enemySize = newSize;
            }
        }
    }

    public void RemoveEnemyAsTarget(Fighter enemyStats)
    {
            if(enemySelected == enemyStats.gameObject)
            {
                enemySelected = null;
            }
           
        //go through all units and remove the enemy as target
        foreach(GameObject unit in unitList)
        {

            Unit attackerStats = unit.GetComponent<Unit>();
            if(attackerStats.targetStats == enemyStats)
            {
                attackerStats.targetStats = null;
                attackerStats.target = null;
            }
        }
    }
    public void RemoveUnitAsTarget(Fighter unitStats)
    {            
        if(unitsSelected.Contains(unitStats.gameObject))
        {
            unitsSelected.Remove(unitStats.gameObject);
        } 

        foreach(GameObject enemy in enemiesList)
        { 
            Fighter attackerStats = enemy.GetComponent<Fighter>();
            if(attackerStats.targetStats == unitStats)
            {
                attackerStats.targetStats = null;
                attackerStats.target = null;
            }
        }
    }
}
