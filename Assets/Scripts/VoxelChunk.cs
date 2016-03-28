using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct TerrainVoxel
{
    public Vector3 pos;
    public float health;
    public int textureID;
};

public struct VertexOut
{
    public Vector3 position;
    public Vector3 normal;
    public Vector4 color;
    public Vector2 texCoords;
};

public class VoxelChunk : MonoBehaviour {

    public Material meshMaterial;
	public int chunkWidth;
	public int chunkHeight;
	public int chunkDepth;
    public Vector3 chunkPosition;
    public bool generated;

    public TerrainVoxel[] voxelArr;

    private VoxelTerrain terrain;

    private GameObject meshGob;
    private Mesh mesh;

    public void Init(VoxelTerrain terrain, Vector3 position, Material mat, int dimSize) {
        this.terrain = terrain;
        chunkPosition = position;
        meshMaterial = mat;
        chunkWidth = dimSize;
        chunkHeight = dimSize;
        chunkDepth = dimSize;

        // initialize the child mesh game object and its components
        meshGob = new GameObject("Voxel Mesh");
        mesh = new Mesh();

        meshGob.AddComponent<MeshFilter>();
        meshGob.AddComponent<MeshRenderer>();
        meshGob.GetComponent<Renderer>().sharedMaterial = meshMaterial;
        meshGob.GetComponent<MeshFilter>().mesh = mesh;

        meshGob.transform.parent = this.transform;
        meshGob.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        meshGob.isStatic = true;

        meshGob.AddComponent<MeshCollider>();

        // initialize voxel array
        voxelArr = new TerrainVoxel[(chunkWidth) * (chunkHeight) * (chunkDepth)];
        /*for (int i = 0; i < chunkWidth; i++)
        {
            for (int j = 0; j < chunkHeight; j++)
            {
                for (int k = 0; k < chunkDepth; k++)
                {
                    TerrainVoxel v;
                    v.pos = new Vector3(i, j, k);
                    // make a flat plane
                    if (j <= chunkHeight / 2)
                        v.health = 10.0f;
                    else
                        v.health = 0.0f;
                    v.textureID = 1;
                    voxelArr[i + j * chunkWidth + k * chunkWidth * chunkHeight] = v;
                }
            }
        }*/
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator UpdateVoxelArray(TerrainVoxel[] verts, int size)
    {
        yield return null;
    }

    public IEnumerator UpdateMesh(VertexOut[] verts, int size) {

        List<Vector3> positions = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> index = new List<int>();

        int idx = 0;
        for (int i = 0; i < size; i++)
        {
            if (verts[i].color.w != -1)
            {
                positions.Add(verts[i].position);
                normals.Add(verts[i].normal);
                index.Add(idx++);
            }
        }

        mesh.Clear();
        mesh.vertices = positions.ToArray();
        mesh.normals = normals.ToArray();
        mesh.bounds = new Bounds(new Vector3(chunkWidth / 2, chunkHeight / 2, chunkDepth / 2), new Vector3(chunkWidth, chunkHeight, chunkDepth));
        mesh.SetTriangles(index.ToArray(), 0);

        meshGob.GetComponent<MeshCollider>().sharedMesh = mesh;


        yield return null;
    }

    public void BreakVoxel(Vector3 pos)
    {
        Vector3 local = meshGob.transform.InverseTransformPoint(pos);

        int ix = (int)Mathf.Round(local.x);
        int iy = (int)Mathf.Round(local.y);
        int iz = (int)Mathf.Round(local.z);

        if (ix < 0 || ix >= chunkWidth) return;
        if (iy < 0 || iy >= chunkHeight) return;
        if (iz < 0 || iz >= chunkDepth) return;

        voxelArr[(ix) + (iy) * chunkWidth + (iz) * chunkWidth * chunkHeight].health = 0.0f;

        // check if the voxel modified is an edge voxel.
        // if so, we need to update the buffers on our other chunks as well.
        if (ix == chunkWidth-1 || iy == chunkHeight-1 || iz == chunkDepth-1 || ix == 0 || iy == 0 || iz == 0)
            terrain.BreakEdgeVoxel(this, new Vector3(ix, iy, iz) + chunkPosition);

        terrain.StageForUpdate(this);
    }

    public void BreakEdgeVoxel(Vector3 pos)
    {
        Vector3 local = meshGob.transform.InverseTransformPoint(pos);

        int ix = (int)Mathf.Round(local.x);
        int iy = (int)Mathf.Round(local.y);
        int iz = (int)Mathf.Round(local.z);

        if (ix < 0 || ix >= chunkWidth) return;
        if (iy < 0 || iy >= chunkHeight) return;
        if (iz < 0 || iz >= chunkDepth) return;

        voxelArr[(ix) + (iy) * chunkWidth + (iz) * chunkWidth * chunkHeight].health = 0.0f;

        terrain.StageForUpdate(this);
    }

    public void BuildVoxel(Vector3 pos, Vector3 norm)
    {
        Vector3 local = meshGob.transform.InverseTransformPoint(pos);
        Vector3 buildPoint = local + norm / 2;

        int ix = (int)Mathf.Round(buildPoint.x);
        int iy = (int)Mathf.Round(buildPoint.y);
        int iz = (int)Mathf.Round(buildPoint.z);

        if (ix < 0 || ix >= chunkWidth) return;
        if (iy < 0 || iy >= chunkHeight) return;
        if (iz < 0 || iz >= chunkDepth) return;

        voxelArr[(ix) + (iy) * chunkWidth + (iz) * chunkWidth * chunkHeight].health = 10.0f;

        // check if the voxel modified is an edge voxel.
        // if so, we need to update the buffers on our other chunks as well.
        if (ix == chunkWidth-1 || iy == chunkHeight-1 || iz == chunkDepth-1 || ix == 0 || iy == 0 || iz == 0)
            terrain.BuildEdgeVoxel(this, new Vector3(ix, iy, iz) + chunkPosition, norm);

        terrain.StageForUpdate(this);
    }

    public void BuildEdgeVoxel(Vector3 pos, Vector3 norm)
    {
        Vector3 local = meshGob.transform.InverseTransformPoint(pos);

        int ix = (int)Mathf.Round(local.x);
        int iy = (int)Mathf.Round(local.y);
        int iz = (int)Mathf.Round(local.z);

        if (ix < 0 || ix >= chunkWidth) return;
        if (iy < 0 || iy >= chunkHeight) return;
        if (iz < 0 || iz >= chunkDepth) return;

        voxelArr[(ix) + (iy) * chunkWidth + (iz) * chunkWidth * chunkHeight].health = 10.0f;

        terrain.StageForUpdate(this);
    }

}
