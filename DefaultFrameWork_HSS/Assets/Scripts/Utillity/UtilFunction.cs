using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HSS
{
    public static  class UtilFunction
    {
        public static void CreateInstantiateUIObject(out GameObject objTarget, GameObject objPrefab, GameObject objParent)
        {
            objTarget = GameObject.Instantiate(objPrefab, objParent.transform, false) as GameObject;
            objTarget.name = objPrefab.name;

            SetRectTransform(objTarget, objPrefab);
        }

        public static void SetRectTransform(GameObject objTarget, GameObject objOrigin)
        {
            SetRectTransform(objTarget.GetComponent<RectTransform>(), objOrigin.GetComponent<RectTransform>());
        }

        public static void SetRectTransform(RectTransform rectTarget, RectTransform rectOrigin)
        {
            if (rectTarget == null || rectOrigin == null)
                return;

            rectTarget.anchoredPosition = rectOrigin.anchoredPosition;
            rectTarget.anchoredPosition3D = rectOrigin.anchoredPosition3D;
            rectTarget.anchorMax = rectOrigin.anchorMax;
            rectTarget.anchorMin = rectOrigin.anchorMin;
            rectTarget.offsetMax = rectOrigin.offsetMax;
            rectTarget.offsetMin = rectOrigin.offsetMin;
            rectTarget.pivot = rectOrigin.pivot;
            rectTarget.sizeDelta = rectOrigin.sizeDelta;
            rectTarget.localScale = rectOrigin.localScale;
        }

        public static void SetActiveCheck(GameObject obj, bool isActive)
        {
            if (obj != null && obj.activeSelf != isActive)
                obj.SetActive(isActive);
        }

        public static string GetFullPath(Transform tr, string path = null)
        {
            StringBuilder sb = new StringBuilder(path ?? string.Empty);
            Transform t = tr;

            while (t != null)
            {
                sb.Insert(0, $"{t.name}/");
                t = t.parent;
            }

            return sb.ToString().TrimEnd('/');
        }

        #region Find Object

        // path가 경로 또는 이름

        private static Transform FindTransform(Transform tr, string path)
        {
            Transform t = tr.Find(path);
            if (t == null)
            {
                HSSLog.LogWarning($"Child Not Found : path ={GetFullPath(tr, path)}");
                return null;
            }

            return t;
        }

        public static GameObject Find(Transform tr, string path)
        {
            Transform t = FindTransform(tr, path);
            return t == null ? null : t.gameObject;
        }

        public static T Find<T>(Transform tr) where T : Component
        {
            if (tr.TryGetComponent(out T t))
                return t;

            HSSLog.LogWarning($"{typeof(T).Name} Not Found : path={GetFullPath(tr, "")}");
            return null;
        }

        public static T Find<T>(Transform tr, string path) where T : Component
        {
            Transform trans = FindTransform(tr, path);

            if (trans.TryGetComponent(out T t))
                return t;

            HSSLog.LogWarning($"{typeof(T).Name} Not Found : path={GetFullPath(tr, path)}");
            return null;
        }

        public static GameObject Find(this GameObject obj, string path)
        {
            return Find(obj.transform, path);
        }

        public static T Find<T>(this GameObject obj, string path) where T : Component
        {
            return Find<T>(obj.transform, path);
        }

        public static GameObject Find(Component component, string path)
        {
            return Find(component.transform, path);
        }

        public static T Find<T>(Component component, string path) where T : Component
        {
            return Find<T>(component.transform, path);
        }

        public static List<GameObject> FindAll(Transform tr, string path)
        {
            List<GameObject> all = new List<GameObject>();
            Transform findTr = FindTransform(tr, path);

            if (findTr?.parent != null)
            {
                foreach(Transform childTr in findTr.parent)
                {
                    if(childTr.name == findTr.name)
                        all.Add(childTr.gameObject);
                }
            }

            return all;
        }

        public static List<T> FindAll<T>(Transform tr, string path) where T : Component
        {
            List<T> all = new List<T>();
            foreach (GameObject obj in FindAll(tr, path))
            {
                T component = obj.GetComponent<T>();
                if (component != null)
                    all.Add(component);
            }

            return all;
        }

        public static List<GameObject> FindAll(this GameObject obj, string path)
        {
            return FindAll(obj.transform, path);
        }

        public static List<T> FindAll<T>(this GameObject obj, string path) where T : Component
        {
            return FindAll<T>(obj.transform, path);
        }

        public static T FindInParents<T>(this GameObject obj) where T : Component
        {
            if (obj == null) 
                return null;

            Transform tr = obj.transform;
            while (tr != null)
            {
                if (tr.TryGetComponent(out T comp))
                    return comp;

                tr = tr.parent;
            }

            return null;
        }

        public static GameObject FindParentObject(Transform target)
        {
            return target?.parent?.gameObject ?? null;
        }

        public static GameObject FindChild(GameObject obj, string strName)
        {
            if (obj == null)
                return null;

            if (obj.name == strName)
                return obj;

            Transform tr = obj.transform.Find(strName);
            if (tr != null)
                return tr.gameObject;

            foreach (Transform chlidTr in obj.transform)
            {
                if (chlidTr == null)
                    continue;

                GameObject returnObj = FindChild(chlidTr.gameObject, strName);
                if (returnObj != null)
                    return returnObj;
            }

            return null;
        }

        #endregion Find Object
    }
}