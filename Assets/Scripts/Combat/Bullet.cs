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

    private MeshRenderer m_Mesh;
    private Collider m_Collider;

    [HideInInspector]
    public bool m_Dirty = true;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();
        m_Mesh = transform.GetChild(0).GetComponent<MeshRenderer>(); // Mesh is on the first child... 
    }
  
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
        //gameObject.SetActive(true);

        // We aren't setting things to active/inactive anymore. Instead we hide the mesh, collider, ect. Also we need to clean the bullets dirty state after we shoot it.
        //m_Dirty = false;
        Unhide();
        m_Rigidbody.AddForce((destination - origin).normalized * 60.0f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform != m_Owner && collision.gameObject.layer != 2 && collision.gameObject.layer != 13) // Prevents collision with it's own agent and things on the ignore raycast layer.
        { 
            if (m_Target != null)
            {
                if (collision.gameObject == m_Target)
                {
                    Inventory agentInv;
                    m_Target.TryGetComponent<Inventory>(out agentInv);
                    agentInv?.TakeDamage(m_Damage);
                    if (agentInv.Owner.Possessed)
                    {
                        agentInv.m_CurrentWeapon.AddDamageCameraShake();
                    }
                }


                // Can't get contact points when using triggers. That's okay because enemies don't need bullet hit markers.
                //GameManager.s_Instance?.PlaceDecal(collision.transform, collision.GetContact(0).point, collision.GetContact(0).normal);
            }

            //gameObject.SetActive(false);

            // We aren't setting the object to active/inactive anymore. We also have to set it's dirty state to true after it has been "Destroyed".
            // This is so the bullet pooler knows that it can use this specific bullet again.
            m_Rigidbody.velocity = Vector3.zero;
            //m_Dirty = true;
            Hide();
            

            // Important, if we don't clear the velocity the bullets may stray the next time they are fired!
        }
    }

    /// <summary>
    /// Because SetActive() is laggy in the editor, instead of setting the gameobject to false, we will hide the mesh, rigidbody and collider.
    /// This should virtuall be the same thing but more performant.
    /// </summary>
    public void Hide()
    {
        m_Dirty = true;
        m_Collider.enabled = false;
        m_Rigidbody.isKinematic = true;
        m_Mesh.enabled = false;
    }

    public void Unhide()
    {
        m_Dirty = false;
        m_Collider.enabled = true;
        m_Rigidbody.isKinematic = false;
        m_Mesh.enabled = true;
        m_Rigidbody.velocity = Vector3.zero;
    }
}
