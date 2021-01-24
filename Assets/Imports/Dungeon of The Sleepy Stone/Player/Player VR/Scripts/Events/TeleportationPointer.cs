using UnityEngine;
using Valve.VR;

public class TeleportationPointer : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean teleportAction;

    public GameObject laserPrefab;
    public Transform cameraRigTransform;
    public GameObject teleportReticlePrefab;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;
    public LayerMask teleportBarrierMask;
    public WallCollision wallCollision;

    private GameObject _laser;
    private Transform _laserTransform;
    private Vector3 _hitPoint;
    private GameObject _reticle;
    private GameObject _playerBody;
    private Transform _teleportReticleTransform;
    private bool _shouldTeleport;
    private bool _allowTeleportOnce;
    //public Collider playerFloor;

    private Color laserOnColor = new Color(0f, 1f, 0f, 0.5f);
    private Color laserOffColor = new Color(1f, 0f, 0f, 0.5f);

    void Start()
    {
        _laser = Instantiate(laserPrefab);
        _laserTransform = _laser.transform;
        _reticle = Instantiate(teleportReticlePrefab);
        _teleportReticleTransform = _reticle.transform;
        _playerBody = GameObject.Find("PlayerBody");
    }
    
    public void AllowTeleportOnce()
    {
        _allowTeleportOnce = true;

        this.enabled = true;
        
    }
    
    private void FixedUpdate()
    {
        if (teleportAction.GetState(handType) || _allowTeleportOnce)
        {
            RaycastHit hit;
            if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, layerMask: teleportMask, maxDistance: 100))
            {
                _hitPoint = hit.point;
                
                //... functional code
                Vector3 controllerPointOnBodyHeight = new Vector3 (
                    controllerPose.transform.position.x, 
                    _playerBody.transform.position.y, 
                    controllerPose.transform.position.z
                );

                bool bodyToControllerOnBodyHeightIsFree = PathIsClear(_playerBody.transform.position, controllerPointOnBodyHeight);
                bool controllerToTargetOnBodyHeightIsFree = PathIsClear(controllerPointOnBodyHeight, _hitPoint);
                bool controllerToTargetIsFree = PathIsClear(controllerPose.transform.position, _hitPoint);
                
                if (bodyToControllerOnBodyHeightIsFree && controllerToTargetIsFree && controllerToTargetOnBodyHeightIsFree && hit.distance <= 20)
                {
                    ActivateTeleportLaser(hit);
                }
                else
                {
                    DeactivateTeleportLaser(hit);
                }                  
            }
            else
            {
                _shouldTeleport = false;
                _reticle.SetActive(false);
                _laser.SetActive(false);
            }
        }
        else
        {
            _reticle.SetActive(false);
            _laser.SetActive(false);
        }

        if (teleportAction.GetLastStateUp(handType) && _shouldTeleport)
        {
            Teleport();
            if (_allowTeleportOnce)
            {
                _allowTeleportOnce = false;
                enabled = false;
                _reticle.SetActive(false);
                _laser.SetActive(false);
            }
        }
    }

    private void ActivateTeleportLaser(RaycastHit hit)
    {
        _shouldTeleport = true;
        ShowLaser(hit);
        _teleportReticleTransform.position = _hitPoint + teleportReticleOffset;
        _reticle.SetActive(true);
        _laser.GetComponent<MeshRenderer>().material.color = laserOnColor;
        _reticle.GetComponent<MeshRenderer>().material.color = laserOnColor;
    }

    private void DeactivateTeleportLaser(RaycastHit hit)
    {
        _shouldTeleport = false;
        ShowLaser(hit);
        _reticle.SetActive(false);
        _reticle.GetComponent<MeshRenderer>().material.color = laserOffColor;
        _laser.GetComponent<MeshRenderer>().material.color = laserOffColor;
    }

    private bool PathIsClear(Vector3 source, Vector3 target)
    {
        Vector3 forward = target - source;
        RaycastHit[] hits = Physics.RaycastAll(source, forward, forward.magnitude-0.25f, teleportBarrierMask);
        Debug.DrawLine(source, target, Color.blue);
        if (hits!=null && hits.Length>0)
        {
            return false;
        }
        return true;
    }

    private void ShowLaser(RaycastHit hit)
    {
        _laser.SetActive(true);
        _laserTransform.position = Vector3.Lerp(controllerPose.transform.position, _hitPoint, .5f);
        _laserTransform.LookAt(_hitPoint);
        _laserTransform.localScale = new Vector3(_laserTransform.localScale.x, _laserTransform.localScale.y, hit.distance);
    }

    private void Teleport()
    {
        _shouldTeleport = false;
        _reticle.SetActive(false);
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0;
        cameraRigTransform.position = _hitPoint + difference;
        if (wallCollision)
        {
            wallCollision.UpdatePreviousPosition();
        }
    }
}