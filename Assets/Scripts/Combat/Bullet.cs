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

    public void SetTarget(GameObject target, int damage)
    {
        m_Target = target;
        m_Damage = damage;
    }

    public void Fire(Vector3 origin, Vector3 destination)
    {
        transform.position = origin;
        gameObject.SetActive(true);
        m_Rigidbody.AddForce((destination - origin).normalized * 60.0f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_Target != null)
        {
            if (collision.gameObject == m_Target)
                m_Target.GetComponent<Inventory>()?.TakeDamage(m_Damage);

            GameManager.s_Instance?.PlaceDecal(collision.transform, collision.GetContact(0).point, collision.GetContact(0).normal);
        }

        gameObject.SetActive(false);
    }
}
