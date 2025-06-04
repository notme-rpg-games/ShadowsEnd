using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;

public abstract class Fighter : MonoBehaviour
{  
    public AudioClip GruntSound;
    //0= not available, 1= available, 2= in use, details in fighterclasses script
    public List<WeaponStats> itemObjectList;
    public List<WeaponStats> itemList;

    //stats
    public int maxHP;
    public int currentHP;
    public int maxStamina;
    public int currentStamina;
    public int dmg;
    
    public int pDmg;
    public int mDmg;
    public int mAcc;
    public int mEva;
    public int rAcc;
    public int rEva;
    public float speed;

    public int armor;
    public int armorPierce; //armor destruct and pierce
    public int armorBypass; //completely ignore armor
    public int armorDestruction; //don't do damage through but destroy armor

    //objects
    public Slider hpBar;
    public Slider staminaBar;
    public Slider debuffBar;
    public string unitName = "Warrior";
    public Sprite unitImage;
    public GameObject hpBarCanvas;
    public GameObject blockObject;
    public GameObject missObject;
    //can be made privat if setup is working
    public GameObject hitObject;
    public ParticleSystem hitEffect;
    public GameObject burningObject;
    
    public VisualEffect burningEffect;
    //public AudioSource camAudio;
    public AudioSource myAudio;
    public AudioClip deathSound;
    public AudioClip attackSound;
    private AudioClip evadeSound;
    private AudioClip blockedSound;
    private AudioClip levelUpSound;
    private AudioClip shieldBreakingSound;
    private AudioClip weaponBreakingSound;
    public Animator Anim;
    public NavMeshAgent myAgent;
    public Fighter targetStats;

    public GameObject projectilePrefab;
    //private Rigidbody fighterRb;


    
    public GameObject target;


    public int range;
    public int aggroRange;
    public float attackRate;
    public float canAttack = 1f;    
    public int attackCost;
    public int baseAttackCost;
    public int currentWeaponType;
    public int dmgType;
    public int dmg2;
    public int dmgType2;

    public bool initiation;

    //skills
    public float currentXP;
    public int xpToNextLevel;    
    public int level;
    public int potential;
    public int skillPoints;
    public int attributePoints;
    public int fighterClass;
    public int promotedFighterClass;
    public List<int> resistanceList;
    //tier1 - weapons
    public int swordSkill;  //equipweapon
    public int axeSkill;    //equipweapon
    public int maceSkill;   //equipweapon
    public int spearSkill;  //equipweapon
    public int crossbowSkill;   //equipweapon
    public int bowSkill;    //equipweapon
    public int shieldSkill; //equipweapon
    public int staffSkill;  //equipweapon

    //tier2 - weapon independent
    public int quickdrawSkill;  //reduces weapon swap time - skill4
    public int oneHandedSkill;
    public int dualwieldingSkill;   //equipweapon
    public int twohandedSkill;  //equipweapon
    public int shieldWallSkill;
    public int tinkerSkill;
    public int acrobatSkill; //evasion cost - takedamage
    public int defenderSkill;   //less block cost   -takedamage
    public int athleteSkill; //speed - in setup
    public int armorhabituationSkill; //less stamina for armor - inventorysystem
    public int armormasterSkill;    //armor amplification - inventorysystem
    public int magicFlowSkill;      //magic damage increase - weapon equip

    //tier3 - weapon specialisations
    public int sworddancerSkill;    //more attack speed with swords - //equipweapon
    public int armorbreakerSkill; //more armordmg with axes - //equipweapon
    public int basherSkill; //bash chance on base attack with maces - //equipweapon
    public int holdTheLineSkill; //higher accuracy with spearwall - // equipweapon
    public int hawkeyeSkill;    //more range for bows and crossbows - //equipweapon
    public int quickshotSkill;  //more attack speed for bows - //equipweapon
    public int bufferSkill;     //increased magic buff effects - in attack method
    public int debufferSkill;   //increased magic debuff effects - in attack method
    public int wisdomSkill;     //less stamina cost for magic - //equipweapon

    //tier4 - proffessionell skills

