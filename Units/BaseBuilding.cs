using System.Collections;
using RTS_LEARN.Units;
using Unity.VisualScripting;
using UnityEngine;


namespace RTS_LEARN.Units
{
    public class BaseBuilding : AbstractCommandable
    {
        public void BuildUnit(UnitSO unit)
        {
            //wait for build time
            StartCoroutine(DoBuildUnit(unit));
        }

        private IEnumerator DoBuildUnit(UnitSO unit)
        {
            Debug.Log("start the coroutine!");
            yield return new WaitForSeconds(unit.BuildTime);
            Debug.Log("build time has elapsed! instantiating the unit!");
            Instantiate(unit.Prefab, transform.position, Quaternion.identity);
        }


    }

}


