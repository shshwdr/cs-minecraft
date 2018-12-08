using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIO : Singleton<PlayerIO>
{
    Camera camera;
    public float maxDistance = 5;
    public byte selectedBrickByte = 0;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, .5f, .5f));
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, maxDistance))
            {
                Chunk chunk = hit.transform.GetComponent<Chunk>();
                if (!chunk)
                {
                    Debug.LogError("we hit something that is not a chunk: " + hit.transform.name);
                    return;
                }
                Vector3 p = hit.point;
                //hack for brick height
                p.y /= World.Instance.brickHeight;
                if (selectedBrickByte == 0)
                {
                    p -= hit.normal / 4f;
                    selectedBrickByte = chunk.GetByte(p);
                    chunk.SetBrick(0, p);
                } else
                {
                    p += hit.normal / 4f;
                    chunk.SetBrick(selectedBrickByte, p);
                    selectedBrickByte = 0;
                }
                Debug.Log(chunk.GetByte(p)+" byte on "+hit.transform.name);
            }
            else
            {
                Debug.Log("we didn't hit anything");
            }
            
        }
    }
}
