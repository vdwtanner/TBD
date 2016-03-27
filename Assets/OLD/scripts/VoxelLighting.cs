using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class VoxelLighting {
	public Queue<LightNode> lightPropogationQueue;
	public Queue<LightNode> sunlightPropogationQueue;
	Thread propogation;
	World world;
	bool propogatingLight=false;

	public struct LightNode{
		public LightNode(short indx, Chunk ch){index=indx; chunk = ch;}
		public short index;//Linearize WorldPos according to: index = y * chunkWidth * chunkWidth + z * chunkWidth + x
		public Chunk chunk;
	}

	public VoxelLighting(World world){
		lightPropogationQueue = new Queue<LightNode>();
		sunlightPropogationQueue = new Queue<LightNode>();
		this.world = world;
		propogation = new Thread(() => {PropogateLight(); /*PropogateSunlight();*/ propogation.Abort();});
	}

	//Checks if lgiht propogation or deletion is needed and then starts threads to do each if needed
	public void CheckAndThread(){
		if((lightPropogationQueue.Count>0 /*|| sunlightPropogationQueue.Count>0*/) && !propogatingLight){
			propogatingLight = true;
			propogation = new Thread(() => {PropogateLight(); /*PropogateSunlight();*/ propogation.Abort();});
			propogation.Start();
		}
	}

	void PropogateLight(){
		while(lightPropogationQueue.Count > 0){
			LightNode node = lightPropogationQueue.Dequeue();

			int index = node.index;
			Chunk chunk = node.chunk;

			int x = index % Config.Env.ChunkSize;
			int y = index /(Config.Env.ChunkSize * Config.Env.ChunkSize);
			int z = (index %(Config.Env.ChunkSize * Config.Env.ChunkSize))/Config.Env.ChunkSize;


			int lightLevel = chunk.GetPointLight(new WorldPos(x,y,z));
			if(lightLevel<=1){
				continue;
			}
			//Left
			WorldPos block = new WorldPos(x-1, y, z);
			PropogateLightForBlock(chunk, block, lightLevel);
			//Right
			block = new WorldPos(x+1, y, z);
			PropogateLightForBlock(chunk, block, lightLevel);
			//Up
			block = new WorldPos(x, y+1, z);
			PropogateLightForBlock(chunk, block, lightLevel);
			//Down
			block = new WorldPos(x, y-1, z);
			PropogateLightForBlock(chunk, block, lightLevel);
			//Forward
			block = new WorldPos(x, y, z+1);
			PropogateLightForBlock(chunk, block, lightLevel);
			//Backward
			block = new WorldPos(x, y, z-1);
			PropogateLightForBlock(chunk, block, lightLevel);
		}
		propogatingLight =false;
	}

	bool PropogateLightForBlock(Chunk chunk, WorldPos block, int lightLevel){
		Block b = chunk.GetBlock(block);
		if(b==null){
			return false;
		}
		if(b.transparent && chunk.GetPointLight(block) + 2 <= lightLevel){
			if(!chunk.SetPointLight(block, lightLevel-1)){
				return false;
			}
			/*short index = (short)(block.y * Config.Env.ChunkSize * Config.Env.ChunkSize
			                           + block.z * Config.Env.ChunkSize
			                           + block.x);*/
			//lightPropogationQueue.Enqueue(new LightNode(index, chunk));
			return true;
		}
		return false;
	}

	bool PropogateSunlightForBlock(Chunk chunk, WorldPos block, int lightLevel, bool down){
		Block b = chunk.GetBlock(block);
		if(b==null){
			return false;
		}
		if(b.transparent ){
			if(lightLevel==15 && down){
				chunk.SetSunlight(block, 15);
				short index = (short)(block.y * Config.Env.ChunkSize * Config.Env.ChunkSize
				                      + block.z * Config.Env.ChunkSize
				                      + block.x);
				sunlightPropogationQueue.Enqueue(new LightNode(index, chunk));
				return true;
			}
			if(chunk.GetSunlight(block) + 2 <= lightLevel){
				chunk.SetSunlight(block, lightLevel-1);
				short index = (short)(block.y * Config.Env.ChunkSize * Config.Env.ChunkSize
				                      + block.z * Config.Env.ChunkSize
				                      + block.x);
				sunlightPropogationQueue.Enqueue(new LightNode(index, chunk));
				return true;
			}
			return false;
		}
		return false;
	}
	
	void PropogateSunlight(){
		while(sunlightPropogationQueue.Count > 0){
			LightNode node = sunlightPropogationQueue.Dequeue();
			int index = node.index;
			Chunk chunk = node.chunk;
			if(chunk == null){
				//sunlightPropogationQueue.Enqueue(node);
				continue;
			}
				

			int x = index % Config.Env.ChunkSize;
			int y = index /(Config.Env.ChunkSize * Config.Env.ChunkSize);
			int z = (index %(Config.Env.ChunkSize * Config.Env.ChunkSize))/Config.Env.ChunkSize;

			int lightLevel = chunk.GetSunlight(new WorldPos(x,y,z));
			if(lightLevel<=0){
				continue;
			}
			//Left
			WorldPos block = new WorldPos(x-1, y, z);
			PropogateSunlightForBlock(chunk, block, lightLevel, false);
			//Right
			block = new WorldPos(x+1, y, z);
			PropogateSunlightForBlock(chunk, block, lightLevel, false);
			//Up - Not needed for sunlight
			//block = new WorldPos(x, y+1, z);
			//PropogateLightForBlock(chunk, block, lightLevel);
			//Down - Special case defined for sunlight that is moving down
			block = new WorldPos(x, y-1, z);
			PropogateSunlightForBlock(chunk, block, lightLevel, true);
			//Forward
			block = new WorldPos(x, y, z+1);
			PropogateSunlightForBlock(chunk, block, lightLevel, false);
			//Backward
			block = new WorldPos(x, y, z-1);
			PropogateSunlightForBlock(chunk, block, lightLevel, false);
		}
	}
}
