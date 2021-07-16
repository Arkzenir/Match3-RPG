using UnityEngine;

[CreateAssetMenu()]
public class HeroSO : EntitySO
{
   
    public GemSO.GemColor associatedGem;
    public int maxCharge = 100;
    public int chargeIncrease = 10;
}
