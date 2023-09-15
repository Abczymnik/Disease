using UnityEngine;
using UnityEngine.InputSystem;

public class LanternCollider : MonoBehaviour
{
    private Rigidbody rigbody;
    private CursorFollower lanternHandle;

    private void Awake()
    {
        rigbody = GetComponent<Rigidbody>();
        rigbody.isKinematic = true;
    }

    private void Start()
    {
        lanternHandle = GameObject.Find("/Cursor Drag Point").GetComponent<CursorFollower>();
        Cursor.lockState = CursorLockMode.Confined;
        rigbody.isKinematic = false;
        rigbody.constraints = RigidbodyConstraints.FreezePositionY;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contactCount == 0) return;
        Vector3 collisionNormal = collision.GetContact(0).normal;
        PushCursorFromCollider(collisionNormal);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.contactCount == 0) return;
        Vector3 collisionNormal = collision.GetContact(0).normal;
        PushCursorFromCollider(collisionNormal);
    }

    //Bounce back after collision in collision.normal direction
    private void PushCursorFromCollider(Vector3 collisionNormal)
    {
        int cursorXAxisOffset = (int)Mathf.Ceil(collisionNormal.x * rigbody.velocity.magnitude * 5);
        int cursorYAxisOffset = (int)Mathf.Ceil(collisionNormal.z * rigbody.velocity.magnitude * 5);
        Vector2 cursorOffset = new Vector2(cursorXAxisOffset, cursorYAxisOffset);
        lanternHandle.LastCursorPosition += cursorOffset; 
        Mouse.current.WarpCursorPosition(lanternHandle.LastCursorPosition);
    }
}
