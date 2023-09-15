using UnityEngine;

public class SwordHit : MonoBehaviour
{
    private Attack playerAttack;

    private void OnDisable()
    {
        GetComponent<MeshCollider>().isTrigger = false;
    }

    private void OnEnable()
    {
        GetComponent<MeshCollider>().isTrigger = true;
    }

    private void Start()
    {
        playerAttack = GameObject.Find("/Player").GetComponent<Attack>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Mobs_zombie"))
        {
            playerAttack.Hit(collider);
            Debug.Log(collider.name);
        }
    }


}
