using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    [SerializeField] private Attack playerAttack;

    private void Start()
    {
        if (playerAttack == null) playerAttack = GameObject.Find("/Player").GetComponent<Attack>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        playerAttack.EnemyHit(collision);
    }
}
