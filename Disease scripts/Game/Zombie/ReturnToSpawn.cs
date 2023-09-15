using System.Text;
using UnityEngine;
using UnityEngine.AI;

public class ReturnToSpawn : GAction
{
    public override string ActionName { get => "Return to spawn"; }
    public override string ActionType { get => "Static movement"; }
    public override string TargetTag { get => "Spawn point"; }
    public override NavMeshAgent Agent { get; protected set; }

    private Transform player;

    private new void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        MinRange = 2f;
        PreConditionsVisual = SetPreconditions();
        AfterEffectsVisual = SetAfterEffects();

        base.Awake();
    }

    private void Start()
    {
        player = GameObject.Find("/Player").transform;
        Target = GameObject.Find("/SpawnPoints/" + GetTargetSpawnName());
    }

    //Set Preconditions for this action
    private WorldState[] SetPreconditions()
    {
        WorldState[] preConditions =
        {
            new WorldState("lost", 1)
        };

        return preConditions;
    }

    //Set Effects for this action
    private WorldState[] SetAfterEffects()
    {
        WorldState[] afterEffects =
        {
            new WorldState("on spawn", 1)
        };

        return afterEffects;
    }

    //Do before action
    public override bool PrePerform()
    {
        Agent.SetDestination(Target.transform.position);
        zombieAnimator.SetBool("move", true);
        return true;
    }

    //Do after action
    public override bool PostPerform()
    {
        beliefs.RemoveState("lost");
        zombieAnimator.SetBool("move", false);
        return true;
    }
    
    //Perform action
    public override bool Func()
    {
        float distToTarget = Vector3.Distance(transform.position, player.position);
        if (distToTarget >= 12f) { return true; }
        beliefs.RemoveState("lost");
        return false;
    }

    //Conditions for action success
    public override bool Success()
    {
        float distToTarget = Vector3.Distance(transform.position, Target.transform.position);
        if (distToTarget < MinRange) { return true; }
        return false;
    }

    //Find this zombie spawn object
    private string GetTargetSpawnName()
    {
        StringBuilder targetName = new StringBuilder(transform.name);
        targetName.Remove(0, 7);
        targetName.Insert(0, "SpawnPoint ");
        return targetName.ToString();
    }
}
