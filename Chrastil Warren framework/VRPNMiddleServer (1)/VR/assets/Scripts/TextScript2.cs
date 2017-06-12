using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.VR;


public class TextScript2 : MonoBehaviour
{
    public Text SensorReadings;


    void Start()
    {
        Input.gyro.enabled = true;
    }


    void Update()
    {
        setSensorReadings();
    }

    void setSensorReadings()
    {
		SensorReadings.text = "X Axis" + "\nSteps taken: " + OnlineBodyView.steps + 
			//"\nStep Start Time: " + OnlineBodyView.stepStartTime + "\nLast Step Start: " + OnlineBodyView.lastStepStart +
			//"\nStep End Time: " + OnlineBodyView.stepEndTime + "\nLast Step End: " + OnlineBodyView.lastStepEnd +
			"\nRate: " + OnlineBodyView.rate + "\nVelocity: " + OnlineBodyView.velocity + "\nSeqStart: " + OnlineBodyView.sequenceStartTime +
			"\nTime: " + Time.time +
	//	 + "\n" +
	//		"Last Step Time: " + OnlineBodyView.lastStepTime  + "\nFootIsLeft: " + OnlineBodyView.footIsLeft + 
			"\nAngleL: " + OnlineBodyView.angleL + "\nAngleR: " + OnlineBodyView.angleR + "\n" + 
			(Time.time - OnlineBodyView.firstFootTime - 1f);
			//Mathf.Exp ((OnlineBodyView.sequenceStartTime - Time.time) / 0.2f)  + "\n" + (OnlineBodyView.sequenceStartTime);
            // Input.gyro.enabled.ToString() +
            // "\nX: " + Input.gyro.userAcceleration.x.ToString()
            //"Y: " + Input.gyro.userAcceleration.y.ToString() +
            //"\nTime: " + Time.time +
            // "\nZ: " + Input.gyro.userAcceleration.z.ToString() + 
            // "X Axis\nYaw (x): " + InputTracking.GetLocalRotation(VRNode.Head).eulerAngles.x.ToString() +
           // "\nX Axis\nPitch (y): " + InputTracking.GetLocalRotation(VRNode.Head).eulerAngles.y.ToString();
            //"\nRoll (z): " + InputTracking.GetLocalRotation(VRNode.Head).eulerAngles.z.ToString();
    }

}

