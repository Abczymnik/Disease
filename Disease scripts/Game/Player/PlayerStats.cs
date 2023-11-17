using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private HealthBar playerHealth;
    [SerializeField] private Experience playerExperience;
    private PlayerMovement playerMovement;
    private Attack playerAttack;
    private Animator playerAnimator;

    private UnityAction onPlayerDeath;
    private UnityAction onLevelUp;
    private UnityAction<object> onPlayerDamage;
    private UnityAction<object> onPlayerExperience;

    private Coroutine restoreHealthCoroutine;

    public float AttackDamage { get; private set; } = 12f;
    public float AttackSpeed { get; private set; } = 1f;
    public float MovementSpeed { get; private set; } = 3.5f;
    public float TimeForHealthRefill { get; private set; } = 15f;
    public bool IsDead { get; private set; } = false;

    public float MaxHealth { get => playerHealth.MaxHealth; private set => playerHealth.MaxHealth = value; }
    public float MaxExp { get => playerExperience.MaxExp; private set => playerExperience.MaxExp = value; }

    public float CurrentHealth
    {
        get { return playerHealth.CurrentHealth; }
        private set
        {
            if(value <= 0)
            {
                playerHealth.CurrentHealth = 0;
                EventManager.TriggerEvent("PlayerDeath");
                return;
            }

            playerHealth.CurrentHealth = value;
        }
    }

    public float CurrentExp
    {
        get { return playerExperience.CurrentExp; }
        private set
        {
            if(value >= MaxExp)
            {
                playerExperience.CurrentExp = value - MaxExp;
                EventManager.TriggerEvent("LevelUp");
                return;
            }

            playerExperience.CurrentExp = value;
        }
    }

    private void Awake()
    {
        if (playerHealth == null) playerHealth = transform.Find("Health orb").GetComponent<HealthBar>();
        if (playerExperience == null) playerExperience = transform.Find("Experience").GetComponent<Experience>();
    }

    private void OnEnable()
    {
        onPlayerDeath += OnPlayerDeath;
        EventManager.StartListening("PlayerDeath", onPlayerDeath);
        onLevelUp += OnLevelUp;
        EventManager.StartListening("LevelUp", onLevelUp);
        onPlayerDamage += OnPlayerDamage;
        EventManager.StartListening("DamagePlayer", onPlayerDamage);
        onPlayerExperience += OnPlayerExperience;
        EventManager.StartListening("AddExperience", onPlayerExperience);
    }

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<Attack>();
        MaxHealth = 100f;
        CurrentHealth = MaxHealth;
        MaxExp = 100f;
        CurrentExp = 0f;
    }

    IEnumerator RestoreHealthCoroutine()
    {
        yield return new WaitForSeconds(TimeForHealthRefill);

        while (CurrentHealth < MaxHealth)
        {
            CurrentHealth += 1f * Time.deltaTime;
            yield return null;
        }

        restoreHealthCoroutine = null;
    }

    private void OnPlayerExperience(object experienceData)
    {
        float experienceToAdd = (float)experienceData;
        CurrentExp += experienceToAdd;
    }

    private void OnPlayerDamage(object damageData)
    {
        float damage = (float)damageData;
        CurrentHealth -= damage;
        if (CurrentHealth < 0) { return; }

        if (restoreHealthCoroutine != null) StopCoroutine(restoreHealthCoroutine);

        restoreHealthCoroutine = StartCoroutine(RestoreHealthCoroutine());
    }

    private void OnLevelUp()
    {
        MaxHealth += 10;
        CurrentHealth = MaxHealth;
        MaxExp += 10;
        AttackDamage += 2;
        AttackSpeed += 0.1f;
        MovementSpeed += 0.2f;
    }

    private void OnPlayerDeath()
    {
        IsDead = true;
        playerAnimator.SetBool("dead", true);
        EventManager.StopListening("LevelUp", onLevelUp);
        EventManager.StopListening("DamagePlayer", onPlayerDamage);
        EventManager.StopListening("AddExperience", onPlayerExperience);
        PlayerUI.BlockInput();
        playerMovement.enabled = false;
        playerAttack.enabled = false;
        if (restoreHealthCoroutine != null) StopCoroutine(restoreHealthCoroutine);
    }

    private void OnDisable()
    {
        EventManager.StopListening("PlayerDeath", onPlayerDeath);
        EventManager.StopListening("LevelUp", onLevelUp);
        EventManager.StopListening("DamagePlayer", onPlayerDamage);
        EventManager.StopListening("AddExperience", onPlayerExperience);
    }
}


