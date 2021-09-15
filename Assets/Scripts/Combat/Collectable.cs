using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Collectable : MonoBehaviour
{
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
