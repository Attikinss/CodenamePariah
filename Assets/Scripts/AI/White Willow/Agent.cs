using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace WhiteWillow
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(HostController))]
    public class Agent : MonoBehaviour
    {
        public BehaviourTree InputTree;
        public GameObject Target;

        // TODO: Add to sensory data class/struct
        [Min(0.0f)]
        public float m_ViewRange = 15.0f;
        [Min(0.0f)]
        public float m_FiringRange = 15.0f;

        [Tooltip("Defines what the agent can't see through.")]
        [SerializeField]
        private LayerMask m_IgnoreMask;

        private BehaviourTree m_RuntimeTree;
        private NavMeshAgent m_NavAgent;
        public Query CurrentQuery;
        public EnvironmentQuerySystem.EQSNode CurrentNode { get; set; }
        public Weapon Weapon { get => m_HostController.m_Inventory.m_CurrentWeapon; }
        [Tooltip("The mesh that we apply the possession shader to.")]
        public GameObject m_Mesh;
        public Material m_PossessionMaterial;
        public ParticleSystem m_PossessionHeart;
        public ParticleSystem m_PossessionPulse;

        [SerializeField]
        private Animator m_Animator;

        [HideInInspector]
        public PariahController PariahController;
        [HideInInspector]
        public HostController m_HostController;

        public bool Alive { get; private set; } = true;
        public bool Possessed { get; private set; } = false;
        public bool PossessedPreviously { get; private set; }
        public bool EngagingTarget { get; private set; } = true;
        public Transform Orientation { get => m_HostController.m_Orientation; }
        public Vector3 Destination { get; private set; }
        public Vector3 FacingDirection { get; private set; }

        // These references here are used to handle "selecting" the agent with the possession shader.
        // We have to store it's original material and also have a reference to its mesh renderer so we can apply a new material.
        private Material m_OriginalMat;
        private SkinnedMeshRenderer m_CurrentMat;
        private MeshRenderer m_CurrentMatNoRenderer; // Because the soldiers don't have an animation yet, we have the temporary case of needing to
                                                     // use a normal MeshRenderer instead.

        // We have to move the firing position into the agent script instead of the Weapon script because
        // when it's in the weapon script it has to be attached to the weapon itself. The firing position in the
        // scene hirearchy is not a child of the weapon so in order to have an easier time with prefabs,
        // I've decided to move the firing position here.
        [Tooltip("Defines the position at which bullets spawn during firing.")]
        public Transform m_FiringPosition;

        // Another thing we have to duplicate for agents specifically is the muzzle flash. For the player
        // it is positioned on the FPS gun however we need to position it on the agents rig to make it 
        // look right for the AI. The solution I have come up with is to just have another muzzle flash for the
        // agents.
        [Tooltip("Muzzle flash for the AI.")]
        public VisualEffect m_AIMuzzleFlash;

        private Vector3 m_CachedAgentVelocity = Vector3.zero;

        private void Awake()
		{
            m_RuntimeTree = InputTree?.Clone(gameObject.name);
            m_RuntimeTree?.SetAgent(this);

            m_NavAgent = GetComponent<NavMeshAgent>();
            m_HostController = GetComponent<HostController>();

            PariahController = FindObjectOfType<PariahController>();

            if (PariahController)
            {
                GameObject target = PariahController.CurrentHost != null ?
                    PariahController.CurrentHost.gameObject : PariahController.gameObject;

                m_RuntimeTree.Blackboard?.UpdateEntryValue<GameObject>("Target", target);
            }

            CacheOriginalMaterial();
        }
        private void Update()
        {

            if (Possessed)
            {
                if (!PauseMenu.m_GameIsPaused)
                    FacingDirection = m_HostController.m_Orientation.rotation * m_HostController.m_Orientation.forward;
            }
            else
            {
                // Pause and resume AI behaviours
                if (PauseMenu.m_GameIsPaused)
                {
                    if (!m_NavAgent.isStopped)
                    {
                        m_NavAgent.isStopped = true;
                        m_CachedAgentVelocity = m_NavAgent.velocity;
                        m_NavAgent.velocity = Vector3.zero;
                        m_Animator.speed = 0.0f;
                    }

                    return;
                }
                else
                {
                    m_Animator.speed = 1.0f;

                    if (m_NavAgent.isStopped)
                    {
                        m_NavAgent.velocity = m_CachedAgentVelocity;
                        m_NavAgent.isStopped = false;
                    }
                }

                if (!Alive)
                    return;

                m_RuntimeTree?.Tick();

                if (MovingToPosition())
                {
                    Vector3 faceDirection = m_NavAgent.velocity;
                        
                    if (faceDirection != Vector3.zero)
                    {
                        faceDirection.y = 0.0f;
                        m_HostController.m_Orientation.rotation = Quaternion.Lerp(m_HostController.m_Orientation.rotation,
                            Quaternion.LookRotation(faceDirection.normalized, Vector3.up), 0.5f);
                        FacingDirection = m_HostController.m_Orientation.eulerAngles;
                    }
                }
            }
        }

        private void OnApplicationQuit()
        {
            EnvironmentQuerySystem.Shutdown();
        }

        public void MoveToPosition()
        {
            if (m_NavAgent.isStopped)
                m_NavAgent.isStopped = false;

            PlayAnimation("Run", true);

            if (Vector3.Distance(m_NavAgent.destination, Destination)
                >= Mathf.Epsilon + m_NavAgent.stoppingDistance)
            {
                m_NavAgent.SetDestination(Destination);
                m_NavAgent.isStopped = false;
            }
        }

        public bool SetDestination(Vector3 destination)
        {
            Destination = destination;
            return true;
        }

        public void Stop(bool playIdleAnim = true)
        {
            if (playIdleAnim)
                PlayAnimation("IdleCombat", true, 0.1f);

            m_NavAgent.velocity = Vector3.zero;
            m_NavAgent.ResetPath();
            m_NavAgent.isStopped = true;
        }

        public bool DestinationAttainable(Vector3 position)
        {
            NavMeshPath path = new NavMeshPath();
            m_NavAgent.CalculatePath(position, path);
            return path.status == NavMeshPathStatus.PathComplete;
        }

        public bool MovingToPosition() => m_NavAgent.hasPath;
        public bool AtPosition() => Vector3.Distance(transform.position - Vector3.up, Destination) < 0.2f + m_NavAgent.stoppingDistance;
        public bool Stuck() => false;

        public void Possess()
        {
            Possessed = true;
            PossessedPreviously = true;
            m_NavAgent.ResetPath();
            m_NavAgent.enabled = false;

            m_RuntimeTree.Blackboard?.UpdateEntryValue<GameObject>("Target", this.gameObject);

            PariahController?.Disable();
            m_HostController?.Enable();
        }

        public void Release()
        {
            Possessed = false;
            m_NavAgent.enabled = true;

            // The issue of pariah not being orientated the same way as the soldier or scientist on release comes from if the soldier/scientist parent prefab
            // is rotated. The parent prefab is the one that has the mesh.
            // The problem is solved in the FixedUpdate() in PariahController.cs.
       
            m_HostController?.Disable();
            PariahController?.Enable();
            m_RuntimeTree.Blackboard?.UpdateEntryValue<GameObject>("Target", PariahController.gameObject);
        }

        public void Kill()
        {
            if (!Alive)
                return;

            m_HostController.GetCurrentWeapon().StopSounds(); // Stops sounds like reload from playing after the agent has been destroyed.

            if (Possessed)
            { 
                Release();
                // Also apply extra damage to Pariah's life essence.
                PariahController?.TakeDamage(m_HostController.GetOnDestroyDamage());
            }

            Alive = false;
            PlayAnimation("Death", true, 0.1f);
            StartCoroutine(DelayExecuteFunc(3.5f, () =>
            {
                GetComponent<Collider>().enabled = false;
                Destroy(gameObject);
            }));
        }

        public void LookAt(Vector3 position)
        {
            Vector3 bodyTargetDir = position - transform.position;
            Vector3 camTargetDir = m_HostController.Camera.transform.position;
            camTargetDir.y = bodyTargetDir.y;
            bodyTargetDir.y = 0.0f;

            Orientation.rotation = Quaternion.Lerp(Orientation.rotation,
                Quaternion.LookRotation(bodyTargetDir), Time.deltaTime * 7.5f);

            // Find the proper way to rotate camera to look at player        
            //m_HostController.Camera.transform.localRotation = Quaternion.Lerp(m_HostController.Camera.transform.localRotation,
            //    Quaternion.LookRotation(camTargetDir), Time.deltaTime * 7.5f);

            FacingDirection = Orientation.eulerAngles;
        }

        public void LookTowards(Vector3 direction)
        {
            direction.y = 0.0f;
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * m_NavAgent.angularSpeed * 0.1f);
        }

        public bool ShootAt(GameObject target, bool forceMiss = false)
        {
            if (PariahController.Transitioning)
                return false;

            m_HostController.GetCurrentWeapon()?.FireAt(target);
            return true;
        }

        public void PlayAnimation(string name, bool forcePlay = false, float transitionSpeed = 0.25f)
        {
            if (m_Animator != null)
            {
                var animInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

                if (!animInfo.IsName(name) || forcePlay)
                    m_Animator.CrossFade(name, transitionSpeed, 0, 0.0f);
            }
        }

        public bool TargetWithinRange(GameObject target, float distance = 0.0f)
        {
            if (Mathf.Approximately(distance, 0.0f))
                distance = m_ViewRange;

            return (target.transform.position - transform.position).sqrMagnitude < distance * distance;
        }

        public bool TargetWithinViewRange(GameObject target, float angle)
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0.0f;
            float currentAngle = Vector3.Angle(direction, Orientation.forward);

            // TODO: Add agent variable/data structure for handling sensory values
            return currentAngle >= -angle && currentAngle <= angle;
        }

        public bool TargetVisible(GameObject target)
        {
            // Do a simple raycast to target
            if (Physics.Raycast(transform.position, (target.transform.position
                - transform.position).normalized, out RaycastHit hitInfo, m_ViewRange, m_IgnoreMask))
            {
                //Debug.Log($"{hitInfo.transform.gameObject} - {target}");
                if (hitInfo.transform.gameObject == target)
                    return true;
            }

            return false;
        }

        public bool TargetVisible(GameObject target, out float distance)
        {
            if (target == null)
            {
                distance = 0.0f;
                return false;
            }

            // Do a simple raycast to target
            if (Physics.Raycast(transform.position, (target.transform.position
                - transform.position).normalized, out RaycastHit hitInfo, m_ViewRange, m_IgnoreMask))
            {
                distance = hitInfo.distance;

                if (hitInfo.transform.gameObject == target)
                    return true;
            }

            distance = 0.0f;

            return false;
        }

        public bool ShotgunRaycast(GameObject target, Vector3 direction, int raycastLayers, int raycastDensity, float spacing)
        {
            // If the player is obscured, try to "shotgun raycast" to find
            // them just in case the matchstick issue may be occuring
            // TODO: This section will need a rework if performance becomes an issue

            for (int j = 0; j < raycastLayers; j++)
            {
                float angle = 0.0f;
                for (int i = 0; i < raycastDensity; i++)
                {
                    float x = Mathf.Sin(angle);
                    float y = Mathf.Cos(angle);
                    angle += (2 * Mathf.PI) / raycastDensity;

                    Vector3 offset;
                    offset.x = x * spacing * j;
                    offset.y = y * spacing * j;
                    offset.z = 1.0f;

                    direction = transform.TransformDirection(offset);

                    if (Physics.Raycast(transform.position + Vector3.up * 0.25f, direction.normalized, out RaycastHit hitInfo, m_ViewRange, m_IgnoreMask))
                    {
                        if (hitInfo.transform.gameObject == target)
                            return true;
                    }
                }
            }

            return false;
        }

        private void CacheOriginalMaterial()
        {
            if (m_Mesh)
            {
                if (m_Mesh.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer renderer))
                {
                    m_CurrentMat = renderer;
                }
                else if (m_Mesh.TryGetComponent<MeshRenderer>(out MeshRenderer rendererLame))
                {
                    m_CurrentMatNoRenderer = rendererLame;
                }

                if (m_CurrentMat)
                    m_OriginalMat = m_CurrentMat.material;
                else if (m_CurrentMatNoRenderer)
                    m_OriginalMat = m_CurrentMatNoRenderer.material;
            }
            else
            {
                Debug.LogWarning("An agent is missing a reference to their Mesh! Possession selection will not work.");
            }
        }
        public void SelectAgent()
        {
            if (m_Mesh) // If m_Mesh hasn't been set, then neither has m_CurrentMat.
            {
                if (m_CurrentMat)
                    m_CurrentMat.material = m_PossessionMaterial;
                else if (m_CurrentMatNoRenderer)
                    m_CurrentMatNoRenderer.material = m_PossessionMaterial;
            }
            if (m_PossessionHeart && m_PossessionPulse) // If they have set these, we can turn them on.
            {
                m_PossessionHeart.Play();
                m_PossessionPulse.Play();
            }
        }
        public void DeselectAgent()
        {
            if (m_Mesh) // If m_Mesh hasn't been set, then neither has m_CurrentMat.
            {
                if (m_CurrentMat)
                    m_CurrentMat.material = m_OriginalMat;
                else if (m_CurrentMatNoRenderer)
                    m_CurrentMatNoRenderer.material = m_OriginalMat;
            }
            if (m_PossessionHeart && m_PossessionPulse) // If they have set these, we can turn them off.
            {
                m_PossessionHeart.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                m_PossessionPulse.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
        private void OnDrawGizmos()
        {
            DrawArrow(transform.position - Vector3.up, Destination, Color.white);

            GameObject target = m_RuntimeTree?.Blackboard?.GetEntry<GameObject>("Target").Value as GameObject;
            if (target != null)
                DrawArrow(transform.position, (target.transform.position - transform.position).normalized, m_ViewRange, Color.red);
        }

        private static void DrawArrow(Vector3 origin, Vector3 destination, Color colour)
        {
            Gizmos.color = colour;
            Gizmos.DrawLine(origin, destination);

            Vector3 right = Quaternion.LookRotation(destination - origin) * Quaternion.Euler(0, 180 + 35.0f, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(destination - origin) * Quaternion.Euler(0, 180 - 35.0f, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawLine(destination, destination + right * 0.45f);
            Gizmos.DrawLine(destination, destination + left * 0.45f);
        }

        private static void DrawArrow(Vector3 origin, Vector3 direction, float length, Color colour)
        {
            Gizmos.color = colour;
            Gizmos.DrawRay(origin, direction * length);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 35.0f, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 35.0f, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(origin + direction * length, right * 0.25f);
            Gizmos.DrawRay(origin + direction * length, left * 0.25f);
        }

        /// <summary>
        /// This function is used when we instantiate a new agent when the player spawns
        /// at a checkpoint. The new UI system requires references to UI elements which
        /// are normally set by hand, however, since we are instantiating the new agent,
        /// we must set those references via script.
        /// </summary>
        public void AttachUIReferences()
        {
            Weapon weapon = m_HostController.GetCurrentWeapon();

            if (m_HostController.m_type == EnemyTypes.SCIENTIST)
            {
                // We are dealing with a scientist.
                weapon.m_CharIcon = UIManager.s_Instance?.m_ScientistCharIcon;
                weapon.m_CharName = UIManager.s_Instance?.m_ScientistCharName;
                weapon.m_WeaponIcon = UIManager.s_Instance?.m_PistolWeaponIcon;
                weapon.m_WeaponAmmoText = UIManager.s_Instance?.m_PistolWeaponAmmoText;
            }
            else if (m_HostController.m_type == EnemyTypes.SOLDIER)
            {
                // We are dealing with a soldier.
                weapon.m_CharIcon = UIManager.s_Instance?.m_SoldierCharIcon;
                weapon.m_CharName = UIManager.s_Instance?.m_SoldierCharName;
                weapon.m_WeaponIcon = UIManager.s_Instance?.m_RifleWeaponIcon;
                weapon.m_WeaponAmmoText = UIManager.s_Instance?.m_RifleWeaponAmmoText;
            }
            else
            {
                // lol this can't happen but I guess I should put an else here.
                Debug.LogError("AttachUIReferences() could not find type of agent!");
            }
        }

        private IEnumerator DelayExecuteFunc(float delaySeconds, Action func)
        {
            yield return new WaitForSeconds(delaySeconds);

            func();
        }
    }
}