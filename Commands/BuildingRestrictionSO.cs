using UnityEngine;
using UnityEngine.AI;

namespace RTS_LEARN.Commands
{
    [CreateAssetMenu(fileName = "Building Restriction", menuName = "Buildings/Restrictions", order = 7)]
    public class BuildingRestrictionSO : ScriptableObject
    {
        [field: SerializeField] public float Radius { get; private set; } = 1f;
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
        [field: SerializeField] public OverlapStyle HitDetectionStyle { get; private set; } = OverlapStyle.Sphere;

        [field: SerializeField] public bool MustBeFullyOnNavmesh { get; private set; } = true;
        [field: SerializeField] public int NavMeshAgentTypeId { get; private set; }
        [field: SerializeField] public float NavMeshTolerance { get; private set; } = 0.1f;
        [field: SerializeField] public Vector3 Extents { get; private set; } = Vector3.one;

        private Collider[] hitColliders = new Collider[1];

        public bool CanPlace(Vector3 position)
        {
            int hits = HitDetectionStyle switch
            {
                OverlapStyle.Sphere => Physics.OverlapSphereNonAlloc(position, Radius, hitColliders, LayerMask),
                OverlapStyle.Box => Physics.OverlapBoxNonAlloc(
                    position, Extents, hitColliders, Quaternion.identity, LayerMask
                ),
                _ => throw new System.NotImplementedException()
            };

            if (MustBeFullyOnNavmesh)
            {
                NavMeshQueryFilter queryFilter = new()
                {
                    areaMask = NavMesh.AllAreas,
                    agentTypeID = NavMeshAgentTypeId
                };

                bool isOnNavMesh = IsFullyOnNavMesh(position, queryFilter);
                return hits == 0 && isOnNavMesh;
            }

            return hits == 0;
        }

        private bool IsFullyOnNavMesh(Vector3 position, NavMeshQueryFilter queryFilter)
        {   //4 corners of the building
            // Check if all corners of the building's extents are on the NavMesh
            bool isOnNavMesh = NavMesh.SamplePosition(
                                position + new Vector3(Extents.x, 0, Extents.z),
                                out NavMeshHit _, NavMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(
                                position + new Vector3(Extents.x, 0, -Extents.z),
                                out NavMeshHit _, NavMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(
                                position + new Vector3(-Extents.x, 0, -Extents.z),
                                out NavMeshHit _, NavMeshTolerance, queryFilter);
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(
                                position + new Vector3(-Extents.x, 0, Extents.z),
                                out NavMeshHit _, NavMeshTolerance, queryFilter);
            return isOnNavMesh;
        }

        public enum OverlapStyle
        {
            Sphere,
            Box
        }

    }

}