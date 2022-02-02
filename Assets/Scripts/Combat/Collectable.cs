using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public enum CollectableAction
{ 
    PICKUP,
    UPGRADE_WEAPON
}

public class Collectable : MonoBehaviour
{
    public CollectableAction m_Action;

    public GameObject m_ItemPrefab;

    public WEAPONTYPE m_RequiredTarget;

    [SerializeField]
    [Tooltip("Speed at which the collectable rotates at.")]
    private float m_RotationSpeed = 60f;

    [SerializeField]
    [Tooltip("Distance in height of which the collectable will bob above and below starting position.")]
    private float m_BobHeight = 0.125f;

    [SerializeField]
    [Tooltip("Speed at which the collectable will bob.")]
    private float m_BobSpeed = 2f;

    private Vector3 m_StartPosition;

    private void Start()
    {
        m_StartPosition = transform.position;   
    }

    private void OnTriggerEnter(Collider other) // potential capsule collider rather than a sphere collider.
    {
        if (other.TryGetComponent(out WhiteWillow.Agent agent))
        {
            if (agent.Possessed)
            {
                //Do the things then destroy or set inactive.
                Inventory inv = agent.GetComponent<Inventory>();
                if (!inv)
                    Debug.LogError("You walked over a pickup but you are missing an Inventory.cs script!");

                if(Activate(inv))
                    Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, m_RotationSpeed * Time.deltaTime);

        Vector3 currentPosition = transform.position;
        currentPosition.y = m_StartPosition.y + Mathf.Sin(Time.time * m_BobSpeed) * m_BobHeight;
        transform.position = currentPosition;
    }

    private bool Activate(Inventory inv)
    {
        switch (m_Action)
        {
            case CollectableAction.PICKUP:
                return inv.AddWeapon(m_ItemPrefab);
            case CollectableAction.UPGRADE_WEAPON:
                return inv.UpgradeWeapon(inv.GetWeaponNum(), m_ItemPrefab, m_RequiredTarget);
            default:
                return false;
        }
    }
}
