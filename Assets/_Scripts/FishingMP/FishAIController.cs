using System.Collections;
using Photon.Pun;
using UnityEngine;

public class FishAIController : MonoBehaviour
{

    public FishScriptable _scriptable;
    private Bounds _bounds;

    public Transform target;
    public float pullForce;
    public bool doNotUpdateTarget;
    public float stamina = 1f;
    public float stamina_move = 0.2f;
    public float HealthBar = 1f;
    public float StaminaBar = 0.5f;
    private float frameTime=0.019f;
    public float getOffHookTime = 10f;
    public float currentOffHookTime;
    public bool iscatched = false;
    public bool isTouchingGround = false;
    public bool isUpgradeFishingRod = false;
    float mindistance = 0.6f;
    private bool isStaminaBarStarted = true;
    public FishEntity fishEntity;
    public delegate bool FishPlayer_FishCtrBoolAction();
    public static event FishPlayer_FishCtrBoolAction OnisDestroyFloat;
    public static event FishPlayer_FishCtrBoolAction Onisfishing;
    public static event FishPlayer_FishCtrBoolAction OnisFishCaught;
    private void Start()
    {
        StaminaBar = 0.5f;
        isStaminaBarStarted = true;
    }
    private Vector3 CalculateBoundsVector()
    {
        return _bounds.Contains(transform.position) ? Vector3.zero : (_bounds.center - transform.position).normalized;
    }

