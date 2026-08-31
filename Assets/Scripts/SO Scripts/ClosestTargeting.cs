using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefence/Targeting/Closest")]
public class ClosestTargeting : TargetingStratedgy
{
    public override Transform SelectTarget(Vector3 pos, float range, IEnumerable<Transform> enemies)
        => enemies.OrderBy(e => Vector3.Distance(pos, e.position)).FirstOrDefault();
}
