using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlideCameraTo : MonoBehaviour
{
    public static SlideCameraTo Instance;
    
    [SerializeField]
    Vector3 direction;

    [SerializeField]
    bool moveToOrigin;
    [SerializeField]
    float smoothTime, maxSpeed;

    Vector3 incr;

    bool isRoutRunin;
    Vector3 curVelocity;
    Vector3 targetPos;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        
        incr.x = Camera.main.pixelWidth / 50; // x*2/100
        incr.y = Camera.main.pixelHeight / 50;
        incr.z = 0;
        
        // GetComponent<Button>().onClick.AddListener(SlideCamera);
        
        if(moveToOrigin)
            SlideCamera(Vector3.zero);
    }

    public void SlideCamera()
    {
        if (!isRoutRunin)
        {
            targetPos = Camera.main.transform.position + GetIncr();
            StartCoroutine(LerpRout());
        }
        else
        {
            targetPos += GetIncr();
        }
    }

    public void SlideCamera(Vector3 newPos)
    {
        targetPos = newPos;
        
        if (!isRoutRunin)
        {
            StartCoroutine(LerpRout());
        }
    }

    IEnumerator LerpRout()
    {
        isRoutRunin = true;
        while(Vector2.Distance(Camera.main.transform.position, targetPos) > 0.01f)
        {
            Vector3 curPos = Camera.main.transform.position;
            curPos = Vector3.SmoothDamp(curPos, targetPos, ref curVelocity, smoothTime, maxSpeed, Time.smoothDeltaTime);

            var pos1 = Camera.main.transform.position;
            pos1.x = curPos.x;
            //pos1.y = curPos.y;
            pos1.y = 0;
            Camera.main.transform.position = pos1;

            yield return null;
        }

        var pos2 = Camera.main.transform.position;
        pos2.x = targetPos.x;
        //pos2.y = targetPos.y;
        pos2.y = 0;
        Camera.main.transform.position = pos2;

        isRoutRunin = false;
    }

    Vector3 GetIncr()
    {
        return new(incr.x * direction.x, incr.y * direction.y, 0);
    }
}
