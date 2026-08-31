using System.Collections.Generic;
using UnityEngine;

public interface ITargetingStrategy 
{
    Transform SelectTarget(Vector3 towerPosition, float range, IEnumerable<Transform> enemiesInRange);
}
