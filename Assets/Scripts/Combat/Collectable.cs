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

    [SerializeField]
    private float m_RotationSpeed = 60f;

    [SerializeField]
    private float m_BobHeight = 0.125f;

    [SerializeField]
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

                Activate(inv);
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

    private void Activate(Inventory inv)
    {
        switch (m_Action)
        {
            case CollectableAction.PICKUP:
                inv.AddWeapon(m_ItemPrefab);
                break;
            case CollectableAction.UPGRADE_WEAPON:
                inv.UpgradeWeapon(inv.GetWeaponNum(), m_ItemPrefab);
                break;

        }
    }
}
