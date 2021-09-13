using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This GameManager is a temporary class by Daniel. I'm using it now just as a place to store all the weapon shot decals.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int m_MaxDecals = 35;

    public List<Decal> m_allDecals = new List<Decal>();

    private List<Decal> m_decalPool = new List<Decal>();

    private List<GameObject> m_DecalSprite = new List<GameObject>();

    public GameObject m_DecalObject;

    public void TogglePause(bool toggle)
    { }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        // super temporary decal system. todo completely rework it.

        if (m_DecalObject)
        {
            for (int i = 0; i < m_MaxDecals; i++)
            {
                m_DecalSprite.Add(GameObject.Instantiate(m_DecalObject));
                m_DecalSprite[i].transform.position = new Vector3(-1000, -1000, -1000); // temporary!!

            }
            for (int i = 0; i < m_MaxDecals; i++)
            {
                m_decalPool.Add(new Decal(m_DecalSprite[i]));
            }
        }
        else
        {
            for (int i = 0; i < m_MaxDecals; i++)
            {
                m_decalPool.Add(new Decal());
            }
        }

        // Initialise the unused pool of decals.

    }

    // Update is called once per frame
    void Update()
    {
        UpdateDecals();
    }

    public void AddDecal(Transform obj, Vector3 hitPoint, Vector3 normal)
    {
        m_decalPool[0].SetDecal(obj, hitPoint, normal);

        Decal m_oldFirst = m_decalPool[0];
        m_decalPool.RemoveAt(0); // Removing the first one.
        m_decalPool.Add(m_oldFirst); // Adding it back so it's at the back of the list.
    }

    public void AddDecal(Transform obj, Vector3 hitPoint, Vector3 normal, GameObject decal)
    {
        m_decalPool[0].SetDecal(obj, hitPoint, normal, decal);

        Decal m_oldFirst = m_decalPool[0];
        m_decalPool.RemoveAt(0); // Removing the first one.
        m_decalPool.Add(m_oldFirst); // Adding it back so it's at the back of the list.
    }

    public void PopDecal()
    { }

    private void UpdateDecals()
    {
        if (m_decalPool.Count > 0)
        {
            for (int i = 0; i < m_decalPool.Count; i++)
            {
                m_decalPool[i].Update();
            }
        }
    }

	private void OnDrawGizmos()
	{
        //for (int i = 0; i < m_decalPool.Count; i++)
        //{
        //    m_decalPool[i].DrawGizmo();
        //}
    }
}
