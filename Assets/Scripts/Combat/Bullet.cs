using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    [ReadOnly]
    private GameObject m_Target;

    private int m_Damage = 0;
    private Rigidbody m_Rigidbody;

    private void Awake() => m_Rigidbody = GetComponent<Rigidbody>();
  
	// The owner of the bullet is a reference to the last agent that shot it. It's
	// used to prevent the bullet from colliding with its own agent.
	public Transform m_Owner;

    public void SetTarget(GameObject target, int damage)
    {
        m_Target = target;
        m_Damage = damage;
    }

    public void Fire(Vector3 origin, Vector3 destination, Vector3 forward, Transform owner)
    {
        m_Owner = owner;
        transform.position = origin;
        transform.forward = (destination - origin).normalized;
        gameObject.SetActive(true);
        m_Rigidbody.AddForce((destination - origin).normalized * 60.0f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform != m_Owner && collision.gameObject.layer != 2 && collision.gameObject.layer != 13) // Prevents collision with it's own agent and things on the ignore raycast layer.
        { 
            if (m_Target != null)
            {
                if (collision.gameObject == m_Target)
                    m_Target.GetComponent<Inventory>()?.TakeDamage(m_Damage);


                // Can't get contact points when using triggers. That's okay because enemies don't need bullet hit markers.
                //GameManager.s_Instance?.PlaceDecal(collision.transform, collision.GetContact(0).point, collision.GetContact(0).normal);
            }

            gameObject.SetActive(false);

            // Important, if we don't clear the velocity the bullets may stray the next time they are fired!
            m_Rigidbody.velocity = Vector3.zero;
        }
    }
}
