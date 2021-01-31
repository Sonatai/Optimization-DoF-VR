using System;
using UnityEngine;
using Valve.VR;

public class Controlls : MonoBehaviour
{
    
    
    [SerializeField]private SteamVR_Input_Sources handType; 
    [SerializeField]private SteamVR_Behaviour_Pose controllerPose;
    [SerializeField]private SteamVR_Action_Boolean focusAction;
    private float focalLength = 25f;

    [SerializeField] private GameObject laserPrefab; 
    private GameObject laser; 
    private Transform laserTransform; 
    private Vector3 hitPoint; 
    

    [SerializeField]private Transform cameraRigTransform;
    [SerializeField]private Transform headTransform;

    public  float FocalLength
    {
        get =>  focalLength;
    }
    
    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(controllerPose.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x,
            laserTransform.localScale.y,
            hit.distance);
    }
    
    private void Focus()
    {
        Vector3 focalVec = hitPoint - headTransform.position; 
       focalLength = (float) Math.Sqrt(Math.Pow(focalVec.x, 2) + Math.Pow(focalVec.y, 2) + Math.Pow(focalVec.z, 2)); 
    }


    private void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    private void Update()
    {
        if (focusAction.GetState(handType))
        {
            RaycastHit hit;
            if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100))
            {
                hitPoint = hit.point;
                ShowLaser(hit);
            }
        }
        else 
        {
            laser.SetActive(false);
            
        }
        
        Focus();
    }
}
