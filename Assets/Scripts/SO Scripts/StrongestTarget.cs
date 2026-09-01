using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefence/Targeting/Strongest")]
public class StrongestTarget : TargetingStratedgy
{
    public override Transform SelectTarget(Vector3 pos, float range, IEnumerable<Transform> enemies)
    {
        return enemies
            .Where(e => e.GetComponentInParent<IDamageable>() != null)
            .OrderByDescending(e => e.GetComponentInParent<IDamageable>().CurrentHealth)
            .FirstOrDefault();
    }
}
