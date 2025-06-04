using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using SoftKitty.MasterCharacterCreator;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{    
    private int lastWindowWidth;
    private int lastWindowHeight;

    public float defaultSpeed;
    private readonly string verschluesselung = "kartoffel";
    public bool cityMap; // is it a shopping/quest map or if false battle map
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    public List<GameObject> OptionsButtonFramesList;
    public GameObject GoldIndicator;
    public GameObject PrestigeIndicator;
    public GameObject OptionsCanvas;
    public GameObject ConfirmMenu;
    public GameObject QuitGameMenu;
    public GameObject RestartGameMenu;
    public GameObject ScoreboardContinueButton;
    public TMP_Text MissionText;
    private int whichTutorialTip;
    public Slider BGMSlider;
    public Slider SFXSlider;
    public float sfxVolume = 0.5f;
    public float bgmVolume = 0.5f;

    public int mapEdgeRight;
    public int mapEdgeLeft;
    public int mapEdgeTop;
    public int mapEdgeBot;
    public int mapEdgeGround;
    public int mapEdgeSky;

    public int aggroRange;

    public int gold;
    public int prestige;
    public int maxTroopStrength;
    public int arenaLevel = 0;
    private bool gameStart;
    public AudioClip SkillSound;
    public AudioClip SelectSound;
    public AudioClip WarningSound;
    public AudioClip ClickSound;
    public AudioClip ClickDenySound;
    public AudioClip BattleSound;
    public AudioClip VictorySound;
    public AudioClip SaveSound;
    public AudioClip LoadSound;
    public AudioClip flipPageSound;
    public AudioClip BattleBGM;
    public AudioClip CityBGM;
    public AudioClip reviveSound;
    public AudioClip GameOverSound;
    public GameObject Warning;
    public bool tutorialTips = true;
    public GameObject tutorialTipsToggle;
    public GameObject TutorialCanvas;
    public TMP_Text TutorialText;
    public int tipIndex;
    public Toggle FullScreenToggle;
    private bool blockFullscreenCallback = false;

    //score stats
    public GameObject ScoreBoard;
    public TMP_Text FloorLevel;
    public TMP_Text battleTime;
    public TMP_Text breakCounter;
    public TMP_Text totalDamage;
    public TMP_Text totalArmorDestruction;
    public TMP_Text totalFightersLost;
    public TMP_Text totalBuffTime;
    public TMP_Text totalDebuffTime;   
    public TMP_Text totalScore;    


    public float fightTime;
    public int pauseCounter;
    public int damageDealt;
    public int armorDestroyed;
    public int fightersLost;
    public float buffTimer;
    public float debuffTimer;



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
    }
    void Start()
    {        
        // Store the initial screen size on startup
        lastWindowWidth = Screen.width;
        lastWindowHeight = Screen.height;
        
        ChangeGold(0);
        ChangePrestige(0);
        GetComponent<AudioSource>().volume = bgmVolume;

        if(!gameStart)
        {        
            UnitSelections.Instance.CallOptions();
            Unit player1 = UnitSelections.Instance.inActiveUnitList[0].GetComponent<Unit>();
            switch(Random.Range(1,8))
            {
                case 1:
                    player1.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/1HShortSword");
                    ChangeGold(-20);
                break;
                case 2:
                    player1.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/1HAxe");
                    ChangeGold(-5);
                break;
                case 3:
                    player1.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/1HSpear");
                break;
                case 4:
                    player1.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/1HBludgeon");
                break;
                case 5:
                    player1.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/Crossbow");
                    ChangeGold(-30);
                break;
                case 6:
                    player1.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/WoodenBow");
                    ChangeGold(-20);
                break;
                case 7:
                    player1.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/1HBludgeon");
                    ChangeGold(-55);
                break;
            }
            //player1.LoadItems();
            for(int i=0; i<9; i++)
            {
                if(player1.itemObjectList[i]!=null)
                {
                    player1.itemList[i]= Instantiate(player1.itemObjectList[i]);
                    DontDestroyOnLoad(player1.itemList[i]);
                } 
                else
                {
                    player1.itemList[i]= null;
                }
            } 
            player1.LoadItems();
            //UnitSelections.Instance.inActiveUnitList[0].GetComponent<Unit>().LoadItems();
            UnitSelections.Instance.SelectNextUnit();
            gameStart = true;
        }
    }

    public void ShowScoreboard()
    {
        GetComponent<AudioSource>().PlayOneShot(flipPageSound, sfxVolume);
        if(UnitSelections.Instance.gameOver)
        {
            ScoreboardContinueButton.SetActive(false);
        }
        else
        {      
            ScoreboardContinueButton.SetActive(true);
        }
        ScoreBoard.SetActive(true);
        FloorLevel.text = ""+arenaLevel;
        battleTime.text = ""+Mathf.FloorToInt(fightTime);
        breakCounter.text = ""+pauseCounter;
        totalDamage.text = ""+damageDealt;
        totalArmorDestruction.text = ""+armorDestroyed;
        totalFightersLost.text = ""+fightersLost;
        totalBuffTime.text = ""+Mathf.FloorToInt(buffTimer);
        totalDebuffTime.text = ""+Mathf.FloorToInt(debuffTimer);
        //add score and reduce by 1% for pause 10% for fighters unconcsious and divide by time
        totalScore.text = ""+Mathf.FloorToInt(arenaLevel*1000+damageDealt+armorDestroyed+Mathf.FloorToInt(buffTimer*10)+Mathf.FloorToInt(debuffTimer*5)-fightersLost*100-pauseCounter*10);
    }

    public void CloseScoreboard()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickSound, sfxVolume);
        ScoreBoard.SetActive(false);
    }

    public void ChangeGold(int goldChange)
    {
        gold += goldChange;
        GoldIndicator.GetComponentInChildren<TMP_Text>().text = ""+gold;
    }
    public void ChangePrestige(int prestigeChange)
    {
        //used for shop item selection and old for troop strength
        prestige += prestigeChange;
    }

    public void InitializeOptionsMenu()
    {
        GetComponent<AudioSource>().PlayOneShot(flipPageSound, sfxVolume);
        OptionsCanvas.SetActive(true);
        BGMSlider.value = bgmVolume;
        SFXSlider.value = sfxVolume;
        //don't activate toggle by setting the value
        blockFullscreenCallback = true;
        FullScreenToggle.isOn = Screen.fullScreen;
        blockFullscreenCallback = false;
        GetTutorialTipText(whichTutorialTip);
    }

    public void ToggleTutorialTips()
    {
        if(tutorialTips)
        {
            GetComponent<AudioSource>().PlayOneShot(ClickDenySound, sfxVolume);
            tutorialTipsToggle.SetActive(false);
            InventorySystem.Instance.tutorialTips = false;
            ShopSystem.Instance.tutorialTips = false;
            HireSystem.Instance.tutorialTips = false;
            SkillSystem.Instance.tutorialTips = false;
            tutorialTips = false;
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(ClickSound, sfxVolume);
            tutorialTipsToggle.SetActive(true);
            tutorialTips = true;
            //resets the tutorial tips for the menus
            InventorySystem.Instance.tutorialTips = true;
            ShopSystem.Instance.tutorialTips = true;
            HireSystem.Instance.tutorialTips = true;
            SkillSystem.Instance.tutorialTips = true;
        }
    }

    public void ToggleFullScreen()
    {
        if(blockFullscreenCallback) return;

        if (Screen.fullScreen)
        {
            // Leaving fullscreen → return to previous window size
            Screen.SetResolution(lastWindowWidth, lastWindowHeight, false);
        }
        else
        {
            // Store current window size before going fullscreen
            lastWindowWidth = Screen.width;
            lastWindowHeight = Screen.height;

            // Switch to fullscreen
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
    }

    public void Tutorial()
    {
        TutorialCanvas.SetActive(true);
        if(cityMap)
        {
            if(UnitSelections.Instance.hireScreen)
            {
                int tipIndextotal = 4;  
                if(tipIndex > tipIndextotal)
                {
                    tipIndex = 1;
                }
                switch(tipIndex)
                {
                    case 1:
                        TutorialText.text = "This is the Hire Screen. You can open and close it by pressing the hotkey H. Your possible roster size increases with arena wins. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 2:
                        TutorialText.text = "On the left side you can see the possible recruits. Their class determines their level up attributes and the possible skills they will be able to learn. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 3:
                        TutorialText.text = "The level determines the attribute and skill points of a character, though be careful their possible skillpoints are limited by their potential. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 4:
                        TutorialText.text = "Don't forget to check the second recruitment page as well. If you later on feel like firing on of your recruited character just click on their name on the roster list on the right side. "+tipIndex+"/"+tipIndextotal;
                    break;
                }
            }
            else if(UnitSelections.Instance.shopScreen)
            {
                int tipIndextotal = 5;  
                if(tipIndex > tipIndextotal)
                {
                    tipIndex = 1;
                }
                switch(tipIndex)
                {
                    case 1:
                        TutorialText.text = "This is the market screen where you can buy new items and repair old ones. You can open and close it by pressing the hotkey M. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 2:
                        TutorialText.text = "Left-click to buy items or activate other shop buttons. Right-click on items to get more details, like e.g. the weapon-specific abilities. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 3:
                        TutorialText.text = "You can sort items by clicking the images in the left of middle window of the shop screen, repair all your items by clicking on the repair button and refresh the shop items with the refresh button. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 4:
                        TutorialText.text = "The quality and number of the shown items depend on the number of won arena fights. You can flip through the shop pages like the inventory pages, with the small numbers if you got more items. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 5:
                        TutorialText.text = "You can still use your inventory to equip and unequip and in this screen also to sell and repair single items. A small window next to the unit inventory indicates the main attributes. "+tipIndex+"/"+tipIndextotal;
                    break;
                }
            }
            else if(UnitSelections.Instance.inventoryScreen)
            {
                int tipIndextotal = 5;  
                if(tipIndex > tipIndextotal)
                {
                    tipIndex = 1;
                }
                switch(tipIndex)
                {
                    case 1:
                        TutorialText.text = "This is the inventory screen where you can equip your characters by left-clicking. You can open and close it by pressing the hotkey I. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 2:
                        TutorialText.text = "On the left side is the attribute page. If your character has attribute points, you can click on the '+' buttons to increase the respective attribute. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 3:
                        TutorialText.text = "By right-clicking an item you get further options like equipping it to the offhand if it is a one-handed melee weapon of the same category as the main-hand weapon. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 4:
                        TutorialText.text = "You can swap between a characters first and second item set by clicking on the numbers 1 or 2 above the weapons in the character inventory. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 5:
                        TutorialText.text = "Besides choosing fitting weapons for a characters abilities you should keep an eye on a characters stamina, so he will have stamina left in battle. "+tipIndex+"/"+tipIndextotal;
                    break;
                }
            }       
            else if(UnitSelections.Instance.skillScreen)
            {
                int tipIndextotal = 5;  
                if(tipIndex > tipIndextotal)
                {
                    tipIndex = 1;
                }
                switch(tipIndex)
                {
                    case 1:
                        TutorialText.text = "This is the levelling screen where you use a characters attribute and skillpoints. You can open and close it by pressing the hotkey L. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 2:
                        TutorialText.text = "Every character gains 4 skill points with each level up, but aside from the main character 2 of these will be distributed automatically according to their class. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 3:
                        TutorialText.text = "Until a character reaches his potential it gains a skill point every second level. The available skills are limited by a characters' level and class. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 4:
                        TutorialText.text = "Characters can get a second promotion class that buff themselves or the whole group. To get to know more about the requirements and quirks of each promotion class you can select the help button underneath 'Promotion Class'. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 5:
                        TutorialText.text = "Take a look at the skills and use your attribute points now as you see fit to prepare for battle. "+tipIndex+"/"+tipIndextotal; 
                    break;
                }
            }
        }
        //arena = bool citymap == false
        else
        {   
            if(arenaLevel==3)
            {
                int tipIndextotal = 4;   
                if(tipIndex > tipIndextotal)
                {
                    tipIndex = 1;
                }
                switch(tipIndex)
                {
                    case 1:
                        TutorialText.text = "Most enemies attack the character that is closest to them, but ranged humans and these monsters here behave differently. They attack the target with the lowest HP. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 2:
                        TutorialText.text = "You can force them to switch their target by attacking them. If their target is out of their range, and they get attacked, they will change their target to the character attacking them. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 3:
                        TutorialText.text = "To protect your backline character, you can use the Hold Position Command(D) on all your characters to hold formation and give move commands. This way, your frontliners can attack the enemies while they charge at your backline and change their target. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 4:
                        TutorialText.text = "You can use the pause that is initiating the fight to already issue these commands.  "+tipIndex+"/"+tipIndextotal;
                    break;
                }
            }               
            else if(arenaLevel==2)
            {
                int tipIndextotal = 2;   
                if(tipIndex > tipIndextotal)
                {
                    tipIndex = 1;
                }
                switch(tipIndex)
                {
                    case 1:
                        TutorialText.text = "These little demons have pretty weak stats, but they attack with a shadow debuff, which decreases max hp and stamina for a short duration. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 2:
                        TutorialText.text = "They have increased resistance versus physical and shadow attacks, but are weak to light attacks. "+tipIndex+"/"+tipIndextotal;
                    break;
                }
            }   
            else if(arenaLevel==0)
            {       
                int tipIndextotal = 6;   
                if(tipIndex > tipIndextotal)
                {
                    tipIndex = 1;
                }
                switch(tipIndex)
                {
                    case 1:
                        TutorialText.text = "If you have your character on Attack command he will attack the closest target within 100m. Your enemy has not a lot of defense. If you still have your initial weapon equipped activate hard hit(Q) for a bit to see your enemy loose hp quickly. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 2:
                        TutorialText.text = "Use the left mouse button to select units and abilites and the right-mouse button to attack enemies. Zoom with the mouse wheel. For quicker commands use the indicated Hotkeys. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 3:
                        TutorialText.text = "Every ability doubles the stamina cost, so be aware of your characters' current stamina indicated by the lower green bar above your character(s). Without stamina there is no attack or defense. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 4:
                        TutorialText.text = "By scrolling to the right and viewing your enemies before you unpause, you can prepare your tactics. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 5:
                        TutorialText.text = "Every character will get fully healed after a fight, but unconscious characters reduce your final score. "+tipIndex+"/"+tipIndextotal;
                    break;
                    case 6:
                        TutorialText.text = "By pressing ALT+1, +2 or +3, you can assign the currently selected character(s) to a group that you can then select by only pressing the numbers 1,2 or 3. "+tipIndex+"/"+tipIndextotal;
                    break;
                }
            }
        }
    }

    public void CloseTutorial()
    {
        TutorialCanvas.SetActive(false);
        if(UnitSelections.Instance.hireScreen)
        {
            HireSystem.Instance.tutorialTips = false;
            HireSystem.Instance.InitializeHireSystem();
        }
        else if(UnitSelections.Instance.shopScreen)
        {
            ShopSystem.Instance.tutorialTips = false;
            ShopSystem.Instance.EnableInput();
            InventorySystem.Instance.EnableInput();
            ShopSystem.Instance.InitializeShop();
        }
        else if(UnitSelections.Instance.inventoryScreen)
        {
            InventorySystem.Instance.tutorialTips = false;
            InventorySystem.Instance.EnableInput();
            InventorySystem.Instance.InitializeInventory();
            
        } 
        else if(UnitSelections.Instance.skillScreen)
        {
            SkillSystem.Instance.tutorialTips = false;
            SkillSystem.Instance.InitializeSkillScreen();
        }
        UnitSelections.Instance.EnableUnitSelections();
    }

    public void NextTutorialTip()
    {
        tipIndex++;
        Tutorial();
    }

    public void GetTutorialTipText(int buttonIndex)
    {
        foreach(GameObject frame in OptionsButtonFramesList)
        {
            frame.SetActive(false);
        }
        OptionsButtonFramesList[buttonIndex].SetActive(true);
        GetComponent<AudioSource>().PlayOneShot(flipPageSound, sfxVolume);
        whichTutorialTip = buttonIndex;
        switch(whichTutorialTip)
        {
            case 0:
                MissionText.text = "Take a look around before joining your first battle. You can hover buttons to get more infos. If you don't like tutorial tips uncheck the checkbox below. You can click the on the Tutorial tip buttons on the top for more general info! Good luck and have fun!";
            break;
            case 1:
                MissionText.text = "<color=#DAA520>(L)eveling</color> allows you to increase your characters' stats and skills, which depend on their base class! A unit's level is indicated in the XP bar on the unit image at the bottom of the screen. Defeated enemies give xp that will be distributed to all conscious party members!";
            break;
            case 2:
                MissionText.text = "In the <color=#DAA520>(I)nventory</color> equip your characters. You can right-click for further options, like equipping a one-handed weapon to the offhand! Use a unit's skillbar at the bottom and click on the weapon swap ability to get access to the second weapon set!";
            break;
            case 3:
                MissionText.text = "<color=#DAA520>(H)ire</color> new units! Take into account that the possibilities of a character depend on their class, their level and potential. The higher the starting level and potential, the more expensive a unit is. Also think about the fact that your roster size is limited by your prestige, which is gained by winning battles and your money!";
            break;
            case 4:
                MissionText.text = "The <color=#DAA520>(M)arket</color> gives you the opportunity to buy new equipment. You can sort by equipment type for a better overview. The higher your prestige, the more items will be offered, and it also increases your chances for higher-quality items. For a small fee, you can use the refresh button to roll for new items. Repair all your items with the item repair button or single items, by right-clicking them in the inventory.";
            break;
            case 5:
                MissionText.text = "Don't start the <color=#DAA520>(B)attle</color> until you're ready. In the arena, you control your units from the bird's view. Zoom in and out with the mouse wheel. You can move the view by edge- or middle-mousebutton panning. Left-click to select units. You can also press shift to add units to the selection or just drag the mouse to select several units at once. Press Tab to select the next character and Space to focus the camera on the currently selected unit. Right-click to select a target.";
            break;
            case 6:
                MissionText.text = "Depending on their <color=#DAA520>weapons</color>, characters have different active skills. Each activated skill doubles the stamina cost for each attack. If you have no stamina left, you can neither defend nor attack. Every character has the attack command to constantly attack its closest enemy. Stop: to stay at its current location without attacking and the hold command, to hold its position and attack every enemy that comes within range.";
            break;
            case 7:
                MissionText.text = "<color=#DAA520>Swords</color> are expensive and have good stats, but they are on the lower end when it comes to dealing with armor. The first sword ability makes up a bit for this by ignoring the armor to an extent, but this doesn't destroy armor. The second skill increases the attack speed, as advanced swordmasters can do by default with the sworddancer skill.";
            break;
            case 8:
                MissionText.text = "<color=#DAA520>Axes</color> deal a bit better with armor than swords. Their first skill allows them to increase their damage and the second to increase the armor destroyed, which each hit drastically. The Thug class mastery Armorbreaker skill allows them to destroy more armor with each hit by default.";
            break;
            case 9:
                MissionText.text = "<color=#DAA520>Maces</color> are the best weapons against armor without skill usage, but they are harder to block with and have the highest base attack stamina cost. The first ability allows you to increase the damage and the second allows for a chance to stun the enemy. Making them unable to attack and lowering their defense. Brawlers can use their class mastery skill at Tier 3 to make sure every hit stuns their opponent.";
            break;
            case 10:
                MissionText.text = "<color=#DAA520>Spears</color> are cheap weapons with good accuracy and, due to their range, also good defensive capabilities, though they are the worst weapon type versus armor. The first spear ability tries to minimize that flow by giving the attack a percentage of damage that bypasses armor exactly like the sword ability. The second ability is especially good at keeping enemies away as it pushes them backwards, hopefully out of their attack range. The guards' class specialization skill improves their second ability even further.";
            break;
            case 11:
                MissionText.text = "<color=#DAA520>Shields</color> increase the fighters' melee and defense block chance. There are two types of shields: Bucklers add the ability to stun the enemy with each attack like a mace, without dealing extra damage. Other shields add the ability to increase the blocking abilities even more for a movement speed penalty and the usual increased stamina cost for base attacks, of course.";
            break;
            case 12:
                MissionText.text = "<color=#DAA520>Crossbows</color> are relatively easy to handle and have a high accuracy, which makes them a good choice for damage at low levels. The crossbow doesn't destroy much armor on hit, but it bypasses a lot of it instead. With the first skill, it can bypass even more, making enemy armor almost useless. The second skill increases its range and accuracy.";
            break;
            case 13:
                MissionText.text = "<color=#DAA520>Bows</color> give the same abilities as crossbows, but are vastly different otherwise. A bow is harder to use and gives lower accuracy at first, but a character that has mastered the bow shoots accurately and in quick succession. Even though the hunter class doesn't have defensive abilities, it has enormous damage capabilities from a great distance.";
            break;
            case 14:
                MissionText.text = "<color=#DAA520>Staffs</color> shoot projectiles and therefore use the intelligence stat for damage instead of strength. Apprentices start with a simple arcane staff, but can also use staffs of all elements. The elements add status effects to their targets (Fire=burn-damage, Water=slow, Earth=stun, Wind=push, Light=blind, Shadow=weaken). The first ability increases damage, the second reduces damage, but instead of a single target, a whole area will be affected with increased hit chance and the third ability reverses the status effects to buff allies.";
            break;
        }
    }

    public void CloseOptionsMenu()
    {
        OptionsCanvas.SetActive(false);
        UnitSelections.Instance.CallOptions();
    }

    public void SetSFXVolume()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickSound, sfxVolume);
        sfxVolume = SFXSlider.value;
    }    
    public void SetBGMVolume()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickSound, sfxVolume);
        bgmVolume = BGMSlider.value;
        GetComponent<AudioSource>().volume = bgmVolume;
        if(cityMap)
        {
            GameObject.Find("BGSTavern").GetComponent<AudioSource>().volume = bgmVolume*0.5f;
            GameObject.Find("BGSForge").GetComponent<AudioSource>().volume = bgmVolume;
        }
    }
    
    public void QuitGameButton()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickSound, sfxVolume);
        ConfirmMenu.SetActive(true);
        QuitGameMenu.SetActive(true);
    }
    public void CancelQuitGame()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickDenySound, sfxVolume);
        ConfirmMenu.SetActive(false);
        QuitGameMenu.SetActive(false);
    }

    public void QuitGame()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickSound, sfxVolume);
        Application.Quit();
    }

    public void RestartButton()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickSound, sfxVolume);
        ConfirmMenu.SetActive(true);
        RestartGameMenu.SetActive(true);
    }
    public void CancelRestart()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickDenySound, sfxVolume);
        ConfirmMenu.SetActive(false);
        RestartGameMenu.SetActive(false);
    }

    public void Restart()
    {
        GetComponent<AudioSource>().PlayOneShot(ClickSound, sfxVolume);
        ConfirmMenu.SetActive(false);
        RestartGameMenu.SetActive(false);
        StartCoroutine(LoadingSignal());
        ResetFromArenaToTavern();

        //reset data
            //before stop list for right items
            arenaLevel = 0;
            maxTroopStrength = 2;
            gold = 1000;
            ChangeGold(0);
            prestige = 730;
            ChangePrestige(0);
            fightTime = 0;
            pauseCounter = 0;
            damageDealt = 0;
            armorDestroyed = 0;
            fightersLost = 0;
            buffTimer = 0;
            debuffTimer = 0;
            gameStart = false;

            //activate the unit game objects in hierarchy that were saved as active
            UnitSelections.Instance.unitList.Clear();

            for(int i=0; i<UnitSelections.Instance.inActiveUnitList.Count; i++)
            {
                //only set main player as actibe
                if(i==0)
                {
                    UnitSelections.Instance.inActiveUnitList[i].SetActive(true);
                    UnitSelections.Instance.unitList.Add(UnitSelections.Instance.inActiveUnitList[i]);
                }
                else
                {
                    UnitSelections.Instance.inActiveUnitList[i].SetActive(false);                
                    //reset entity appearances
                    Unit stats = UnitSelections.Instance.inActiveUnitList[i].GetComponent<Unit>(); 
                    FighterClasses.Instance.GetPromotedClassBoni(stats, stats.promotedFighterClass, -1);
                    //unequip
                    CharacterEntity myEntity = stats.GetComponent<CharacterEntity>();
                    if(stats.skill4Active)
                    {
                        WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], -1);
                    }
                    else
                    {
                        WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], -1);
                    }
                    for(int j=4; j<9; j++)
                    {
                        if(stats.itemList[j]!=null)
                        {
                            myEntity.Unequip(stats.itemList[j].mySlot);
                            InventorySystem.Instance.EquipItemStats(stats.itemList[j], stats, -1); 
                        }
                    }
                    //remove fighter classes, so that they don't remove multiple auras over multiple restarts
                    for(int j=0; j<stats.promotionClassList.Count; j++)
                    {
                        stats.promotionClassList[j] = 0;
                    }
                    stats.promotedFighterClass = 0;
                }
            }

            for(int i=0; i<UnitSelections.Instance.unitList.Count; i++)
            {


                Unit stats = UnitSelections.Instance.unitList[i].GetComponent<Unit>(); 
                FighterClasses.Instance.GetPromotedClassBoni(stats, stats.promotedFighterClass, -1);
                //unequip
                CharacterEntity myEntity = stats.GetComponent<CharacterEntity>();
                if(stats.skill4Active)
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], -1);
                }
                else
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], -1);
                }
                for(int j=4; j<9; j++)
                {
                    if(stats.itemList[j]!=null)
                    {
                        myEntity.Unequip(stats.itemList[j].mySlot);
                        InventorySystem.Instance.EquipItemStats(stats.itemList[j], stats, -1); 
                    }
                }

                stats.unitName = "Matt";

                stats.maxHP= 100;
                stats.currentHP = 100;
                stats.maxStamina = 100;
                stats.currentStamina = 100;
                //data.dmg.Add(stats.dmg);
                stats.pDmg = 10;
                stats.mDmg = 10;
                stats.mAcc = 10;
                stats.mEva = 10;
                stats.rAcc = 10;
                stats.rEva = 10;

                stats.armor = 0;

                stats.speed = defaultSpeed;

                stats.aggroRange = aggroRange;
                //secondary attributes done by equipment see at bottom
                //levelsystem
                stats.currentXP = 0;
                stats.xpToNextLevel = 200;
                stats.level = 10;
                stats.potential = 37;
                stats.skillPoints = 6;
                stats.attributePoints = 36;
                stats.fighterClass = 0;
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
                //stats.quickdrawSkill = 0;
                stats.oneHandedSkill = 0;
                stats.dualwieldingSkill = 0;
                stats.twohandedSkill = 0;
                stats.shieldWallSkill = 0;
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
                stats.tinkerSkill = 0;
                stats.hawkeyeSkill = 0;
                stats.quickshotSkill = 0;
                stats.bufferSkill = 0;
                stats.debufferSkill = 0;
                stats.wisdomSkill = 0;
                //veteranskills
                //stats.specialistSkill = 0;
                stats.conterSkill = 0;
                stats.whirlWindSkill = 0;
                stats.stormTrooperSkill = 0;
                stats.lionheartSkill = 0;
                stats.marathonerSkill = 0;
                stats.unstoppableSkill = 0;
                stats.survivalistSkill = 0;
                stats.elementalistSkill = 0;
                stats.elementalistSkillElement = 0;
                stats.magicDiffusionSkill = 0;
                stats.magicResistanceSkill = 0;
                //appearance
                stats.appearance = "Matt";
                stats.unitImage = Resources.Load<Sprite>("Faces/face_"+stats.appearance);
                myEntity.LoadFromResourceFile("MasterCharacterCreator/CustomBlueprints/Characters/"+stats.appearance);
                //random mainhand
                switch(Random.Range(1,8))
                {
                    case 1:
                        stats.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/1HShortSword");
                        ChangeGold(-20);
                    break;
                    case 2:
                        stats.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/1HAxe");
                        ChangeGold(-5);
                    break;
                    case 3:
                        stats.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/1HSpear");
                    break;
                    case 4:
                        stats.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/1HBludgeon");
                    break;
                    case 5:
                        stats.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/Crossbow");
                        ChangeGold(-30);
                    break;
                    case 6:
                        stats.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/WoodenBow");
                        ChangeGold(-20);
                    break;
                    case 7:
                        stats.itemObjectList[0] = Resources.Load<WeaponStats>("Equipment/Weapon/StaffArcane");
                        ChangeGold(-55);
                    break;
                }
                //player1.LoadItems();
                for(int j=0; j<9; j++)
                {
                    if(stats.itemObjectList[j]!=null)
                    {
                        stats.itemList[j]= Instantiate(stats.itemObjectList[j]);
                        stats.itemList[j].armor = stats.itemList[j].maxArmor;
                        DontDestroyOnLoad(stats.itemList[j]);
                    } 
                    else
                    {
                        stats.itemList[j]= null;
                    }
                } 
                stats.LoadItems();
                stats.Setup();
            }

            for(int i=0; i<InventorySystem.Instance.itemStatsList.Count; i++)
            {
                InventorySystem.Instance.itemStatsList[i]=null;
            }




            ShopSystem.Instance.itemStatsList.Clear();
            ShopSystem.Instance.RefreshShop();

            UnitSelections.Instance.unitInitialPositionList.Clear();         
            UnitSelections.Instance.unitInitialPositionList.Add(UnitSelections.Instance.unitList[0].transform.position);
            
            
            HireSystem.Instance.nameList.Clear();
            HireSystem.Instance.appearanceList.Clear();
            HireSystem.Instance.classList.Clear();
            for(int i=0; i<14;i++)
            {
                HireSystem.Instance.appearanceList.Add("unit"+(i+1));
            }
            HireSystem.Instance.classList = new List<int>(){1,1,2,2,3,3,4,4,5,5,6,6,7,7};
            HireSystem.Instance.nameList = new List<string>(){"Gus-tough", "Jeremy", "Torlaf", "Kristoffersen", "Krogan", "Alrik", "Spencer", "Sid", "Krieg", "Kirto", "Yuen", "Amari", "Klaus", "Jordan"};
            HireSystem.Instance.Start();


        
        //UnitSelections.Instance.CallOptions();
        UnitSelections.Instance.SelectNextUnit();
        //Start();
        ScoreBoard.SetActive(false);
        
    }

    public IEnumerator SaveOnlyInCityWarning()
    {
        Warning.GetComponentInChildren<TMP_Text>().text = "Saving only in City!";
        Warning.SetActive(true);
        GetComponent<AudioSource>().PlayOneShot(WarningSound,sfxVolume);
        //need to use real time since game manager stops ingame time
        yield return new WaitForSecondsRealtime(2);
        Warning.SetActive(false);
    }    
    
    public IEnumerator SavingSignal()
    {
        Warning.GetComponentInChildren<TMP_Text>().text = "Saving...";
        Warning.SetActive(true);
        GetComponent<AudioSource>().PlayOneShot(SaveSound,sfxVolume);
        yield return new WaitForSecondsRealtime(2);
        Warning.SetActive(false);
    }    
    public IEnumerator LoadingSignal()
    {
        Warning.GetComponentInChildren<TMP_Text>().text = "Loading...";
        Warning.SetActive(true);
        GetComponent<AudioSource>().PlayOneShot(LoadSound,sfxVolume);
        yield return new WaitForSecondsRealtime(2);
        Warning.SetActive(false);
    }


    [System.Serializable]
    class SaveData
    {
        public int saveArenaLevel;
        public int saveMaxTroopStrenth;
        public int savePrestige;
        public int saveGold;
        public float saveFightTime;
        public int savePauseCounter;
        public int saveDamageDealt;
        public int saveArmorDestroyed;
        public int saveFightersLost;
        public float saveBuffTimer;
        public float saveDebuffTimer;
        public List<Vector3> unitInitialPosList = new List<Vector3>();
        public List<string> inventory = new List<string>();
        public List<int> inventoryItemSlotType = new List<int>();
        public List<int> inventoryArmor = new List<int>();

        public List<string> shopItems = new List<string>();
        public List<int> shopItemSlotType = new List<int>();

        public List<string> hireNames = new List<string>();
        public List<string> hireAppearances = new List<string>();
        public List<int> hireClasses = new List<int>();
        public List<int> hireLevels = new List<int>();
        public List<int> hirePotentials = new List<int>();
        public List<int> hireCosts = new List<int>();

        public List<bool> whichUnitisActive = new List<bool>();
        public List<string> unitNameList = new List<string>();
        //public List<WeaponStats> itemObjectLists = new List<WeaponStats>();
        //public List<WeaponStatsData> itemObjectLists = new List<WeaponStatsData>();
        public List<string> itemObjectLists = new List<string>();
        public List<int> itemArmorLists = new List<int>();
        public List<int> itemSlotTypeLists = new List<int>();

        public List<int> maxHP = new List<int>();
        public List<int> currentHP = new List<int>();
        public List<int> maxStamina = new List<int>();
        public List<int> currentStamina = new List<int>();
        //public List<int> dmg = new List<int>();
        public List<int> pDmg = new List<int>();
        public List<int> mDmg = new List<int>();
        public List<int> mAcc = new List<int>();
        public List<int> mEva = new List<int>();
        public List<int> rAcc = new List<int>();
        public List<int> rEva = new List<int>();
        public List<float> speed = new List<float>();
        //armor not needed as well as other secondary stats
        //levelsystem
        public List<float> currentXP = new List<float>();
        public List<int> xpToNextLevel = new List<int>();
        public List<int> level = new List<int>();
        public List<int> potential = new List<int>();
        public List<int> skillPoints = new List<int>();
        public List<int> attributePoints = new List<int>();
        public List<int> fighterClass = new List<int>();
        public List<int> promotedFighterClass = new List<int>();
        public List<int> promotionClassLists = new List<int>();
        public List<int> resistanceList = new List<int>();
        //weaponskills
        public List<int> swordSkill = new List<int>();
        public List<int> axeSkill = new List<int>();
        public List<int> maceSkill = new List<int>();
        public List<int> spearSkill = new List<int>();
        public List<int> crossbowSkill = new List<int>();
        public List<int> bowSkill = new List<int>();
        public List<int> shieldSkill = new List<int>();
        public List<int> staffSkill = new List<int>();
        //utilityskills
        //public List<int> quickdrawSkill = new List<int>();
        public List<int> oneHandedSkill = new List<int>();
        public List<int> dualwieldingSkill = new List<int>();
        public List<int> twohandedSkill = new List<int>();
        public List<int> shieldWallSkill = new List<int>();
        public List<int> tinkerSkill = new List<int>();
        public List<int> acrobatSkill = new List<int>();
        public List<int> defenderSkill = new List<int>();
        public List<int> athleteSkill = new List<int>();
        public List<int> armorhabituationSkill = new List<int>();
        public List<int> armormasterSkill = new List<int>();
        public List<int> magicFlowSkill = new List<int>();
        //specialisationskills
        public List<int> sworddancerSkill = new List<int>();
        public List<int> armorbreakerSkill = new List<int>();
        public List<int> basherSkill = new List<int>();
        public List<int> holdTheLineSkill = new List<int>();
        public List<int> hawkeyeSkill = new List<int>();
        public List<int> quickshotSkill = new List<int>();
        public List<int> bufferSkill = new List<int>();
        public List<int> debufferSkill = new List<int>();
        public List<int> wisdomSkill = new List<int>();
        //veteranskills
        //public List<int> specialistSkill = new List<int>();
        public List<int> conterSkill = new List<int>();
        public List<int> whirlWindSkill = new List<int>();
        public List<int> stormTrooperSkill = new List<int>();
        public List<int> lionheartSkill = new List<int>();
        public List<int> marathonerSkill = new List<int>();
        public List<int> unstoppableSkill = new List<int>();
        public List<int> survivalistSkill = new List<int>();
        public List<int> elementalistSkill = new List<int>();
        public List<int> elementalistSkillElement = new List<int>();
        public List<int> magicDiffusionSkill = new List<int>();
        public List<int> magicResistanceSkill = new List<int>();
        public List<string> appearance = new List<string>();
        


    }

    public void SaveGame()
    {
        if(cityMap)
        {
            StartCoroutine(SavingSignal());

        SaveData data = new SaveData();
    
        data.saveArenaLevel = arenaLevel;
        data.saveMaxTroopStrenth = maxTroopStrength;
        data.saveGold = gold;
        data.savePrestige = prestige;
        data.saveFightTime = fightTime;
        data.savePauseCounter = pauseCounter;
        data.saveDamageDealt = damageDealt;
        data.saveArmorDestroyed = armorDestroyed;
        data.saveFightersLost = fightersLost;
        data.saveBuffTimer = buffTimer;
        data.saveDebuffTimer = debuffTimer;

        //menu lists
        data.unitInitialPosList.AddRange(UnitSelections.Instance.unitInitialPositionList);
        for(int i=0; i<InventorySystem.Instance.itemStatsList.Count; i++)
        {
            if(InventorySystem.Instance.itemStatsList[i] != null)
            {
                data.inventory.Add(InventorySystem.Instance.itemStatsList[i].name);
                data.inventoryArmor.Add(InventorySystem.Instance.itemStatsList[i].armor);
                data.inventoryItemSlotType.Add(InventorySystem.Instance.itemStatsList[i].itemSlotType);
            }
            else
            {
                data.inventory.Add(null);
                data.inventoryArmor.Add(0);
                data.inventoryItemSlotType.Add(0);
            }
        }
        for(int i=0; i<ShopSystem.Instance.itemStatsList.Count; i++)
        {
            if(ShopSystem.Instance.itemStatsList[i] != null)
            {
                data.shopItems.Add(ShopSystem.Instance.itemStatsList[i].name);
                data.shopItemSlotType.Add(ShopSystem.Instance.itemStatsList[i].itemSlotType);
            }
            else
            {
                data.shopItems.Add(null);
                data.shopItemSlotType.Add(0);
            }
        }
        
        data.hireNames.AddRange(HireSystem.Instance.nameList);
        data.hireAppearances.AddRange(HireSystem.Instance.appearanceList);
        data.hireClasses.AddRange(HireSystem.Instance.classList);
        data.hireLevels.AddRange(HireSystem.Instance.levelList);
        data.hirePotentials.AddRange(HireSystem.Instance.potentialList);
        data.hireCosts.AddRange(HireSystem.Instance.costList);
        //save which gameobjects to activate after load
        for(int i=0; i<UnitSelections.Instance.inActiveUnitList.Count;i++)
        {
            if(UnitSelections.Instance.inActiveUnitList[i].activeInHierarchy)
            {
                data.whichUnitisActive.Add(true);
            }
            else
            {
                data.whichUnitisActive.Add(false);
            }
        }

        for(int i=0; i< UnitSelections.Instance.unitList.Count; i++)
        {
            Unit stats = UnitSelections.Instance.unitList[i].GetComponent<Unit>(); 
            FighterClasses.Instance.GetPromotedClassBoni(stats, stats.promotedFighterClass, -1);
            //unequip
            CharacterEntity myEntity = stats.GetComponent<CharacterEntity>();
            if(stats.skill4Active)
            {
                WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], -1);
            }
            else
            {
                WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], -1);
            }
            for(int j=4; j<9; j++)
            {
                if(stats.itemList[j]!=null)
                {
                    myEntity.Unequip(stats.itemList[j].mySlot);
                    InventorySystem.Instance.EquipItemStats(stats.itemList[j], stats, -1); 
                }
            }
            data.unitNameList.Add(stats.unitName);
            //main attributes
            data.maxHP.Add(stats.maxHP);
            data.currentHP.Add(stats.currentHP);
            data.maxStamina.Add(stats.maxStamina);
            data.currentStamina.Add(stats.currentStamina);
            //data.dmg.Add(stats.dmg);
            data.pDmg.Add(stats.pDmg);
            data.mDmg.Add(stats.mDmg);
            data.mAcc.Add(stats.mAcc);
            data.mEva.Add(stats.mEva);
            data.rAcc.Add(stats.rAcc);
            data.rEva.Add(stats.rEva);
            data.speed.Add(stats.speed);
            //secondary attributes done by equipment
