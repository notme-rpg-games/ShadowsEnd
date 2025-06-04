using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitClick : MonoBehaviour
{
    public GameObject groundMarker;

    public LayerMask clickable;
    public LayerMask ground;
    public LayerMask enemy;
    public LayerMask UI;
    

    // Update is called once per frame
    void Update()
    {
        //left click selection
        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            //Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                //if we hit a clickable object
                Debug.Log("test unit");
                
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    //shift clicked
                    UnitSelections.Instance.ShiftClickSelect(hit.collider.gameObject);
                }
                else
                {
                    //normal clicked
                    UnitSelections.Instance.ClickSelect(hit.collider.gameObject);
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemy))
            {
                UnitSelections.Instance.DeselectEnemy();
                Debug.Log("test enemy");
                UnitSelections.Instance.ClickSelectEnemy(hit.collider.gameObject);
            }            
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                
                Debug.Log("test ground");
                //if we don't && we are not shift clicking
                if(!Input.GetKey(KeyCode.LeftShift))
                {
                    UnitSelections.Instance.DeselectAll();
                }
            }
        }
    }
}
