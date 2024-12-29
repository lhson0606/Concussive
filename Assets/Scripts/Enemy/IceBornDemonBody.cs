using System;
using UnityEngine;

public class IceBornDemonBody : MonoBehaviour
{
    public event Action OnLaunchIceImpaler;

    // Call from animation
    public void Launch()
    {
        OnLaunchIceImpaler?.Invoke();
    }
}
