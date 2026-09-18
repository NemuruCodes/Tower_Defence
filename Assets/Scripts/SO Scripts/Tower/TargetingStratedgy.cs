using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetingStratedgy", menuName = "Scriptable Objects/TargetingStratedgy")]
public abstract class TargetingStratedgy : ScriptableObject
{
    public abstract Transform SelectTarget(Vector3 towerPos, float range, IEnumerable<Transform> enemies);
}
