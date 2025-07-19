using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


namespace RTS_LEARN.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        private Queue<UnitSO> buildingQueue = new(MAX_QUEUE_SIZE);

        private const int MAX_QUEUE_SIZE = 5;
        public void BuildUnit(UnitSO unit)
        {
            if (buildingQueue.Count == MAX_QUEUE_SIZE)
            {
                Debug.Log("buildingQueue is now full! This is not supported!!");
                return;
            }

            buildingQueue.Enqueue(unit);
            if (buildingQueue.Count == 1)
            {
                StartCoroutine(DoBuildUnit());
            }



        }

        private IEnumerator DoBuildUnit()
        {
            while (buildingQueue.Count > 0)
            {
                UnitSO unit = buildingQueue.Peek();
                Debug.Log("start the coroutine!");
                yield return new WaitForSeconds(unit.BuildTime);
                Debug.Log("build time has elapsed! instantiating the unit!");
                Instantiate(unit.Prefab, transform.position, Quaternion.identity);
                buildingQueue.Dequeue();
            }

        }


    }

}


