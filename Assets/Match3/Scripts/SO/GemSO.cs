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
    }

    public int damage = 10;
    public GemColor color;
    public Sprite sprite;

}
