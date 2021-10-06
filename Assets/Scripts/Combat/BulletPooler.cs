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
            newBullet.gameObject.SetActive(false);
            m_Pool.Add(newBullet);
        }
    }

    public Bullet GetNextAvailable()
    {
        Bullet bullet = m_Pool.FirstOrDefault(b => !b.gameObject.activeSelf);

        if (bullet == null)
            Debug.LogWarning("No bullets are available! Increase the bullet pool size.");

        return bullet;
    }
}
