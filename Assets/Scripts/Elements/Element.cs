using UnityEngine;

public class Element
{
    [SerializeField]
    protected ElementType type { get; private set; } = ElementType.NONE;
    [SerializeField]
    protected Sprite icon { get; private set; } = null;
    [SerializeField]
    protected Effect effect { get; private set; } = null;

    public bool IsElemental => type != ElementType.NONE;
}
