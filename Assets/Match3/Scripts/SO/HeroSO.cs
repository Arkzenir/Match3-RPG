using UnityEngine;

[CreateAssetMenu()]
public class HeroSO : ScriptableObject
{
    // Start is called before the first frame update
    public GemSO.GemColor associatedGem;
    public int health;
    public int maxCharge = 100;
    public int chargeIncrease = 10;
}
