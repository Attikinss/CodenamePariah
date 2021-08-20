using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace WhiteWillow
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(HostController))]
    public class Agent : MonoBehaviour
    {
        public BehaviourTree InputTree;

        [ReadOnly]
        [SerializeField]
        private bool m_Possessed = false;

        private BehaviourTree m_RuntimeTree;
        private NavMeshAgent m_NavAgent;
        private HostController m_HostController;

        [SerializeField]
        private GameObject m_Pariah;

        private Vector3 m_MovePosition = Vector3.positiveInfinity;
        private Vector3 m_LastPosition;
        public Vector3 FacingDirection { get; private set; }

        private void Start()
        {
            m_RuntimeTree = InputTree?.Clone(gameObject.name);
            m_RuntimeTree?.SetAgent(this);
            m_NavAgent = GetComponent<NavMeshAgent>();
            m_HostController = GetComponent<HostController>();
            m_LastPosition = transform.position;
        }

        private void Update()
        {
            if (m_Possessed)
            {
                FacingDirection = m_HostController.m_Orientation.rotation * m_HostController.m_Orientation.forward;
            }
            else
            {
                m_RuntimeTree?.Tick();

                Vector3 faceFirection = m_NavAgent.velocity;
                faceFirection.y = 0.0f;
                m_HostController.m_Orientation.rotation = Quaternion.Lerp(m_HostController.m_Orientation.rotation, Quaternion.LookRotation(faceFirection.normalized, Vector3.up), 0.02f);
                FacingDirection = m_HostController.m_Orientation.eulerAngles;
            }

            m_LastPosition = transform.position;
        }

        public bool FacingTarget(Transform target)
        {
            return Vector3.Dot(transform.forward, target.position - transform.position) < 10.0f;
        }

        public void RotateToFaceTarget(Transform target)
        {
            Vector3 direction = (transform.position - target.position).normalized;
            float rotation = Mathf.Atan2(direction.z, direction.x);
            Quaternion targetRotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
        }

        public void MoveToPosition()
        {
            if (m_MovePosition != Vector3.positiveInfinity && m_MovePosition != Vector3.negativeInfinity)
            {
                m_NavAgent.SetDestination(m_MovePosition);
            }
        }

        public bool SetDestination(Vector3 destination)
        {
            m_MovePosition = destination;
            return NavMesh.SamplePosition(m_MovePosition, out NavMeshHit hitInfo, 1.0f, NavMesh.AllAreas);
        }

        public bool MovingToPosition() => m_NavAgent.destination == m_MovePosition && transform.position != m_LastPosition;
        public bool AtPosition() => m_NavAgent.remainingDistance <= m_NavAgent.stoppingDistance;

        public void Possess()
        {
            m_Possessed = true;
            m_NavAgent.ResetPath();
            m_NavAgent.enabled = false;
            m_Pariah?.GetComponent<PariahController>().Disable();
            m_HostController?.Enable();
        }

        public void Reliquinsh()
        {
            m_Possessed = false;
            m_NavAgent.enabled = true;
            m_HostController?.Disable();
            m_Pariah?.GetComponent<PariahController>().Enable();
        }
    }
}