    //tier5 - weapon independent skills
    public int conterSkill;
    public int whirlWindSkill;
    public int stormTrooperSkill;
    public int specialistSkill; //special attacks cost less - in toggle skill methods
    public int lionheartSkill; // more hp - when skilling & in stats skill
    public int marathonerSkill; //more sta - when skilling & in stats skill
    public int unstoppableSkill; //reduced chance to get stunned or slowed
    public int survivalistSkill; //reduced chance to get affected by status effects
    public int elementalistSkill; //increased damage with specified element
    public int elementalistSkillElement; // determines the chosen element
    public int magicDiffusionSkill;
    public int magicResistanceSkill;
    public bool skill1Active;
    public bool skill2Active;
    public bool skill3Active;
    public bool skill4Active;
    public bool skill3available;
    public bool commandActive5;
    public bool commandActive6;
    public bool commandActive7;
    public bool weaponSwap = false; //weapon toggling right now
    public float moveResistance;
    public float mindResistance;
    public float bashAttack;
    public float pushAttack;
    public float dmgOverTime;
    public float slowAttack;
    public float blindingAttack;
    public float weakeningAttack;
    public float attackRateDebuffValue;
    public bool supportAttack;
    private GameObject SupportMagic;     //the green sphere around the magic projectile to indicate support magic
    public int aoeAttack;
    public int hunkerdownBonus;
    public int reachAdvantageBonus;
    public bool usingShield;
    public List<int> promotionClassList;
    //private float staminaRegenMultiplier;
    public string appearance;
    public float size;
    public float enemySize;
    public bool isEnemy;
    public Rigidbody rb;
    public bool isCountering;
    public GameObject WarningCanvas;
    private bool reviveSkillAvailable;



/*     public ParticleSystem hitEffect;
    public GameObject hit; */
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Setup()
    {
        rb = GetComponent<Rigidbody>();    

        if(GameManager.Instance.cityMap)
        {
            initiation = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            hpBarCanvas.gameObject.SetActive(false);
        }
        else
        {
            hpBarCanvas.gameObject.SetActive(true);

            hpBar.maxValue = maxHP;
            hpBar.value = currentHP;
            hpBar.fillRect.gameObject.SetActive(true);
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
            staminaBar.fillRect.gameObject.SetActive(true);
            
            myAgent = GetComponent<NavMeshAgent>();
            GetComponent<NavMeshAgent>().enabled = true;

            myAgent.speed = speed;
            myAgent.destination = transform.position;
            myAgent.isStopped = true;

            Anim = GetComponent<Animator>();

            SupportMagic = Resources.Load<GameObject>("Projectiles/Support");

            evadeSound = Resources.Load<AudioClip>("SE/MedievalAction/Sword Swish 1");
            blockedSound = Resources.Load<AudioClip>("SE/shield-block");
            levelUpSound = Resources.Load<AudioClip>("SE/Magic1");
            shieldBreakingSound = Resources.Load<AudioClip>("SE/Damage3");
            weaponBreakingSound = Resources.Load<AudioClip>("SE/BowHammerSE/HammerFlesh2");

            hitObject = GameObject.Find("HitEffect");
            hitEffect = hitObject.GetComponent<ParticleSystem>();

            //camAudio = GameManager.Instance.GetComponent<AudioSource>();
            myAudio = GetComponent<AudioSource>();

            //appropriate distance for large enemies
            size = GetComponent<CapsuleCollider>().height/2;

            if(lionheartSkill >0)
            {
                reviveSkillAvailable = true;
            }
            else
            {
                reviveSkillAvailable = false;
            }


            initiation = true;

        }
    }


    
    //specify this values to make clarify they are method values not the target or attacker stats
    public virtual void TakeDamage(Fighter attackerStats, int aDmg, int aDmgType, int aArmorPierce, int aArmorBypass, int aArmorDestruction, int aAcc, int targetEva, int targetArmor, float modBashAttack, float modpushAttack, float modDmgoverTime, float modSlowEffect, float modBlinded, float modWeakened) 
    {
        //if conter ability and one handed equipped and target in range roll if counter attack
        if(isCountering)
        {
            if(Vector3.Distance(transform.position, attackerStats.transform.position)<range && SkillSystem.Instance.conterEffect - Random.Range(0,100) > 0)
            {
                StartCoroutine(CounterAttack(attackerStats));
/*                 Anim.SetTrigger("Attack");
                attackerStats.TakeDamage(attackerStats, dmg, dmgType, armorPierce, armorBypass, armorDestruction, mAcc, attackerStats.mEva, attackerStats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack); */
            }
        }

        //Debug.Log("attacker"+attackerStats+", dmg:"+aDmg +" armP " +aArmorPierce+"  acc " +aAcc+" eva " +targetEva+" armor " + targetArmor);

        //switch enemy target to current attacker if current target out of range
        if(isEnemy && target != null && Vector3.Distance(transform.position, target.transform.position) > range+enemySize)
        {
            targetStats = attackerStats;
            target = attackerStats.gameObject;
        }

        int hitChance = 100 + aAcc - targetEva - Random.Range(0, 100); 
/*         if(hitChance <0)
        {
            StartCoroutine(Missed());
        }
        else
        { */
            int armorDamage = Mathf.RoundToInt(aDmg *(aArmorPierce+aArmorDestruction)/ 100f);

            int blockCost = Mathf.RoundToInt(aDmg);
            //reduce target stamina since it has to block
            if(!usingShield && acrobatSkill==1) //no shield + acrobat
            {   
                blockCost /=2;
            }
            else if(usingShield && defenderSkill==1) //shield + defender
            {
                blockCost /=2;
            }

            if(currentStamina<= blockCost)
            {
                hitChance =1;
            }
            else
            {
                currentStamina -= blockCost;
            }
            Debug.Log(this +" "+blockCost);
            staminaBar.value = currentStamina;

            //hitChance -= (targetEva +50- Random.Range(0,100));

        if(hitChance > 0)
        {
            

            hitObject.transform.position = transform.position;
            hitEffect.Play();
            float armorbypassDmg= aDmg*(aArmorPierce+aArmorBypass)/100f *((100-resistanceList[aDmgType])/100f); //reduce health by armor piercing damage AND multiplay with resistance of current dmgtype
/*             //for hit zones
            currentHP -= Mathf.RoundToInt(Mathf.Max((aDmg-bodyPartArmor)*((100-(aArmorPierce+aArmorBypass))/100), 0)) *((100-resistanceList[aDmgType])/100); //reduce health by the non armor piercing dmg - armor if > 0 AND multiplay with resistance of current dmgtype */
            //float armorpiercingDmg= (aDmg-armor)*((100-(aArmorPierce+aArmorBypass))/100f)*((100-resistanceList[aDmgType])/100); //reduce health by the non armor piercing dmg - armor if > 0 AND multiplay with resistance of current dmgtype
            float armorpiercingDmg= Mathf.Max((aDmg-armor)*((100-(aArmorPierce+aArmorBypass))/100f), 0)*((100-resistanceList[aDmgType])/100f); //reduce health by the non armor piercing dmg - armor if > 0 AND multiplay with resistance of current dmgtype
            //Debug.Log("bypassdmg:"+armorbypassDmg +"piercedmg" +armorpiercingDmg);
            //Debug.Log("dmg:"+aDmg +"armor" +armor +"bypass:" +aArmorBypass);
            currentHP -= Mathf.RoundToInt(armorpiercingDmg + armorbypassDmg);
            hpBar.value = currentHP;
            
            if (currentHP <= 0)
            {
                //Destroy(this.gameObject);
                StartCoroutine(Death());  //if not destroyed enemies still try to access target
            }

            ArmorDestructionCalculation(armorDamage);
            
            
            if(isEnemy)
            {
                GameManager.Instance.damageDealt += Mathf.RoundToInt(armorpiercingDmg + armorbypassDmg);
                GameManager.Instance.armorDestroyed += armorDamage;
            }

            //debuff effects
            if(modDmgoverTime >0)
            {
                StartCoroutine(DebuffDamageOverTime(aDmg,aDmgType, modDmgoverTime));
            }
            if(modSlowEffect >0)
            {
                StartCoroutine(DebuffSlowed(aDmg, modSlowEffect));
            }
            if(modBashAttack > 0)
            {
                int bashChance = Random.Range(0,101);
                if(modBashAttack >bashChance)
                {
                    StartCoroutine(DebuffBashed(aDmg));
                }
            }
            if(modpushAttack >0)
            {               
                StartCoroutine(DebuffPushed(modpushAttack)); //transform.position -= Vector3.back*modpushAttack;
            }
            if(modBlinded >0)
            {
                StartCoroutine(DebuffBlinded(aDmg,modBlinded));
            }
            if(modWeakened >0)
            {
                StartCoroutine(DebuffWeakened(aDmg,modWeakened));
            }
        }
        else
        {
            StartCoroutine(Blocked(armorDamage));
            attackerStats.InvokeCoroutine(nameof(Missed));
        }
        //}
    }

