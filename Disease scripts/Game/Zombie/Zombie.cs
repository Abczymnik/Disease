using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private ZHealthBar healthBar;
    private Animator zombieAnimator;
    private PlayerStats player;
    private Inventory playerInventory;

    private float _maxHealth = 100f;
    private float _currentHealth = 100f;
    private float experiencePoints = 40f;

    public float DropChance { get; private set; } = 1f;
    public float AttackDamage { get; private set; } = 10;

    public bool IsDead { get; private set; }

    public float MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value;
            healthBar.MaxHealth = value;
        }
    }

    public float CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            if (value <= 0)
            {
                _currentHealth = 0;
                healthBar.CurrentHealth = _currentHealth;
                Die();
                return;
            }
            _currentHealth = value;
            healthBar.CurrentHealth = value;
        }
    }

    private void Awake()
    {
        zombieAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.Find("/Player");
        player = playerObj.GetComponent<PlayerStats>();
        playerInventory = playerObj.transform.GetChild(3).GetComponent<Inventory>();
        healthBar = transform.GetChild(2).GetChild(0).GetComponent<ZHealthBar>();
    }

    public void TakeDmg(float hit)
    {
        CurrentHealth -= hit;
    }    

    //Enable Note object from Zombie pocket and increment notes available for player
    private void DropItem(float chance)
    {
        float drop = Random.Range(0, 1f);
        if (drop > chance) return;
        GameObject note = transform.GetChild(0).GetChild(0).gameObject; //find note object
        playerInventory.NotesOnMap++;
        note.SetActive(true);
    }



    //Perform zombie death
    private void Die()
    {
        IsDead = true;
        player.GiveExp(experiencePoints);
        zombieAnimator.SetInteger("deadType", Random.Range(0, 2)); //forward fall or back fall
        zombieAnimator.SetBool("dead", true);
        zombieAnimator.SetBool("attack", false);
        GetComponent<Collider>().enabled = false;
        GetComponent<RootMotion>().enabled = false;
        GetComponent<AttackPlayer>().enabled = false;
        GetComponent<ReturnToSpawn>().enabled = false;
        GetComponent<CatchPlayer>().enabled = false;
        GetComponent<LookForPlayer>().enabled = false;
        GetComponent<ZombieGoals>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        transform.GetChild(2).gameObject.SetActive(false); //hp bar off
        DropItem(DropChance);
    }
}
