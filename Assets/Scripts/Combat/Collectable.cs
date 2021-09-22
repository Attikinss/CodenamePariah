using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Collectable : MonoBehaviour
{
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
}
