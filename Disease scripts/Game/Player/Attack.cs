using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    private Animator attackAnimator;
    private ClickToMove playerMovement;
    private PlayerStats playerStats;
    private SwordHit swordTrigger;
    private InputAction attack;

    private float attackDamage;
    private bool slashAvailable = true;
    private List<Collider> enemyHits = new List<Collider>();
    private Coroutine slashCoroutine;

    private Coroutine rotationCoroutine;
    private float smoothRotation;

    public float AttackDamage { get; private set; }

    private float _attackSpeed;
    public float AttackSpeed
    {
        get { return _attackSpeed;  }
        private set
        {
            _attackSpeed = value;
            attackAnimator.SetFloat("AttackSpeed", _attackSpeed);
        }
    }

    void Awake()
    {
        attackAnimator = GetComponent<Animator>();
        attack = PlayerUI.inputActions.Gameplay.Attack;
        attack.performed += TriggerAttack;
    }

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        AttackSpeed = playerStats.AttackSpeed;
        attackDamage = playerStats.AttackDamage;
        playerMovement = GetComponent<ClickToMove>();
        smoothRotation = playerMovement.SmoothRot;
        swordTrigger = GameObject.Find("Sword_3").GetComponent<SwordHit>();
    }

    //UI
    private void TriggerAttack(InputAction.CallbackContext context)
    {
        if (slashAvailable)
        {
            swordTrigger.enabled = true; //Enable sword trigger attached to sword mesh
            Slash();
        }
    }

    private void Slash()
    {
        slashCoroutine = StartCoroutine(AttackPressed()); //simulate attackButton hold
    }

    IEnumerator AttackPressed()
    {
        attackAnimator.SetBool("attack", true);

        while (attack.IsPressed())
        {
            swordTrigger.enabled = true;
            playerMovement.MovementUIOff(); //Block movementUI
            slashAvailable = false; //Block start new coroutine from trigger attack
            playerMovement.Stop(1 / AttackSpeed + 1f); //Stop character

            //Rotate character
            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = CalcTargetRot();
            float rotationPercentage = 0f;

            while (rotationPercentage < 1)
            {
                rotationPercentage += Time.deltaTime * smoothRotation;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationPercentage);
                yield return null;
            }

            while (playerMovement.Velocity > 1f) //Wait for character to almost stop
            {
                yield return null;
            }

            attackAnimator.SetInteger("attackType", Random.Range(1, 4)); //Random attack animation
            attackAnimator.SetTrigger("triggerAttack");
            enemyHits.Clear(); //Clear enemy hits List
            yield return new WaitForSeconds(0.9f / AttackSpeed); //Let attack animation end
            slashAvailable = true; //Enable next attack
            playerMovement.MovementUIOn();
            swordTrigger.enabled = false; //Disable sword trigger
        }

        attackAnimator.SetBool("attack", false);
    }

    //Calc direction to rotate based on mouse pos
    private Quaternion CalcTargetRot()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 clickPos = Vector3.zero;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out hit, 1000))
        {
            clickPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }

        Quaternion targetRotation = Quaternion.LookRotation(clickPos - transform.position);
        targetRotation.x = 0;
        return targetRotation;
    }

    //Capture sword collision with enemies
    public void Hit(Collider enemy)
    {
        if (!enemyHits.Contains(enemy))
        {
            enemyHits.Add(enemy);
            enemy.GetComponent<Zombie>().TakeDmg(attackDamage);
        }
    }

    //Update AttackDamage and AttackSpeed
    public void LevelUp()
    {
        AttackDamage = playerStats.AttackDamage;
        AttackSpeed = playerStats.AttackSpeed;
    }
}