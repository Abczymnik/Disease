using UnityEngine;
using UnityEngine.AI;

public class SpecialZombie : MonoBehaviour
{
    private Animator zombieAnimator;
    private NavMeshAgent zombieAgent;

    private void Awake()
    {
        zombieAnimator = GetComponent<Animator>();
        zombieAgent = GetComponent<NavMeshAgent>();
    }

    //Cinematic zombie triggered by "Nowa gra" button
    public void LetZombieFree()
    {
        zombieAnimator.SetBool("move", true);
        zombieAnimator.SetFloat("distanceToPlayer", 6f);
        zombieAgent.SetDestination(new Vector3(32, 2, 0));
    }
}
