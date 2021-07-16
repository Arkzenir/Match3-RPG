using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GemPool : MonoBehaviour
{
    public static GemPool instance;
    private Queue<GameObject> pool;
    public int size;
    public GameObject gemPrefab;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(this);
    }

    void Start()
    {
        var perGem = (Match3.instance.GetGridHeight() * Match3.instance.GetGridWidth()) / 2f;
        size = (Mathf.CeilToInt(perGem) + 1) * Match3.instance.GetLevelSO().gemList.Count;
        
        pool = new Queue<GameObject>();
        if (gemPrefab != null)
        {
            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(gemPrefab, transform, true);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }
    }

    public GameObject SpawnFromPool(Vector3 pos, Quaternion rot, Sprite sprite)
    {
        GameObject result = pool.Dequeue();

        result.SetActive(true);
        result.transform.position = pos;
        result.transform.rotation = rot;
        result.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = sprite;
        
        pool.Enqueue(result);

        return result;
    }

    public void PutInPool(GameObject obj, float delay)
    {
        StartCoroutine(WaitDestroy(obj, delay));
    }

    private IEnumerator WaitDestroy(GameObject obj,float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
