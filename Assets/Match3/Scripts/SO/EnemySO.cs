using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class EnemySO : EntitySO
{
    [SerializeField, SerializeReference]
    public int size;
    public Sprite sprite;

}
