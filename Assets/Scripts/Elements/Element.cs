using UnityEngine;

public class Element : MonoBehaviour
{
    [SerializeField]
    protected ElementType type = ElementType.NONE;
    [SerializeField]
    protected Effect effect = null;

    public bool IsElemental => type != ElementType.NONE;
    public ElementType Type => type;
    public Effect Effect => effect;
    public bool AttachToTarget => effect.AttachToTarget;
}
