using UnityEngine;

public class MoveObjectToNextIncr : MonoBehaviour
{
    [SerializeField]
    int iter;

    [SerializeField]
    Vector3 direction;

    public void Move()
    {
        transform.position = GetIncr();
    }

    Vector3 GetIncr()
    {
        Vector3 incr;

        incr.x = Camera.main.pixelWidth / 50; // x*2/100
        incr.y = Camera.main.pixelHeight / 50;
        incr.z = 0;

        return new(incr.x * iter * direction.x, incr.y * iter * direction.y, 0);
    }
}
