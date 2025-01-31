using UnityEngine;

public class DeadlockCautionSingleton : MonoBehaviour
{
    public static DeadlockCautionSingleton Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
