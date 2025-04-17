using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ZP_Tools
{
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance;
        
        private static GameObject _poolManagerObject;
        
        public static PoolManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                if (_poolManagerObject == null)
                {
                    _poolManagerObject = new GameObject("PoolManager");
                }
                
                _instance = _poolManagerObject.AddComponent<PoolManager>();
                DontDestroyOnLoad(_poolManagerObject);
                return _instance;
            }
        }

        private Dictionary<System.Type, object> _pools = new();

        public ObjectPool<T> GetPool<T>(T prefab, uint initialSize = 1, Transform parent = null) where T : MonoBehaviour
        {
            var type = typeof(T);

            if (_pools.TryGetValue(type, out var pool1))
                return (ObjectPool<T>)pool1;

            parent ??= _poolManagerObject.transform;
            var pool = new ObjectPool<T>(prefab, initialSize, parent);
            _pools[type] = pool;
            return pool;
        }
    }

    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _pool = new();
        private readonly T _prefab;
        private readonly Transform _parent;
        
        public ObjectPool(T prefab, uint initialSize = 1, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            
            for (uint i = 0; i < initialSize; i++)
            {
                InstantiateNewObject();
            }
        }

        public T Get()
        {
            if (_pool.Count == 0)
            {
                InstantiateNewObject();
            }

            T pooledObj = _pool.Dequeue();
            
            return pooledObj;
        }
        
        private void InstantiateNewObject()
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }

        public void ReturnToPool(T obj)
        {
            if (obj == null)
                return;
            
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

}