    public void ArmorDestructionCalculation(int armorDestruction)
    {
        //start by a random armor part
        int i = Random.Range(4,9);
        //go through all armor equipment slots until every piece's armor is reduced to 0 or the armordestruction value is 0 by reducing the armor parts by its value
        for(int j=0; j<5 && armorDestruction>0 && armor>0; j++)
        {
            if(itemList[i]!=null)
            {
                if(armorDestruction>itemList[i].armor)
                {
                    armorDestruction -= itemList[i].armor;
                    armor -= itemList[i].armor; 
                    itemList[i].armor = 0;
                }
                else
                {
                    itemList[i].armor -= armorDestruction;
                    armor -= armorDestruction;
                    armorDestruction = 0;
                }
            }
            //jump to the next armor piece in the list
            if(i>7)
            {
                i=4;
            }
            else
            {
                i++;
            }
        }
    }


    public virtual void GetBuffed(int aDmg, int aDmgType, int aArmorPierce, int aArmorBypass, int aArmorDestruction, int aAcc, int targetEva, int targetArmor, float modBashAttack, float modpushAttack, float modDmgoverTime, float modSlowEffect, float modBlinded, float modWeakened) 
    {
        if(modDmgoverTime >0)
        {
            StartCoroutine(HealOverTime(aDmg, modDmgoverTime));
        }
        if(modSlowEffect >0)
        {
            StartCoroutine(AttackSpeedBuff(aDmg, modSlowEffect));
        }
        if(modBashAttack > 0)
        {
            StartCoroutine(ResistanceBuff(aDmg, modBashAttack)); //increase duration by 0.2 times for each skillpoint 
        }
        if(modpushAttack >0)
        {               
            StartCoroutine(Haste(aDmg, modpushAttack)); 
        }
        if(modBlinded >0)
        {
            StartCoroutine(Illuminated(aDmg,modBlinded));
        }
        if(modWeakened >0)
        {
            StartCoroutine(Strengthened(aDmg,modWeakened));
        }
    }

    //Buffs
    public IEnumerator HealOverTime(int effect, float duration)
    {
        if(!isEnemy)
        {
            GameManager.Instance.buffTimer += duration;
        }
        //change selectedmarker color
        //transform.GetChild(0).GetComponent<Renderer>().material.color = new Color32 (0,0,255,255);
/*         if(debuffBar.value < duration)
        {
        debuffBar.maxValue = duration;
        debuffBar.value = duration;
        debuffBar.gameObject.SetActive(true);
        } */

        currentHP += effect/2;

        effect = effect/10;
        
        int i=0;
        
        while(i<duration && initiation)
        {
            i++;
            currentHP += effect;
            if(currentHP > maxHP)
            {
                currentHP = maxHP;
            }
            hpBar.value = currentHP;
            yield return new WaitForSeconds(1);
        }
    } 

    public IEnumerator ResistanceBuff(int effect, float duration)
    {
        if(!isEnemy)
        {
            GameManager.Instance.buffTimer += duration;
        }
/*         if(debuffBar.value < duration)
        {
        debuffBar.maxValue = duration;
        debuffBar.value = duration;
        debuffBar.gameObject.SetActive(true);
        } */

        //effect = effect/2;
        int armorEffect = effect/2;
        armor += armorEffect;
/*         moveResistance += effect;
        mindResistance += effect; */

        for(int i=0; i<duration*10 && initiation; i++)
        {            
            yield return new WaitForSeconds(1f);
        }
        armor -= armorEffect;
/*         moveResistance -= effect;
        mindResistance -= effect; */

    } 
    public IEnumerator Haste(int effect, float duration)
    {
        if(!isEnemy)
        {
            GameManager.Instance.buffTimer += duration;
        }
/*         if(debuffBar.value < duration)
        {
        debuffBar.maxValue = duration;
        debuffBar.value = duration;
        debuffBar.gameObject.SetActive(true);
        } */

        int StaminaEffect = Mathf.FloorToInt(effect/10);
        float SpeedEffect = myAgent.speed * Mathf.Pow(1.01f, effect);
        myAgent.speed += SpeedEffect;

        for(int i=0; i<duration*10 && initiation; i++)
        {            
            yield return new WaitForSeconds(1f);
        }

        myAgent.speed -= SpeedEffect;
    }     
    
