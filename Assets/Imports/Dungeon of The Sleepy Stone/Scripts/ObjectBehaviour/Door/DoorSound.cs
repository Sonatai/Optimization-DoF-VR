using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorSound : MonoBehaviour
{
    //Deprecated
    public AudioClip openDoor;
    public AudioClip closeDoor;

    // Update is called once per frame
    private void openSound()
    {
        FindObjectOfType<AudioManager>().Play("tür_öffnen");
    }

    private void closeSound()
    {
        FindObjectOfType<AudioManager>().Play("tür_schließen");
    }

    private void latCloseSound()
    {
        FindObjectOfType<AudioManager>().Play("lattice_schließen");
    }
}
