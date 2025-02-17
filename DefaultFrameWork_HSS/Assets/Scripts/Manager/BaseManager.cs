using System.Collections;
using UnityEngine;

namespace HSS
{
    public class BaseManager : MonoBehaviour
    {
        protected bool isReady = false;

        /// <summary>
        /// 매니저 초기화
        /// </summary>
        public virtual IEnumerator Co_Init()
        {
            yield return null;

            HSSLog.Log($"{gameObject.name} is Ready");
            isReady = true;
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public virtual void Init()
        {

        }
    }
}