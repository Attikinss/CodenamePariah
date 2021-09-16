using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAgentTrigger : MonoBehaviour
{
    public PariahController m_Pariah;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
        Inventory human;
        PariahController pariah;
        if (other.TryGetComponent<PariahController>(out pariah))
        {
            // Thing that entered was Pariah.
            pariah.TakeDamage(1000); // Destroys Pariah.
        }
        else if (other.TryGetComponent<Inventory>(out human))
        {
            // Thing that entered was either an agent or a player controlling an agent since it has an inventory.
            human.TakeDamage(1000);
            m_Pariah.TakeDamage(1000);
        }
	}
}
