using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletPooler : MonoBehaviour
{
    [SerializeField]
    private GameObject m_BulletPrefab;

    [SerializeField]
    private int m_MaxBulletCount = 100;

    [ReadOnly]
    [SerializeField]
    private List<Bullet> m_Pool;

    public static BulletPooler Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogWarning("An instance of the BulletPooler already exists in the scene! New instance destroyed.");
            Destroy(this);
        }
    }

    private void Start()
    {
        m_Pool = new List<Bullet>(m_MaxBulletCount);

        for (int i = 0; i < m_MaxBulletCount; i++)
        {
            Bullet newBullet = Instantiate(m_BulletPrefab, transform).GetComponent<Bullet>();

            newBullet.Hide(); // Hiding bullets when they spawn in so we don't see them.

            //newBullet.gameObject.SetActive(false); // Commented this out because we don't want to be setting bullets active/inactive.
            m_Pool.Add(newBullet);
        }
    }

    public Bullet GetNextAvailable()
    {
        Bullet bullet = m_Pool.FirstOrDefault(/*b => b.gameObject.activeSelf*/ b => b.m_Dirty); // Because I've changed bullets to always being active, we have to grab the first
                                                                                                // dirty bullet instead of the first inactive bullet.
        if (bullet == null)
            Debug.LogWarning("No bullets are available! Increase the bullet pool size.");

        return bullet;
    }
}
