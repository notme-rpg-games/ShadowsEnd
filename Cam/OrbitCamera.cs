using System.Numerics;
using UnityEngine;

public class OrbitCamera : MonoBehaviour {


    private float sensitivity = 5f;
    private float orbitRadius = 2f;

    private float minimumOrbitDistance = 1f;
    private float maximumOrbitDistance = 2f;

    private float yaw;
    private float pitch;
    public UnityEngine.Vector3 targetPos = new UnityEngine.Vector3(0,1.5f,0);

    void Start() {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update() {
        if (Input.GetMouseButton(1)) {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * sensitivity;
            pitch -= mouseY * sensitivity;

            transform.rotation = UnityEngine.Quaternion.Euler(pitch, yaw, 0);
        }

        orbitRadius -= Input.mouseScrollDelta.y / sensitivity;
        orbitRadius = Mathf.Clamp(orbitRadius, minimumOrbitDistance, maximumOrbitDistance);

        transform.position = targetPos - transform.forward * orbitRadius;
        //transform.position = UnitSelections.Instance.unitList[UnitSelections.Instance.selectedUnitnumber].transform.position + new UnityEngine.Vector3(0,1,0) - transform.forward * orbitRadius;
        if(transform.position.y <0.1)
        {
            transform.position = new UnityEngine.Vector3(transform.position.x, 0.1f, transform.position.z);
        }

    }

}