/*             List<WeaponStatsData> itemList = new List<WeaponStatsData>();
            for(int j=0; j<9;j++)
            {
                if(stats.itemList!=null)
                {
                    itemList.Add(new WeaponStatsData(stats.itemList[j]));
                }
                else
                {
                    itemList.Add(null);
                }
            }
            data.itemObjectLists.AddRange(itemList); */
            for(int j=0; j<9;j++)
            {
                if(stats.itemList[j]!=null)
                {
                    data.itemObjectLists.Add(stats.itemList[j].name);
                    Debug.Log(data.itemObjectLists[j]);
                    data.itemArmorLists.Add(stats.itemList[j].armor);
                    data.itemSlotTypeLists.Add(stats.itemList[j].itemSlotType);
                }
                else
                {
                    data.itemObjectLists.Add(null);
                    data.itemArmorLists.Add(0);
                    data.itemSlotTypeLists.Add(0);
                }
            }
            //levelsystem
            data.currentXP.Add(stats.currentXP);
            data.xpToNextLevel.Add(stats.xpToNextLevel);
            data.level.Add(stats.level);
            data.potential.Add(stats.potential);
            data.skillPoints.Add(stats.skillPoints);
            data.attributePoints.Add(stats.attributePoints);
            data.fighterClass.Add(stats.fighterClass);
            data.promotedFighterClass.Add(stats.promotedFighterClass);
            data.promotionClassLists.AddRange(stats.promotionClassList);
            data.resistanceList.AddRange(stats.resistanceList);
            //skills
            //weapons
            data.swordSkill.Add(stats.swordSkill);
            data.axeSkill.Add(stats.axeSkill);
            data.maceSkill.Add(stats.maceSkill);
            data.spearSkill.Add(stats.spearSkill);
            data.crossbowSkill.Add(stats.crossbowSkill);
            data.bowSkill.Add(stats.bowSkill);
            data.shieldSkill.Add(stats.shieldSkill);
            data.staffSkill.Add(stats.staffSkill);
            //utility
            //data.quickdrawSkill.Add(stats.quickdrawSkill);
            data.oneHandedSkill.Add(stats.oneHandedSkill);
            data.dualwieldingSkill.Add(stats.dualwieldingSkill);
            data.twohandedSkill.Add(stats.twohandedSkill);
            data.shieldWallSkill.Add(stats.shieldWallSkill);
            data.tinkerSkill.Add(stats.tinkerSkill);
            data.acrobatSkill.Add(stats.acrobatSkill);
            data.defenderSkill.Add(stats.defenderSkill);
            data.athleteSkill.Add(stats.athleteSkill);
            data.armorhabituationSkill.Add(stats.armorhabituationSkill);
            data.armormasterSkill.Add(stats.armormasterSkill);
            data.magicFlowSkill.Add(stats.magicFlowSkill);
            //specialisations
            data.sworddancerSkill.Add(stats.sworddancerSkill);
            data.armorbreakerSkill.Add(stats.armorbreakerSkill);
            data.basherSkill.Add(stats.basherSkill);
            data.holdTheLineSkill.Add(stats.holdTheLineSkill);
            data.hawkeyeSkill.Add(stats.hawkeyeSkill);
            data.quickshotSkill.Add(stats.quickshotSkill);
            data.bufferSkill.Add(stats.bufferSkill);
            data.debufferSkill.Add(stats.debufferSkill);
            data.wisdomSkill.Add(stats.wisdomSkill);
            //veteranskills
            //data.specialistSkill.Add(stats.specialistSkill);
            data.conterSkill.Add(stats.conterSkill);
            data.whirlWindSkill.Add(stats.whirlWindSkill);
            data.stormTrooperSkill.Add(stats.stormTrooperSkill);
            data.lionheartSkill.Add(stats.lionheartSkill);
            data.marathonerSkill.Add(stats.marathonerSkill);
            data.unstoppableSkill.Add(stats.unstoppableSkill);
            data.survivalistSkill.Add(stats.survivalistSkill);
            data.elementalistSkill.Add(stats.elementalistSkill);
            data.elementalistSkillElement.Add(stats.elementalistSkillElement);
            data.magicDiffusionSkill.Add(stats.magicDiffusionSkill);
            data.magicResistanceSkill.Add(stats.magicResistanceSkill);
            //appearance
            data.appearance.Add(stats.appearance);


            //re-equip
            if(stats.skill4Active)
            {
                WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], 1);
            }
            else
            {
                WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], 1);
            }
            for(int j=4; j<9; j++)
            {
                if(stats.itemList[j]!=null)
                {
                    myEntity.Equip(stats.itemList[j].myItemAppearance);
                    InventorySystem.Instance.EquipItemStats(stats.itemList[j], stats, 1); 
                }
            }

            
            FighterClasses.Instance.GetPromotedClassBoni(stats, stats.promotedFighterClass, 1);

            
        }
        

        string json = JsonUtility.ToJson(data);

        json = EncryptDecrypt(json);
        if(!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }
        File.WriteAllText(Path.Combine(Application.persistentDataPath +"/savefile.json"), json);

        }
        else
        {
            StartCoroutine(SaveOnlyInCityWarning());
        }
    }


    public void LoadGame()
    {
        StartCoroutine(LoadingSignal());
        ResetFromArenaToTavern();

        string path = Path.Combine(Application.persistentDataPath + "/savefile.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            json = EncryptDecrypt(json);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            //activate the unit game objects in hierarchy that were saved as active
            UnitSelections.Instance.unitList.Clear();
            for(int i=0; i<UnitSelections.Instance.inActiveUnitList.Count;i++)
            {
                UnitSelections.Instance.inActiveUnitList[i].GetComponent<NavMeshAgent>().enabled = false;
                UnitSelections.Instance.inActiveUnitList[i].SetActive(data.whichUnitisActive[i]);
                if(data.whichUnitisActive[i])
                {
                    UnitSelections.Instance.unitList.Add(UnitSelections.Instance.inActiveUnitList[i]);
                }
            }

            for(int i=0; i<UnitSelections.Instance.unitList.Count; i++)
            {


                Unit stats = UnitSelections.Instance.unitList[i].GetComponent<Unit>(); 
                //unequip
                CharacterEntity myEntity = stats.GetComponent<CharacterEntity>();
                if(stats.skill4Active)
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[2], stats.itemList[3], -1);
                }
                else
                {
                    WeaponEquip.Instance.EquipWeapon(stats, stats.itemList[0], stats.itemList[1], -1);
                }
                for(int j=4; j<9; j++)
                {
                    if(stats.itemList[j]!=null)
                    {
                        myEntity.Unequip(stats.itemList[j].mySlot);
                        InventorySystem.Instance.EquipItemStats(stats.itemList[j], stats, -1); 
                    }
                }

                stats.unitName = data.unitNameList[i];

                stats.maxHP= data.maxHP[i];
                stats.currentHP = data.currentHP[i];
                stats.maxStamina = data.maxStamina[i];
                stats.currentStamina = data.currentStamina[i];
                //data.dmg.Add(stats.dmg);
                stats.pDmg = data.pDmg[i];
                stats.mDmg = data.mDmg[i];
                stats.mAcc = data.mAcc[i];
                stats.mEva = data.mEva[i];
                stats.rAcc = data.rAcc[i];
                stats.rEva = data.rEva[i];
                stats.speed = data.speed[i];

                stats.aggroRange = aggroRange;
                //secondary attributes done by equipment see at bottom
                //levelsystem
                stats.currentXP = data.currentXP[i];
                stats.xpToNextLevel = data.xpToNextLevel[i];
                stats.level = data.level[i];
                stats.potential = data.potential[i];
                stats.skillPoints = data.skillPoints[i];
                stats.attributePoints = data.attributePoints[i];
                stats.fighterClass = data.fighterClass[i];
                stats.promotedFighterClass = data.promotedFighterClass[i];
                for(int j=0; j< stats.promotionClassList.Count; j++)
                {
                    stats.promotionClassList[j] = data.promotionClassLists[i*stats.promotionClassList.Count+j];
                }
                for(int j=0; j<8;j++)
                {
                    stats.resistanceList[j] = data.resistanceList[i*8+j];
                }
                //skills
                //weapons
                stats.swordSkill = data.swordSkill[i];
                stats.axeSkill = data.axeSkill[i];
                stats.maceSkill = data.maceSkill[i];
                stats.spearSkill = data.spearSkill[i];
                stats.crossbowSkill = data.crossbowSkill[i];
                stats.bowSkill = data.bowSkill[i];
                stats.shieldSkill = data.shieldSkill[i];
                stats.staffSkill = data.staffSkill[i];
                //utility
                //stats.quickdrawSkill = data.quickdrawSkill[i];
                stats.oneHandedSkill = data.oneHandedSkill[i];
                stats.dualwieldingSkill = data.dualwieldingSkill[i];
                stats.twohandedSkill = data.twohandedSkill[i];
                stats.shieldWallSkill = data.shieldWallSkill[i];
                stats.tinkerSkill = data.tinkerSkill[i];
                stats.acrobatSkill = data.acrobatSkill[i];
                stats.defenderSkill = data.defenderSkill[i];
                stats.athleteSkill = data.athleteSkill[i];
                stats.armorhabituationSkill = data.armorhabituationSkill[i];
                stats.armormasterSkill = data.armormasterSkill[i];
                stats.magicFlowSkill = data.magicFlowSkill[i];
                //specialisations
                stats.sworddancerSkill = data.sworddancerSkill[i];
                stats.armorbreakerSkill = data.armorbreakerSkill[i];
                stats.basherSkill = data.basherSkill[i];
                stats.holdTheLineSkill = data.holdTheLineSkill[i];
                stats.hawkeyeSkill = data.hawkeyeSkill[i];
                stats.quickshotSkill = data.quickshotSkill[i];
                stats.bufferSkill = data.bufferSkill[i];
                stats.debufferSkill = data.debufferSkill[i];
                stats.wisdomSkill = data.wisdomSkill[i];
                //veteranskills
                stats.conterSkill = data.conterSkill[i];
                stats.whirlWindSkill = data.whirlWindSkill[i];
                stats.stormTrooperSkill = data.stormTrooperSkill[i];
                //stats.specialistSkill = data.specialistSkill[i];
                stats.lionheartSkill = data.lionheartSkill[i];
                stats.marathonerSkill = data.marathonerSkill[i];
                stats.unstoppableSkill = data.unstoppableSkill[i];
                stats.survivalistSkill = data.survivalistSkill[i];
                stats.elementalistSkill = data.elementalistSkill[i];
                stats.elementalistSkillElement = data.elementalistSkillElement[i];
                stats.magicDiffusionSkill = data.magicDiffusionSkill[i];
                stats.magicResistanceSkill = data.magicResistanceSkill[i];
                //appearance
                stats.appearance = data.appearance[i];
                stats.unitImage = Resources.Load<Sprite>("Faces/face_"+stats.appearance);
                myEntity.LoadFromResourceFile("MasterCharacterCreator/CustomBlueprints/Characters/"+stats.appearance);
                //secondary attributes done by equipment
/*                 stats.itemList = new List<WeaponStats>();
                for(int j=0; j<9; j++)
                {
                    if(data.itemObjectLists[i*9+j] != null)
                    {
                        stats.itemObjectList.Add(data.itemObjectLists[i*9+j].ToWeaponStats());
                    }
                    else
                    {
                        stats.itemObjectList.Add(null);
                    }
                }  */
                for(int j=0; j<9; j++)
                {
                    string itemName = data.itemObjectLists[i*9+j];
                    if(itemName.EndsWith("(Clone)"))
                    {
                        itemName = itemName.Substring(0, itemName.Length -7);
                    }
                    switch(data.itemSlotTypeLists[i*9+j])
                    {
                        
                        case 0:
                            stats.itemList[j]=null;
                        break;
                        case 1: 
                            stats.itemList[j] = Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName);
                            stats.itemList[j].armor = data.itemArmorLists[i*9+j];
                        break;
                        case 2: 
                            stats.itemList[j] = Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName);
                            stats.itemList[j].armor = data.itemArmorLists[i*9+j];
                        break;
                        case 3: 
                            stats.itemList[j] = Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName);
                            stats.itemList[j].armor = data.itemArmorLists[i*9+j];
                        break;
                        case 4: 
                            stats.itemList[j] = Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName);
                            stats.itemList[j].armor = data.itemArmorLists[i*9+j];
                            stats.itemList[j].itemSlotType = data.itemSlotTypeLists[i*9+j];
                        break;
                        case 11: 
                            stats.itemList[j] = Resources.Load<WeaponStats>("Equipment/Helmet/"+itemName);
                            stats.itemList[j].armor = data.itemArmorLists[i*9+j];
                        break;
                        case 12: 
                            stats.itemList[j] = Resources.Load<WeaponStats>("Equipment/Armor/"+itemName);
                            stats.itemList[j].armor = data.itemArmorLists[i*9+j];
                        break;
                        case 13: 
                            stats.itemList[j] = Resources.Load<WeaponStats>("Equipment/Gloves/"+itemName);
                            stats.itemList[j].armor = data.itemArmorLists[i*9+j];
                        break;
                        case 14: 
                            stats.itemList[j] = Resources.Load<WeaponStats>("Equipment/Pants/"+itemName);
                            stats.itemList[j].armor = data.itemArmorLists[i*9+j];
                        break;
                        case 15: 
                            stats.itemList[j] = Resources.Load<WeaponStats>("Equipment/Boots/"+itemName);
                            stats.itemList[j].armor = data.itemArmorLists[i*9+j];
                        break;
                    }
                    // Debugging: Ensure item is loaded correctly
                    if (stats.itemList[j] == null && data.itemSlotTypeLists[i * 9 + j] != 0)
                    {
                        Debug.LogError("Failed to load item: " + itemName);
                    }
                    /* Debug.Log(data.itemObjectLists[i*9+j]);
                    Debug.Log(stats.itemList[j]); */
                    
                }
