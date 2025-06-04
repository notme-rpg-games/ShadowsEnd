using System.Collections;
using UnityEditor;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{  
    //public AudioClip hitSound;
    public Fighter attacker;
    public float speed = 42f;
    public int dmg;  
    public int dmgType;
    public int armorPierce;
    public int armorBypass;
    public int armorDestruction;
    public float acc; //float to reduce acc by distance
    public int accuracyAfterFlight; //final int
    public Vector3 direction;
    public GameObject target;
    public Vector3 targetPosition;
    public AudioClip attackSound;
    public bool initiated;
    public int aoe;
    public float pushAttack;
    public float bashAttack;
    public float dmgOverTime;
    public float slowAttack;
    public float blindingAttack;
    public float weakeningAttack;
    public bool supportAttack;

    //true if enemy shoots projectile
    public bool enemyProjectile;
    

/*     public void Move()
    {
        int i=0;
        float distance = 10;
        while(distance >0.5 && i<100000)
        {
        i++;
        distance = Vector3.Distance(transform.position, target.transform.position);
        //Debug.Log(transform.position +", " + target.transform.position);
        transform.position = Vector3.Slerp(transform.position, new Vector3 (target.transform.position.x, target.transform.position.y +1.5f, target.transform.position.z) , Time.deltaTime*speed);
        }
        Fighter targetstats = target.GetComponent<Fighter>();
        targetstats.TakeDamage(dmg, armorPierce, armorBypass, acc, targetstats.rEva, targetstats.armor, false);
        Destroy(this.gameObject);

    } */
    public void OnTriggerEnter (Collider other)
    {
        
        Debug.Log("projectile:" +target);
        if(other.gameObject == target)
        {
            Debug.Log("projectile enter -hit");
            StartCoroutine(Impact());
        }
        else
        {
            
            Debug.Log("projectile enter -other"+other);
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if(distanceToTarget < 7 && distanceToTarget > 2)      //reduce acc if target blocked
            {
                acc -= 20;
            }
        }
    }

    private void CheckForEnemies()
    {
        Debug.Log("projectile enemy check");
        Collider[] enemies = Physics.OverlapSphere(transform.position, aoe); //could probably use a 3rd variable layermask, but 8 for enemy is not working
        foreach(Collider enemy in enemies)
        {
            if(enemyProjectile || supportAttack)
            {
                if(enemy.gameObject.tag == "Player")
                {
                Fighter targetstats = enemy.GetComponent<Fighter>();
                Debug.Log(targetstats);
                if(supportAttack)
                {
                    targetstats.GetComponent<AudioSource>().PlayOneShot(attackSound, GameManager.Instance.sfxVolume);
                    targetstats.GetBuffed(dmg, dmgType, armorPierce, armorBypass, armorDestruction, accuracyAfterFlight, targetstats.rEva, targetstats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);   
                }
                else
                {
                    targetstats.GetComponent<AudioSource>().PlayOneShot(attackSound, GameManager.Instance.sfxVolume);
                    targetstats.TakeDamage(attacker, dmg, dmgType, armorPierce, armorBypass, armorDestruction, accuracyAfterFlight, targetstats.rEva, targetstats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);
                }
                }
            }
            else
            {
                if(enemy.gameObject.tag == "Enemy")
                {
                Fighter targetstats = enemy.GetComponent<Fighter>();
                Debug.Log(targetstats);
                if(supportAttack)
                {
                    targetstats.GetComponent<AudioSource>().PlayOneShot(attackSound, GameManager.Instance.sfxVolume);
                    targetstats.GetBuffed(dmg, dmgType, armorPierce, armorBypass, armorDestruction, accuracyAfterFlight, targetstats.rEva, targetstats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);   
                }
                else
                {
                    targetstats.GetComponent<AudioSource>().PlayOneShot(attackSound, GameManager.Instance.sfxVolume);
                    targetstats.TakeDamage(attacker, dmg, dmgType, armorPierce, armorBypass, armorDestruction, accuracyAfterFlight, targetstats.rEva, targetstats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);
                }
                }
            }
            
        }
    }

    private IEnumerator Impact()
    {
        Debug.Log("projectile impact");
            accuracyAfterFlight = Mathf.FloorToInt(acc);
            //Debug.Log(accuracyAfterFlight);
            if(aoe>0)
            {
                acc+=100;
                transform.localScale *= (aoe/2);
                CheckForEnemies();
            }
            else
            {
            Fighter targetstats = target.GetComponent<Fighter>();
            if(supportAttack)
            {
                //Debug.Log(supportAttack);
                targetstats.GetComponent<AudioSource>().PlayOneShot(attackSound, GameManager.Instance.sfxVolume);
                targetstats.GetBuffed(dmg, dmgType, armorPierce, armorBypass, armorDestruction, accuracyAfterFlight, targetstats.rEva, targetstats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);   
            }
            else
            {
                //Debug.Log(supportAttack);
                targetstats.GetComponent<AudioSource>().PlayOneShot(attackSound, GameManager.Instance.sfxVolume);
                targetstats.TakeDamage(attacker, dmg, dmgType, armorPierce, armorBypass, armorDestruction, accuracyAfterFlight, targetstats.rEva, targetstats.armor, bashAttack, pushAttack, dmgOverTime, slowAttack, blindingAttack, weakeningAttack);
            }
            }

            //destroy arrows instantly
            if(dmgType!=0)
            {
                yield return new WaitForSecondsRealtime(0.5f);
            }
            yield return new WaitForEndOfFrame();
            Destroy(this.gameObject);

    }

}