    public IEnumerator AttackSpeedBuff(float effect, float duration)
    {
        if(!isEnemy)
        {
            GameManager.Instance.buffTimer += duration;
        }
/*         if(debuffBar.value < duration)
        {
        debuffBar.maxValue = duration;
        debuffBar.value = duration;
        debuffBar.gameObject.SetActive(true);
        } */

        effect = attackRate*Mathf.Pow(0.99f, effect);

        attackRate -= effect;
        for(int i=0; i<duration*10 && initiation; i++)
        {            
            yield return new WaitForSeconds(1f);
        }
        attackRate += effect;
    }     
    
    public IEnumerator Illuminated(int effect, float duration)
    {
        if(!isEnemy)
        {
            GameManager.Instance.buffTimer += duration;
        }
/*         if(debuffBar.value < duration)
        {
        debuffBar.maxValue = duration;
        debuffBar.value = duration;
        debuffBar.gameObject.SetActive(true);
        } */


        mAcc += effect;
        mEva += effect;
        rAcc += effect;
        rEva += effect;

        for(int i=0; i<duration*10 && initiation; i++)
        {            
            yield return new WaitForSeconds(1f);
        }

        mAcc -= effect;
        mEva -= effect;
        rAcc -= effect;
        rEva -= effect;
    } 

    public IEnumerator Strengthened(int effect, float duration)
    {
        if(!isEnemy)
        {
            GameManager.Instance.buffTimer += duration;
        }
/*         if(debuffBar.value < duration)
        {
            debuffBar.maxValue = duration;
            debuffBar.value = duration;
            debuffBar.gameObject.SetActive(true);
        } */


        maxHP += effect;
        currentHP += effect;
        maxStamina += effect;
        currentStamina += effect;

        
        for(int i=0; i<duration*10 && initiation; i++)
        {            
            yield return new WaitForSeconds(1f);
        }

        maxHP -= effect;
        currentHP -= effect;
        maxStamina -= effect;
        currentStamina -= effect;
        if(currentHP<0)
        {
            //Destroy(this.gameObject);
            StartCoroutine(Death());
        }
        if(currentStamina<0)
        {
            currentStamina = 0;
        }
    } 


//debuffs
    public IEnumerator DebuffDamageOverTime(int dmg,int dmgType, float duration)
    {
        duration -= duration*mindResistance/100;
        if(duration>0)
        {
            if(isEnemy)
            {
                GameManager.Instance.debuffTimer += duration;
            }
/*             if(debuffBar.value < duration)
            {
            debuffBar.maxValue = duration;
            debuffBar.value = duration;
            debuffBar.gameObject.SetActive(true);
            } */

            
            GameObject BurnEffect = Instantiate(Resources.Load<GameObject>("StatusEffects/BurningEffect"), transform);
                //burningObject.SetActive(true);
            

            dmg = Mathf.FloorToInt(dmg/10);
            int i=0;
            
            while(i<duration && initiation)
            {
                i++;
                currentHP -= dmg;
                if(currentHP<=0)
                {
                    //Destroy(this.gameObject);
                    StartCoroutine(Death());
                }
                hpBar.value = currentHP;
                yield return new WaitForSeconds(1f);
            }
            //burningObject.SetActive(false);            
            
            Destroy(BurnEffect);
            //burningObject.SetActive(true);
            

        }
    } 

    public IEnumerator DebuffPushed(float pushDistance)
    {
        float duration = 10-10*moveResistance/100;

        if(duration>0)
        {
            if(isEnemy)
            {
                GameManager.Instance.debuffTimer += duration;
            }

            if(myAgent.isStopped)
            {
                rb.constraints = RigidbodyConstraints.None;
            }
            int i = 0;
            while(i<duration)
            {
                i++;
                transform.position -= transform.forward*pushDistance/10;
                yield return new WaitForSeconds(0.05f);
            }
        }
    } 

    public IEnumerator DebuffBlinded(int dmg, float duration)
    {
        duration -= duration*mindResistance/100;
        if(duration>0)
        {
            if(isEnemy)
            {
                GameManager.Instance.debuffTimer += duration;
            }
/*             if(debuffBar.value < duration)
            {
            debuffBar.maxValue = duration;
            debuffBar.value = duration;
            debuffBar.gameObject.SetActive(true);
            } */
            GameObject BlindEffect = Instantiate(Resources.Load<GameObject>("StatusEffects/BlindEffect"), transform);

            dmg = Mathf.FloorToInt(dmg/2);
            mAcc -=dmg;
            rAcc -=dmg;
            mEva -=dmg;
            rEva -=dmg;
                
            for(int i=0; i<duration && initiation; i++)
            {            
                yield return new WaitForSeconds(1f);
            }
                
            mAcc +=dmg;
            rAcc +=dmg;
            mEva +=dmg;
            rEva +=dmg;

            Destroy(BlindEffect);
        }
    }     
    
    public IEnumerator DebuffWeakened(int dmg, float duration)
    {
        duration -= duration*mindResistance/100;
        if(duration>0)
        {
            if(isEnemy)
            {
                GameManager.Instance.debuffTimer += duration;
            }
/*             if(debuffBar.value < duration)
            {
            debuffBar.maxValue = duration;
            debuffBar.value = duration;
            debuffBar.gameObject.SetActive(true);
            } */
            GameObject WeakenEffect = Instantiate(Resources.Load<GameObject>("StatusEffects/WeakenEffect"), transform);

            if(maxStamina >dmg)
            {
                maxStamina -= dmg;
            }
            if(maxHP > dmg)
            {
                maxHP -= dmg;
            }
                
            for(int i=0; i<duration && initiation; i++)
            {            
                yield return new WaitForSeconds(1f);
            }
                
            maxStamina += dmg;
            maxHP += dmg;
            Destroy(WeakenEffect);
        }
    }     

