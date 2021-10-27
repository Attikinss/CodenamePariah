using System;
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

        [Tooltip("Defines what the agent can't see through.")]
        [SerializeField]
        private LayerMask m_IgnoreMask;

        private BehaviourTree m_RuntimeTree;
        private NavMeshAgent m_NavAgent;
        public Query CurrentQuery;
        public Weapon m_Weapon;
        [Tooltip("The mesh that we apply the possession shader to.")]
        public GameObject m_Mesh;
        public Material m_PossessionMaterial;
        public ParticleSystem m_PossessionHeart;
        public ParticleSystem m_PossessionPulse;

        [SerializeField]
        private Animator m_Animator;

        [HideInInspector]
        public PariahController PariahController;
        private HostController m_HostController;

        public bool Possessed { get; private set; } = false;
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

		private void Awake()
		{
            m_RuntimeTree = InputTree?.Clone(gameObject.name);
            m_RuntimeTree?.SetAgent(this);

            m_NavAgent = GetComponent<NavMeshAgent>();
            m_HostController = GetComponent<HostController>();

            PariahController = FindObjectOfType<PariahController>();
            m_RuntimeTree.Blackboard?.UpdateEntryValue<GameObject>("Target", PariahController?.gameObject);

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
                        Stop();
                        m_NavAgent.velocity = Vector3.zero;
                    }

                    return;
                }
                else
                {
                    if (m_NavAgent.isStopped)
                    {
                        m_NavAgent.isStopped = true;
                        MoveToPosition();
                    }
                }

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
            PlayAnimation("Run");

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

        public void Stop()
        {
            PlayAnimation("IdleCombat");

            m_NavAgent.ResetPath();
            m_NavAgent.isStopped = true;
        }
        
        public bool MovingToPosition() => m_NavAgent.hasPath;
        public bool AtPosition() => Vector3.Distance(transform.position - Vector3.up, Destination) < 0.2f + m_NavAgent.stoppingDistance;
        public bool Stuck() => false;

        public void Possess()
        {
            Possessed = true;
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
            if (Possessed)
            { 
                Release();
                // Also apply extra damage to Pariah's life essence.
                PariahController?.TakeDamage(m_HostController.GetOnDestroyDamage());
            }

            // TODO: Use object pooling / queued destruction system
            Destroy(gameObject);
        }

        public void LookAt(Vector3 position)
        {
            Debug.Log("Look At");

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

        public void ShootAt(GameObject target, bool forceMiss = false)
        {
            m_HostController.GetCurrentWeapon()?.Fire();
            
            //if (TempPrefab)
            //{
            //    var bullet = Instantiate(TempPrefab, transform.position + transform.forward + Vector3.up, Quaternion.identity);
            //    bullet.GetComponent<Rigidbody>().AddForce(((target.transform.position + Vector3.up * 0.7f) - transform.position).normalized * 60.0f, ForceMode.Impulse);
            //}
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
                Debug.Log($"{hitInfo.transform.gameObject} - {target}");
                if (hitInfo.transform.gameObject == target)
                {
                    return true;
                }
            }

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
    }
}