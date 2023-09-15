using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToMove : MonoBehaviour
{
    private Animator playerAnimator;
    private CharacterController controller;
    private InputAction move;

    private PlayerStats playerStats;

    private float minMove; // min dist to do smooth max rotation
    private float tToReachTarget;
    private float actualRunTime;
    private float tInStage;
    private float tMidStage;
    private float tOutStage;
    private float tInDispl;
    private float tOutDispl;
    private float accelerationTime = 0.35f;
    private Coroutine moveCoroutine;

    private float rotationPercentage;
    private Coroutine rotateCoroutine;

    private bool _isRunning;
    public bool IsRunning
    {
        get { return _isRunning; }
        private set
        {
            _isRunning = value;
            if (value == false) { Velocity = 0f; }
            actualRunTime = 0f;
        }
    }

    private float _velocity;
    public float Velocity
    {
        get { return _velocity; }
        private set
        {
            _velocity = value;
            playerAnimator.SetFloat("Velocity", _velocity);
        }
    }

    public float MaxVelocity { get; private set; } = 3.5f;
    public float Acceleration { get; private set; }
    public float SmoothRot { get; private set; } = 6f;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        move = PlayerUI.inputActions.Gameplay.Move;
        MovementUIOn();
    }

    private void OnDisable()
    {
        MovementUIOff();
    }

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        MaxVelocity = playerStats.MovementSpeed;
        PlayerUI.SwitchActionMap(PlayerUI.inputActions.Gameplay); //Enable Gameplay actionMap
        minMove = 0.5f/SmoothRot; // minMove based on rotation speed
        Acceleration = MaxVelocity / accelerationTime; // v = a * t
    }

    private void Update()
    {
        if (IsRunning) { MoveToPosition(); }
    }

    //Return actual minimal possible move based on actual velocity
    private float ActualMinMove()
    {
        float tempMinMove = Velocity*0.5f*Velocity/Acceleration;
        if (tempMinMove > minMove)
        {
            return tempMinMove;
        }
        return minMove;
    }

    //Target movement destination
    private void TargetInput(InputAction.CallbackContext context)
    {
        if (context.control.device.displayName == "Mouse" && UIHelper.IsPointerOverUI()) { return; } // Check if mouse trigger move over UI

        StopAllCoroutines(); //Stop WASD coroutine, rotation coroutine and waiting coroutine
        Vector2 inputVector = move.ReadValue<Vector2>();
        Vector3 clickPos = Vector3.zero;
        float tempDist = 0f;

        //Get Mouse input point
        if (context.control.device.displayName == "Mouse")
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(inputVector);

            if (Physics.Raycast(ray, out hit, 1000))
            {
                clickPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                tempDist = Vector3.Distance(transform.position, clickPos);
                if (ActualMinMove() > tempDist) return;
            }

            IsRunning = true;
            CalcMovementTime(clickPos);
        }

        else { moveCoroutine = StartCoroutine(MoveCoroutine()); } // WASD movement

        //end of func

        // local coroutine for WASD movement
        IEnumerator MoveCoroutine()
        {
            while(true)
            {
                tempDist = ActualMinMove();
                clickPos = transform.position + new Vector3(inputVector.x, 0, inputVector.y) * tempDist * (MaxVelocity-1f);

                IsRunning = true;
                CalcMovementTime(clickPos);
                yield return null;
            }
        }
    }

    //Stop WASD coroutine if Move button release
    private void MoveCanceled(InputAction.CallbackContext context)
    {
        if (context.control.device.displayName == "Mouse") return;

        StopCoroutine(moveCoroutine);
    }

    private void CalcMovementTime(Vector3 targetPos)
    {
        //3 stage movement accelerating in accelerationTime to MaxVelocity, then constant, then 0m/s in accelerationTime
        //If interrupted then calculate new movement based on actual velocity
        Vector3 startPos = transform.position;
        float distToTarget = Vector3.Distance(targetPos, startPos);

        //Start Rotation Coroutine
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - startPos);
        targetRotation.x = 0;
        if (rotateCoroutine != null) { StopCoroutine(rotateCoroutine); }
        rotateCoroutine = StartCoroutine(RotateCoroutine(targetRotation));

        tInStage = (-Velocity + Mathf.Sqrt(Velocity * Velocity + 2 * Acceleration * distToTarget)) / Acceleration;  //Quadratic equation

        float vMaxInStage = Velocity + Acceleration * tInStage;
        if(vMaxInStage <= MaxVelocity)
        {
            tMidStage = 0f;
            tOutStage = vMaxInStage/Acceleration;
        }
        else
        {
            tInStage = (MaxVelocity - Velocity) / Acceleration;
            tOutStage = accelerationTime;
            float inStageDist = (MaxVelocity + Velocity) * 0.5f * tInStage;
            float outStageDist = 0.5f * Acceleration * accelerationTime * accelerationTime;
            tMidStage = (distToTarget - inStageDist - outStageDist) / MaxVelocity;
        }
        tOutDispl = accelerationTime - tOutStage;
        tInDispl = Velocity / Acceleration;
        tToReachTarget = tInStage + tMidStage + tOutStage;
    }

    //Easing func
    float QuadEaseIn(float t) => Acceleration*t; // v = a*t
    float QuadEaseOut(float t)
    {
        t -= tInStage + tMidStage;
        return MaxVelocity - (Acceleration * t);
    }

    //Calc actual Velocity for movement func
    private void CalcVelocity(float t)
    {
        if(actualRunTime <= tInStage)
        {
            Velocity = QuadEaseIn(t+tInDispl);
            return;
        }
        if(actualRunTime > tInStage+tMidStage)
        {
            Velocity = QuadEaseOut(t+tOutDispl);
            return;
        }
        Velocity = MaxVelocity;
    }

    //Straight movement based on actual Velocity
    void MoveToPosition()
    {
        actualRunTime += Time.deltaTime;
        if (actualRunTime > tToReachTarget)
        {
            IsRunning = false;
            Velocity = 0f;
            return;
        }
        CalcVelocity(actualRunTime);
        controller.SimpleMove(transform.forward * Velocity);
    }

    //Perform rotation
    private IEnumerator RotateCoroutine(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        rotationPercentage = 0f;

        while (rotationPercentage < 1)
        {
            rotationPercentage += Time.deltaTime * SmoothRot;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationPercentage);
            yield return null;
        }
    }

    //Stop and then wait for seconds
    public void Stop(float seconds)
    {
        if (Velocity > 0) //Calc braking trace
        {
            IsRunning = true;
            float minMove = ActualMinMove();
            Vector3 targetPos = transform.position + minMove * transform.forward;
            CalcMovementTime(targetPos);
        }

        if (moveCoroutine != null) { StopCoroutine(moveCoroutine); } // Stop WASD move coroutine if enabled

        StartCoroutine(WaitForStop());

        //Local Coroutine waiting for stop
        IEnumerator WaitForStop()
        {
            while(Velocity != 0)
            {
                yield return null;
            }

            yield return new WaitForSeconds(seconds);
        }
    }

    //Update Max velocity and acceleration
    public void LevelUp()
    {
        MaxVelocity = playerStats.MovementSpeed;
        Acceleration = MaxVelocity / accelerationTime;
    }

    //Enable movement UI
    public void MovementUIOn()
    {
        move.performed += TargetInput;
        move.canceled += MoveCanceled;
    }

    //Disable movement UI
    public void MovementUIOff()
    {
        move.performed -= TargetInput;
        move.canceled -= MoveCanceled;
    }
}