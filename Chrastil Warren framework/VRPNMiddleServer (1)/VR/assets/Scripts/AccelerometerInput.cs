using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class AccelerometerInput : MonoBehaviour {
    private float yaw;
    private float rad;
    private float xVal;
    private float zVal;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        
        if (Input.gyro.userAcceleration.y >= 0.105f || Input.gyro.userAcceleration.y <= -0.105f)
        {
            yaw = InputTracking.GetLocalRotation(VRNode.Head).eulerAngles.y;
            rad = yaw * Mathf.Deg2Rad;
            zVal = 0.35f * Mathf.Cos(rad);
            xVal = 0.35f * Mathf.Sin(rad);
            
            transform.Translate(xVal, 0, zVal);
           
            rb.transform.Translate(xVal, 0, zVal);
           
        }

    }
}
