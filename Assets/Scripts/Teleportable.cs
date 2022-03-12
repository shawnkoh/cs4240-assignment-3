using UnityEngine;

enum TeleportState
{
    Resting,
    AimingValid,
    AimingInvalid,
}

public class Teleportable : MonoBehaviour
{
    public GameObject Indicator;
    private LineRenderer _lineRenderer;

    public string InputIdentifier;
    public PlayerTeleportedSO PlayerTeleported;

    private int _layerMask;

    private TeleportState _state;

    private RaycastHit _raycastHit;
    void Start()
    {
        _layerMask = 1 << 7; // what does this do?
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        _lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        if (Input.GetAxis(InputIdentifier) != 1)
        {
            switch (_state)
            {
                case TeleportState.Resting:
                    // Do nothing.
                    break;
                case TeleportState.AimingInvalid:
                    _state = TeleportState.Resting;
                    break;
                case TeleportState.AimingValid:
                    Teleport(_raycastHit.point);
                    _state = TeleportState.Resting;
                    break;
            }
            UpdateIndicator();
            return;
        }

        if (Physics.Raycast((transform.position), transform.forward, out _raycastHit, Mathf.Infinity, _layerMask))
        {
            _state = TeleportState.AimingValid;
        }
        else
        {
            _state = TeleportState.AimingInvalid;
        }
        
        UpdateIndicator();
    }

    private void Teleport(Vector3 position)
    {
        if (PlayerTeleported == null)
            return;
        PlayerTeleported.Publish(position);
    }

    private void UpdateIndicator()
    {
        switch (_state)
        {
            case TeleportState.AimingValid:
                Indicator.transform.position = _raycastHit.point;
                Indicator.SetActive(true);
                _lineRenderer.enabled = true;
                _lineRenderer.SetPosition(0, transform.position);
                _lineRenderer.SetPosition(1, _raycastHit.point);
                return;
            default:
                Indicator.SetActive(false);
                _lineRenderer.enabled = false;
                return;
        }
    }
}
