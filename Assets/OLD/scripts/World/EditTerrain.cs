using UnityEngine;
using System.Collections;

public static class EditTerrain
{
    public static WorldPos GetBlockPos(Vector3 pos)
    {
        WorldPos blockPos = new WorldPos(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.y),
            Mathf.RoundToInt(pos.z)
            );

        return blockPos;
    }

	public static WorldPos GetBlockPos(RaycastHit hit){
		return GetBlockPos(hit, false);
	}

    public static WorldPos GetBlockPos(RaycastHit hit, bool adjacent)
    {
        Vector3 pos = new Vector3(
            MoveWithinBlock(hit.point.x, hit.normal.x, adjacent),
            MoveWithinBlock(hit.point.y, hit.normal.y, adjacent),
            MoveWithinBlock(hit.point.z, hit.normal.z, adjacent)
            );
        return GetBlockPos(pos);
    }

	static float MoveWithinBlock(float pos, float norm){
		return MoveWithinBlock(pos, norm, false);
	}

    static float MoveWithinBlock(float pos, float norm, bool adjacent)
    {
        if (pos - (int)pos == 0.5f || pos - (int)pos == -0.5f)
        {
            if (adjacent)
            {
                pos += (norm / 2);
            }
            else
            {
                pos -= (norm / 2);
            }
        }

        return (float)pos;
    }

	public static bool SetBlock(RaycastHit hit, Block block){
		return SetBlock (hit, block, false);
	}

    public static bool SetBlock(RaycastHit hit, Block block, bool adjacent)
    {
        Chunk chunk = hit.collider.GetComponent<Chunk>();
        if (chunk == null)
            return false;

        WorldPos pos = GetBlockPos(hit, adjacent);

        chunk.world.SetBlock(pos.x, pos.y, pos.z, block);

        return true;
    }

	public static bool SetBlockPointLight(RaycastHit hit, int intensity){
		Chunk chunk = hit.collider.GetComponent<Chunk>();
		if (chunk == null)
			return false;
		
		WorldPos pos = GetBlockPos(hit, true);
		pos.x -= chunk.pos.x;
		pos.y -= chunk.pos.y;
		pos.z -= chunk.pos.z;
		//pos -= hit.normal;
		chunk.SetPointLight(pos, intensity);
		
		return true;
	}

	public static Block GetBlock(RaycastHit hit){
		return GetBlock (hit, false);
	}

    public static Block GetBlock(RaycastHit hit, bool adjacent)
    {
        Chunk chunk = hit.collider.GetComponent<Chunk>();
        if (chunk == null)
            return null;

        WorldPos pos = GetBlockPos(hit, adjacent);
        Block block = chunk.world.GetBlock(pos.x, pos.y, pos.z);
		if(adjacent){
			Debug.Log(pos +" " + block.pos);
		}
		if(block==null){
			Debug.Log("Null block...");
		}
        return block;
    }
}