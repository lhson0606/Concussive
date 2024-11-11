using UnityEngine;

public class Element : MonoBehaviour
{
    [SerializeField]
    protected ElementType type = ElementType.NONE;
    [SerializeField]
    protected Sprite icon = null;
    [SerializeField]
    protected Effect effect = null;

    public bool IsElemental => type != ElementType.NONE;
    public ElementType Type => type;
    public Sprite Icon => icon;
    public Effect Effect => effect;
}
