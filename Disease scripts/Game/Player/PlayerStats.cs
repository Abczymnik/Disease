using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private Animator playerAnimator;
    private HealthOrb healthOrb;
    private Experience experience;
    private ClickToMove playerMovement;
    private Attack playerAttack;
    private PerformDeath backToMenu;

    public float AttackDamage { get; private set; } = 12f;
    public float AttackSpeed { get; private set; } = 1f;
    public float MovementSpeed { get; private set; } = 3.5f;
    public float MaxHealth { get => healthOrb.MaxHealth; private set => healthOrb.MaxHealth = value; }
    public float MaxExp { get => experience.MaxExp; private set => experience.MaxExp = value; }

    public float TimeHpRef { get; private set; } = 15f;
    private Coroutine restoreHealthCoroutine;
    public bool IsDead { get; private set; } = false;

    //UI properties
    public float CurrentHealth
    {
        get { return healthOrb.CurrentHealth; }
        private set
        {
            healthOrb.CurrentHealth = value;

            if (healthOrb.CurrentHealth <= 0)
            {
                healthOrb.CurrentHealth = 0;
                Die();
            }
        }
    }

    public float CurrentExp
    {
        get { return experience.CurrentExp; }
        private set
        {
            experience.CurrentExp = value;

            if (experience.CurrentExp >= experience.MaxExp)
            {
                experience.CurrentExp = experience.CurrentExp - experience.MaxExp;
                LevelUp();
            }
        }
    }

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        healthOrb = transform.Find("Health orb").GetComponent<HealthOrb>();
        experience = transform.Find("Experience").GetComponent<Experience>();
    }

    private void Start()
    {
        playerMovement = GetComponent<ClickToMove>();
        playerAttack = GetComponent<Attack>();
        backToMenu = GetComponent<PerformDeath>();
        MaxHealth = 100f;
        CurrentHealth = 100f;
        MaxExp = 100f;
        CurrentExp = 40f;
    }

    //Deal dmg to player and start hp restore coroutine
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth < 0) { return; }
        if(restoreHealthCoroutine == null)
        {
            restoreHealthCoroutine = StartCoroutine(RestoreHealthCoroutine());
            return;
        }

        StopCoroutine(restoreHealthCoroutine);
        restoreHealthCoroutine = StartCoroutine(RestoreHealthCoroutine());
        
    }
    
    //Add experience points to player
    public void GiveExp(float experiencePoints)
    {
        CurrentExp += experiencePoints;
    }

    //Perform Level up
    private void LevelUp()
    {
        MaxHealth += 10;
        CurrentHealth = MaxHealth;
        MaxExp += 10;
        AttackDamage += 2;
        AttackSpeed += 0.1f;
        MovementSpeed += 0.2f;
        playerMovement.LevelUp();
        playerAttack.LevelUp();
    }

    //Player died
    void Die()
    {
        IsDead = true;
        playerAnimator.SetBool("dead", true);
        PlayerUI.BlockInput();
        playerMovement.enabled = false;
        playerAttack.enabled = false;
        StopAllCoroutines();
        backToMenu.enabled = true;
    }

    IEnumerator RestoreHealthCoroutine()
    {
        yield return new WaitForSeconds(TimeHpRef);

        while(CurrentHealth < MaxHealth)
        {
            CurrentHealth += 1f * Time.deltaTime;
            yield return null;
        }

        restoreHealthCoroutine = null;
    }


}