    public IEnumerator DebuffSlowed(int dmg, float duration)
    {
        //Debug.Log("slowed"+attackRate +" " +myAgent.speed);
        duration -= duration*moveResistance/100;
        if(duration>0)
        {
            if(isEnemy)
            {
                GameManager.Instance.debuffTimer += duration;
            }
/*             if(debuffBar.value < duration)
            {
            debuffBar.maxValue = duration;
            debuffBar.value = duration;
            debuffBar.gameObject.SetActive(true);
            } */

            GameObject SlowEffect = Instantiate(Resources.Load<GameObject>("StatusEffects/SlowEffect"), transform);

            float attackSlowEffect = attackRate*Mathf.Pow(1.01f, duration);
            float speedEffect = myAgent.speed / 3 * Mathf.Pow(1.01f, duration);
            
            //Debug.Log(attackSlowEffect +"" +speedEffect);
            attackRate += attackSlowEffect;                   //apply slow to attack rate
            myAgent.speed -= speedEffect; //min value 0
                
            for(int i=0; i<duration && initiation; i++)
            {            
                yield return new WaitForSeconds(1f);
            }
                
            attackRate -= attackSlowEffect;                   //remove attack slow
            myAgent.speed += speedEffect; //if value was lower than 3 before, it gets speed up here
            
            Destroy(SlowEffect);
        }
        //Debug.Log("stop slowed"+attackRate +" " +myAgent.speed);
    }   
    
    public IEnumerator DebuffBashed(int aDmg)
    {
        float duration = aDmg/20 - aDmg/20*moveResistance/100; 
        if(duration>0)
        {
            if(isEnemy)
            {
                GameManager.Instance.debuffTimer += duration;
            }
/*             if(debuffBar.value <duration)
            {
            debuffBar.maxValue = duration;
            debuffBar.value = duration;
            debuffBar.gameObject.SetActive(true);
            } */
            GameObject BashEffect = Instantiate(Resources.Load<GameObject>("StatusEffects/BashEffect"), transform);

            aDmg = Mathf.FloorToInt(aDmg/2);
            mEva -= aDmg;
            rEva -= aDmg;
            canAttack += duration;
            float saveCurrentSpeed= myAgent.speed;
            myAgent.speed -= saveCurrentSpeed;
            for(int i=0; i<duration && initiation; i++)
            {            
                yield return new WaitForSeconds(1f);
            }
            mEva += aDmg;
            rEva += aDmg;
            myAgent.speed += saveCurrentSpeed;

            Destroy(BashEffect);
        }
    } 

