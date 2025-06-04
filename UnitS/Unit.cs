using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.VFX;
using SoftKitty.MasterCharacterCreator;

public class Unit : Fighter
{
/*     private ParticleSystem hitEffect;
    public GameObject hit; */
    public bool playerCommand;
    public LayerMask ground;
    public LayerMask enemy;
    public LayerMask unit;
    //public GameObject selectedTarget; //player selected target
    
    //canvas children indicators
    public GameObject skillIndicator1;
    public GameObject skillIndicator2;
    public GameObject skillIndicator3;
    public GameObject groundMarker;
    public GameObject enemyMarker;

    //Camera myCam;
    private List<float> targetDistance = new List<float>(); //list of all distances to player units
    private float distanceToTarget;
    //public GameObject aggroTarget; //automatic aggro target
    public GameObject attackObject;
    public VisualEffect attackEffect;
    public bool startItemsLoaded= false;


    // Start is called before the first frame update
    void Start()
    {
        
/*         if(!startItemsLoaded)
        {
            LoadItems();
        } */
        //myCam = Camera.main;


        //add unit to unit list
        //UnitSelections.Instance.unitList.Add(this.gameObject);
        Anim = GetComponent<Animator>();



        Setup();
    }


    void Update()
    {
        if(initiation)
        {
            
            //if the unit is SELECTED add right-click commands
            if(UnitSelections.Instance.unitsSelected.Contains(this.gameObject))
            {

                if (Input.GetMouseButtonDown(1))
                {
                    //if the player gives a command stop automatic action

                    myAgent.isStopped = false;
                    rb.constraints = RigidbodyConstraints.None;
                    Anim.SetFloat("Speed", 1); 

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    
                    if(Physics.Raycast(ray, out hit, Mathf.Infinity, enemy) && !supportAttack)
                    {
                        target = hit.collider.gameObject;
                        targetStats = target.GetComponent<Fighter>();
                        myAgent.SetDestination(target.transform.position);
                        StartCoroutine(EnemySelectedIndicator());
/*                         enemyMarker.transform.position = hit.point;
                        enemyMarker.SetActive(false);
                        enemyMarker.SetActive(true); */
                    }
                    else if(Physics.Raycast(ray, out hit, Mathf.Infinity, unit) && supportAttack)
                    {
                        target = hit.collider.gameObject;
                        targetStats = target.GetComponent<Fighter>();
                        myAgent.SetDestination(target.transform.position);
                        StartCoroutine(AllySelectedIndicator());
/*                         enemyMarker.transform.position = hit.point;
                        enemyMarker.SetActive(false);
                        enemyMarker.SetActive(true); */
                    }
                    else if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                    {
                        playerCommand = true;
                        myAgent.SetDestination(hit.point);

                        target = null;
                        targetStats = null;
                        
                        groundMarker.transform.position = hit.point;
                        //groundMarker.GetComponent<Animator>().speed = 1f / Time.realtimeSinceStartup;
                        groundMarker.SetActive(false);
                        groundMarker.SetActive(true);
                    }
                }
            }


            //stop running animation when unit reached destination or the destination is set by aggro
            if(target != null || myAgent.isStopped || playerCommand && Vector3.Distance(myAgent.destination, transform.position) < 0.5f)
            {
                Anim.SetFloat("Speed", 0);
                myAgent.isStopped = true;
                rb.constraints = RigidbodyConstraints.FreezePosition;
                playerCommand = false;
            }

    
        

            //if there is no destination and no target selected search for enemy in aggro range
            if (!playerCommand)
            {
                if (target == null)
                {
                    targetDistance.Clear();

                    if(supportAttack)
                    {          
                        
                            foreach(var unit in UnitSelections.Instance.unitList)
                            {
                                distanceToTarget = unit.GetComponent<Fighter>().currentHP;
                                //priorize other units
                                if(unit == this.gameObject)
                                {
                                    distanceToTarget += range;
                                }
                                targetDistance.Add(distanceToTarget);

                                
                            }
                            int minDistanceIndex = targetDistance.IndexOf(targetDistance.Min());

                            //get real distance because hp distance can be over 999 or anything
                            if(Vector3.Distance(transform.position, UnitSelections.Instance.unitList[minDistanceIndex].transform.position) < aggroRange)
                            {
                                //aggroTarget = UnitSelections.Instance.unitList[minDistanceIndex];
                                target = UnitSelections.Instance.unitList[minDistanceIndex];
                                targetStats = target.GetComponent<Fighter>();
                                myAgent.SetDestination(target.transform.position);
                                myAgent.isStopped = false;
                                rb.constraints = RigidbodyConstraints.None;
                                Anim.SetFloat("Speed", 1); 
                                Debug.Log("supp target: " +target);
                            }
                            else
                            {
                                target = null;
                                targetStats = null;
                            }
                        

                    }
                    else
                    {
                        if (UnitSelections.Instance.enemiesList.Count != 0)
                        {
                            foreach(var enemy in UnitSelections.Instance.enemiesList)
                            {
                                distanceToTarget = Vector3.Distance(transform.position, enemy.transform.position);
                                targetDistance.Add(distanceToTarget);
                            }
                            int minDistanceIndex = targetDistance.IndexOf(targetDistance.Min());

                            if(targetDistance[minDistanceIndex] < aggroRange)
                            {
                                target = UnitSelections.Instance.enemiesList[minDistanceIndex];
                                targetStats = target.GetComponent<Fighter>();
                                enemySize= targetStats.size;
                                myAgent.SetDestination(target.transform.position);
                                myAgent.isStopped = false;
                                rb.constraints = RigidbodyConstraints.None;
                                Anim.SetFloat("Speed", 1);
                            }
                            else
                            {
                                target = null;
                                targetStats = null;
                            }
                        }
                    }

                }
            }

            if(target !=null)
            {
                distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
                if(distanceToTarget < range + enemySize -1)
                {
                    Attack();
                }
                else
                {
                    myAgent.SetDestination(target.transform.position);
                    myAgent.isStopped = false;
                    rb.constraints = RigidbodyConstraints.None;
                    Anim.SetFloat("Speed", 1);
                }
/*                 else
                {
                    myAgent.isStopped = false;
                    rb.constraints = RigidbodyConstraints.None;
                    Anim.SetFloat("Speed", 1); 
                } */
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


    IEnumerator AllySelectedIndicator()
    {
        target.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color32 (0,255,0,255);
        target.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        target.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color32 (255,255,255,73);
        if(!UnitSelections.Instance.unitList.Contains(target))
        {
            target.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    IEnumerator EnemySelectedIndicator()
    {
        target.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color32 (255,0,0,255);
        target.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        target.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color32 (255,255,255,73);
        target.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void LoadItems()
    {
            CharacterEntity myEntity = GetComponent<CharacterEntity>();
            for(int i=3; i<9; i++ )
            {
                if(itemList[i]!=null)
                {
                    //itemList[i]= Instantiate(itemObjectList[i]);
                    //DontDestroyOnLoad(itemList[i]);
                   /*  if(i>3)
                    {    */
                        myEntity.Equip(itemList[i].myItemAppearance);
                        InventorySystem.Instance.EquipItemStats(itemList[i], this, 1);
                    //}
                } 
            } 
            WeaponEquip.Instance.EquipWeapon(this, itemList[0], itemList[1], 1);
            
            StartCoroutine(StaminaRegen());

            xpToNextLevel = 10*level+ level*level;

            AttackCommand();

            startItemsLoaded = true;
    }
    

    public void GetXP(float xp)
    {
        currentXP += xp;
/*         Debug.Log(xp);
        Debug.Log(currentXP); */

        while(currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            StartCoroutine(LevelUP(true));
            xpToNextLevel = 10*level+level*level;
        }
    }

    void OnDestroy()
    {
        UnitSelections.Instance.unitList.Remove(this.gameObject);
        UnitSelections.Instance.unitsSelected.Remove(this.gameObject);
    }

}
