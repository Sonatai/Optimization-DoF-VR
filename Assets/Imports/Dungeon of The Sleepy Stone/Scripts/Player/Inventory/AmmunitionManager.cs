using System;
using UnityEngine;

public class AmmunitionManager : MonoBehaviour
{
    [Header("BoltPrefabs")]
    [SerializeField]
    private Rigidbody bodkinBolt;
    [SerializeField]
    private Rigidbody fireBolt;
    [SerializeField]
    private Rigidbody standardBolt;

    [Header("BoltSkins")]
    [SerializeField]
    private GameObject skinBodkinBolt;
    [SerializeField]
    private GameObject skinFireBolt;
    [SerializeField]
    private GameObject skinStandardBolt;

    [Header("BoltAnimation")]
    [SerializeField]
    private Animation bodkinAnim;
    [SerializeField]
    private Animation fireAnim;
    [SerializeField]
    private Animation standardAnim;

    [Header("BoltDummys")]
    [SerializeField]
    private GameObject dummyBodkin;
    [SerializeField]
    private GameObject dummyFire;
    [SerializeField]
    private GameObject dummyStandard;
    
    private Crossbow _scriptCrossbowController;
    public Crossbow ScriptCrossbowController
    {
        set => _scriptCrossbowController = value;
        get => _scriptCrossbowController;
    }
    private void Start()
    {
        skinBodkinBolt.SetActive(false);
        skinFireBolt.SetActive(false);
        skinStandardBolt.SetActive(false);

        dummyBodkin.SetActive(false);
        dummyFire.SetActive(false);
        dummyStandard.SetActive(false);
    }

    private void DeactivateCrossbowEntity()
    {
        _scriptCrossbowController.BoltSkin.SetActive(false);
        _scriptCrossbowController.BoltDummy.SetActive(false);
    }
    
    public void SetBodkin()
    {
        if(_scriptCrossbowController.BoltSkin && _scriptCrossbowController.BoltDummy)
            DeactivateCrossbowEntity();
        _scriptCrossbowController.BoltAnim = bodkinAnim;
        _scriptCrossbowController.BoltDummy = dummyBodkin;
        _scriptCrossbowController.BoltRigibody = bodkinBolt;
        _scriptCrossbowController.BoltSkin = skinBodkinBolt;
        _scriptCrossbowController.Loaded = false;
    }

    public void SetFlaming()
    {
        if (_scriptCrossbowController.BoltSkin && _scriptCrossbowController.BoltDummy)
            DeactivateCrossbowEntity();
        _scriptCrossbowController.BoltAnim = fireAnim;
        _scriptCrossbowController.BoltDummy = dummyFire;
        _scriptCrossbowController.BoltRigibody = fireBolt;
        _scriptCrossbowController.BoltSkin = skinFireBolt; 
        _scriptCrossbowController.Loaded = false;
    }

    public void SetStandard()
    {
        if (_scriptCrossbowController.BoltSkin && _scriptCrossbowController.BoltDummy)
            DeactivateCrossbowEntity();
        _scriptCrossbowController.BoltAnim = standardAnim;
        _scriptCrossbowController.BoltDummy = dummyStandard;
        _scriptCrossbowController.BoltRigibody = standardBolt;
        _scriptCrossbowController.BoltSkin = skinStandardBolt;
        _scriptCrossbowController.Loaded = false;
    }
}

