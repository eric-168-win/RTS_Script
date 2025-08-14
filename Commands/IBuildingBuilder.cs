using RTS_LEARN.Units;
using UnityEngine;

namespace RTS_LEARN.Units
{
    public interface IBuildingBuilder
    {
        public bool IsBuilding { get; }
        public Owner Owner { get; }
        public GameObject Build(BuildingSO building, Vector3 targetLocation);
        public void ResumeBuilding(BaseBuilding building);
        public void CancelBuilding();
    }
}
