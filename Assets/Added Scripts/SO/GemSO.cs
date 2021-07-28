using UnityEngine;

[CreateAssetMenu()]
public class GemSO : ScriptableObject {

    public enum GemColor
    {
        Blue,
        Green,
        Orange,
        Purple,
        Red,
        Booster,
    }
    
    public enum GemType
    {
        Standard,
        VerticalRocket,
        HorizontalRocket,
        Star,
        Bomb,
        Crystal,
    }

    public int damage = 10;
    public Booster booster;
    
    public GemColor color;
    public GemType type;
    public Sprite sprite;

}
