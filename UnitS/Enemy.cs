using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using SoftKitty.MasterCharacterCreator;

public class Enemy : Fighter
{
    public GameObject hit; 
    public LayerMask enemy;
    public GameObject attackObject;
    public VisualEffect attackEffect;
    private float distanceToTarget;

    
    private List<float> targetDistance = new List<float>(); //list of all distances to player units

    private bool gameOver = false;
    private bool startItemsLoaded=false;
    public int targetingAI;



    // Start is called before the first frame update
    void Start()
    {
        UnitSelections.Instance.enemiesList.Add(this.gameObject); 

        isEnemy = true;

        if(!startItemsLoaded)
        {
            CharacterEntity myEntity = GetComponent<CharacterEntity>();
            for(int i=0; i<9 && !startItemsLoaded; i++ )
            {
                if(itemObjectList[i]!=null)
                {
                    itemList[i]= Instantiate(itemObjectList[i]);
                    DontDestroyOnLoad(itemList[i]);
                    if(i>3)
                    {   
                        myEntity.Equip(itemList[i].myItemAppearance);
                        InventorySystem.Instance.EquipItemStats(itemList[i], this, 1);
                    }
                } 
            } 
            if( itemList[0]!=null)
            {
                WeaponEquip.Instance.EquipWeapon(this, itemList[0], itemList[1], 1);
            }
            StartCoroutine(StaminaRegen());
            startItemsLoaded = true;
        }
        
        Setup();
    } 

    void Update()
    {
        if(!gameOver && initiation  && !UnitSelections.Instance.pause)
        {

        //attack current target OR find closest target from unit selection list
        if (target != null)
        {
            //check if target in distance else move
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

            if(distanceToTarget > range+enemySize) //if enemy far away check for closer enemy
            {
                
                    target = UnitSelections.Instance.unitList[Targeting()];
                    targetStats = target.GetComponent<Fighter>();
                    enemySize = targetStats.size;

                
                myAgent.isStopped = false;
                rb.constraints = RigidbodyConstraints.None;
                Anim.SetFloat("Speed", 1);
                myAgent.SetDestination(target.transform.position);
            }
            else if(distanceToTarget < range+enemySize - 1)
            {
                Attack();
            }
            else
            {
                myAgent.isStopped = false;
                rb.constraints = RigidbodyConstraints.None;
                Anim.SetFloat("Speed", 1);
                myAgent.SetDestination(target.transform.position);
            }
        }
        else
        {

            if (UnitSelections.Instance.unitList.Count != 0)
            {
                target = UnitSelections.Instance.unitList[Targeting()];
                targetStats = target.GetComponent<Fighter>();
                enemySize = targetStats.size;
                
                myAgent.isStopped = false;
                rb.constraints = RigidbodyConstraints.None;
                Anim.SetFloat("Speed", 1);
                myAgent.SetDestination(target.transform.position);
            }
            else
            {
                gameOver = true;
            }
            
        }



        if(debuffBar.gameObject.activeSelf)
        {
            debuffBar.value -= Time.deltaTime;
            if(debuffBar.value <= 0)
            {
                debuffBar.gameObject.SetActive(false);
            }
        }
            
        if(myAgent.isStopped)
        {
            rb.velocity = UnityEngine.Vector3.zero;
            Anim.SetFloat("Speed", 0); 
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        }
        hpBarCanvas.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

        

    }
    

    public override void CheckForEnemiesMelee()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, aoeAttack); //could probably use a 3rd variable layermask, but 8 for enemy is not working
        foreach(Collider enemy in enemies)
        {
            if(enemy.gameObject.tag == "Player")
            {
                Fighter stats = enemy.GetComponent<Fighter>();
                stats.TakeDamage(this, dmg, dmgType, armorPierce, armorBypass, armorDestruction,mAcc, targetStats.mEva, targetStats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);
            }
        }
    }

    private int Targeting()
    {
        targetDistance.Clear();
        switch(targetingAI)
        {
            //closest target
            case 0:
                foreach(var unit in UnitSelections.Instance.unitList)
                {
                    distanceToTarget = Vector3.Distance(transform.position, unit.transform.position);
                    targetDistance.Add(distanceToTarget);
                }
            break;
            //lowest hp target
            case 1:
                foreach(var unit in UnitSelections.Instance.unitList)
                {
                    distanceToTarget = unit.GetComponent<Fighter>().currentHP;
/*                     //add actual distance   
                    if(Vector3.Distance(transform.position, unit.transform.position) > range)
                    {
                        distanceToTarget += 9999;
                    }  */
                    targetDistance.Add(distanceToTarget);
                }
            break;
        }
        
        int minDistanceIndex = targetDistance.IndexOf(targetDistance.Min());
        return minDistanceIndex;
    }



    void OnDestroy()
    {
        UnitSelections.Instance.enemiesList.Remove(this.gameObject);
    }
}
