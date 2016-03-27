﻿using UnityEngine;
using System.Collections;
using SimplexNoise;

public class TerrainGen
{

    float stoneBaseHeight = -24;
    float stoneBaseNoise = 0.05f;
    float stoneBaseNoiseHeight = 4;

    float stoneMountainHeight = 48;
    float stoneMountainFrequency = 0.008f;
    float stoneMinHeight = -12;

    float dirtBaseHeight = 1;
    float dirtNoise = 0.04f;
    float dirtNoiseHeight = 3;

    float caveFrequency = 0.025f;
    int caveSize = 7;

    float treeFrequency = 0.2f;
    int treeDensity = 3;

    public Chunk ChunkGen(Chunk chunk)
    {
        for (int x = chunk.pos.x - 3; x < chunk.pos.x + Config.Env.ChunkSize + 3; x++)//We have the padding of 3 for doing trees
        {
            for (int z = chunk.pos.z - 3; z < chunk.pos.z + Config.Env.ChunkSize + 3; z++)
            {
                chunk = ChunkColumnGen(chunk, x, z);
            }
        }
        return chunk;
    }

    public Chunk ChunkColumnGen(Chunk chunk, int x, int z)
    {
        int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);
        stoneHeight += GetNoise(x, 0, z, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight));

        if (stoneHeight < stoneMinHeight)
            stoneHeight = Mathf.FloorToInt(stoneMinHeight);

        stoneHeight += GetNoise(x, 0, z, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight));

        int dirtHeight = stoneHeight + Mathf.FloorToInt(dirtBaseHeight);
        dirtHeight += GetNoise(x, 100, z, dirtNoise, Mathf.FloorToInt(dirtNoiseHeight));

        for (int y = chunk.pos.y - 8; y < chunk.pos.y + Config.Env.ChunkSize; y++)
		//for(int y = chunk.pos.y - Config.Env.ChunkSize/2; y < chunk.pos.y + Config.Env.ChunkSize/2; y++)
        {
            //Get a value to base cave generation on
            int caveChance = GetNoise(x, y, z, caveFrequency, 100);

            if (y <= stoneHeight && caveSize < caveChance)
            {
                SetBlock(x, y, z, new Block(new WorldPos(x, y, z)), chunk);
            }
            else if (y <= dirtHeight && caveSize < caveChance)
            {
				SetBlock(x, y, z, new BlockGrass(new WorldPos(x, y, z)), chunk);
				
				if (y == dirtHeight && GetNoise(x, 0, z, treeFrequency, 100) < treeDensity)
                    CreateTree(x, y + 1, z, chunk);
            }
            else
            {
				SetBlock(x, y, z, new BlockAir(new WorldPos(x, y, z)), chunk);
			}

        }

        return chunk;
    }

    void CreateTree(int x, int y, int z, Chunk chunk)
    {
		//chunk.SetPointLight(new WorldPos(x-chunk.pos.x+1, y-chunk.pos.y+2, z-chunk.pos.z), 15);
        //create leaves
        for (int xi = -2; xi <= 2; xi++)
        {
            for (int yi = 4; yi <= 8; yi++)
            {
                for (int zi = -2; zi <= 2; zi++)
                {
					SetBlock(x + xi, y + yi, z + zi, new BlockLeaves(new WorldPos(x, y, z)), chunk, true);
                }
            }
        }

        //create trunk
        for (int yt = 0; yt < 6; yt++)
        {
			SetBlock(x, y + yt, z, new BlockWood(new WorldPos(x, y, z)), chunk, true);
        }
    }

	public static void SetBlock(int x, int y, int z, Block block, Chunk chunk){
		SetBlock (x, y, z, block, chunk, false);
	}

    public static void SetBlock(int x, int y, int z, Block block, Chunk chunk, bool replaceBlocks)
    {
        x -= chunk.pos.x;
        y -= chunk.pos.y;
        z -= chunk.pos.z;

        if (Chunk.InRange(x) && Chunk.InRange(y) && Chunk.InRange(z))
        {
            if (replaceBlocks || chunk.blocks[x, y, z] == null)
                chunk.SetBlock(x, y, z, block);
        }
    }

    public static int GetNoise(int x, int y, int z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1f) * (max / 2f));
    }
}