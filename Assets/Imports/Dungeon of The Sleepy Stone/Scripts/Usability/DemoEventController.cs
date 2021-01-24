using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoEventController : MonoBehaviour
{
    bool orangeJewel = false;
    bool lilaJewel = false;
    bool greenJewel = false;

    public GameObject orangeJewelObj;
    public GameObject lilaJewelObj;
    public GameObject greenJewelObj;

    bool gotHit = false;

    private void Start()
    {
        Debug.Log("Name: " + this.name);
    }
    void TurnLightOff()
    {
        GetComponentInChildren<Light>().enabled = false;
    }

    IEnumerator SetLight()
    {
        yield return new WaitForSeconds(5f);
        TurnLightOff();
    }

    private void Update()
    {
        if (greenJewel)
        {
            greenJewelObj.SetActive(true);
            StartCoroutine(SetLight());
            greenJewel = false;
        }
        if (lilaJewel)
        {
            lilaJewelObj.SetActive(true);
            StartCoroutine(SetLight());
            lilaJewel = false;
        }
        if (orangeJewel)
        {
            orangeJewelObj.SetActive(true);
            StartCoroutine(SetLight());
            orangeJewel = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Name of other: " + other.name);
        if(other.name.Contains("Standard") && this.name.Contains("Standard") && !gotHit)
        {
            Debug.Log("Standard hitted");
            greenJewel = true;
            gotHit = false;

        }else if (other.name.Contains("Bodkin") && this.name.Contains("Bodkin") && !gotHit)
        {
            lilaJewel = true;
            gotHit = false;

        }
        else if (other.name.Contains("Flam") && this.name.Contains("Flam") && !gotHit)
        {
            orangeJewel = true;
            gotHit = false;

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name.Contains("Standard") && this.name.Contains("Standard") && !gotHit)
        {
            Debug.Log("Standard hitted");
            greenJewel = true;
            gotHit = false;

        }
        else if (other.name.Contains("Bodkin") && this.name.Contains("Bodkin") && !gotHit)
        {
            lilaJewel = true;
            gotHit = false;

        }
        else if (other.name.Contains("Flam") && this.name.Contains("Flam") && !gotHit)
        {
            orangeJewel = true;
            gotHit = false;

        }
    }
}
