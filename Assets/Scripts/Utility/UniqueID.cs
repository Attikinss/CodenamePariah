using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueIdentifierAttribute : PropertyAttribute { }

public class UniqueID
{
    [UniqueIdentifier]
    public string uniqueID;

}
