using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerAttack : MonoBehaviour
{
    private const float ANIMATION_LENGHT = 0.9f;
    private const float ANIMATION_ERROR = 0.2f;

    private Animator attackAnimator;
    private PlayerMovement playerMovement;
    private PlayerStats playerStats;

    private List<Collider> enemiesHitList = new List<Collider>();
    private bool slashAvailable = true;
    private Coroutine attackCoroutine;

    private float rotationSpeed;

    private InputAction attackInput;
    private UnityAction onLevelUp;

    public float AttackDamage { get; private set; }
    private float _attackSpeed;
    public float AttackSpeed
    {
        get { return _attackSpeed; }
        private set
        {
            _attackSpeed = value;
            attackAnimator.SetFloat("AttackSpeed", _attackSpeed);
        }
    }

    private void OnEnable()
    {
        onLevelUp += OnLevelUp;
        EventManager.StartListening("LevelUp", onLevelUp);
    }

    private void Awake()
    {
        attackAnimator = GetComponent<Animator>();
        attackInput = PlayerUI.inputActions.Gameplay.Attack;
        attackInput.performed += OnPlayerAttack;
    }

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        AttackSpeed = playerStats.AttackSpeed;
        AttackDamage = playerStats.AttackDamage;
        playerMovement = GetComponent<PlayerMovement>();
        rotationSpeed = playerMovement.RotationSpeed;
    }

    private void OnPlayerAttack(InputAction.CallbackContext context)
    {
        if (!slashAvailable) return;

        attackCoroutine = StartCoroutine(PerformAttackCoroutine());
    }

    IEnumerator PerformAttackCoroutine()
    {
        attackAnimator.SetBool("attack", true);

        while (attackInput.IsPressed())
        {
            PrepareToAttack();

            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = CalcTargetRotation();
            float rotationPercentage = 0f;

            while (rotationPercentage < 1)
            {
                rotationPercentage += Time.deltaTime * rotationSpeed;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationPercentage);
                yield return null;
            }

            while (playerMovement.Velocity > 1f)
            {
                yield return null;
            }

            DrawAttackAndSlash();
            yield return new WaitForSeconds(ANIMATION_LENGHT / AttackSpeed);
            RecoverAfterAttack();
        }

        attackAnimator.SetBool("attack", false);
    }

    private void PrepareToAttack()
    {
        playerMovement.MovementUIOff();
        slashAvailable = false;
        playerMovement.StopMovementForSeconds(1 / AttackSpeed + ANIMATION_ERROR);
    }

    private void DrawAttackAndSlash()
    {
        enemiesHitList.Clear();
        attackAnimator.SetInteger("attackType", Random.Range(1, 4));
        attackAnimator.SetTrigger("triggerAttack");
    }

    private void RecoverAfterAttack()
    {
        slashAvailable = true;
        playerMovement.MovementUIOn();
    }

    private Quaternion CalcTargetRotation()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 clickPos = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000))
        {
            clickPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }

        Quaternion targetRotation = Quaternion.LookRotation(clickPos - transform.position);
        targetRotation.x = 0;
        return targetRotation;
    }

    public void EnemyHit(Collision enemyHit)
    {
        if (enemiesHitList.Contains(enemyHit.collider)) return;

        enemiesHitList.Add(enemyHit.collider);
        enemyHit.gameObject.GetComponent<Zombie>().TakeDamage(AttackDamage);

        Vector3 effectPos = enemyHit.contacts[0].point;
        Vector3 effectDir = new Vector3(enemyHit.contacts[0].normal.x, 0, enemyHit.contacts[0].normal.z);
        BloodSplashOnHit(effectPos, effectDir);
    }

    private void BloodSplashOnHit(Vector3 effectPos, Vector3 effectDir)
    {
        GameObject bloodHitObj = BloodHitPool.Instance.GetPooledObject();
        if (bloodHitObj == null) return; 

        bloodHitObj.transform.position = effectPos;
        bloodHitObj.transform.forward = effectDir;
        bloodHitObj.SetActive(true);

        VisualEffect bloodHitVfx = bloodHitObj.GetComponent<VisualEffect>();
        bloodHitVfx.SetFloat("HitPointYValue", -bloodHitObj.transform.position.y);
        bloodHitVfx.Play();

        StartCoroutine(ReturnObjToPool());

        IEnumerator ReturnObjToPool()
        {
            yield return new WaitForSeconds(3f);
            bloodHitObj.SetActive(false);
        }
    }

    private void OnLevelUp()
    {
        AttackDamage = playerStats.AttackDamage;
        AttackSpeed = playerStats.AttackSpeed;
    }

    private void OnDisable()
    {
        EventManager.StopListening("LevelUp", onLevelUp);
    }
}