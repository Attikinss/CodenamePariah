using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public Transform m_Player;
    public GameObject m_Target;
    private float m_Distance;
    // Update is called once per frame

    private void Start()
    {
        if (m_Target != null)
        {
            m_Distance = this.gameObject.transform.position.y - m_Target.transform.position.y;
        }
        else
        {
            m_Distance = 0;
        }
    }

    void Update()
    {
        this.transform.LookAt(m_Player);


        //possibly fixed update or late update
        if (m_Target != null)
        {
            this.gameObject.transform.position = new Vector3(m_Target.transform.position.x, m_Target.transform.position.y + m_Distance, m_Target.transform.position.z);
        }
    }
}
