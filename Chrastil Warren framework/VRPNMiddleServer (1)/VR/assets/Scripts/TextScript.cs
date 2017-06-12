using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.VR;


public class TextScript : MonoBehaviour
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
		SensorReadings.text = "Z Axis" + "\nPos.x: " + MyNetworkServer._pos.x
		+ "\nPos.y: " + MyNetworkServer._pos.y
		+ "\nPos.y: " + MyNetworkServer._pos.z;
    }

}
