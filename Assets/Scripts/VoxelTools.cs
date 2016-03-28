using UnityEngine;
using System.Collections;

public class VoxelTools : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        bool leftDown = Input.GetMouseButton(0);
        bool rightDown = Input.GetMouseButton(1);


        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 50.0f))
        {

            GameObject gob = hit.collider.gameObject;
            VoxelChunk chunk = gob.transform.parent.gameObject.GetComponent<VoxelChunk>();
            if (chunk)
            {
                //terr.DisplayTargetVoxel(hit.point, hit.normal);
                if (leftDown)
                {
                    chunk.BreakVoxel(hit.point);
                }
                if (rightDown)
                {
                    chunk.BuildVoxel(hit.point, hit.normal);
                }
            }
        }
    }
}
