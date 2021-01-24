using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Valve.VR;

public class Crossbow : Grabable
{
    [Header("Crossbow Setting")]
    [SerializeField]
    private Transform spawn;
    [FormerlySerializedAs("anim")] [SerializeField]
    private Animator animator;
    private float scratch = 30f;
    [SerializeField] 
    private RadialMenu radialMenuLeft;
    [SerializeField] 
    private RadialMenu radialMenuRight;
    
    
    [FormerlySerializedAs("attackSpeed")]
    [Header("Player Stat")]
    [SerializeField]
    private float reloadTime = 3f; //... It's called in every single game attackspeed ..... wtf

    //... Crossbow State
    private float reloadTimeLeft;
    private bool loaded = false;
    private bool _triggerPressed = false;
    private Scene _currentScene;
    
    //... Bolt Settings
    private Animation boltAnimation;
    private Rigidbody boltRigidbody;
    private GameObject boltDummy;
    private GameObject boltSkin;
    private Rigidbody shotBoltRigidbody;
    private RadialMenu radialMenu;
    
    //Used to calculate DeltaVector()!
    private Vector3 _lastPosition;
    
    //... Inventory control variabel
    private RadialMenu _radialMenu;
    private static readonly int ReloadSpeed = Animator.StringToHash("reloadSpeed");
    private static readonly int Scratch = Animator.StringToHash("scratch");

    private void Start()
    {
        
        GameObject.Find("Inventory").GetComponent<AmmunitionManager>().ScriptCrossbowController = this;
        reloadTimeLeft = reloadTime;
        _currentScene = SceneManager.GetActiveScene();
    }

    public override void ButtonUsed(HandController hand, ButtonEventKind buttonEventKind)
    {
        if (buttonEventKind == ButtonEventKind.TriggerButtonDown)
        {
            if(!attachedHand)
            {
                if (_currentScene.name == "Level1")
                {
                    var level1MainPuzzle = GameObject.FindGameObjectWithTag("EventHandler").GetComponent<Level1_MainPuzzle>();
                    level1MainPuzzle.crossbowFound = true;
                    level1MainPuzzle.CheckOpenExit();
                }

                transform.position = hand.gameObject.transform.position; //+ new Vector3(0f,0f,0f);
                //Vector3 crossRota =  new Vector3(hand.gameObject.transform.eulerAngles.x, hand.gameObject.transform.eulerAngles.y, hand.gameObject.transform.eulerAngles.z);
                //transform.rotation = Quaternion.Euler(crossRota);
                transform.rotation = hand.gameObject.transform.rotation;
                
                SwitchColliderEnabled(false);
                
                AttachToHand(hand);

                radialMenu = attachedHand.name.Contains("right") ? radialMenuRight : radialMenuLeft;
                attachedHand.GetComponent<InventoryController>().SetRadialMenu = radialMenu;
                _radialMenu = radialMenu;
                
                attachedHand.GetComponent<InventoryController>().AddListeners();
                attachedHand.GetComponent<TeleportationPointer>().enabled = false;

            }
            else if (boltRigidbody != null && boltDummy != null && reloadTimeLeft <= 0)
            {
                _triggerPressed = true;
            }
        }
        else if (buttonEventKind == ButtonEventKind.GrabButtonDown && attachedHand)
        {
            radialMenu.Show(false);
            attachedHand.GetComponent<InventoryController>().RemoveListeners();
            attachedHand.GetComponent<InventoryController>().SetRadialMenu = null;
            attachedHand.GetComponent<TeleportationPointer>().enabled = true;

            SwitchColliderEnabled(true);
            
            ReleaseFromHand();
        }
    }

    private void SwitchColliderEnabled(bool value)
    {
        foreach (Collider collider in GetComponents<Collider>())
        {
            collider.enabled = value;
        }
    }

    //TODO: Conflict beim Schießen + Muni Auswahl
    private void Update()
    {
        _lastPosition = gameObject.transform.position;
        
        reloadTimeLeft -= Time.deltaTime;

        scratch -= Time.deltaTime;
        
        if(scratch < 0)
        {
            animator.SetFloat(Scratch, scratch);
            scratch = 30f;
        }

        if (_triggerPressed && reloadTimeLeft <= 0f)
        {
            _radialMenu.IsShooting = true;
            _triggerPressed = false;
            
            if (boltRigidbody && boltDummy)
            {
                ShootBolt();
                ReloadBolt();
            } 
        }
        
        if (!loaded && boltSkin)
        {
            ReloadBolt();
        }

    }

    private void ReloadBolt()
    {
        _radialMenu.IsReloading = true;
        
        boltSkin.SetActive(true);
        
        SetReloadAnimationSpeed(3f / reloadTime); //original animation took 3 Seconds
        
        animator.Play("Armature|Reload_Crossbow");
        
        boltAnimation.Play();
        
        loaded = true;
        
        StartCoroutine(Reloading(reloadTime));
    }

    private void SetReloadAnimationSpeed(float animationSpeed)
    {
        foreach (AnimationState state in boltAnimation)
        {
            state.speed = animationSpeed;
        }
        
        animator.SetFloat(ReloadSpeed, animationSpeed);
        
    }

    private void ShootBolt()
    {
        var rotation = attachedHand.transform.rotation;
        boltDummy.SetActive(false);

        animator.Play("Armature|Shoot_Crossbow");

        shotBoltRigidbody = Instantiate(boltRigidbody, spawn.position, Quaternion.identity); 
        shotBoltRigidbody.transform.rotation = rotation;
        shotBoltRigidbody.AddForce(spawn.forward * 10, ForceMode.Impulse); 

        reloadTimeLeft = reloadTime;
        loaded = false;
    }

    private IEnumerator Reloading(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        boltSkin.SetActive(false);
        boltDummy.SetActive(true);
        _radialMenu.IsReloading = false;
        _radialMenu.IsShooting = false;
    }

    public Vector3 DeltaVector // is used as force vector for smashing things! 
    {
        get => gameObject.transform.position - _lastPosition;
    }

    public Animation BoltAnim
    {
        set => boltAnimation = value;
        get => boltAnimation; 
    }

    public Rigidbody BoltRigibody
    {
        set => boltRigidbody = value;
        get => boltRigidbody; 
    }
    public GameObject BoltDummy
    {
        set => boltDummy = value;
        get => boltDummy; 
    }
    public GameObject BoltSkin
    {
        set => boltSkin = value;
        get => boltSkin;
    }
    public bool Loaded 
    {
        set => loaded = value;
        get => loaded;
    }
}