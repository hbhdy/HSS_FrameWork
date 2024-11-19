using HSS;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    None,

    [UIAttrType("UIPopup_Common")]
    UIPopup_Common,
    [UIAttrType("UIPopup_Option")]
    UIPopup_Option,
}

public enum Canvas_SortOrder
{
    SCREEN = 100,
    POPUP = 3000,
}

namespace HSS
{
    public class UIManager
    {
        // ----- Param -----

        private Dictionary<UIType, UIBase> dicUI = new Dictionary<UIType, UIBase>();
        public Transform trScreen { get; private set; }
        public Transform trPopup { get; private set; }

        // ----- Init -----

        public void Init()
        {

        }

        // ----- Set ----- 

        public void SetTrScreen(Transform tr)
        {
            trScreen = tr;
        }
        public void SetTrPopup(Transform tr)
        {
            trPopup = tr;
        }

        // ----- Main ----- 

        public void OpenUI(UIType uiType, Canvas_SortOrder sortOrder = Canvas_SortOrder.POPUP, Action openAfter = null, Action closeAtger = null)
        {
            UIBase openUI = null;

            if (dicUI.ContainsKey(uiType) == true)
                openUI = dicUI[uiType];
            else
                openUI = LoadUI<UIBase>(uiType, sortOrder);

            if (openUI == null)
                return;

            openUI.callOpenAfter = openAfter;
            openUI.callCloseAfter = closeAtger;
            _OpenUI(openUI);
        }

        private T LoadUI<T>(UIType uiType, Canvas_SortOrder sortOrder = Canvas_SortOrder.POPUP) where T : MonoBehaviour
        {
            GameObject objParent = sortOrder == Canvas_SortOrder.POPUP ? trPopup.gameObject : trScreen.gameObject;

            return LoadUI<T>(UIAttrUtil.GetUIAttributeResourceName(uiType), objParent);
        }

        private T LoadUI<T>(string uiName, GameObject parent) where T : MonoBehaviour
        {
            // TODO: 이후에 전체폴더 탐색으로 처리해야함 
            string path = string.Format("Prefabs/UI/{0}", uiName);

            GameObject obj = Resources.Load<GameObject>(path);
            GameObject popup = GameObject.Instantiate(obj);
            popup.transform.parent = parent.transform;

            UtilFunction.SetRectTransform(popup, obj);

            T findComponent = obj.GetComponent<T>();
            UIBase uiBase = findComponent as UIBase;

            if (uiBase != null)
                dicUI.Add(uiBase.UIType, uiBase);

            return findComponent;
        }

        private void _OpenUI(UIBase openUI)
        {
            Type type = typeof(UIBase);
            System.Reflection.MethodInfo method = type.GetMethod("OpenUI", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            method.Invoke(openUI, null);
        }
    }
}