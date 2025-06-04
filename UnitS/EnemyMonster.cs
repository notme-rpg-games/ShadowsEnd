using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class EnemyMonster : Fighter
{
    public GameObject hit; 
    public LayerMask enemy;
    public GameObject attackObject;
    public VisualEffect attackEffect;
    private float distanceToTarget;

    
    private List<float> targetDistance = new List<float>(); //list of all distances to player units
    //public GameObject UnitSelectionsObject; //reference to the object with the unitslist
    //private List<GameObject> playerUnits = new List<GameObject>(); //reference to unitselections script to get the current player units.

    private bool gameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        UnitSelections.Instance.enemiesList.Add(this.gameObject); 

        //hitEffect = hit.GetComponent<ParticleSystem>();
/*         hpBar.maxValue = maxHealth;
        hpBar.value = currentHealth;
        hpBar.fillRect.gameObject.SetActive(true);  */
        myAgent = GetComponent<NavMeshAgent>();
        
        //playerUnits = UnitSelectionsObject.GetComponent<UnitSelections>().unitList;

    } 

    void Update()
    {
        if(!gameOver && initiation  && !UnitSelections.Instance.pause)
        {

        //attack current target OR find closest target from unit selection list
        if (target != null)
        {
            myAgent.isStopped = false;
            Anim.SetFloat("Speed", 1);
            myAgent.SetDestination(target.transform.position);
            //check if target in distance else move
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

            if(distanceToTarget > 2*range) //if enemy far away check for closer enemy
            {
                targetDistance.Clear();
                foreach(var unit in UnitSelections.Instance.unitList)
                {
                    distanceToTarget = Vector3.Distance(transform.position, unit.transform.position);
                    targetDistance.Add(distanceToTarget);
                }
                int minDistanceIndex = targetDistance.IndexOf(targetDistance.Min());
                
                if(targetDistance[minDistanceIndex] < range)
                {
                    target = UnitSelections.Instance.unitList[minDistanceIndex];
                    targetStats = target.GetComponent<Fighter>();

                }
            }
            else if(distanceToTarget > range)
            {
                myAgent.isStopped = false;
                Anim.SetFloat("Speed", 1);
                myAgent.SetDestination(target.transform.position);
            }
            else 
            {
                Attack();

                
            }
        }
        else
        {
            targetDistance.Clear();
            //playerUnits = UnitSelectionsObject.GetComponent<UnitSelections>().unitList;

            if (UnitSelections.Instance.unitList.Count != 0)
            {
                foreach(var unit in UnitSelections.Instance.unitList)
                {
                    distanceToTarget = Vector3.Distance(transform.position, unit.transform.position);
                    targetDistance.Add(distanceToTarget);
                }
                int minDistanceIndex = targetDistance.IndexOf(targetDistance.Min());
                target = UnitSelections.Instance.unitList[minDistanceIndex];
                targetStats = target.GetComponent<Fighter>();
                
                myAgent.isStopped = false;
            }
            else
            {
                gameOver = true;
            }
            
        }

        hpBarCanvas.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);


        if(debuffBar.gameObject.activeSelf)
        {
            debuffBar.value -= Time.deltaTime;
            if(debuffBar.value <= 0)
            {
                debuffBar.gameObject.SetActive(false);
            }
        }
        }

        

    }

/*     public IEnumerator Attack()
    {
        myAgent.updatePosition = false;
        yield return new WaitForSeconds(0.7f);
        FaceTarget(target.transform.position);
        //myAgent.SetDestination(transform.position); //another solution

        if(Time.time > canAttack) //attack if cd 
        {

            //update cooldown
            canAttack = Time.time + attackRate;
            
            //attackObject.transform.position = transform.position; //vfx attack
            //attackObject.transform.rotation = transform.rotation;
            //attackEffect.Play();

            Anim.SetFloat("Speed", 0);
            Anim.SetTrigger("Attack");
            
            currentStamina -= attackCost;
            staminaBar.value = currentStamina;

            
            targetStats.TakeDamage(dmg, armorPierce, armorBypass,mAcc, targetStats.mEva, targetStats.armor); 
        }
    } */

    public override void CheckForEnemiesMelee()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, aoeAttack); //could probably use a 3rd variable layermask, but 8 for enemy is not working
        foreach(Collider enemy in enemies)
        {
            if(enemy.gameObject.tag == "Player")
            {
                targetStats.TakeDamage(this, dmg, dmgType, armorPierce, armorBypass, armorDestruction,mAcc, targetStats.mEva, targetStats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);
            }
        }
    }



    public override IEnumerator Death()
    {
        UnitSelections.Instance.enemiesList.Remove(this.gameObject);
        initiation = false;
        Anim.SetTrigger("Death");
        yield return new WaitForSeconds(2);
        Destroy(this);
    }

    void OnDestroy()
    {
        UnitSelections.Instance.enemiesList.Remove(this.gameObject);
    }
}
