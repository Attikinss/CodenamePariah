using System;

[Serializable]
public class EmptyEntry : BlackboardEntry
{
    public override object Value { get => null; set { } }
}