    private static readonly Vector3[] _directions = { Vector3.left, Vector3.right, Vector3.up, Vector3.down };
    private Vector3 _currentAvoidanceVector;
    private Vector3 CalculateAvoidanceDirVector()
    {
        if (_currentAvoidanceVector != Vector3.zero)
        {
            if (!Physics.Raycast(transform.position, transform.forward, _scriptable.avoidanceDist, _scriptable.avoidanceMask))
            {
                return _currentAvoidanceVector;
            }
        }
        float maxDistance = int.MinValue;
        Vector3 result = Vector3.zero;
        for (int i = 0; i < _directions.Length; i++)
        {
            Vector3 currentDirection = transform.TransformDirection(_directions[i].normalized);
            if (Physics.Raycast(transform.position, currentDirection, out RaycastHit hitInfo, _scriptable.avoidanceDist, _scriptable.avoidanceMask))
            {
                float distance = (hitInfo.point - transform.position).sqrMagnitude;
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    result = currentDirection;
                }
            }
            else
            {
                result = currentDirection;
                _currentAvoidanceVector = currentDirection.normalized;
                return result.normalized;
            }
        }
        return result.normalized;
    }

    public float fearfulness;

    private Vector3 CalculateFearVector()
    {
        return target != null ? ((target.position + transform.position) * fearfulness).normalized : Vector3.zero;
    }

    private Vector3 CalculateTargetVector()
    {
        return (target != null && fearfulness < .1f) ? (target.position - transform.position).normalized : Vector3.zero;
    }

    private Vector3 CalculateAvoidanceVector()
    {
        Vector3 result = Vector3.zero;
        if (Physics.Raycast(transform.position, transform.forward, _scriptable.avoidanceDist, _scriptable.avoidanceMask))
        {
            result = CalculateAvoidanceDirVector();
        }
        else
        {
            _currentAvoidanceVector = Vector3.zero;
        }
        return result;
    }

    private float CalculateSpeed()
    {
        return 2f;
    }

    [SerializeField] private bool inWater = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            inWater = true;
        }
        if (other.CompareTag("Ground"))
        {
            isTouchingGround = true;
            // Debug.Log("Fish is touching the ground.");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            inWater = false;
        }
        if (other.CompareTag("Ground"))
        {
            isTouchingGround = false;
            //  Debug.Log("Fish is no longer touching the ground.");
        }
    }

    private Vector3 _currentVelocity;
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        bool isDestroyFloat = OnisDestroyFloat?.Invoke() ?? false;
        if (isDestroyFloat)
        {
            StaminaBar = 0.5f;
            isStaminaBarStarted = true;

        }
        bool isfishing = Onisfishing?.Invoke() ?? false;
        if (pullForce > .0f && isfishing )
        {
            pullForce -= frameTime * 1.6f;
            if (!isUpgradeFishingRod)
            {
                HealthBar -= frameTime * .12f;
                stamina -= frameTime * .2f;
            }
            StaminaBar -= frameTime * .3f;
            isStaminaBarStarted = false;
            if (StaminaBar < 0.25 && stamina < 0.60)
            {
                StaminaBar = ExponentialDecrease(StaminaBar, 0.27f, 8f, 1.2f); // Adjust exponent as needed
                if (!isUpgradeFishingRod) { HealthBar -= frameTime * .135f; }

            }

            if (StaminaBar > 0.4 && StaminaBar < 0.6 && stamina < 0.85)
            {
                if (!isUpgradeFishingRod) { HealthBar -= frameTime * .145f; }
                StaminaBar += frameTime * .10f;
            }

        }
        else 
        {
            if (StaminaBar >= 0.77  && stamina != 1 && stamina < 8)
            {
                StaminaBar = ExponentialIncrease(StaminaBar, 0.27f, 25f);
                if (!isUpgradeFishingRod) { HealthBar -= frameTime * .19f; }
            }
            if (StaminaBar > 0.4 && StaminaBar < 0.6 && stamina != 1 && stamina < 8)
            {
                StaminaBar  -= frameTime * .3f;
            }
           
            stamina += frameTime * .3f;

            if (!isStaminaBarStarted)
            {
                StaminaBar += frameTime * .5f;
            }
            else if (doNotUpdateTarget && isStaminaBarStarted)
            {
                StaminaBar += frameTime * .4f;
            }

        }

        if (stamina > .9f)
        {
            currentOffHookTime += frameTime;
        }
        else
        {
            if (currentOffHookTime > 0f) currentOffHookTime -= frameTime;
        }

        pullForce = Mathf.Clamp(pullForce, .0f, 1f);
        stamina = Mathf.Clamp(stamina, .0f, 1f);
        HealthBar = Mathf.Clamp(HealthBar, .0f, 1f);
        StaminaBar = Mathf.Clamp(StaminaBar, .0f, 1f);
        fearfulness = Mathf.Clamp(fearfulness, .0f, 1f);
        Vector3 movementVector = (CalculateBoundsVector() * _scriptable.boundsVectorWeight) +
            (CalculateAvoidanceVector() * _scriptable.avoidanceVectorWeight);
        //  + (CalculateFearVector() * 1f);
        // + (CalculateTargetVector() * 2f);

        // if (target)
        //     movementVector = movementVector.WithY(.1f);
        if (!inWater)
        {
            movementVector = Vector3.down;
            movementVector = Vector3.SmoothDamp(transform.forward, movementVector, ref _currentVelocity, .1f);
            movementVector = movementVector.normalized * (CalculateSpeed() * (doNotUpdateTarget ? 1.5f : 1f) * 1.4f);
        }
        else
        {
            movementVector = Vector3.SmoothDamp(transform.forward, movementVector, ref _currentVelocity, _scriptable.smoothTime);
            movementVector = movementVector.normalized * (CalculateSpeed() * (doNotUpdateTarget ? 1.5f : 1f));
        }

        transform.forward = movementVector;
        Vector3 movement;
        if (stamina > 0.2)
        {
            movement = movementVector * stamina;
        }
        else
        { movement = movementVector * stamina_move; }

        bool isFishCaught = OnisFishCaught?.Invoke() ?? false;

        if (pullForce > 0.0f && target != null && isFishCaught)
        {
            //pullForce = 0;
            Vector3 targetDirection = target.position - transform.position;
            Vector3 horizontalDirection = new Vector3(targetDirection.x, 0.0f, targetDirection.z);
            if (((Vector3.Distance(target.position, transform.position) < 2f || isTouchingGround)))
            {
                Vector3 pull = targetDirection.normalized * 6f;
                movement += pull;
            }
            else
            {
                Vector3 pull = horizontalDirection.normalized * 6f;
                movement += pull;
            }

            if (Vector3.Distance(target.position, transform.position) < mindistance)
            {
                iscatched = true;
            }
            else
            {
                iscatched = false;
            }

        }

        transform.position += movement * frameTime;
        // transform.position += ((movementVector * stamina) + (pullForce > .0f ? (target.position - transform.position) * (.2f * pullForce / stamina / stamina) : Vector3.zero)) * Time.deltaTime;
    }

    private IEnumerator CustomUpdateLoop()
    { // (Server)
        while (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(.4f);
            UpdateTarget(); // Vision
        }
    }

    public void Setup(FishScriptable fishScriptable)
    {
        _scriptable = fishScriptable;
        _bounds = new Bounds(transform.position + _scriptable.boundsSize.y * .5f * Vector3.down, _scriptable.boundsSize);
        StartCoroutine(CustomUpdateLoop());
    }

    public void SetBounds(Vector3 newBounds)
    {
        _bounds = new Bounds(transform.position, newBounds);
    }

    private void UpdateTarget()
    {
        if (doNotUpdateTarget)
        {
            return;
        }

        target = null;
        Collider[] result = Physics.OverlapSphere(transform.position, _scriptable.visionDistance, _scriptable.targetMask);
        for (int i = 0; i < result.Length; i++)
        {
            Transform target = result[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (!Physics.Raycast(transform.position, dirToTarget, Vector3.Distance(transform.position, target.position), _scriptable.avoidanceMask))
            {
                this.target = target;
                break;
            }
        }
    }
    private float ExponentialDecrease(float current, float target, float speed, float exponent)
    {
        float t = Mathf.Clamp01(frameTime * speed);
        float easedT = EaseIn(t);
        float lerpedValue = Mathf.Lerp(current, target, easedT);
        return 1.0f - Mathf.Pow(1.0f - lerpedValue, exponent);
    }
    private float EaseIn(float t)
    {
        return t * t;
    }

    private float ExponentialIncrease(float current, float target, float speed)
    {
        float t = Mathf.Clamp01(frameTime * speed);
        float easedT = EaseIn(t);
        return Mathf.Lerp(current, target, easedT);
    }

#if UNITY_EDITOR // (Editor)
    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    }
#endif
}
