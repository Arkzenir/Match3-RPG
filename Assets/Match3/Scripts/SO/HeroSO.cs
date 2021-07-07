using UnityEngine;

[CreateAssetMenu()]
public class HeroSO : EntitySO
{
   
    public GemSO.GemColor associatedGem;
    [SerializeField, SerializeReference]
    public int maxCharge = 100;
    public int chargeIncrease = 10;

    public Sprite sprite;
}
