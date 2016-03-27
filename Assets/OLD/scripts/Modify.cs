using UnityEngine;
using System.Collections;

public class Modify : MonoBehaviour
{

    Vector2 rot;
	World world;
	public Transform cube;

	void Start(){
		world = GameObject.Find("World").GetComponent<World>();
	}

    void Update()
    {
		RaycastHit hit;
		if (Physics.Raycast(transform.position+Vector3.back, transform.forward, out hit, 15))
		{
			Block b = EditTerrain.GetBlock(hit, true);
			if(b==null){
				return;
			}
			WorldPos block = b.GetAbsolutePos();
			cube.transform.position = new Vector3(block.x, block.y, block.z);
			//Chunk chunk = EditTerrain.get
		}

        if (Input.GetKeyDown(KeyCode.E))
        {
            //RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                EditTerrain.GetBlock(hit).Break();
            }
        }
		if((Input.GetKeyDown(KeyCode.F))){
			//RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
			{
				EditTerrain.SetBlockPointLight(hit, 15);
			}
		}
    }
}