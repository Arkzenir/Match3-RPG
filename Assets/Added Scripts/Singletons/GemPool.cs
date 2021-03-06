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
        size = (Mathf.CeilToInt(perGem)) * Match3.instance.GetLevelSO().gemList.Count;
        
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

    public GameObject SpawnFromPool(Vector3 pos, Quaternion rot, GemSO gem)
    {
        GameObject result = pool.Dequeue();

        
        Color gemColor = Color.white;
        
        result.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = gem.sprite;
        result.transform.Find("sprite").GetComponent<SpriteRenderer>().color = gemColor;
        
        if (gem.type != GemSO.GemType.Standard)
        {
            switch (gem.color)
            {
                case GemSO.GemColor.Blue:
                    gemColor = Color.blue;
                    break;
                case GemSO.GemColor.Green:
                    gemColor = Color.green;
                    break;
                case GemSO.GemColor.Orange:
                    gemColor = Color.yellow;
                    break;
                case GemSO.GemColor.Purple:
                    gemColor = Color.magenta;
                    break;
                case GemSO.GemColor.Red:
                    gemColor = Color.red;
                    break;
            }
            
            result.transform.Find("sprite").GetComponent<SpriteRenderer>().color = gemColor;
        }
        
        result.transform.position = pos;
        result.transform.rotation = rot;
        result.SetActive(true);

        return result;
    }
    

    public void PutInPool(GameObject obj, float delay)
    {
        obj.transform.Find("defaultParticles").gameObject.SetActive(false);
        StartCoroutine(WaitDestroy(obj, delay));
    }
    
    
    private IEnumerator WaitDestroy(GameObject obj,float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        pool.Enqueue(obj);
    }
}
