using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct WorldPos
{
    public int x, y, z;

    public WorldPos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

	public WorldPos(Vector3 pos){
		x = (int)pos.x;
		y = (int)pos.y;
		z = (int)pos.z;
	}

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 47;

            hash = hash * 227 + x.GetHashCode();
            hash = hash * 227 + y.GetHashCode();
            hash = hash * 227 + z.GetHashCode();

            return hash;
        }
    }

    public override bool Equals(object obj)
    {
        if (GetHashCode() == obj.GetHashCode())
            return true;
        return false;
    }

	public static WorldPos operator +(WorldPos p1, WorldPos p2){
		return new WorldPos(p1.x+p2.x, p1.y+p2.y, p1.z+p2.z);
	}

	public override string ToString ()
	{
		return string.Format ("[WorldPos] X:{0}, Y:{1}, Z:{2}", x, y, z);
	}
}