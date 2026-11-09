using System.Collections.Generic;
using UnityEngine;

public class SimplePool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform container;
    private readonly Stack<T> inactive = new Stack<T>();

    public SimplePool(T prefab, int prewarmCount, string containerName)
    {
        this.prefab = prefab;
        container = new GameObject(containerName).transform;

        for (int i = 0; i < prewarmCount; i++)
        {
            inactive.Push(Create());
        }
    }

    private T Create()
    {
        container.gameObject.SetActive(false);
        T obj = Object.Instantiate(prefab, container);
        obj.gameObject.SetActive(false);
        container.gameObject.SetActive(true);
        return obj;
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj = inactive.Count > 0 ? inactive.Pop() : Create();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(T obj)
    {
        if (!obj.gameObject.activeSelf) return;

        obj.gameObject.SetActive(false);
        inactive.Push(obj);
    }
}