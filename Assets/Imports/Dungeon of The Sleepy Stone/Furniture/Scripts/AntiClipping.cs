using UnityEngine;
using System.Collections;
 
public class AntiClipping : MonoBehaviour
{
    public bool sendTriggerMessage = true; 	

    public LayerMask searchMask = -1; //make sure we aren't in this layer
    private Collider _collider;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _collider = gameObject.GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        //ckecks if linecast between current position and predicted next frame position collides with Object
        RaycastHit rayHit;
        var nextFramepositionPrediciton = transform.position + _rigidbody.velocity * 0.02f;
        
        if (Physics.Linecast(transform.position, nextFramepositionPrediciton, out rayHit, layerMask: searchMask))
        {
            transform.position = transform.position;
            //Debug.Log("Colliding with " + rayHit.collider.gameObject);
            TrySendingColliderMessage(rayHit);
        }
    }

    private void TrySendingColliderMessage(RaycastHit rayHit)
    {
        if (sendTriggerMessage && rayHit.collider.isTrigger && _collider)
        {
            rayHit.collider.SendMessage("OnTriggerEnter", _collider, SendMessageOptions.DontRequireReceiver);
            gameObject.SendMessage("OnTriggerEnter", rayHit.collider, SendMessageOptions.DontRequireReceiver);
        }
    }
}