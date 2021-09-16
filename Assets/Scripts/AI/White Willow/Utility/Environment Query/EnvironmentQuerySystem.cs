using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public static class EnvironmentQuerySystem
{
    private static List<EQSThread> s_QueryThreads = new List<EQSThread>(10);

    private static Queue<QueryRequest> s_Requests = new Queue<QueryRequest>();
    private static bool s_RequestsLocked = false;

    private static List<Query> s_ResolvedQueries = new List<Query>();
    private static bool s_QueriesLocked = false;

    public static void NewQuery(QueryRequest request)
    {
        // Find the first thread available.
        EQSThread thread = s_QueryThreads.Find(t => !t.Busy);
        if (thread == null)
        {
            if (s_QueryThreads.Count < s_QueryThreads.Capacity)
            {
                // If there are none and the maximum thread count hasn't been
                // reached, create and add a new thread to resolve the new request
                Debug.Log("New thread");
                s_QueryThreads.Add(new EQSThread(request));
            }
            else
            {
                // Otherwise just add it to the queue for resolving
                s_Requests.Enqueue(request);
            }
        }
        else
        {
            thread.SetTask(request);
        }
    }

    public static Query RetrieveQuery(QueryRequest request)
    {
        return s_ResolvedQueries.Find(query => query.ID == request.ID);
    }

    public static bool RetrieveQuery(QueryRequest request, out Query query)
    {
        if (!s_QueriesLocked)
        {
            s_QueriesLocked = true;

            lock (s_ResolvedQueries)
            {
                query = s_ResolvedQueries.Find(q => q.ID == request.ID && request.QueryResolved);
            }

            s_QueriesLocked = false;
            return query != null;
        }

        query = null;
        return false;
    }

    /*
    public static Query NewQuery(Vector3 origin, float range, int radialDensity = 30, float spacing = 1.5f)
    {
        Query query = new Query("");
        int positionRingCount = Mathf.RoundToInt(range / spacing);
    
        // Generate positions within a range with a set density
        for (int i = 1; i <= positionRingCount; i++)
        {
            for (int j = 0; j < radialDensity; j++)
            {
                // Get position on same y level
                Vector3 offset = Vector3.forward;
                offset = Quaternion.AngleAxis(360.0f / radialDensity * j, Vector3.up) * offset;
                offset *= (spacing * i);
                offset += origin;
        
                // Check if point is on navmesh, if not place it on closest point
                if (NavMesh.SamplePosition(offset, out NavMeshHit hitInfo, 3.0f, NavMesh.AllAreas))
                {
                    if (hitInfo.position.y <= origin.y + 0.1f)
                        offset = hitInfo.position;
                    else continue;
                }
                else continue;
        
                // Check if point is inside geometry
                var colliders = Physics.OverlapSphere(offset, 0.5f);
                bool insideGeometry = false;
                foreach (var collider in colliders)
                {
                    if (!collider.gameObject.CompareTag("Ground") || collider.bounds.Contains(offset))
                    {
                        insideGeometry = true;
                        break;
                    }
                }
        
                if (!insideGeometry)
                    query.AddPosition(offset);
            }
        }
    
        return query;
    }
    */

    public static Query NewQuery(Vector3 origin, float range, int radialDensity = 30, float spacing = 1.5f)
    {
        Query query = new Query("");
        int positionRingCount = Mathf.RoundToInt(range / spacing);
        /*List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < radialDensity; i++)
        {
            for (int j = 1; j <= positionRingCount; j++)
            {
                Vector3 offset = Vector3.forward;
                offset = Quaternion.AngleAxis(360.0f / radialDensity * i, Vector3.up) * offset;
                offset *= (spacing * j);
                offset += origin;
                
                // Check if point is on navmesh, if not place it on closest point
                if (NavMesh.SamplePosition(offset, out NavMeshHit navHitInfo, 3.0f, NavMesh.AllAreas))
                {
                    if (navHitInfo.position.y <= origin.y + 0.1f)
                        offset = navHitInfo.position;
                    else continue;
                }
                else continue;
        
                positions.Add(offset);
            }
        
            if (Physics.Raycast(origin, positions.Last() - origin, out RaycastHit hitInfo, range))
                positions.RemoveAll(pos => (pos - origin).magnitude >= hitInfo.distance - 0.5f);
        }
        
        positions.ForEach(pos => query.AddPosition(pos));*/

        // Generate positions within a range with a set density
        for (int i = 1; i <= positionRingCount; i++)
        {
            for (int j = 0; j < radialDensity; j++)
            {
                // Get position on same y level
                Vector3 offset = Vector3.forward;
                offset = Quaternion.AngleAxis(360.0f / radialDensity * j, Vector3.up) * offset;
                offset *= (spacing * i);
                offset += origin;

                // Check if point is on navmesh, if not place it on closest point
                if (NavMesh.SamplePosition(offset, out NavMeshHit hitInfo, 3.0f, NavMesh.AllAreas))
                {
                    if (hitInfo.position.y <= origin.y + 0.1f)
                        offset = hitInfo.position;
                    else continue;
                }
                else continue;

                // Check if point is inside geometry
                var colliders = Physics.OverlapSphere(offset, 0.5f);
                bool insideGeometry = false;
                foreach (var collider in colliders)
                {
                    if (!collider.gameObject.CompareTag("Ground") || collider.bounds.Contains(offset))
                    {
                        insideGeometry = true;
                        break;
                    }
                }

                if (!insideGeometry)
                    query.AddPosition(offset);
            }
        }

        return query;
    }

    private static bool AddToResolvedQueries(Query query)
    {
        if (!s_QueriesLocked)
        {
            s_QueriesLocked = true;

            lock (s_ResolvedQueries)
            {
                s_ResolvedQueries.Add(query);
            }

            s_QueriesLocked = false;
            return true;
        }

        return false;
    }

    private static QueryRequest GetNextRequest()
    {
        QueryRequest request = null;
        if (!s_RequestsLocked)
        {
            s_RequestsLocked = true;

            lock (s_Requests)
            {
                request = s_Requests.Dequeue();
            }

            s_RequestsLocked = false;
        }

        return request;
    }

    [System.Serializable]
    public class EQSNode
    {
        public enum Type
        {
            Vent,
            Guard,
            LowPriority,
            HighPriority,
        }

        [ReadOnly]
        [Tooltip("Unique ID for the node.")]
        public string ID;

        [ReadOnly]
        [Tooltip("Whether the node has been reserved for another agent.")]
        public bool Taken = false;

        [Tooltip("The type of the node which defines agent interaction.")]
        public Type NodeType = Type.LowPriority;

        public Color VacantColour;
        public Color TakenColour;

        public float Size = 0.5f;

        [HideInInspector]
        public Vector3 Position;

        public EQSNode()
        {
            ID = System.Guid.NewGuid().ToString();
        }
    }

    public class QueryRequest
    {
        public string ID { get; }
        public bool QueryResolved { get; private set; }

        public Vector3 Origin { get; }
        public float Range { get; }
        public List<EQSNode> Nodes;

        //public int RadialDensity { get; }
        //public float Spacing { get; }

        public QueryRequest(Vector3 origin, float range, List<EQSNode> nodes/*int radialDensity = 30, float spacing = 1.5f*/)
        {
            ID = System.Guid.NewGuid().ToString();
            Origin = origin;
            Range = range;
            Nodes = nodes;
            //RadialDensity = radialDensity;
            //Spacing = spacing;
        }

        public void Resolve()
        {
            QueryResolved = true;
        }
    }

    private class EQSThread
    {
        public bool Busy { get; private set; }
        public bool Active { get; private set; }

        private Thread m_Thread;
        private QueryRequest m_Request;
        private Query m_Query;

        public EQSThread(QueryRequest request)
        {
            Active = true;
            m_Request = request;

            m_Thread = new Thread(() =>
            {
                while (Active)
                    ExecuteTask();
            });

            m_Thread.Start();
        }

        private void ExecuteTask()
        {
            if (m_Query != null)
            {
                // A query has already been created and needs
                // to submitted to the resolved queries list
                if (AddToResolvedQueries(m_Query))
                {
                    m_Request.Resolve();
                    m_Request = null;
                }
                else
                    return;
            }

            if (m_Request == null)
            {
                // Retieve next free request
                m_Request = GetNextRequest();

                // If the request is null, the request queue is
                // currently locked. Bail until the next cycle
                if (m_Request == null)
                    return;
            }

            ResolveCurrentRequest();
        }

        public void SetTask(QueryRequest request)
        {
            m_Request = request;
        }

        public void Start()
        {
            if (Active)
                return;

            m_Thread = new Thread(() =>
            {
                while (Active)
                    ExecuteTask();
            });
        }

        public void Stop(bool forceStop)
        {
            if (forceStop)
                m_Thread.Abort();
            else
                Active = false;
        }

        private void ResolveCurrentRequest()
        {
            m_Query = new Query(m_Request.ID);
            Debug.Log("Executing...");

            foreach (EQSNode node in m_Request.Nodes)
            {

            }
        }

        /*
        private void ResolveCurrentRequest()
        {
            m_Query = new Query(m_Request.ID);
            int positionRingCount = Mathf.RoundToInt(m_Request.Range / m_Request.Spacing);
            Debug.Log("Executing...");

            // Generate positions within a range with a set density
            for (int i = 1; i <= positionRingCount; i++)
            {
                for (int j = 0; j < m_Request.RadialDensity; j++)
                {
                    // Get position on same y level
                    Vector3 offset = Vector3.forward;
                    offset = Quaternion.AngleAxis(360.0f / m_Request.RadialDensity * j, Vector3.up) * offset;
                    offset *= (m_Request.Spacing * i);
                    offset += m_Request.Origin;

                    // Check if point is on navmesh, if not place it on closest point
                    // --- [ Not thread safe ] ---
                    if (NavMesh.SamplePosition(offset, out NavMeshHit hitInfo, 3.0f, NavMesh.AllAreas))
                    {
                        // --- [ Not thread safe ] ---
                        if (hitInfo.position.y <= m_Request.Origin.y + 0.1f)
                            offset = hitInfo.position;
                        else continue;
                    }
                    else continue;

                    // Check if point is inside geometry
                    // --- [ Not thread safe ] ---
                    var colliders = Physics.OverlapSphere(offset, 0.5f);
                    bool insideGeometry = false;
                    foreach (var collider in colliders)
                    {
                        // --- [ Not thread safe ] ---
                        if (!collider.gameObject.CompareTag("Ground") || collider.bounds.Contains(offset))
                        {
                            insideGeometry = true;
                            break;
                        }
                    }

                    if (!insideGeometry)
                        m_Query.AddPosition(offset);
                }
            }
        }
         */
    }
}
