using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private float m_RotationSpeed = 60f;

    private void OnTriggerEnter(Collider other) // potential capsule collider rather than a sphere collider.
    {
        if (other.GetComponent<PlayerInput>().enabled == true && other.GetComponent<HostController>())
        {
            //Do the things then destroy or set inactive.
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, m_RotationSpeed * Time.deltaTime);
    }
}
