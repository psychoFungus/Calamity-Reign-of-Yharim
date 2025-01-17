using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    private int count = 0;

    public static int internalTime;
    public static int internalHours;
    public static int amOrPm;
    public static string displayTime;
    public string displayAmOrPm;
    public float rotationAngle = -45;

    [SerializeField] private Transform orbitPoint;
    [SerializeField] private Vector3 rotation;

    void Start()
    {
        count = 0;
        internalTime = 30;
        internalHours = 4;
        amOrPm = 1;
    }
    private void Update()
    {
        if (count > 60) //if count is greater than 60
        {
            internalTime++; //increase time (minutes) by 1
            rotationAngle += -0.1f;
            if (internalTime == 60) //if time is 60 (60 minutes)
            {
                internalTime = 0; //reset time to 0
                internalHours++; //increase hours by 1
                if (internalHours == 13) //if hours equals 13
                {
                    internalHours = 1; //reset hours to 1
                }
                if (internalHours == 12) //if hours equals 12
                {
                    amOrPm++; //set to next AM or PM (increase by one)
                    if (amOrPm == 3) //if AM or PM equals 3
                    {
                        amOrPm = 1; //reset AM or PM to 1
                    }
                }
            }
            count = 1; //reset count to 1
        }
        if (amOrPm == 1) //if Am or PM equals 1
        {
            displayAmOrPm = "AM"; //set AM or PM display to AM
        }
        else
        {
            displayAmOrPm = "PM"; //set AM or PM display to PM
        }
        if (internalTime < 10) //if internal time (minutes) is less than 10
        {
            displayTime = internalHours + ":0" + internalTime + " " + displayAmOrPm; //display time with 0
        }
        else
        {
            displayTime = internalHours + ":" + internalTime + " " + displayAmOrPm; //display time without 0
        }

        orbitPoint.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

        if (internalHours == 4 && internalTime == 30 && amOrPm == 1) //if the time is 4:30 AM (1)
        {
            rotationAngle = -45;
        }
        if (internalHours == 7 && internalTime == 30 && amOrPm == 2) //if the time is 7:30 PM (2)
        {
            rotationAngle = -135;
        }
        count++; //increase count by 1 every frame
    }
}
