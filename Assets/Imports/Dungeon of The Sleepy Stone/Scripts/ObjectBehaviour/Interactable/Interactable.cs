using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public void OnEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            HandController hand = other.gameObject.GetComponent<HandController>();
            hand.EnterObject(gameObject);
        }
        else
        {
            OnOtherEnter(other);
        }
    }

    protected virtual void OnOtherEnter(Collider other)
    {
        //is needed to be overwritten
    }

    public void OnExit(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            HandController hand = other.gameObject.GetComponent<HandController>();
            hand.ExitObject(gameObject);
        }
        else
        {
            OnOtherExit(other);
        }
    }

    protected virtual void OnOtherExit(Collider other)
    {
        //is needed to be overwritten
    }
    
    private void OnTriggerEnter(Collider other)
    {
        OnEnter(other);
    }

    /*private void OnTriggerStay(Collider other)
    {
        OnEnter(other);
    }*/

    private void OnTriggerExit(Collider other)
    {
        OnExit(other);
    }
    public abstract void ButtonUsed(HandController hand, ButtonEventKind buttonEventKind);
    
}