using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockAir : Block
{
    public BlockAir(WorldPos position)
        : base(position)
    {
		transparent=true;
    }

    public override MeshData Blockdata(Chunk chunk, int x, int y, int z, MeshData meshData)
    {		
		return meshData;
    }

	public override MeshData BlockLightingData(Chunk chunk, int x, int y, int z, MeshData meshData)
	{
		return meshData;
	}

    public override bool IsSolid(Block.Direction direction)
    {
        return false;
    }
}