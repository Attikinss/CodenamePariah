using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WhiteWillow;

public class AIDirector : MonoBehaviour
{
    public static AIDirector Instance
    {
        get
        {
            if (m_Instance == null)
            {
                var director= new GameObject("AIDirector");
                m_Instance = director.AddComponent<AIDirector>();
            }

            return m_Instance;
        }
    }

    private static AIDirector m_Instance;

    [SerializeField]
    private int m_MaxTokens = 5;

    [SerializeField]
    [ReadOnly]
    private List<CombatToken> m_Tokens;

    public List<Agent> Agents { get; private set; } = new List<Agent>();

    private void Awake()
    {
        m_Tokens = new List<CombatToken>(m_MaxTokens);
        for (int i = 0; i < m_MaxTokens; i++)
            m_Tokens.Add(new CombatToken());
    }

    public void AddAgent(Agent agent)
    {
        if (!Agents.Contains(agent))
            Agents.Add(agent);
    }

    public void RemoveAgent(Agent agent)
    {
        if (Agents.Contains(agent))
            Agents.Remove(agent);
    }

    public List<Vector3> GetPositions(params Agent[] exclude)
    {
        return Agents.Where(agent => !exclude.Any(excl => excl == agent)).Select(x => x.transform.position).ToList();
    }

    public CombatToken RequestToken()
    {
        return m_Tokens.Find(t => t.Holder == null);
    }

    public bool RequestToken(out CombatToken token)
    {
        token = m_Tokens.Find(t => t.Holder == null);
        return token != null;
    }
}