/*                 for(int j=0; j<9; j++)
                {
                    stats.itemObjectList[j] = data.itemObjectLists[i*9+j];
                    Debug.Log(data.itemObjectLists[i*9+j]);
                    Debug.Log(stats.itemObjectList[j]);
                } */
                FighterClasses.Instance.GetPromotedClassBoni(stats, stats.promotedFighterClass, 1);
                stats.Setup();
                stats.LoadItems();
            }

            InventorySystem.Instance.itemStatsList.Clear();
            for(int i=0; i<data.inventory.Count; i++)
            {
                if(data.inventoryItemSlotType[i]!=0)
                {

                    string itemName = data.inventory[i];
                    if(itemName.EndsWith("(Clone)"))
                    {
                        itemName = itemName.Substring(0, itemName.Length -7);
                    }
                    
                    switch(data.inventoryItemSlotType[i])
                    {
                        case 1: 
                            InventorySystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName));
                            InventorySystem.Instance.itemStatsList[i].armor = data.inventoryArmor[i];
                        break;
                        case 2: 
                            InventorySystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName));
                            InventorySystem.Instance.itemStatsList[i].armor = data.inventoryArmor[i];
                        break;
                        case 3: 
                            InventorySystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName));
                            InventorySystem.Instance.itemStatsList[i].armor = data.inventoryArmor[i];
                        break;
                        case 4: 
                            InventorySystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName));
                            InventorySystem.Instance.itemStatsList[i].armor = data.inventoryArmor[i];
                        break;
                        case 11: 
                            InventorySystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Helmet/"+itemName));
                            InventorySystem.Instance.itemStatsList[i].armor = data.inventoryArmor[i];
                        break;
                        case 12: 
                            InventorySystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Armor/"+itemName));
                            InventorySystem.Instance.itemStatsList[i].armor = data.inventoryArmor[i];
                        break;
                        case 13: 
                            InventorySystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Gloves/"+itemName));
                            InventorySystem.Instance.itemStatsList[i].armor = data.inventoryArmor[i];
                        break;
                        case 14: 
                            InventorySystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Pants/"+itemName));
                            InventorySystem.Instance.itemStatsList[i].armor = data.inventoryArmor[i];
                        break;
                        case 15: 
                            InventorySystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Boots/"+itemName));
                            InventorySystem.Instance.itemStatsList[i].armor = data.inventoryArmor[i];
                        break;
                    }
                }
                else
                {
                    InventorySystem.Instance.itemStatsList.Add(null);
                }
                

            }


            
            ShopSystem.Instance.itemStatsList.Clear();
            for(int i=0; i<data.shopItems.Count; i++)
            {
                if(data.shopItemSlotType[i]!=0)
                {
                    string itemName = data.shopItems[i];
                    if(itemName.EndsWith("(Clone)"))
                    {
                        itemName = itemName.Substring(0, itemName.Length -7);
                    }

                    switch(data.shopItemSlotType[i])
                    {
                        case 1: 
                            ShopSystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName));
                        break;
                        case 2: 
                            ShopSystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName));
                        break;
                        case 3: 
                            ShopSystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName));
                        break;
                        case 4: 
                            ShopSystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Weapon/"+itemName));
                        break;
                        case 11: 
                            ShopSystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Helmet/"+itemName));
                        break;
                        case 12: 
                            ShopSystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Armor/"+itemName));
                        break;
                        case 13: 
                            ShopSystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Gloves/"+itemName));
                        break;
                        case 14: 
                            ShopSystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Pants/"+itemName));
                        break;
                        case 15: 
                            ShopSystem.Instance.itemStatsList.Add(Resources.Load<WeaponStats>("Equipment/Boots/"+itemName));
                        break;
                    }
                }

            }

            UnitSelections.Instance.unitInitialPositionList.Clear();
            UnitSelections.Instance.unitInitialPositionList.AddRange(data.unitInitialPosList);
            
            HireSystem.Instance.nameList.Clear();
            HireSystem.Instance.appearanceList.Clear();
            HireSystem.Instance.classList.Clear();
            HireSystem.Instance.levelList.Clear();
            HireSystem.Instance.potentialList.Clear();
            HireSystem.Instance.costList.Clear();
            HireSystem.Instance.nameList.AddRange(data.hireNames);
            HireSystem.Instance.appearanceList.AddRange(data.hireAppearances);
            HireSystem.Instance.classList.AddRange(data.hireClasses);
            HireSystem.Instance.levelList.AddRange(data.hireLevels);
            HireSystem.Instance.potentialList.AddRange(data.hirePotentials);
            HireSystem.Instance.costList.AddRange(data.hireCosts);


            arenaLevel = data.saveArenaLevel;
            maxTroopStrength = data.saveMaxTroopStrenth;
            gold = data.saveGold;
            ChangeGold(0);
            prestige = data.savePrestige;
            ChangePrestige(0);
            fightTime = data.saveFightTime;
            pauseCounter = data.savePauseCounter;
            damageDealt = data.saveDamageDealt;
            armorDestroyed = data.saveArmorDestroyed;
            fightersLost = data.saveFightersLost;
            buffTimer = data.saveBuffTimer;
            debuffTimer = data.saveDebuffTimer;

        }
        UnitSelections.Instance.selectedUnitnumber=0;
        UnitSelections.Instance.SelectNextUnit();
        ScoreBoard.SetActive(false);
    }     

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for(int i=0; i< data.Length; i++)
        {
            modifiedData += (char) (data[i] ^ verschluesselung[i % verschluesselung.Length]);
        }
        return modifiedData;
    }

    public void ResetFromArenaToTavern()
    {
        if(!cityMap)
        { 
            UnitSelections.Instance.unitList.Clear();
            foreach(GameObject unit in UnitSelections.Instance.inActiveUnitList)
            {
                if(unit.activeSelf)
                {
                    UnitSelections.Instance.unitList.Add(unit);
                }
            }
            for(int i=0; i<UnitSelections.Instance.unitList.Count; i++)
            {
                Unit unitStats = UnitSelections.Instance.unitList[i].GetComponent<Unit>();
                if(unitStats.currentHP <= 0)
                {
                    unitStats.GetComponent<Animator>().SetFloat("Speed", 0);
                    unitStats.GetComponent<Animator>().SetTrigger("Revive");
                } 
                unitStats.rb.velocity = Vector3.zero; 
                UnitSelections.Instance.unitList[i].transform.position = UnitSelections.Instance.unitInitialPositionList[i];
                unitStats.myAgent.Warp(UnitSelections.Instance.unitInitialPositionList[i]);
                unitStats.myAgent.isStopped= true;
                unitStats.myAgent.enabled= false;
                unitStats.target = null;
                unitStats.initiation = false;
                unitStats.hpBarCanvas.SetActive(false);
            } 
            cityMap = true;
            
            if(UnitSelections.Instance.gameOver)
            {
                UnitSelections.Instance.CloseGameOverScreen();
            }
            if(UnitSelections.Instance.pause)
            {
                UnitSelections.Instance.PauseGame();
            }
            
            SceneManager.LoadScene("Tavern");
            GetComponent<AudioSource>().clip = CityBGM;  
            GetComponent<AudioSource>().Play();
            
        }
    }
}
