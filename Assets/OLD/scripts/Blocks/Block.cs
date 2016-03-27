using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Block
{
	public enum Direction { up, down, south, north, west, east};

    public struct Tile { public int x; public int y;}
    const float tileSize = 0.25f;
    public bool changed = true;
	public bool transparent=false;
	public int[] blockFaceLighting = new int[6]; //each corresponds to the order layed out by direction enum
	public byte blockLightLevel;
	public WorldPos pos;
	public Chunk chunk;

    //Base block constructor
    public Block(WorldPos position)
    {
		blockLightLevel = 5;
		SetBlockFaceLighting(6);
		chunk = World.world.GetChunk(position);
		if(chunk==null){
			float multiple = Config.Env.ChunkSize;
			pos.x = Mathf.FloorToInt(position.x / multiple) * Config.Env.ChunkSize;
			pos.y = Mathf.FloorToInt(position.y / multiple) * Config.Env.ChunkSize;
			pos.z = Mathf.FloorToInt(position.z / multiple) * Config.Env.ChunkSize;
		}else{
			pos = new WorldPos(position.x-chunk.pos.x, position.y-chunk.pos.y, position.z-chunk.pos.z);
		}


    }

	///Updates meshData with the position, texture, and lighting data for this block
    public virtual MeshData Blockdata(Chunk chunk, int x, int y, int z, MeshData meshData)
    {

        meshData.useRenderDataForCol = true;

        if (!chunk.GetBlock(x, y + 1, z).IsSolid(Direction.down))
        {
            meshData = FaceDataUp(chunk, x, y, z, meshData);
			//meshData = BlockLightingUV(chunk, x, y, z, meshData);
        }

        if (!chunk.GetBlock(x, y - 1, z).IsSolid(Direction.up))
        {
			meshData = FaceDataDown(chunk, x, y, z, meshData);
			//meshData = BlockLightingUV(chunk, x, y, z, meshData);
		}
		
		if (!chunk.GetBlock(x, y, z + 1).IsSolid(Direction.south))
        {
			meshData = FaceDataNorth(chunk, x, y, z, meshData);
			//meshData = BlockLightingUV(chunk, x, y, z, meshData);
		}
		
		if (!chunk.GetBlock(x, y, z - 1).IsSolid(Direction.north))
        {
			meshData = FaceDataSouth(chunk, x, y, z, meshData);
			//meshData = BlockLightingUV(chunk, x, y, z, meshData);
		}
		
		if (!chunk.GetBlock(x + 1, y, z).IsSolid(Direction.west))
        {
			meshData = FaceDataEast(chunk, x, y, z, meshData);
			//meshData = BlockLightingUV(chunk, x, y, z, meshData);
		}
		
		if (!chunk.GetBlock(x - 1, y, z).IsSolid(Direction.east))
        {
			meshData = FaceDataWest(chunk, x, y, z, meshData);
			//meshData = BlockLightingUV(chunk, x, y, z, meshData);
		}
		
		return meshData;

    }

	///Updates meshData with the Lighting data for this block
	public virtual MeshData BlockLightingData(Chunk chunk, int x, int y, int z, MeshData meshData){
		if (!chunk.GetBlock(x, y + 1, z).IsSolid(Direction.down))
		{
			meshData = BlockLightingUV(chunk, x, y, z, GetNeighbor(chunk, Direction.down).GetBlockLightLevel(), meshData);
		}
		
		if (!chunk.GetBlock(x, y - 1, z).IsSolid(Direction.up))
		{
			meshData = BlockLightingUV(chunk, x, y, z, GetNeighbor(chunk, Direction.up).GetBlockLightLevel(), meshData);
		}
		
		if (!chunk.GetBlock(x, y, z + 1).IsSolid(Direction.south))
		{
			meshData = BlockLightingUV(chunk, x, y, z, GetNeighbor(chunk, Direction.south).GetBlockLightLevel(), meshData);
		}
		
		if (!chunk.GetBlock(x, y, z - 1).IsSolid(Direction.north))
		{
			meshData = BlockLightingUV(chunk, x, y, z, GetNeighbor(chunk, Direction.north).GetBlockLightLevel(), meshData);
		}
		
		if (!chunk.GetBlock(x + 1, y, z).IsSolid(Direction.west))
		{
			meshData = BlockLightingUV(chunk, x, y, z, GetNeighbor(chunk, Direction.west).GetBlockLightLevel(), meshData);
		}
		
		if (!chunk.GetBlock(x - 1, y, z).IsSolid(Direction.east))
		{
			meshData = BlockLightingUV(chunk, x, y, z, GetNeighbor(chunk, Direction.east).GetBlockLightLevel(), meshData);
		}
		
		return meshData;
	}

	protected virtual MeshData BlockLightingUV(Chunk chunk, int x, int y, int z, int lightLevel, MeshData meshData){
		return BlockLightingUV(chunk, new WorldPos(x, y, z), lightLevel, meshData);
	}

	protected virtual MeshData BlockLightingUV(Chunk chunk, WorldPos pos, int lightLevel, MeshData meshData){
		Vector2[] UVs = new Vector2[4];
		int sun = chunk.GetSunlight(pos);
		int point = chunk.GetPointLight(pos);
		int intensity = Math.Max(sun, point);
		if(intensity != 0){
			//Debug.Log (chunk.pos);
		}
		//int intensity = (chunk.GetSunlight(pos) > chunk.GetPointLight(pos)) ? chunk.GetSunlight(pos) : chunk.GetPointLight(pos);
		//Tile tilePos = new Tile();
		//tilePos.x = intensity%4;
		//tilePos.y = intensity/4;
		//float shadowTileSize = .25f;
		//UVs[0] = new Vector2(shadowTileSize * tilePos.x, shadowTileSize * tilePos.y);
		//UVs[1] = new Vector2(shadowTileSize * tilePos.x + shadowTileSize, shadowTileSize * tilePos.y);
		//UVs[2] = new Vector2(shadowTileSize * tilePos.x + shadowTileSize, shadowTileSize * tilePos.y);
		//UVs[3] = new Vector2(shadowTileSize * tilePos.x, shadowTileSize * tilePos.y + shadowTileSize);
		UVs[0] = new Vector2(lightLevel, 0);
		UVs[1] = new Vector2(lightLevel, 0);
		UVs[2] = new Vector2(lightLevel, 0);
		UVs[3] = new Vector2(lightLevel, 0);
		meshData.uv2.AddRange(UVs);
		return meshData;
	}
	
	protected virtual MeshData FaceDataUp
		(Chunk chunk, int x, int y, int z, MeshData meshData)
	{
		meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.up));
        return meshData;
    }

    protected virtual MeshData FaceDataDown
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.down));
        return meshData;
    }

    protected virtual MeshData FaceDataNorth
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.north));
        return meshData;
    }

    protected virtual MeshData FaceDataEast
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.east));
        return meshData;
    }

    protected virtual MeshData FaceDataSouth
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.south));
        return meshData;
    }

    protected virtual MeshData FaceDataWest
        (Chunk chunk, int x, int y, int z, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(Direction.west));
        return meshData;
    }

    public virtual Tile TexturePosition(Direction direction)
    {
        Tile tile = new Tile();
        tile.x = 0;
        tile.y = 0;

        return tile;
    }

    public virtual Vector2[] FaceUVs(Direction direction)
    {
        Vector2[] UVs = new Vector2[4];
        Tile tilePos = TexturePosition(direction);

        UVs[0] = new Vector2(tileSize * tilePos.x + tileSize, tileSize * tilePos.y);
        UVs[1] = new Vector2(tileSize * tilePos.x + tileSize, tileSize * tilePos.y + tileSize);
        UVs[2] = new Vector2(tileSize * tilePos.x, tileSize * tilePos.y + tileSize);
        UVs[3] = new Vector2(tileSize * tilePos.x, tileSize * tilePos.y);

        return UVs;
    }

    public virtual bool IsSolid(Direction direction)
    {
        switch (direction)
        {
            case Direction.north:
                return true;
            case Direction.east:
                return true;
            case Direction.south:
                return true;
            case Direction.west:
                return true;
            case Direction.up:
                return true;
            case Direction.down:
                return true;
        }

        return false;
    }

	public Block GetNeighbor(Chunk chunk, Direction direction){
		Block neighbor= this;
		switch (direction){
			case Direction.north:
				neighbor = chunk.GetBlock(pos.x, pos.y, pos.z+1);
				break;
			case Direction.east:
				neighbor = chunk.GetBlock(pos.x-1, pos.y, pos.z);
				break;
			case Direction.south:
				neighbor = chunk.GetBlock(pos.x, pos.y, pos.z-1);
				break;
			case Direction.west:
				neighbor = chunk.GetBlock(pos.x+1, pos.y, pos.z);
				break;
			case Direction.up:
				neighbor = chunk.GetBlock(pos.x, pos.y+1, pos.z);
				break;
			case Direction.down:
				neighbor = chunk.GetBlock(pos.x, pos.y-1, pos.z);
				break;
		}
		return neighbor;
	}
	
	public void SetBlockFaceLighting(int lightLevel){
		for(int i = 0; i<6;i++){
			blockFaceLighting[i] = lightLevel;
		}
	}

	public void SetBlockLightLevel(byte lightLevel){
		blockLightLevel = lightLevel;
	}

	public byte GetBlockLightLevel(){
		return blockLightLevel;
	}

	public void Break(){
		chunk.SetBlock(pos.x,pos.y,pos.z, new BlockAir(pos));
	}

	public WorldPos GetAbsolutePos(){
		WorldPos absolute = chunk.pos+this.pos;
		return absolute;
	}
}