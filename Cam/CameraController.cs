using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 MouseScrollStartPos;
    Camera myCam;
    private int panningDamp = 5000; //slows down the panning speed
    private float edgePanningSpeed = 0.04f; //increases the speed of edgepanning
    private float zoomSpeed = 0.4f;
    private int borderSize = 30; //influences the distance when edge panning activates and its max speed
    private int camEdgeLeft;
    private int camEdgeRight;
    private int camEdgeBot;
    private int camEdgeTop;
    private int camEdgeGroundY;
    private int camEdgeSkyY;

    // Start is called before the first frame update
    void Start()
    {
        myCam = GetComponent<Camera>();
        camEdgeLeft = GameManager.Instance.mapEdgeLeft +20; //fit the map borders to the camera view
        camEdgeRight = GameManager.Instance.mapEdgeRight -20;
        camEdgeBot = GameManager.Instance.mapEdgeBot -15;
        camEdgeTop = GameManager.Instance.mapEdgeTop -40;
        camEdgeGroundY = GameManager.Instance.mapEdgeGround +5;
        camEdgeSkyY = GameManager.Instance.mapEdgeSky;

    }

    // Update is called once per frame
    void Update()
    {
        HandleMiddleMousePanning();

        HandleEdgePanning();

        HandleMapSize();

        HandleZooming();


    }

    public void CenterCameraOnUnit()
    {
        transform.position = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].transform.position + new Vector3(0, 8, -8);
    }
    private void HandleMiddleMousePanning()
    {
        if (Input.GetMouseButtonDown(2))
        {
            MouseScrollStartPos = Input.mousePosition;
        }

        Vector3 movement = Vector3.zero;
        Vector3 movementTransformed = Vector3.zero;

        if (Input.GetMouseButton(2))
        {
            movement = (Input.mousePosition - MouseScrollStartPos) / panningDamp *transform.position.y;
            movementTransformed.x = movement.x;
            movementTransformed.y = 0;
            movementTransformed.z = movement.y;
            
        }

        myCam.transform.position -= movementTransformed;
    }

    private void HandleEdgePanning()
    {        
        int distanceToTop = myCam.pixelHeight - (int)Input.mousePosition.y;
        int distanceToBottom = (int)Input.mousePosition.y;
        int distanceToRight = myCam.pixelWidth - (int)Input.mousePosition.x;
        int distanceToLeft = (int)Input.mousePosition.x;

        if (distanceToTop < borderSize && distanceToTop>0)
        {
            myCam.transform.position += Vector3.forward* edgePanningSpeed *transform.position.y; //move into z+ direction
            //myCam.transform.position += Vector3.forward*Time.time * (borderSize-distanceToTop) * edgePanningSpeed *transform.position.y; //move into z+ direction
        } 
        if (distanceToBottom < borderSize && distanceToBottom>0)
        {
            myCam.transform.position += Vector3.back * edgePanningSpeed *transform.position.y; //move into z- direction
            //myCam.transform.position += Vector3.back *Time.time * (borderSize-distanceToBottom) * edgePanningSpeed *transform.position.y; //move into z- direction
        } 
        if (distanceToRight < borderSize && distanceToRight>0)
        {
            myCam.transform.position += Vector3.right * edgePanningSpeed *transform.position.y; //move into x+ direction
            //myCam.transform.position += Vector3.right *Time.time * (borderSize-distanceToRight) * edgePanningSpeed *transform.position.y; //move into x+ direction
        } 
        if (distanceToLeft < borderSize && distanceToLeft >0)
        {
            myCam.transform.position += Vector3.left * edgePanningSpeed *transform.position.y; //move into x- direction
            //myCam.transform.position += Vector3.left *Time.time * (borderSize-distanceToLeft) * edgePanningSpeed *transform.position.y; //move into x- direction
        }
    }

    private void HandleMapSize()
    {
        //restrict cam move according to map size
        if (camEdgeLeft > myCam.transform.position.x)
        {
            myCam.transform.position = new Vector3 (camEdgeLeft, myCam.transform.position.y, myCam.transform.position.z);
        }
        if (camEdgeRight < myCam.transform.position.x)
        {
            myCam.transform.position = new Vector3 (camEdgeRight, myCam.transform.position.y, myCam.transform.position.z);
        }

        if (camEdgeSkyY < myCam.transform.position.y)
        {
            myCam.transform.position = new Vector3 (myCam.transform.position.x,camEdgeSkyY , myCam.transform.position.z);
        }
        if (camEdgeGroundY > myCam.transform.position.y)
        {
            myCam.transform.position = new Vector3 (myCam.transform.position.x, camEdgeGroundY, myCam.transform.position.z);
        }

        if (camEdgeBot > myCam.transform.position.z)
        {
            myCam.transform.position = new Vector3 (myCam.transform.position.x, myCam.transform.position.y, camEdgeBot);
        }

        if (camEdgeTop < myCam.transform.position.z)
        {
            myCam.transform.position = new Vector3 (myCam.transform.position.x, myCam.transform.position.y, camEdgeTop);
        }
    }

    private void HandleZooming()
    {  
        //check if mouse is scrolled and below or above max zoom
        if ((Input.mouseScrollDelta.y > 0 && myCam.transform.position.y > camEdgeGroundY) || (Input.mouseScrollDelta.y < 0 && myCam.transform.position.y < camEdgeSkyY))
        {
            myCam.transform.position = new Vector3 (transform.position.x, transform.position.y - Input.mouseScrollDelta.y  * zoomSpeed,  transform.position.z + Input.mouseScrollDelta.y * zoomSpeed);
        }
    }
}