    public void ChangeAura(int onOff)
    {
        if(onOff == 1)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }

    }

    public void InvokeCoroutine(string methodName)
    {
        StartCoroutine(methodName);
    }
    public IEnumerator Missed()
    {
        missObject.SetActive(true);
        myAudio.PlayOneShot(evadeSound, GameManager.Instance.sfxVolume);
        yield return new WaitForSeconds(0.5f);
        missObject.SetActive(false);
    }    

    IEnumerator Blocked(int armorDamage)
    {
        blockObject.SetActive(true);
        myAudio.PlayOneShot(blockedSound, GameManager.Instance.sfxVolume);
        //reduce wepon durability
        if(usingShield)
        {
            //reduce damage to shields
            if(shieldWallSkill>0)
            {
                armorDamage -= Mathf.RoundToInt(SkillSystem.Instance.shieldWallEffect*shieldWallSkill/100f *armorDamage);
            }

            if(skill4Active)
            {
                if(itemList[3]!=null)
                {
                    itemList[3].armor = Mathf.Max(itemList[3].armor-armorDamage,0);
                    //destroy weapon
                    if(itemList[3].armor ==0)
                    {
                        WeaponEquip.Instance.EquipWeapon(this, itemList[2],itemList[3],-1);
                        itemList[3]=null;
                        WeaponEquip.Instance.EquipWeapon(this, itemList[2],itemList[3],1);
                        myAudio.PlayOneShot(shieldBreakingSound, GameManager.Instance.sfxVolume);
                    }
                }
            }
            else
            {
                if(itemList[1]!=null)
                {
                    itemList[1].armor = Mathf.Max(itemList[1].armor-armorDamage,0);
                    //destroy weapon
                    if(itemList[1].armor ==0)
                    {
                        WeaponEquip.Instance.EquipWeapon(this, itemList[0],itemList[1],-1);
                        itemList[1]=null;
                        WeaponEquip.Instance.EquipWeapon(this, itemList[0],itemList[1],1);
                        myAudio.PlayOneShot(shieldBreakingSound, GameManager.Instance.sfxVolume);
                    }
                }
            }
        }
        else
        {
            if(skill4Active)
            {
                if(itemList[2]!=null)
                {
                    itemList[2].armor = Mathf.Max(itemList[2].armor-armorDamage,0);
                    //destroy weapon
                    if(itemList[2].armor ==0)
                    {
                        WeaponEquip.Instance.EquipWeapon(this, itemList[2],itemList[3],-1);
                        itemList[2]=null;
                        WeaponEquip.Instance.EquipWeapon(this, itemList[2],itemList[3],1);
                        myAudio.PlayOneShot(weaponBreakingSound, GameManager.Instance.sfxVolume);
                    }
                }
            }
            else
            {
                if(itemList[0]!=null)
                {
                    itemList[0].armor = Mathf.Max(itemList[0].armor-armorDamage,0);
                    //destroy weapon
                    if(itemList[0].armor ==0)
                    {
                        WeaponEquip.Instance.EquipWeapon(this, itemList[0],itemList[1],-1);
                        itemList[0]=null;
                        WeaponEquip.Instance.EquipWeapon(this, itemList[0],itemList[1],1);
                        myAudio.PlayOneShot(weaponBreakingSound, GameManager.Instance.sfxVolume);
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        blockObject.SetActive(false);
    }

    public IEnumerator StaminaRegen()
    {
        yield return new WaitForSeconds(1);        
        currentStamina += Mathf.FloorToInt(maxStamina/20);
        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        staminaBar.value = currentStamina;
        StartCoroutine(StaminaRegen());
    }

    public void AttackCommand()
    {
        aggroRange = GameManager.Instance.aggroRange;
        commandActive5 = true;
        commandActive6 = false;
        commandActive7 = false;
        if(!GameManager.Instance.cityMap)
        {
            myAgent.isStopped = false;
        }
    }
    public void StopCommand()
    {
        aggroRange = 0;
        targetStats = null;
        target = null;
        commandActive5 = false;
        commandActive6 = true;
        commandActive7 = false;
        if(!GameManager.Instance.cityMap)
        {
            myAgent.destination = transform.position;
            myAgent.isStopped = true;
        }
    }
    public void HoldCommand()
    {
        aggroRange = range;
        targetStats = null;
        target = null;
        commandActive5 = false;
        commandActive6 = false;
        commandActive7 = true;
        if(!GameManager.Instance.cityMap)
        {
            myAgent.isStopped = true;
        }
    }

    public void ToggleSkill1()
    {
        if(skill1Active)
        {
            skill1Active = false;
            attackCost = Mathf.RoundToInt(attackCost/(2-0.5f*specialistSkill));
            WeaponEquip.Instance.ActivateSkill(this, currentWeaponType, 1, -1); 
            
        }
        else
        {
            attackCost = Mathf.RoundToInt(attackCost*(2-0.5f*specialistSkill));
            if(attackCost > maxStamina)
            {
                StartCoroutine(NotEnoughStamina());
                attackCost = Mathf.RoundToInt(attackCost/(2-0.5f*specialistSkill));
            }
            else
            {
                skill1Active = true;
                WeaponEquip.Instance.ActivateSkill(this, currentWeaponType, 1, 1); 
            }
        }
    }

    public virtual void ToggleSkill2()
    {
        if(skill2Active)
        {
            skill2Active = false;
            attackCost = Mathf.RoundToInt(attackCost/(2-0.5f*specialistSkill));
            WeaponEquip.Instance.ActivateSkill(this, currentWeaponType, 2, -1); 
            
        }
        else
        {
            attackCost = Mathf.RoundToInt(attackCost*(2-0.5f*specialistSkill));
            if(attackCost > maxStamina)
            {
                StartCoroutine(NotEnoughStamina());
                attackCost = Mathf.RoundToInt(attackCost/(2-0.5f*specialistSkill));
            }
            else
            {
                skill2Active = true;
                WeaponEquip.Instance.ActivateSkill(this, currentWeaponType, 2, 1); 
            }
        }
    }   
    public virtual void ToggleSkill3()
    {
        if(skill3available)
        {
            if(skill3Active)
            {
                skill3Active = false;
                //don't change cost for support attacks
                if(currentWeaponType<30)
                {
                    attackCost = Mathf.RoundToInt(attackCost/(2-0.5f*specialistSkill));
                }
                WeaponEquip.Instance.ActivateSkill(this, currentWeaponType, 3, -1); 
                
            }
            else
            {
                //don't change cost for support attacks
                if(currentWeaponType<30)
                {
                    attackCost = Mathf.RoundToInt(attackCost*(2-0.5f*specialistSkill));
                    if(attackCost > maxStamina)
                    {
                        StartCoroutine(NotEnoughStamina());
                        attackCost = Mathf.RoundToInt(attackCost/(2-0.5f*specialistSkill));
                    }
                    else
                    {
                        skill3Active = true;
                        WeaponEquip.Instance.ActivateSkill(this, currentWeaponType, 3, 1); 
                    }
                }
                else
                {  
                    skill3Active = true;
                    WeaponEquip.Instance.ActivateSkill(this, currentWeaponType, 3, 1); 
                }
            }
        }
    }       
    public IEnumerator NotEnoughStamina()
    {
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.WarningSound, GameManager.Instance.sfxVolume*2);
        WarningCanvas.SetActive(true);
        WarningCanvas.GetComponentInChildren<TMP_Text>().text = "Not enough Stamina!"; 
        yield return new WaitForSecondsRealtime(2f);
        WarningCanvas.SetActive(false);
    }  
    
    public void ToggleSkill4(bool instantSwap) //weaponswap
    {
        weaponSwap = true;

        if(skill1Active)
        {
            ToggleSkill1(); 
        }
        
        if(skill2Active)
        {
            ToggleSkill2(); 
        }
        if(skill3Active)
        {
            ToggleSkill3(); 
        }

        if(!instantSwap)
        {
            float swapTime = (2f - 1.5f*quickdrawSkill);

            canAttack += swapTime;        //weapon swap time

            if(debuffBar.value<swapTime)
            {
                debuffBar.maxValue = swapTime;
                debuffBar.value = swapTime;
                debuffBar.gameObject.SetActive(true);
            }
        }
        

        
        //unequip and requip
        if(skill4Active)
        {
            skill4Active = false;
            WeaponEquip.Instance.EquipWeapon(this, itemList[2], itemList[3], -1);
            WeaponEquip.Instance.EquipWeapon(this, itemList[0], itemList[1], 1);
            UnitSelections.Instance.GetSkillImage(currentWeaponType, this);   
        }
        else
        {
            skill4Active = true;
            WeaponEquip.Instance.EquipWeapon(this, itemList[0], itemList[1], -1);
            WeaponEquip.Instance.EquipWeapon(this, itemList[2], itemList[3], 1); UnitSelections.Instance.GetSkillImage(currentWeaponType, this);
        }


        weaponSwap = false;

        

    }   



    public void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 360);  
    } 


    public virtual void Attack()
    {
        //Debug.Log("initiate attack");
        Anim.SetFloat("Speed", 0);
        myAgent.isStopped = true;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        
        FaceTarget(target.transform.position);

        if(supportAttack)       //applying attackmod talents
        {
            if(bufferSkill>0)
            {
                bashAttack*= 1+0.2f*bufferSkill;
                pushAttack*= 1+0.2f*bufferSkill;
                slowAttack*= 1+0.2f*bufferSkill;
                dmgOverTime*= 1+0.2f*bufferSkill;
                weakeningAttack*= 1+0.2f*bufferSkill;
                blindingAttack*= 1+0.2f*bufferSkill;
            }
            //Debug.Log("supp " +target);
        }
        else
        {
            if(debufferSkill>0)
            {
                bashAttack*= 1+0.2f*debufferSkill;
                pushAttack*= 1+0.2f*debufferSkill;
                slowAttack*= 1+0.2f*debufferSkill;
                dmgOverTime*= 1+0.2f*debufferSkill;
                weakeningAttack*= 1+0.2f*debufferSkill;
                blindingAttack*= 1+0.2f*debufferSkill;
            }  
        }

        if(projectilePrefab != null)
        {
            StartCoroutine(AttackRanged());
        }
        else
        {
            StartCoroutine(AttackMelee());
        }

        if(supportAttack)//applying attackmod talents
        {
            if(bufferSkill>0)
            {
                bashAttack/= 1+0.2f*bufferSkill;
                pushAttack/= 1+0.2f*bufferSkill;
                slowAttack/= 1+0.2f*bufferSkill;
                dmgOverTime/= 1+0.2f*bufferSkill;
                weakeningAttack/= 1+0.2f*bufferSkill;
                blindingAttack/= 1+0.2f*bufferSkill;
            }
        }
        else
        {
            if(debufferSkill>0)
            {
                bashAttack/= 1+0.2f*debufferSkill;
                pushAttack/= 1+0.2f*debufferSkill;
                slowAttack/= 1+0.2f*debufferSkill;
                dmgOverTime/= 1+0.2f*debufferSkill;
                weakeningAttack/= 1+0.2f*debufferSkill;
                blindingAttack/= 1+0.2f*debufferSkill;
            }  
        }
    }

    public IEnumerator AttackMelee()
    {       
        //Debug.Log(this+"melee");

        if(Time.time > canAttack) //attack if cd 
        {
        if(currentStamina>attackCost)
            {

            currentStamina -= attackCost;
            staminaBar.value = currentStamina;
            
            //update cooldown
            canAttack = Time.time + attackRate;

            Anim.SetTrigger("Attack");

            if(isEnemy)
            {
                myAudio.PlayOneShot(GruntSound, GameManager.Instance.sfxVolume);
            }
            
            yield return new WaitForSeconds(0.4f); 
            
            
            if(target !=null && Vector3.Distance(transform.position, target.transform.position)< range + enemySize +0.5f)
            {
                myAudio.PlayOneShot(attackSound, GameManager.Instance.sfxVolume);
                
                if(aoeAttack>0)
                {
                    CheckForEnemiesMelee();
                }
                else
                {
                    targetStats.TakeDamage(this, dmg, dmgType, armorPierce, armorBypass, armorDestruction,mAcc, targetStats.mEva, targetStats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);
                }

                if(currentWeaponType >14 && currentWeaponType<19) //dual wielding - double attack
                {
                    yield return new WaitForSeconds(0.1f);
                    myAudio.PlayOneShot(attackSound, GameManager.Instance.sfxVolume);
                    targetStats.TakeDamage(this, dmg2, dmgType2, armorPierce, armorBypass, armorDestruction,mAcc, targetStats.mEva, targetStats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);
                }
                
                //reduce weapon durability with each attack
                if(skill4Active)
                {
                    if(itemList[2]!=null)
                    {
                        itemList[2].armor--;
                    }
                    if(itemList[3]!=null && itemList[3].weaponType==4)
                    {
                        itemList[3].armor--;
                    }
                }
                else
                {
                    if(itemList[0]!=null)
                    {
                        itemList[0].armor--;
                    }
                    if(itemList[1]!=null && itemList[1].weaponType==4)
                    {
                        itemList[1].armor--;
                    }
                }
                
            }
            
            }
            
            

        }  
    
    }    

    //creates an aoe sphere that applies a melee attack to all enemies - also put in enemy for different object tag
    public virtual void CheckForEnemiesMelee()
    {
        int i =0;
        Collider[] enemies = Physics.OverlapSphere(transform.position, aoeAttack); //could probably use a 3rd variable layermask, but 8 for enemy is not working
        foreach(Collider enemy in enemies)
        {
            i++;
            var heading = enemy.gameObject.transform.position - transform.position;
            var dot = Vector3.Dot(heading, transform.forward);
            

            if(enemy.gameObject.tag == "Enemy" && dot>0)
            {
                //Debug.Log("hitaoe" +"," +i);
                Fighter stats = enemy.GetComponent<Fighter>();
                stats.TakeDamage(this, dmg, dmgType, armorPierce, armorBypass, armorDestruction,mAcc, targetStats.mEva, targetStats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);
            }
        }
        
    }

    
    public IEnumerator AttackRanged()
    {
        //Debug.Log(this+"ranged");
        //myAgent.SetDestination(transform.position); //another solution

        if(Time.time > canAttack) //attack if cd 
        {
        if(currentStamina>attackCost)
            {
                //reduce weapon durability with each attack
                if(skill4Active)
                {
                    itemList[2].armor--;
                }
                else
                {
                    itemList[0].armor--;
                }

                currentStamina -= attackCost;
                staminaBar.value = currentStamina;
                Anim.SetTrigger("Attack");

                //update cooldown
                canAttack = Time.time + attackRate;

                if(isEnemy)
                {
                    myAudio.PlayOneShot(GruntSound, GameManager.Instance.sfxVolume);
                }
            
                yield return new WaitForSeconds(0.1f); 
                
                myAudio.PlayOneShot(attackSound, GameManager.Instance.sfxVolume);

                GameObject projectileObject = Instantiate(projectilePrefab, transform.position + new Vector3(0,1.5f,0), Quaternion.Euler(90, 90, 0));
                Projectile projectile = projectileObject.GetComponent<arrowProjectile>();
                projectile.enemyProjectile = isEnemy;
                if(supportAttack)
                {
                    Instantiate(SupportMagic, projectileObject.transform.position, projectileObject.transform.rotation, projectileObject.transform);
                    projectile.supportAttack = true;
                }
                projectile.attacker = this;
                projectile.attackSound = attackSound;
                projectile.target = target;
                projectile.aoe = aoeAttack;
                projectile.dmg = dmg;
                projectile.dmgType = dmgType;
                projectile.armorPierce = armorPierce;
                projectile.armorBypass = armorBypass;
                projectile.acc = rAcc;
                projectile.armorDestruction = armorDestruction;
                projectile.pushAttack = pushAttack;
                projectile.bashAttack = bashAttack;
                projectile.dmgOverTime = dmgOverTime;
                projectile.slowAttack = slowAttack;
                projectile.blindingAttack = blindingAttack;
                projectile.weakeningAttack = weakeningAttack;
                projectile.initiated = true; //starts the update routine inside the projectile
                
            }
        }  
        
    }

    public IEnumerator CounterAttack(Fighter counterTargetStats)
    {       
            Anim.SetTrigger("Attack");
            if(isEnemy)
            {
                myAudio.PlayOneShot(GruntSound, GameManager.Instance.sfxVolume);
            }
            yield return new WaitForSeconds(0.3f); 
            myAudio.PlayOneShot(attackSound, GameManager.Instance.sfxVolume);
            counterTargetStats.TakeDamage(this, dmg, dmgType, armorPierce, armorBypass, armorDestruction,mAcc, counterTargetStats.mEva, counterTargetStats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);
    }    
    
    public IEnumerator LevelUP(bool showEffects)
    {
        level++;
        if(level % 2==0)
        {
            Debug.Log("skillpoint");
            if(level/2<potential)
            {
            Debug.Log("skillpoint potential not reached");
                skillPoints++;
            }
        }
        attributePoints+=2;
        switch(fighterClass)
        {
            case 0:
                attributePoints+=2;
            break;
            case 1:
                mAcc++;
                mEva++;
            break;
            case 2:
                pDmg++;
                mAcc++;
            break;
            case 3:
                pDmg++;
                maxHP += 10 +2*lionheartSkill;
                currentHP += 10 +2*lionheartSkill;
            break;
            case 4:
                mEva++;
                mAcc++;
            break;
            case 5:
                pDmg++;
                rAcc++;
            break;
            case 6:
                maxStamina += 10 +2*marathonerSkill;
                currentStamina += 10 +2*marathonerSkill;
                rAcc++;
            break;
            case 7:
                mDmg++;
                rAcc++;
            break;
        }
        
        if(showEffects)
        {
            GameObject LevelUpObject = Instantiate(Resources.Load<GameObject>("StatusEffects/LevelUpEffect"), transform);
            GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(levelUpSound, GameManager.Instance.sfxVolume*0.5f);
            yield return new WaitForSeconds(2);
            Destroy(LevelUpObject);
        }
    }

    public virtual IEnumerator Death()
    {
        if(reviveSkillAvailable)
        {
            Anim.SetTrigger("Death");
            currentHP = maxHP/2;
            currentStamina = maxStamina/2;
            reviveSkillAvailable = false;
            hpBar.value = currentHP;
            staminaBar.value = currentStamina;
            yield return new WaitForSeconds(1f);
            Anim.ResetTrigger("Death");
            GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.reviveSound, GameManager.Instance.sfxVolume);
            Anim.SetTrigger("Revive");
        }
        else
        {
            GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(deathSound, GameManager.Instance.sfxVolume);
            Debug.Log("death");
            Anim.SetTrigger("Death");
            initiation = false;
            myAgent.isStopped = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.velocity = Vector3.zero;
            if(isEnemy)
            {
                UnitSelections.Instance.enemiesList.Remove(this.gameObject);
                UnitSelections.Instance.RemoveEnemyAsTarget(this);

                //distribute xp to unit group
                foreach(GameObject unit in UnitSelections.Instance.unitList)
                {
                    unit.GetComponent<Unit>().GetXP(level/UnitSelections.Instance.unitList.Count);
                }
            }
            else
            {
                UnitSelections.Instance.unitList.Remove(this.gameObject);
                UnitSelections.Instance.unitsSelected.Remove(this.gameObject);
                UnitSelections.Instance.RemoveUnitAsTarget(this);
            }
            transform.GetChild(0).gameObject.SetActive(false);
            //transform.GetChild(1).gameObject.SetActive(false);
            if(!isEnemy)
            {
                GameManager.Instance.fightersLost++;
            }
            hpBarCanvas.SetActive(false);
            //Rigidbody rb = GetComponent<Rigidbody>();
            yield return new WaitForSeconds(0.1f);
            //Destroy(this.gameObject);
        }
    }
}
