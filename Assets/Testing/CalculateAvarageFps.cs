using System;
using UnityEngine;

public class CalculateAvarageFps : MonoBehaviour
{
    private float tenSecStatic = 1f;
    private float totalSecStatic;
    float avg = 0f;
    private float tenSec;
    public float totalSec = 60;
    bool timerRunning = false;
    public bool reset = false;
    public string nameSpace;
    public string positon;
    public string focuspunkt;

    // https://answers.unity.com/questions/326621/how-to-calculate-an-average-fps.html

    private void Start()
    {
        totalSecStatic = totalSec;
    }

    void Update()
    {
        if (reset)
        {
            tenSec = tenSecStatic;
            totalSec = totalSecStatic;
            avg = 0f;
            timerRunning = true;
            reset = false;
        }

        if (timerRunning)
        {
            tenSec -= Time.smoothDeltaTime;
            totalSec -= Time.smoothDeltaTime;
            if (totalSec >= 0)
            {
                avg += ((Time.deltaTime / Time.timeScale) - avg) * 0.03f;
            }
            else
            {
                timerRunning = false;
            }
            if (tenSec < 0)
            {
                Debug.Log($"{nameSpace} - position {positon} - focuspoint {focuspunkt}");
                Debug.Log(1f / avg);
                tenSec = tenSecStatic;
            }
        }
    }
}