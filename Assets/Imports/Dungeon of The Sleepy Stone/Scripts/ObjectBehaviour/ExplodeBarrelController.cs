using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ExplodeBarrelController : Grabable
{
    public bool isExploded = false;
    private float despawnTimer = 2f;
    public GameObject barrel;
    public GameObject explodEffect;
    public Detonator explode;
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (isExploded)
        {
            despawnTimer -= Time.deltaTime;
            
            if (despawnTimer < 0f)
            {
                DestroyImmediate(barrel);
            }
        }
    }

    public void Explode()
    {
        if (!isExploded)
        {
            _meshRenderer.enabled = false;
            _rigidbody.isKinematic = true;
        
            isExploded = true;
        
            //explodEffect.SetActive(true);
            explode.enabled = true;
        }
        
        if (attachedHand)
        {
            ReleaseFromHand();
        }
    }
}
