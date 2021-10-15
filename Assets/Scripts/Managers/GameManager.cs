using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// This GameManager is a temporary class by Daniel. I'm using it now just as a place to store all the weapon shot decals.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance { get; private set; }

    public int m_MaxDecals = 35;

    public static WhiteWillow.Agent s_StartedAgent;

    public PariahController m_Pariah;

    public Achievements m_Achievements;

    //public List<Decal> m_allDecals = new List<Decal>();

    //private List<Decal> m_decalPool = new List<Decal>();

    //private List<GameObject> m_DecalSprite = new List<GameObject>();

    // Right now the decal system depends on having a prefab to represent the decal. This seems pretty weird to me but until I further discuss this with others I will
    // keep it like this. This m_DecalObject is that prefab, if it is not set in the GameManager, we wont try and instantiate them.
    //public GameObject m_DecalObject;




    // =================== NEW DECAL SYSTEM IN PROGRESS =================== //
    public DecalProjector m_ProjectorObject; // Template to instantiate the others.

    public List<Decal> m_BulletPool; // Decal now contains the projector.
    int m_BulletPoolRingBuffer = 0; // Tracks what we're up to in the bullet pool.

    // ================== Blood Spray Pool System ========================= //
    public GameObject m_BloodSprayPrefab;
    [Tooltip("The blood sprays are bullet pooled. This value will be how many bullet sprays are in the pool.")]
    public uint m_MaxBloodSprays = 10; // I think 10 is a solid number. If we have agents constantly getting shot really fast than this would have to increase.

    public List<BloodSpray> m_BloodSprayPool;
    private int m_BloodSprayPoolRingBuffer = 0;

    private MonoBehaviour m_Monobehaviour;

    public static HostController s_CurrentHost; // Tracks current host

    // To track whether the player is holding down the heal for regeneration statis pod button. // This applies to both pariah and the host controller so I though putting
    // the bool here would be more efficient rather than GetComponenting() the pariah/host controller on the fly.
    public bool IsHoldingHeal { get; set; }

    public Coroutine m_HealingRoutine;
    public bool m_HealingRoutineActive { get; set; }


    // Storing door information for when reloading at a checkpoint.
    public static List<Door> s_AllDoors = new List<Door>();
    public static bool s_IsNotFirstLoad = false;

    public static int s_HighestCheckPointLevel = 0;
    public static Vector3 s_CheckPointPos;


    private void Awake()
	{
        m_Monobehaviour = this;

        m_BulletPool = new List<Decal>();
        m_BloodSprayPool = new List<BloodSpray>();

        if (s_Instance == null)
            s_Instance = this;
        else if (s_Instance != null && s_Instance != this)
        {
            // We have placed multiple GameManager components in the scene.
            Debug.LogError("There are multiple GameManager components in the scene!");
            Destroy(this.gameObject);
        }
	}

	public void TogglePause(bool toggle)
    { }

    // Start is called before the first frame update
    void Start()
    {
        //s_Instance = this;

        if (!m_Pariah)
        {
            // If we haven't set the PariahController in the inspector, we'll attempt to get it by searching the scene.
            Debug.LogWarning("PariahController reference in GameManager has not been set. Attempting to set it by searching through the scene...");
            m_Pariah = GameObject.Find("Pariah").GetComponent<PariahController>();
            if (m_Pariah)
                Debug.LogWarning("Successfully found and set PariahController reference in GameManager.");
        }

        //if (m_DecalObject) // If the GameManager has no decal gameobject set, don't try create them.
        //{
        //    for (int i = 0; i < m_MaxDecals; i++)
        //    {
        //        m_DecalSprite.Add(GameObject.Instantiate(m_DecalObject));
        //        m_DecalSprite[i].transform.position = new Vector3(0, -100, 0); // Hiding the unused decals out of the way.

        //    }
        //    for (int i = 0; i < m_MaxDecals; i++)
        //    {
        //        m_decalPool.Add(new Decal(m_DecalSprite[i]));
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < m_MaxDecals; i++)
        //    {
        //        m_decalPool.Add(new Decal());
        //    }
        //}

        // Initialise the unused pool of decals.


        // ====================== NEW DECAL SYSTEM ====================== //
        for (int i = 0; i < m_MaxDecals - 1; i++) // minus one for now since I'm leaving the first decal projector in the scene. This will have to change if we
        {                                         // load it from resources instead.
            // Instantiate the decals.                                                              // The comment above me is incorrect. You can load prefabs in without them being
            Decal newDecal = new Decal(m_ProjectorObject.gameObject);                               // in the scene.
            m_BulletPool.Add(newDecal);
        }


        // Initializing all the bullet spray objects.
        for (int i = 0; i < m_MaxBloodSprays; i++)
        {
            BloodSpray newSpray = new BloodSpray(m_BloodSprayPrefab);
            m_BloodSprayPool.Add(newSpray);
        }

        // After a scene reload, we refresh the doors.
        if(s_IsNotFirstLoad)
            RefreshDoors();

        if (s_HighestCheckPointLevel > 0)
        {
            // If the player has reached a check point, spawn them there the next time they die.
            SpawnAtCheckPoint();
        }

    }

    // Update is called once per frame
    void Update()
    {
        UpdateBloodSprays();
    }

    public void PlaceDecal(Transform obj, Vector3 hitPoint, Vector3 normal)
    {
        //m_decalPool[0].SetDecal(obj, hitPoint, normal);

        //Decal m_oldFirst = m_decalPool[0];
        //m_decalPool.RemoveAt(0); // Removing the first one.
        //m_decalPool.Add(m_oldFirst); // Adding it back so it's at the back of the list.

        if (m_BulletPoolRingBuffer == m_MaxDecals - 1) // We have exceeded the amount of decals in the world!
            m_BulletPoolRingBuffer = 0; // Set it back to the start.

        m_BulletPool[m_BulletPoolRingBuffer].SetDecal(obj, hitPoint, normal);
        m_BulletPoolRingBuffer++;
        
    }

    public void AddDecal(Transform obj, Vector3 hitPoint, Vector3 normal, GameObject decal)
    {
        //m_decalPool[0].SetDecal(obj, hitPoint, normal, decal);

        //Decal m_oldFirst = m_decalPool[0];
        //m_decalPool.RemoveAt(0); // Removing the first one.
        //m_decalPool.Add(m_oldFirst); // Adding it back so it's at the back of the list.
    }

    public void PopDecal()
    { }
	private void OnDrawGizmos()
	{
        //for (int i = 0; i < m_decalPool.Count; i++)
        //{
        //    m_decalPool[i].DrawGizmo();
        //}
    }

    public static void SetStartAgent(WhiteWillow.Agent startingAgent)
    {
        if (s_StartedAgent == null)
        {
            s_StartedAgent = startingAgent;
            GameManager instance = GameManager.s_Instance;
            // We can do it because there have been no other agents set to be started in.
            if(s_HighestCheckPointLevel == 0) // We'll only ever start in an agent if we are starting a fresh world. If we've hit a check point we wont do this.
                instance.m_Pariah.ForceInstantPossess(s_StartedAgent);
        }
        else
        {
            // If we hit this, that means someone has set more than 1 agent to be started in.
            Debug.LogError("You have set multiple agents to be the starting controller!");
        }
    }

    public void PlaceBulletSpray(Vector3 pos, Transform parent, Vector3 facing)
    {
        m_BloodSprayPool[m_BloodSprayPoolRingBuffer].SetSpray(parent, pos, facing);

        // Play the effect.
        //m_BloodSprayPool[m_BloodSprayPoolRingBuffer].Play();

        IncrementSprayRingBuffer(); // Incrementing the ring buffer so it is prepared for the next PlaceBulletSpray() call.
    }
    private void IncrementSprayRingBuffer()
    {
        if (m_BloodSprayPoolRingBuffer >= m_BloodSprayPool.Count - 1)
        {
            m_BloodSprayPoolRingBuffer = 0;
        }
        else
            m_BloodSprayPoolRingBuffer++;
    }
    private void UpdateBloodSprays()
    {
        for (int i = 0; i < m_BloodSprayPool.Count; i++)
        {
            m_BloodSprayPool[i].Update();
        }
    }

    public void OnEnterEnemy(EnemyTypes type, Animators controller, Weapon weapon)
    {
        m_Achievements.FirstTimeEnemy(type, controller, this, weapon); // The only weapon with multiple arm/gun animators is the dual wield which will
                                                                   // never be used as a first-time equip cinematic thing. So thats why we get the
                                                                   // 0th element of the animators.

        //WStartCoroutine(controller.RunWeaponInspect(5));
    }

    public static void AddDoor(int arenaID, GameObject openDoor, GameObject closeDoor, bool isOpen)
    {
        // To prevent the same monobehaviour ArenaManager's from sending the GameManager their doors on the following reloads of the game, we check
        // the ID of the requested created door with the doors we already have. If they match, it means we already know about that door and don't need it.
        for (int i = 0; i < s_AllDoors.Count; i++)
        {
            if (arenaID == s_AllDoors[i].ID)
            {
                // Although we already have this door, we still need to re-grab the game objects. This is because on a scene reload the old game object
                // references become null.
                s_AllDoors[i].m_openDoorObj = openDoor;
                s_AllDoors[i].m_closedDoorObj = closeDoor;

                return; // Early out, we already have this door!
            }
        }

        // Otherwise, this means that it's the first time we're receiving these doors (The first time the scene is loaded.)

        Door newDoor = new Door(arenaID, openDoor, closeDoor, isOpen);

        s_AllDoors.Add(newDoor);
    }

    /// <summary>
    /// After reloading from a checkpoint, call this function to set doors to their proper open/close state.
    /// </summary>
    public static void RefreshDoors() 
    {
        for (int i = 0; i < s_AllDoors.Count; i++)
        {
            s_AllDoors[i].Toggle(s_AllDoors[i].m_IsOpen);
        }
    }

    /// <summary>
    /// Gets a door with a matching arena ID.
    /// </summary>
    /// <param name="arenaID"></param>
    public static Door GetDoor(int arenaID)
    {
        for (int i = 0; i < s_AllDoors.Count; i++)
        {
            if (s_AllDoors[i].ID == arenaID)
            {
                return s_AllDoors[i];
            }
        }

        return null;
    }

    public static void SetCheckPoint(int checkPointLevel, Vector3 pos)
    {
        if (s_HighestCheckPointLevel == 0)
        {
            // If our highest check point is level 0, we'll take any we can get.
            s_HighestCheckPointLevel = checkPointLevel;
            s_CheckPointPos = pos;
        }
        else if (checkPointLevel > s_HighestCheckPointLevel)
        {
            // We've reached a higher check point, so we'll set ours to that one.
            s_HighestCheckPointLevel = checkPointLevel;
            s_CheckPointPos = pos;
        }
    }

    public void SpawnAtCheckPoint()
    {
        // If we spawn at a check point, we wont spawn into an agent immediately.
        m_Pariah.transform.position = s_CheckPointPos;
    }

}
