using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        int doubles = 0;
        for(int i=0; i<GameManager.Instance.arenaLevel; i++)
        {
            int index = Random.Range(0, enemyList.Count);
            //iniate double for weak enemies
            if(index <2)
            {
                if(i+doubles<11)
                {
                    Instantiate(enemyList[index], new Vector3(20+Random.Range(-1,2), 0,-6+2*i+doubles), transform.rotation);
                    doubles++;
                    Instantiate(enemyList[index], new Vector3(20+Random.Range(-1,2), 0,-6+2*i+doubles), transform.rotation);
                }
                else
                {
                    Instantiate(enemyList[index], new Vector3(23+Random.Range(-1,2), 0,-6+2*i+doubles), transform.rotation);
                    doubles++;
                    Instantiate(enemyList[index], new Vector3(23+Random.Range(-1,2), 0,-6+2*i+doubles), transform.rotation);
                }
            }
            else
            {
                if(i+doubles<11)
                {
                    Instantiate(enemyList[index], new Vector3(20+Random.Range(-1,2), 0,-6+2*i+doubles), transform.rotation);
                }
                else
                {
                    Instantiate(enemyList[index], new Vector3(23+Random.Range(-1,2), 0,-6+2*i+doubles), transform.rotation);
                }
            }

            UnitSelections.Instance.enemySpawnFinished = true;
        }   
    }
}
