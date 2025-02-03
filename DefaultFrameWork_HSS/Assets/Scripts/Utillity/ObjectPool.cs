using System.Collections.Generic;
using UnityEngine;

namespace HSS
{
    public sealed class ObjectPool : MonoBehaviour
    {
        // ----- Param -----

        private static ObjectPool instance;

        private static List<GameObject> tempList = new List<GameObject>();
        private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
        private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

        // ----- instance -----

        public static ObjectPool Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = Object.FindAnyObjectByType<ObjectPool>();
                if (instance != null)
                    return instance;

                var obj = new GameObject("Object_Pool");
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                instance = obj.AddComponent<ObjectPool>();
                return instance;
            }
        }

        // ----- Init -----

        private void Awake()
        {
            instance = this;
        }

        // ----- Create Pool -----

        public static void CreatePool<T>(T prefab, int poolSize) where T : Component
        {
            CreatePool(prefab.gameObject, poolSize);
        }

        public static void CreatePool(GameObject prefab, int poolSize)
        {
            if (prefab != null && !Instance.pooledObjects.ContainsKey(prefab))
            {
                var list = new List<GameObject>();
                instance.pooledObjects.Add(prefab, list);

                if (poolSize > 0)
                {
                    bool active = prefab.activeSelf;
                    prefab.SetActive(false);
                    Transform parent = instance.transform;
                    while (list.Count < poolSize)
                    {
                        var obj = Object.Instantiate(prefab);
                        obj.transform.SetParent(parent);
                        obj.name = $"{prefab.name}_{obj.GetInstanceID().ToString()}";

                        list.Add(obj);
                    }
                    prefab.SetActive(active);
                }
            }
        }

        // ----- Spawn -----

        public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (prefab == null)
                return null;

            List<GameObject> list;
            Transform trans;
            GameObject obj;
            if (Instance.pooledObjects.TryGetValue(prefab, out list))
            {
                obj = null;

                if (list.Count > 0)
                {
                    while (obj == null && list.Count > 0)
                    {
                        obj = list[0];
                        list.RemoveAt(0);
                    }

                    if (obj != null)
                    {
                        trans = obj.transform;
                        trans.SetParent(parent);
                        trans.localPosition = position;
                        trans.localRotation = rotation;
                        trans.localScale = scale;
                        obj.SetActive(true);
                        Instance.spawnedObjects.Add(obj, prefab);

                        return obj;
                    }
                }

                obj = Object.Instantiate(prefab);
                trans = obj.transform;
                trans.SetParent(parent);
                trans.localPosition = position;
                trans.localRotation = rotation;
                trans.localScale = scale;
                Instance.spawnedObjects.Add(obj, prefab);

                return obj;
            }
            else
            {
                obj = Object.Instantiate(prefab);
                trans = obj.GetComponent<Transform>();
                trans.SetParent(parent);
                trans.localPosition = position;
                trans.localRotation = rotation;
                trans.localScale = scale;

                return obj;
            }
        }

        // --

        public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale) where T : Component
        {
            return Spawn(prefab.gameObject, parent, position, rotation, scale).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Vector3 scale) where T : Component
        {
            return Spawn(prefab.gameObject, parent, position, scale).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
        {
            return Spawn(prefab.gameObject, parent, position).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Transform parent) where T : Component
        {
            return Spawn(prefab.gameObject, parent).GetComponent<T>();
        }

        // --

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Vector3 scale) where T : Component
        {
            return Spawn(prefab.gameObject, null, position, rotation, scale).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Vector3 position, Vector3 scale) where T : Component
        {
            return Spawn(prefab.gameObject, null, position, scale).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Vector3 position) where T : Component
        {
            return Spawn(prefab.gameObject, null, position).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab) where T : Component
        {
            return Spawn(prefab.gameObject, null).GetComponent<T>();
        }

        // --

        public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            return Spawn(prefab, parent, position, rotation, Vector3.one);
        }
        public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Vector3 scale)
        {
            return Spawn(prefab, parent, position, Quaternion.identity, scale);
        }
        public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
        {
            return Spawn(prefab, parent, position, Quaternion.identity, Vector3.one);
        }
        public static GameObject Spawn(GameObject prefab, Transform parent)
        {
            return Spawn(prefab, parent, Vector3.zero, Quaternion.identity, Vector3.one);
        }

        // --

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return Spawn(prefab, null, position, rotation, scale);
        }
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Spawn(prefab, null, position, rotation);
        }
        public static GameObject Spawn(GameObject prefab, Vector3 position, Vector3 scale)
        {
            return Spawn(prefab, null, position, scale);
        }
        public static GameObject Spawn(GameObject prefab, Vector3 position)
        {
            return Spawn(prefab, null, position);
        }
        public static GameObject Spawn(GameObject prefab)
        {
            return Spawn(prefab, null);
        }    

        // ----- Recycle -----

        public static void Recycle<T>(T obj) where T : Component
        {
            Recycle(obj.gameObject);
        }

        public static void Recycle(GameObject obj)
        {
            GameObject prefab;
            if (instance.spawnedObjects.TryGetValue(obj, out prefab))
                Recycle(obj, prefab);
            else
                Object.Destroy(obj);
        }

        private static void Recycle(GameObject obj, GameObject prefab)
        {
            instance.pooledObjects[prefab].Add(obj);
            instance.spawnedObjects.Remove(obj);

            obj.transform.SetParent(instance.transform);
            obj.SetActive(false);
        }

        // ----- RecycleAll -----

        public static void RecycleAll<T>(T prefab) where T : Component
        {
            RecycleAll(prefab.gameObject);
        }

        public static void RecycleAll(GameObject prefab)
        {
            foreach (var item in instance.spawnedObjects)
                if (item.Value == prefab)
                    tempList.Add(item.Key);

            for (int i = 0; i < tempList.Count; ++i)
                Recycle(tempList[i]);

            tempList.Clear();
        }

        public static void RecycleAll()
        {
            tempList.AddRange(instance.spawnedObjects.Keys);

            for (int i = 0; i < tempList.Count; ++i)
                Recycle(tempList[i]);

            tempList.Clear();
        }

        // ----- IsCheck -----

        public static bool IsSpawned(GameObject obj)
        {
            return instance.spawnedObjects.ContainsKey(obj);
        }

        public static int IsCountPooled<T>(T prefab) where T : Component
        {
            return IsCountPooled(prefab.gameObject);
        }

        public static int IsCountPooled(GameObject prefab)
        {
            List<GameObject> list;
            if (instance.pooledObjects.TryGetValue(prefab, out list))
                return list.Count;

            return 0;
        }

        public static int IsCountAllPooled()
        {
            int count = 0;
            foreach (var list in instance.pooledObjects.Values)
                count += list.Count;

            return count;
        }

        public static int IsCountSpawned<T>(T prefab) where T : Component
        {
            return IsCountSpawned(prefab.gameObject);
        }

        public static int IsCountSpawned(GameObject prefab)
        {
            int count = 0;
            foreach (var instancePrefab in instance.spawnedObjects.Values)
                if (prefab == instancePrefab)
                    ++count;

            return count;
        }

        public static int IsCountAllSpawned()
        {
            return instance.spawnedObjects.Count;
        }

        // ----- DestroyPooled -----

        public static void DestroyPooled(GameObject prefab)
        {
            List<GameObject> pooled;
            if (instance.pooledObjects.TryGetValue(prefab, out pooled))
            {
                for (int i = 0; i < pooled.Count; ++i)
                    GameObject.Destroy(pooled[i]);

                pooled.Clear();
            }
        }

        public static void DestroyPooled<T>(T prefab) where T : Component
        {
            DestroyPooled(prefab.gameObject);
        }

        // ----- DestroyAll -----

        public static void DestroyAll(GameObject prefab)
        {
            RecycleAll(prefab);
            DestroyPooled(prefab);
        }

        public static void DestroyAll<T>(T prefab) where T : Component
        {
            DestroyAll(prefab.gameObject);
        }
    }

    public static class ObjectPoolExtensions
    {
        // ----- Spawn Extensions -----

        public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation, Vector3 scale) where T : Component
        {
            return ObjectPool.Spawn(prefab, position, rotation, scale);
        }
        public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return ObjectPool.Spawn(prefab, position, rotation);
        }
        public static T Spawn<T>(this T prefab, Vector3 position, Vector3 scale) where T : Component
        {
            return ObjectPool.Spawn(prefab, position, scale);
        }
        public static T Spawn<T>(this T prefab, Vector3 position) where T : Component
        {
            return ObjectPool.Spawn(prefab, position);
        }
        public static T Spawn<T>(this T prefab) where T : Component
        {
            return ObjectPool.Spawn(prefab);
        }

        // --

        public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return ObjectPool.Spawn(prefab, parent, position, rotation, scale);
        }
        public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            return ObjectPool.Spawn(prefab, parent, position, rotation);
        }
        public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Vector3 scale)
        {
            return ObjectPool.Spawn(prefab, parent, position, scale);
        }
        public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position)
        {
            return ObjectPool.Spawn(prefab, parent, position);
        }
        public static GameObject Spawn(this GameObject prefab, Transform parent)
        {
            return ObjectPool.Spawn(prefab, parent);
        }

        // --

        public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return ObjectPool.Spawn(prefab, position, rotation, scale);
        }
        public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return ObjectPool.Spawn(prefab, position, rotation);
        }
        public static GameObject Spawn(this GameObject prefab, Vector3 position, Vector3 scale)
        {
            return ObjectPool.Spawn(prefab, position, scale);
        }
        public static GameObject Spawn(this GameObject prefab, Vector3 position)
        {
            return ObjectPool.Spawn(prefab, position);
        }
        public static GameObject Spawn(this GameObject prefab)
        {
            return ObjectPool.Spawn(prefab);
        }

        // ----- Recycle Extensions -----

        public static void Recycle<T>(this T obj) where T : Component
        {
            ObjectPool.Recycle(obj);
        }

        public static void Recycle(this GameObject obj)
        {
            ObjectPool.Recycle(obj);
        }
    }
}