using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    public List<GameObject> pageButtonFramesList;    
    private int shopItemCapacity;
    public int shopRefreshCost =50;
    public List<WeaponStats> potentialItemStatsList; //items that might appear in the shop
    public List<WeaponStats> shownItemStatsList = new List<WeaponStats>(); //items in the current selection
    public List<WeaponStats> itemStatsList; //all currently available items
    public GameObject GoldIndicator;
    private int whichItemType =0;
    //private AudioSource camAudio;
    public AudioClip BoughtSound;
    public AudioClip FlipPageSound;
/*  0= All   
    1= 1H
    2= 2H
    3= Armor 
    4= consumables */
    public int shopPageLength = 30;
    public List<GameObject> shopButtons;
    public int shopPage = 0;
    public int repairCost;
    public bool tutorialTips = true;
    private static ShopSystem _instance;
    public static ShopSystem Instance { get { return _instance; } }
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

/*         shopItemCapacity = 13+7*GameManager.Instance.arenaLevel;
        RefreshShop(); */
    }  

    void Start()
    {
        //camAudio = Camera.main.GetComponent<AudioSource>();
        RefreshShop();
    }

    public void InitializeShop()
    {
        InventorySystem.Instance.InitializeInventory();
        SortList();
        LoadShopPage();
        CalculateRepairAllCost();
        
        if(tutorialTips)
        {
            GameManager.Instance.tipIndex= 1;
            GameManager.Instance.Tutorial();
            DisableInput();
        }
    }

    public void BuyItem(int buttonIndex)
    {
        int shopSlotIndex = buttonIndex + shopPage*shopPageLength;
        bool foundEmptySlot = false;
        bool moneyAvailable = false;
        //check money
        if(GameManager.Instance.gold >= shownItemStatsList[shopSlotIndex].value)
        {
            moneyAvailable = true;
            //check for empty inventory slot
            for(int i=0; i<InventorySystem.Instance.inventoryCapacity && !foundEmptySlot; i++)
            {
                if(InventorySystem.Instance.itemStatsList[i]==null)
                {
                    foundEmptySlot = true;
                    //write unititemstats in inventory slot i
                    InventorySystem.Instance.itemStatsList[i] = shownItemStatsList[shopSlotIndex];
                    GameManager.Instance.ChangeGold(shownItemStatsList[shopSlotIndex].value*-1);
                    
                    //remove item from itemstatslist before re initializing
                    for(int j=0; j<itemStatsList.Count; j++)
                    {
                        if(itemStatsList[j]==shownItemStatsList[shopSlotIndex])
                        {
                            itemStatsList[j]=null;
                            j=itemStatsList.Count;
                        }
                    }
                    GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(BoughtSound, GameManager.Instance.sfxVolume);
                }
            }
        }
        //let player know there is not enough money
        if(!moneyAvailable)
        {
            StartCoroutine(InventorySystem.Instance.NoMoney());
        }
        //let player know there is no empty slot
        else if(!foundEmptySlot)
        {
            StartCoroutine(InventorySystem.Instance.NoEmptySlot());
        }
        else
        {  
            InitializeShop();
        }
    }

    public void LoadShopPage()
    {
        int buttonIndex = 0; //only 30 buttons always go from 0 - 29


        for(int i=0+shopPage*shopPageLength; i<shopPage*shopPageLength+shopPageLength; i++)
        {
            if(i>=shownItemStatsList.Count)
            {
                shopButtons[buttonIndex].SetActive(false);
            }
            else
            {
                RawImage buttonImage = shopButtons[buttonIndex].GetComponentInChildren<RawImage>();
       
                if(shownItemStatsList[i] == null)
                {
                    shopButtons[buttonIndex].SetActive(false);
                }
                else
                {
                    shopButtons[buttonIndex].SetActive(true);
                    buttonImage.texture = shownItemStatsList[i].myItemIcon;
                }
            }
            buttonIndex++;
        }
    }

    public void SortList()
    {
        //remove all empty fields
        itemStatsList.RemoveAll(item => item==null);
        //repair all items
        foreach(WeaponStats item in itemStatsList)
        {
            item.armor=item.maxArmor;
        }
        
        if(shownItemStatsList !=null)
        {
            shownItemStatsList.Clear();
        }

        switch(whichItemType)
        {  
            case 0: 
                //1H
                foreach(var item in itemStatsList)
                {
                    if(item.itemSlotType <3)
                    {
                        shownItemStatsList.Add(item);
                    }
                }
                //2H
                foreach(var item in itemStatsList)
                {
                    if(item.itemSlotType ==3)
                    {
                        shownItemStatsList.Add(item);
                    }
                }
                //Armor
                foreach(var item in itemStatsList)
                {
                    if(item.itemSlotType >10)
                    {
                        shownItemStatsList.Add(item);
                    }
                }
            break;
            case 1:
                //1H
                foreach(var item in itemStatsList)
                {
                    if(item.itemSlotType <3)
                    {
                        shownItemStatsList.Add(item);
                    }
                }
            break;
            case 2:
                //2H
                foreach(var item in itemStatsList)
                {
                    if(item.itemSlotType ==3)
                    {
                        shownItemStatsList.Add(item);
                    }
                }
            break;
            case 3:
                //Armor
                foreach(var item in itemStatsList)
                {
                    if(item.itemSlotType >10)
                    {
                        shownItemStatsList.Add(item);
                    }
                }
            break;
        }
    }
    public void SetItemType(int buttonIndex)
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        whichItemType = buttonIndex;
        shopPage = 0;
        SortList();
        LoadShopPage();
    }
    public void SetPage(int buttonIndex)
    {
        foreach(GameObject frame in pageButtonFramesList)
        {
            frame.SetActive(false);
        }
        pageButtonFramesList[buttonIndex].SetActive(true);
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        shopPage = buttonIndex;
        LoadShopPage();
    }

    public void RepairAll()
    {
        
        if(repairCost > GameManager.Instance.gold)
        {
            StartCoroutine(InventorySystem.Instance.NoMoney());
        }
        else
        {
            GameManager.Instance.ChangeGold(repairCost);
            foreach(WeaponStats item in InventorySystem.Instance.itemStatsList)
            {
                if(item!=null)
                {
                    item.armor = item.maxArmor;
                }
            }
            foreach(GameObject unit in UnitSelections.Instance.unitList)
            {
                Unit  stats = unit.GetComponent<Unit>();
                for(int i=0; i< stats.itemList.Count; i++)
                {
                    //add armor back to char for all armor equipment
                    if(stats.itemList[i] != null)
                    {
                        if(i>3)
                        {
                            int repairValue = stats.itemList[i].maxArmor - stats.itemList[i].armor;
                            stats.armor += repairValue;
                        }
                        stats.itemList[i].armor = stats.itemList[i].maxArmor;
                    }
                }
            }
        }
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(InventorySystem.Instance.repairSound, GameManager.Instance.sfxVolume);
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(InventorySystem.Instance.SellSound,  GameManager.Instance.sfxVolume);
        InitializeShop();
    }

    int CalculateRepairAllCost()
    {
        repairCost =0;
        foreach(WeaponStats item in InventorySystem.Instance.itemStatsList)
        {
            if(item!=null && item.maxArmor!=0) 
            {
                repairCost += item.value/2*(item.maxArmor-item.armor)/item.maxArmor;
            }              
        }
        foreach(GameObject unit in UnitSelections.Instance.unitList)
        {
            Unit  stats = unit.GetComponent<Unit>();
            foreach(WeaponStats item in stats.itemList)
            {
                if(item!=null && item.maxArmor!=0) 
                {
                    repairCost += item.value/2*(item.maxArmor-item.armor)/item.maxArmor;
                }
            }
        }    
        return repairCost;    
    }

    public void RefreshShop()
    {
        shopItemCapacity = GameManager.Instance.prestige/30;
        itemStatsList.Clear();
        int j= Random.Range(0, potentialItemStatsList.Count);
        for(int i=0; i<shopItemCapacity; i++)
        {
            //reroll if item too valuable but less rerolls the higher the prestige
            int k = GameManager.Instance.prestige/100;
            while(potentialItemStatsList[j].value > GameManager.Instance.prestige/2 && k<20)
            {
                j = Random.Range(0, potentialItemStatsList.Count);
                k++;
            }
            
            itemStatsList.Add(Instantiate(potentialItemStatsList[j]));
            DontDestroyOnLoad(itemStatsList[i]); 
            j = Random.Range(0, potentialItemStatsList.Count);
        }
    }
    public void PlayerRefreshShop()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.flipPageSound, GameManager.Instance.sfxVolume);
        if(GameManager.Instance.gold > shopRefreshCost)
        {
            GameManager.Instance.ChangeGold(-shopRefreshCost);
            shopItemCapacity = GameManager.Instance.prestige/30;
            itemStatsList.Clear();
            int j= Random.Range(0, potentialItemStatsList.Count);
            for(int i=0; i<shopItemCapacity; i++)
            {
                //reroll if item too valuable but less rerolls the higher the prestige
                int k = GameManager.Instance.prestige/100;
                while(potentialItemStatsList[j].value > GameManager.Instance.prestige/2 && k<20)
                {
                    j = Random.Range(0, potentialItemStatsList.Count);
                    k++;
                }
                
                itemStatsList.Add(Instantiate(potentialItemStatsList[j]));
                DontDestroyOnLoad(itemStatsList[i]);
                j = Random.Range(0, potentialItemStatsList.Count);
            }
        }
        else
        {
            InventorySystem.Instance.NoMoney();
        }
        InitializeShop();
    }
    
    public void DisableInput()
    {
        UnitSelections.Instance.DisableUnitSelections();
        InventorySystem.Instance.DisableInput();
        for(int i=0; i<shopButtons.Count; i++)
        {
            shopButtons[i].GetComponent<Button>().enabled = false;
        }
    }
    public void EnableInput()
    {
        for(int i=0; i<shopButtons.Count; i++)
        {
            shopButtons[i].GetComponent<Button>().enabled = true;
        }
    }
}
