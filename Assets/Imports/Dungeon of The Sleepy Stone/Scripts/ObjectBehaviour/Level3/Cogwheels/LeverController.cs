using UnityEngine;

public class LeverController : Triggerable
{
    public GameObject drivenCogwheelBase;
    private CogwheelBaseController _drivenCogwheelBaseController;
    private Animator _animator;
    private Animator _steamgeneratorAnimator;
    private bool _spin = false;

    void Start()
    {
        _drivenCogwheelBaseController = drivenCogwheelBase.GetComponent<CogwheelBaseController>();
        _animator = gameObject.GetComponent<Animator>();
        _steamgeneratorAnimator = GameObject.Find("Steamgenerator").GetComponent<Animator>();
        
        GameObject player = GameObject.Find("PlayerVR");
    }

    public override void Trigger(HandController hand)
    {
        if (!_spin)
        {
            //Debug.Log("Trigger Lever swap");
            _drivenCogwheelBaseController.Spin(CogwheelBaseController.SpinDirection.Right);
            _animator.SetTrigger("changePosition");
            _steamgeneratorAnimator.SetTrigger("swap");
            _spin = true;
        }
        else
        {
            _drivenCogwheelBaseController.StopSpin();
            _animator.SetTrigger("changePosition");
            _steamgeneratorAnimator.SetTrigger("swap");
            _spin = false;
        }
    }
}