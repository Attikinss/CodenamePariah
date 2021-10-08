using System;
using UnityEngine;
using UnityEngine.AI;

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
        
        [HideInInspector]
        public PariahController PariahController;
        private HostController m_HostController;

        private Vector3 m_LastPosition;

        public bool Possessed { get; private set; } = false;
        public bool EngagingTarget { get; private set; } = true;
        public Transform Orientation { get => m_HostController.m_Orientation; }
        public Vector3 Destination { get; private set; }
        public Vector3 FacingDirection { get; private set; }

		private void Awake()
		{
            m_RuntimeTree = InputTree?.Clone(gameObject.name);
            m_RuntimeTree?.SetAgent(this);
            m_LastPosition = transform.position;

            m_NavAgent = GetComponent<NavMeshAgent>();
            m_HostController = GetComponent<HostController>();

            PariahController = FindObjectOfType<PariahController>();
            m_RuntimeTree.Blackboard?.UpdateEntryValue<GameObject>("Target", PariahController?.gameObject);

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
            if (Vector3.Distance(m_NavAgent.destination, Destination)
                >= Mathf.Epsilon + m_NavAgent.stoppingDistance)
            {
                m_NavAgent.SetDestination(Destination);
                m_NavAgent.isStopped = false;
            }
        }

        public bool SetDestination(Vector3 destination)
        {
            m_LastPosition = transform.position;
            Destination = destination;
            return true;
        }

        public void Stop()
        {
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