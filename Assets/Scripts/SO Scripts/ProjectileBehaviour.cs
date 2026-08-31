using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileBehaviour", menuName = "Scriptable Objects/ProjectileBehaviour")]
public abstract class ProjectileBehaviour : ScriptableObject
{
    public abstract void OnHit(Transform target, float damage, Vector3 hitPosition);
}
