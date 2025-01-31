using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeMenuButtonManager : MonoBehaviour
{
    [SerializeField] GameObject levelMenuQuad;
    [SerializeField] GameObject homeMenuQuad;

    [SerializeField] float smoothTimeCamera;

    bool routRunin;

    Vector3 newPos, levelMenuPos, homeMenuPos;
    Vector3 velocity;

    Transform cam;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    private void Start()
    {
        levelMenuPos = levelMenuQuad.transform.position;
        levelMenuPos.z = cam.position.z;
        homeMenuPos = homeMenuQuad.transform.position;
        homeMenuPos.z = cam.position.z;
    }

    private void Update()
    {
        if(routRunin)
        {
            if (Mathf.Abs(newPos.x - cam.position.x) > 0.02f)
            {
                cam.position = Vector3.SmoothDamp(cam.position, newPos, ref velocity, smoothTimeCamera);
            }
            else
            {
                cam.position = newPos;

                routRunin = false;
            }
        }

    }

    void MoveCamera(Vector3 _newPos)
    {
        newPos = _newPos;
        
        if (!routRunin)
        {
            routRunin = true;
        }
    }

    public void GoToLevelMenu()
    {
        MoveCamera(levelMenuPos);
    }

    public void GoToHomeMenu()
    {
        MoveCamera(homeMenuPos);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
