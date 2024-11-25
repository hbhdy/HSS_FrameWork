using System.Collections.Generic;
using UnityEngine;

namespace HSS
{
    public static  class UtilFunction
    {
        public static void SetRectTransform(GameObject objTarget,GameObject objOrigin)
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
        
        public static void SetActiveCheck(GameObject obj , bool isActive)
        {
            if(obj != null)
            {
                if (isActive && !obj.activeSelf)
                    obj.SetActive(true);
                else if (!isActive && obj.activeSelf)
                    obj.SetActive(false);
            }
        }

        public static string GetFullPath(Transform tr, string path = null)
        {
            Transform t = tr;
            string fullPath;
            if (path != null && path.StartsWith("/"))
                fullPath = path;
            else
            {
                fullPath = string.Empty;
                while (t != null)
                {
                    if (string.IsNullOrEmpty(fullPath))
                        fullPath = t.name;
                    else
                        fullPath = string.Concat(t.name, "/", fullPath);

                    t = t.parent;
                }

                if (!string.IsNullOrEmpty(path))
                    fullPath += string.Concat("/", path);
            }
            return fullPath;
        }

        #region Find Object

        private static Transform FindTransform(Transform tr, string path)
        {
            Transform t = tr.Find(path);
            if (t == null)
            {
                Debug.LogWarning($"Child Not Found : path ={GetFullPath(tr, path)}");
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
            T component = tr.GetComponent<T>();
            if (component == null)
            {
                Debug.LogWarning($"{typeof(T).Name} Not Found : path={GetFullPath(tr, "")}");
            }

            return component;
        }

        public static T Find<T>(Transform tr, string path) where T : Component
        {
            Transform t = FindTransform(tr, path);
            if (t == null) 
                return null;

            T component = t.GetComponent<T>();
            if (component == null)
            {
                Debug.LogWarning($"{typeof(T).Name} Not Found : path={GetFullPath(tr, path)}");
            }

            return component;
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
            int index = path.LastIndexOf('/');
            string name = index < 0 ? path : path.Substring(index + 1);

            Transform findTr = FindTransform(tr, path);
            if (findTr != null)
            {
                Transform parent = findTr.parent;
                if (parent != null)
                {
                    int childCount = parent.childCount;
                    for (int i = 0; i < childCount; ++i)
                    {
                        Transform childTr = parent.GetChild(i);
                        if (childTr.name == name)
                            all.Add(childTr.gameObject);
                    }
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
                T comp = tr.gameObject.GetComponent<T>();

                if (comp != null)
                    return comp;

                tr = tr.parent;
            }

            return null;
        }

        public static GameObject FindParentObject(Transform target)
        {
            if (target)
            {
                if (target.parent)
                {
                    GameObject obj = target.parent.gameObject;
                    if (obj)
                        return obj;
                    else
                        return FindParentObject(target.parent);
                }
            }
            return null;
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