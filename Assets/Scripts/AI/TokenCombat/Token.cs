using WhiteWillow;

[System.Serializable]
public class CombatToken
{
    public enum Type { Grenade, Melee, Ranged }

    public Agent Holder { get; private set; }

    public CombatToken Transfer(Agent agent)
    {
        Holder = agent;

        return this;
    }

    public void Use()
    {
        Holder = null;
    }
}
