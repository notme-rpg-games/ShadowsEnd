using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class EnemyBoss : Fighter
{
    public GameObject hit; 
    public LayerMask enemy;
    public GameObject attackObject;
    public VisualEffect attackEffect;
    private float distanceToTarget;

    //boss special
    public int range2;
    public float attackRate2;
    public float canAttack2 = 10f; 
    public int attackCost2;
    public int aoe2;   
    public int saveRange;
    public int armorBypass2;
    public int armorPierce2;
    public int dmgOverTime2;
    public int bashAttack2;
    public int pushAttack2;
    public int slowAttack2;
    public int blindingAttack2;
    public int weakeningAttack2;
    bool powerMode;
    public AudioClip attackSound2;
    public GameObject projectilePrefab2;
    public AudioClip attackSoundSave;
    public GameObject projectilePrefabSave;
    public AudioClip attackSoundPowerMode;
    public GameObject projectilePrefabPowerMode;
    public GameObject root;
    public GameObject targetAreaMarker;
    public GameObject targetAreaEffect;
    public GameObject SpecialAttackEffect;
    bool specialAttackAnimationRunning;
  
    private List<float> targetDistance = new List<float>(); //list of all distances to player units

    private bool gameOver = false;
    private bool bossReviveSkillAvailable;


    // Start is called before the first frame update
    void Start()
    {
        UnitSelections.Instance.enemiesList.Add(this.gameObject); 
        isEnemy = true;

        
        
        for(int i=0; i<9; i++ )
        {
            if(itemObjectList[i]!=null)
            {
                itemList[i]= Instantiate(itemObjectList[i]);
                DontDestroyOnLoad(itemList[i]);
                if(i>3)
                {   
                    InventorySystem.Instance.EquipItemStats(itemList[i], this, 1);
                }
            } 
        } 

        StartCoroutine(StaminaRegen());
        

        Setup();

/*         targetAreaMarker = GameObject.Find("TargetAreaMarker");
        targetAreaEffect = GameObject.Find("AreaBurning"); */

        StartCoroutine(PowerModeInitiation());
        StartCoroutine(StaminaRegen());

        bossReviveSkillAvailable = true;
    } 

    void Update()
    {
        if(!gameOver && initiation  && !UnitSelections.Instance.pause)
        {

        //attack current target OR find closest target from unit selection list
        if (target != null)
        {
            //check if target in distance else move
            float distanceToTarget = UnityEngine.Vector3.Distance(target.transform.position, transform.position);

            if(distanceToTarget > 2*(range+enemySize)) //if enemy far away check for closer enemy
            {
                targetDistance.Clear();
                foreach(var unit in UnitSelections.Instance.unitList)
                {
                    distanceToTarget = UnityEngine.Vector3.Distance(transform.position, unit.transform.position);
                    targetDistance.Add(distanceToTarget);
                }
                int minDistanceIndex = targetDistance.IndexOf(targetDistance.Min());
                
                    target = UnitSelections.Instance.unitList[minDistanceIndex];
                    targetStats = target.GetComponent<Fighter>();
                    enemySize = targetStats.size;

                
                myAgent.isStopped = false;
                rb.constraints = RigidbodyConstraints.None;
                Anim.SetFloat("Speed", 1);
                myAgent.SetDestination(target.transform.position);
            }
            else if(distanceToTarget > range + enemySize)
            {
                myAgent.isStopped = false;
                rb.constraints = RigidbodyConstraints.None;
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
                    distanceToTarget = UnityEngine.Vector3.Distance(transform.position, unit.transform.position);
                    targetDistance.Add(distanceToTarget);
                }
                int minDistanceIndex = targetDistance.IndexOf(targetDistance.Min());
                target = UnitSelections.Instance.unitList[minDistanceIndex];
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

        hpBarCanvas.transform.eulerAngles = new UnityEngine.Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);


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

        hpBarCanvas.transform.eulerAngles = new UnityEngine.Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
        

    }

    IEnumerator PowerModeInitiation()
    {
        yield return new WaitForSeconds(15);
        if(initiation)
        {
            //canAttack = 99999;
            Anim.SetTrigger("TakeOff");
            for(int i=0; i<120; i++)
            {
                transform.position += new UnityEngine.Vector3(0,0.08f,0);
                myAgent.baseOffset += 0.08f;
                yield return new WaitForEndOfFrame();
            }
            size = 1;
            UnitSelections.Instance.ChangetargetSize(this, size);
            saveRange =range;
            range = range2;
            powerMode = true;
            //canAttack2 = 1;
    /*         attackSoundSave = attackSound;
            attackSound = attackSoundPowerMode;
            projectilePrefabSave = projectilePrefab;
            projectilePrefab = projectilePrefabPowerMode; */
            while(specialAttackAnimationRunning)
            {
                yield return new WaitForEndOfFrame();
            }
            StartCoroutine(InitiateCooldownMode());
        }
    }
    IEnumerator InitiateCooldownMode()
    {
        yield return new WaitForSeconds(30);
        if(initiation)
        {
            Anim.SetTrigger("Land");
            for(int i=0; i<120; i++)
            {
                transform.position -= new UnityEngine.Vector3(0,0.08f,0);
                myAgent.baseOffset -= 0.08f;
                yield return new WaitForEndOfFrame();
            }
            size = GetComponent<CapsuleCollider>().height;
            UnitSelections.Instance.ChangetargetSize(this, size);
            range = saveRange;
    /*         attackSound = attackSoundSave;
            projectilePrefab = projectilePrefabSave; */
            powerMode = false;
            //canAttack = 2;

            while(specialAttackAnimationRunning)
            {
                yield return new WaitForEndOfFrame();
            }
            StartCoroutine(PowerModeInitiation());
        }
    }
    public override void Attack()
    {

        myAgent.isStopped = true;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        Anim.SetFloat("Speed", 0);

        if(Time.time < canAttack2)
        {
            //hit on the ground
            if(!powerMode)
            {
                FaceTarget(targetStats.transform.position);
                StartCoroutine(AttackMelee());
            }
            //fireball in the air
            else
            {
                dmgType = 2;
                dmgOverTime = 5;
                StartCoroutine(AttackRanged());
                dmgOverTime = 0;
                dmgType = 0;
            }
        }
        else
        {
            StartCoroutine(SpecialAttack());
        }
    }

    IEnumerator SpecialAttack()
    {

        if(currentStamina>attackCost && canAttack2< Time.time)
        {
                //update cooldown
                canAttack2 = Time.time + attackRate2;
                canAttack += attackRate+2;

                specialAttackAnimationRunning = true;

                currentStamina -= attackCost2;
                staminaBar.value = currentStamina;

                targetAreaMarker.transform.position = target.transform.position;   
                targetAreaEffect.transform.position = target.transform.position;
                targetAreaMarker.SetActive(true);
                FaceTarget(targetAreaMarker.transform.position); 
                myAgent.isStopped = true;
                rb.constraints = RigidbodyConstraints.FreezePosition;

                yield return new WaitForSeconds(4);
                Anim.SetTrigger("SpecialAttack");

            
                myAudio.PlayOneShot(attackSound2, GameManager.Instance.sfxVolume);

/*                 UnityEngine.Vector3 distanceScale = SpecialAttackEffect.transform.localScale;
                distanceScale.y *= UnityEngine.Vector3.Distance(SpecialAttackEffect.transform.position,targetAreaMarker.transform.position);
                SpecialAttackEffect.transform.localScale = distanceScale; */


                
                UnityEngine.Vector3 lookPos = targetAreaMarker.transform.position - SpecialAttackEffect.transform.position;
                UnityEngine.Quaternion rotation = UnityEngine.Quaternion.LookRotation(lookPos);
                SpecialAttackEffect.transform.rotation = rotation; 
                SpecialAttackEffect.SetActive(true); 
                targetAreaMarker.SetActive(false);
                
                targetAreaEffect.SetActive(true);

                Collider[] enemies = Physics.OverlapSphere(targetAreaMarker.transform.position, aoe2); //could probably use a 3rd variable layermask, but 8 for enemy is not working
                foreach(Collider enemy in enemies)
                {
                    if(enemy.gameObject.tag == "Player")
                    {                    
                        Debug.Log(enemy);
                        Fighter stats = enemy.GetComponent<Fighter>();
                        stats.TakeDamage(this, dmg2, dmgType2, armorPierce2, armorBypass2, armorDestruction,mAcc, stats.mEva, stats.armor, bashAttack2, pushAttack2, dmgOverTime2, slowAttack2, blindingAttack2, weakeningAttack2);
                    }
                }
                yield return new WaitForSeconds(1f); 
                SpecialAttackEffect.SetActive(false); 
                yield return new WaitForSeconds(0.2f); 
                targetAreaEffect.SetActive(false);
/*                 GameObject projectileObject = Instantiate(projectilePrefab2, transform.position + new Vector3(0,1.5f,0)+ transform.forward*7, Quaternion.Euler(90, 90, 0));
                Projectile projectile = projectileObject.GetComponent<arrowProjectile>();
                projectile.enemyProjectile = isEnemy;
                if(supportAttack)
                {
                    Instantiate(SupportMagic, projectileObject.transform.position, projectileObject.transform.rotation, projectileObject.transform);
                    projectile.supportAttack = true;
                }
                projectile.attackSound = attackSound2;
                projectile.target = target;
                projectile.aoe = aoe2;
                projectile.dmg = dmg;
                projectile.dmgType = dmgType2;
                projectile.armorPierce = armorPierce2;
                projectile.armorBypass = armorBypass2;
                projectile.acc = rAcc;
                projectile.armorDestruction = armorDestruction;
                projectile.pushAttack = pushAttack2;
                projectile.bashAttack = bashAttack2;
                projectile.dmgOverTime = dmgOverTime2;
                projectile.slowAttack = slowAttack2;
                projectile.blindingAttack = blindingAttack2;
                projectile.weakeningAttack = weakeningAttack2;
                projectile.initiated = true; //starts the update routine inside the projectile
                //projectile.Move(); */
            }
            
            yield return new WaitForSeconds(1f); 
            specialAttackAnimationRunning = false;
            
            Debug.Log("sp attack end");
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



    public override IEnumerator Death()
    {
        if(GameManager.Instance.arenaLevel > 9 && bossReviveSkillAvailable)
        {
            Debug.Log("boss death");
            initiation = false;
            StopCoroutine(InitiateCooldownMode());
            StopCoroutine(PowerModeInitiation()); 
            Debug.Log("boss death 1");

            if(powerMode)
            {
                Debug.Log("death power mode");
                Anim.SetTrigger("Land");
                
                for(int i=0; i<120 && transform.position.y >0.1; i++)
                {
                    transform.position -= new UnityEngine.Vector3(0,0.08f,0);
                    myAgent.baseOffset -= 0.08f;
                    yield return new WaitForEndOfFrame();
                }
                size = GetComponent<CapsuleCollider>().height;
                UnitSelections.Instance.ChangetargetSize(this, size);
                range = saveRange;
                powerMode = false;
                canAttack = 2;
                
            }
            yield return new WaitForEndOfFrame();

            Anim.SetTrigger("Death");
            currentStamina = maxStamina/2;
            bossReviveSkillAvailable = false;
            hpBar.value = currentHP;
            staminaBar.value = currentStamina;
            dmg *=2;
            dmg2 *=2;
            yield return new WaitForSeconds(1f);
            Anim.ResetTrigger("Death");
            myAudio.PlayOneShot(deathSound, GameManager.Instance.sfxVolume);
            Anim.SetTrigger("Revive");
        }
        else
        {
            Debug.Log("boss death");
            initiation = false;
            StopCoroutine(InitiateCooldownMode());
            StopCoroutine(PowerModeInitiation()); 
            Debug.Log("boss death 1");

            if(powerMode)
            {
                Debug.Log("death power mode");
                Anim.SetTrigger("Land");
                
                for(int i=0; i<120 && transform.position.y >0.1; i++)
                {
                    transform.position -= new UnityEngine.Vector3(0,0.08f,0);
                    myAgent.baseOffset -= 0.08f;
                    yield return new WaitForEndOfFrame();
                }
                size = GetComponent<CapsuleCollider>().height;
                UnitSelections.Instance.ChangetargetSize(this, size);
                range = saveRange;
                powerMode = false;
                canAttack = 2;
                
            }
            Debug.Log("boss death 2");
            yield return new WaitForEndOfFrame();
            StartCoroutine(base.Death());
            Debug.Log("boss death 3");
        }
    }

    void OnDestroy()
    {
        UnitSelections.Instance.enemiesList.Remove(this.gameObject);
    }
}
