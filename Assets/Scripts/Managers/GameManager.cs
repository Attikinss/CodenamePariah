using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public List<Decal> m_allDecals = new List<Decal>();

    public void TogglePause(bool toggle)
    { }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDecals();
    }

    public void AddDecal(Decal decal)
    {
        m_allDecals.Add(decal);
    }

    public void PopDecal()
    { }

    private void UpdateDecals()
    {
        if (m_allDecals.Count > 0)
        {
            for (int i = 0; i < m_allDecals.Count; i++)
            {
                m_allDecals[i].Update();
            }
        }
    }

	private void OnDrawGizmos()
	{
        for (int i = 0; i < m_allDecals.Count; i++)
        {
            m_allDecals[i].DrawGizmo();
        }
    }
}
