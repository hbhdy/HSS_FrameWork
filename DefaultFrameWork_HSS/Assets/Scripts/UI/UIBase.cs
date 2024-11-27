using System;
using UnityEngine;

namespace HSS
{
    [AttributeUsage(AttributeTargets.All)]
    public class UIAttrTypeAttribute : Attribute
    {
        public string uiResourceName { get; private set; } = string.Empty;

        public UIAttrTypeAttribute(string uiResourceName)
        {
            this.uiResourceName = uiResourceName;
        }
    }

    public class UIAttrUtil
    {
        static T GetTypeAttribute<T>(Type type, string enumString)
        {
            System.Reflection.MemberInfo[] infos = type.GetMember(enumString);
            if (infos.Length == 0)
                return default;

            object[] attributes = infos[0].GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0)
                return default;

            return (T)attributes[0];
        }

        public static string GetUIAttributeResourceName<T>(T t)
        {
            UIAttrTypeAttribute customAttr = GetTypeAttribute<UIAttrTypeAttribute>(t.GetType(), t.ToString());
            return customAttr == null ? string.Empty : customAttr.uiResourceName;
        }
    }

    public abstract class UIBase : MonoBehaviour
    {
        // ----- Param -----

        public abstract UIType UIType { get; }

        private Canvas baseCanvas;

        public Action callOpenAfter;
        public Action callOpenBefore;
        public Action callCloseAfter;
        public Action callCloseBefore;

        // ----- Init -----

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            baseCanvas = GetComponent<Canvas>();
        }

        protected virtual void OpenUI()
        {
            this.gameObject.SetActive(true);

            CallOpenAfter();
        }

        protected virtual void CloseUI()
        {
            this.gameObject.SetActive(false);
            baseCanvas.sortingOrder = 0;

            CallCloseAfter();
        }

        public virtual void CallOpenAfter()
        {
            callOpenAfter?.Invoke();
            callOpenAfter = null;
        }

        public virtual void CallCloseAfter()
        {
            callCloseAfter?.Invoke();
            callCloseAfter = null;
        }

        // ----- Get -----

        public Canvas GetCanvas()
        {
            return baseCanvas;
        }
    }
}
