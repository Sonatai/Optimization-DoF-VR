using UnityEngine;

public class CalculateAvarageFps : MonoBehaviour
{
    float avg    = 0f;
    public float tenSec = 10;
    bool timerRunning = false;
    public bool reset = false;
    public string nameSpace;
    public string positon;
    public string focuspunkt;
    
    // https://answers.unity.com/questions/1040630/do-something-for-10-sec.html
    void Update()
    {
        if (reset)
        { 
            tenSec = 10;
            avg    = 0f;
            timerRunning = true;
            reset = false;
        }
        
        if(timerRunning){
            tenSec -= Time.smoothDeltaTime;
            if(tenSec >= 0){  
                avg += ((Time.deltaTime / Time.timeScale) - avg) * 0.03f;
            }else{
                Debug.Log($"{nameSpace} - position {positon} - focuspoint {focuspunkt}");
                Debug.Log(1f / avg);
                timerRunning = false;
            }
        }
    }
}
