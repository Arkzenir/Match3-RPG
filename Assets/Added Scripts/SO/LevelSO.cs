using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject {
    
    public List<GemSO> gemList;
    public List<GemSO> boosterList;
    public List<EnemySO> enemyList;
    public List<HeroSO> heroList;
    public int width;
    public int height;

}
