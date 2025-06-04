using UnityEngine;
public class arrowProjectile : Projectile
{  

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        if(initiated)
        {
            if(target == null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                acc -= Time.deltaTime*speed*5;
                transform.position = Vector3.Lerp(transform.position, 
                new Vector3 (target.transform.position.x, target.transform.position.y +1.5f, target.transform.position.z) , 
                Time.deltaTime*speed);
            } 
        }
    }




}
