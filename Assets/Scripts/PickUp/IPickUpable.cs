using UnityEngine;

public interface IPickUpable
{
    void OnSelect(BaseCharacter target);
    void OnDeselect();
    void OnPickUp();
}
