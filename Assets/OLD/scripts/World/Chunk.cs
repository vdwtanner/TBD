using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{


	public Block[, ,] blocks = new Block[Config.Env.ChunkSize, Config.Env.ChunkSize, Config.Env.ChunkSize];
	/// <summary>
	/// Lightmpa data is stored as lightmap[y,x,z] so that we can iterate by y value in a cache friendly way.
	/// </summary>
	public byte[,,] lightmap = new byte[Config.Env.ChunkSize, Config.Env.ChunkSize, Config.Env.ChunkSize]; //MSBs are poinbtlights and LSBs are sunlight
	bool updateLighting = false;

	
    public bool update = false;
    public bool rendered;

	MeshData meshData;
    MeshFilter filter;
    MeshCollider coll;

    public World world;
    public WorldPos pos;

    void Start()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
    }

    //Update is called once per frame
    void Update()
    {
        if (update)
        {
            update = false;
            UpdateChunk();
        }
		if(updateLighting){
			updateLighting = false;
			UpdateLighting();
		}
    }

	public Block GetBlock(WorldPos pos){
		return GetBlock(pos.x, pos.y, pos.z);
	}

    public Block GetBlock(int x, int y, int z)
    {
        if (InRange(x) && InRange(y) && InRange(z))
            return blocks[x, y, z];
        return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
    }

    public static bool InRange(int index)
    {
        if (index < 0 || index >= Config.Env.ChunkSize)
            return false;

        return true;
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        if (InRange(x) && InRange(y) && InRange(z))
        {
            blocks[x, y, z] = block;
        }
        else
        {
            world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
        }
    }

    public void SetBlocksUnmodified()
    {
        foreach (Block block in blocks)
        {
            block.changed = false;
        }
    }

    // Updates the chunk based on its contents
    void UpdateChunk()
    {
        rendered = true;
        meshData = new MeshData();

        for (int x = 0; x < Config.Env.ChunkSize; x++)
        {
            for (int y = 0; y < Config.Env.ChunkSize; y++)
            {
                for (int z = 0; z < Config.Env.ChunkSize; z++)
                {
                    meshData = blocks[x, y, z].Blockdata(this, x, y, z, meshData);
					meshData = blocks[x, y, z].BlockLightingData(this, x, y, z, meshData);
                }
            }
        }
		updateLighting=false;
        RenderMesh(meshData);
    }

	void UpdateLighting(){
		UpdateChunk ();
		if(meshData == null){
			return;
		}
		meshData.uv2 = new List<Vector2>();
		for (int x = 0; x < Config.Env.ChunkSize; x++)
		{
			for (int y = 0; y < Config.Env.ChunkSize; y++)
			{
				for (int z = 0; z < Config.Env.ChunkSize; z++)
				{
					meshData = blocks[x, y, z].BlockLightingData(this, x, y, z, meshData);
				}
			}
		}
		updateLighting = false;
		RenderMesh (meshData);
	}
	
	// Sends the calculated mesh information
	// to the mesh and collision components
	void RenderMesh(MeshData meshData)
    {
        filter.mesh.Clear();
        filter.mesh.vertices = meshData.vertices.ToArray();
        filter.mesh.triangles = meshData.triangles.ToArray();
		if(filter.mesh.vertices.Length != meshData.uv2.ToArray().Length){
			Debug.LogError("IDK how this is happening");
		}
        filter.mesh.uv = meshData.uv.ToArray();
		filter.mesh.uv2 = meshData.uv2.ToArray();
        filter.mesh.RecalculateNormals();

        coll.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.colVertices.ToArray();
        mesh.triangles = meshData.colTriangles.ToArray();
        mesh.RecalculateNormals();

        coll.sharedMesh = mesh;
    }

	/*////\\\\\
	//LIGHTING\\
	\\\\\\////*/
	
	/// Gets the sunlight (lightmap bits XXXX0000).
	public int GetSunlight(WorldPos blockPos){
		if(blockPos.y == Config.Env.WorldMaxY){
			return 15;
		}
		if (InRange(blockPos.x) && InRange(blockPos.y) && InRange(blockPos.z))
			return (lightmap[blockPos.y, blockPos.x, blockPos.z] >> 4) & 0xF;
		Chunk chunk = world.GetChunk(pos.x + blockPos.x, pos.y + blockPos.y, pos.z + blockPos.z);
		if(chunk == null)
			return 0;
		return chunk.GetSunlight(blockPos);
	}
	
	/// Sets the sunlight (lightmap bits XXXX0000).
	public void SetSunlight(WorldPos blockPos, int intensity){
		if (InRange(blockPos.x) && InRange(blockPos.y) && InRange(blockPos.z)){
			lightmap[blockPos.y,blockPos.x,blockPos.z] = (byte)((lightmap[blockPos.y, blockPos.x, blockPos.z] & 0xF) | (intensity << 4));
			updateLighting=true;
		}
		else{
			Chunk chunk = world.GetChunk(pos.x + blockPos.x, pos.y + blockPos.y, pos.z + blockPos.z);
			if(chunk != null)
				chunk.SetSunlight(blockPos, intensity);
		}
	}

	///Build sunlight into top layer of chunk
	public void BuildSunlight(){
		for(int x=0; x<Config.Env.ChunkSize; x++){
			for(int z=0; z<Config.Env.ChunkSize; z++){
				SetSunlight (new WorldPos(x, Config.Env.ChunkSize-1, z), 15);
				/*short index = (short)((Config.Env.ChunkSize-1) * Config.Env.ChunkSize * Config.Env.ChunkSize
				                      + z * Config.Env.ChunkSize
				                      + x);*/
				//world.voxelLighting.sunlightPropogationQueue.Enqueue(new VoxelLighting.LightNode(index, this));
			}
		}
		//updateLighting=true;
	}

	/// Gets the point light (lightmap bits XXXX0000).
	public int GetPointLight(WorldPos blockPos){
		if (InRange(blockPos.x) && InRange(blockPos.y) && InRange(blockPos.z))
			return lightmap[blockPos.y, blockPos.x, blockPos.z] & 0xF;
		Chunk chunk = world.GetChunk(pos.x + blockPos.x, pos.y + blockPos.y, pos.z + blockPos.z);
		if(chunk == null)
			return 0;
		return chunk.GetPointLight(blockPos);

	}
	
	/// Sets the point light (lightmap bits XXXX0000).
	public bool SetPointLight(WorldPos blockPos, int intensity){
		if (InRange(blockPos.x) && InRange(blockPos.y) && InRange(blockPos.z)){
			GetBlock(blockPos).SetBlockLightLevel((byte)intensity);
			lightmap[blockPos.y,blockPos.x,blockPos.z] = (byte)((lightmap[blockPos.y, blockPos.x, blockPos.z] & 0xF0) | (intensity));
			short index = (short)(blockPos.y * Config.Env.ChunkSize * Config.Env.ChunkSize
			                      + blockPos.z * Config.Env.ChunkSize
			                      + blockPos.x);
			world.voxelLighting.lightPropogationQueue.Enqueue(new VoxelLighting.LightNode(index, this));
			updateLighting=true;
			return true;
		}else{
			Chunk chunk = world.GetChunk(pos.x + blockPos.x, pos.y + blockPos.y, pos.z + blockPos.z);
			if(chunk != null){
				chunk.SetPointLight(blockPos, intensity);
				return true;
			}
			return false;
		}

	}
}