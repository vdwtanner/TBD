using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : PregenerationGeneratorBase {
    private int m_numNodesPerCell;
	private const float minDistBetweenNodes = .05f;

    public ResourceGenerator(int numNodesPerCell)
    {
        m_numNodesPerCell = numNodesPerCell;
		m_random = new Random(PreGenerationManager.Instance.seed);
    }

	public override void Generate()
	{
		Vector2[] points = PlacePoints();

	}

	/// <summary>
	/// Creates an initial array of points. m_numNodesPerCell are generated per cell.
	/// Cells are processed column-first.
	/// </summary>
	/// <remarks>
	/// Points are indexed as follows:
	/// points[(row*size + col) * m_numNodesPerCell + currentNodeNum]
	/// </remarks>
	/// <returns></returns>
	protected virtual Vector2[] PlacePoints()
	{
		int size = PreGenerationManager.Instance.size;  //The number of cells we are generating for
		Vector2[] points = new Vector2[size*size*m_numNodesPerCell];
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				for(int nodeNum = 0; nodeNum < m_numNodesPerCell; nodeNum++)
				{
					points[(x*size + y) * m_numNodesPerCell + nodeNum] = new Vector2(x + m_random.value, y + m_random.value);
				}
			}
		}
		return points;
	}

	/*protected virtual List<Vector2> PlacePointsPoisson()
	{
		int size = PreGenerationManager.Instance.size;  //The number of cells we are generating for
		List<Vector2> points = new List<Vector2>((int)(m_numNodesPerCell * .5f) * size * size);//Preallocate space for lots of points (half of what is probably needed, under the assumption that many will be dropped
		Queue<Vector2> processQueue = new Queue<Vector2>();

	}*/
}
