using UnityEngine;

public class SetUp : MonoBehaviour
{
    [SerializeField] bool lockFPS;
    [SerializeField] int frameRate;
    public void Awake()
    {
        if (lockFPS)
            Application.targetFrameRate = frameRate;
    }
}
