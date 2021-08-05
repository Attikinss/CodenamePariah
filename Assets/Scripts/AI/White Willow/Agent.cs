using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace WhiteWillow
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Agent : MonoBehaviour
    {
        public BehaviourTree InputTree;
        public bool m_UserOverride = false;

        private BehaviourTree m_RuntimeTree;
        private NavMeshAgent m_NavAgent;

        private Vector3 m_MovePosition = Vector3.positiveInfinity;
        private Vector3 m_LastPosition;

        private void Start()
        {
            m_RuntimeTree = InputTree?.Clone(gameObject.name);
            m_RuntimeTree?.SetAgent(this);
            m_NavAgent = GetComponent<NavMeshAgent>();
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
        }

        private void Update()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
                m_UserOverride = !m_UserOverride;

            if (m_UserOverride)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo))
                        m_NavAgent.SetDestination(hitInfo.point);
                }
            }
            else
            {
                m_RuntimeTree?.Tick();
                m_LastPosition = transform.position;
            }
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
            bool onNavMesh = NavMesh.SamplePosition(m_MovePosition, out NavMeshHit hitInfo, 2.0f, NavMesh.AllAreas);
            return onNavMesh;
        }

        public bool MovingToPosition() => m_NavAgent.destination == m_MovePosition && transform.position != m_LastPosition;

        public bool AtPosition() => m_NavAgent.remainingDistance <= m_NavAgent.stoppingDistance;
    }
}