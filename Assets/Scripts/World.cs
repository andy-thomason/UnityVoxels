using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	public const int lg_xdim = 5;
	public const int lg_ydim = 5;
	public const int lg_zdim = 5;
	public const int xdim = 1<<lg_xdim;
	public const int ydim = 1<<lg_ydim;
	public const int zdim = 1<<lg_zdim;
	public const int gxdim = 1<<(lg_xdim+Chunk.lg_xdim);
	public const int gydim = 1<<(lg_ydim+Chunk.lg_ydim);
	public const int gzdim = 1<<(lg_zdim+Chunk.lg_zdim);
	Chunk[] chunks = new Chunk[xdim*ydim*zdim];
	public Material world_material;

	// rx + l nx = cx -> l = (cx - rx) / nx

	/*public bool RayCast(Vector3 origin, Vector3 direction, float max_distance, out int oi, out int oj, out int ok) {
		float rx = origin.x;
		float ry = origin.y;
		float rz = origin.z;
		int di = (direction.x >= 0 ? 1 : -1);
		int dj = (direction.y >= 0 ? 1 : -1);
		int dk = (direction.z >= 0 ? 1 : -1);
		float denomx = Mathf.Abs(direction.x);
		float denomy = Mathf.Abs(direction.y);
		float denomz = Mathf.Abs(direction.z);
        int i = Mathf.FloorToInt (rx);
		int j = Mathf.FloorToInt (ry);
		int k = Mathf.FloorToInt (rz);
		int ci = i >> Chunk.lg_xdim;
		int cj = j >> Chunk.lg_ydim;
		int ck = k >> Chunk.lg_zdim;
		int qi = i & (Chunk.xdim-1);
		int qj = j & (Chunk.ydim-1);
		int qk = k & (Chunk.zdim-1);

		Chunk ch = get_chunk (i, j, k);
		if (ch.GetValue (qi, qj, qk)) {
			oi = i;
			oj = j;
			ok = k;
			return true;
        }

		float numerx = i + di - rx;
		float numery = j + dj - ry;
        float numerz = k + dk - rz;

		if (numerx * denomy < numery * denomx && numerx * denomz < numerz * denomx) {
			rx = i += di;
			ry += direction.y * (numerx / denomx);
			rz += direction.z * (numerx / denomx);
		} else if (numery * denomz < numerz * denomy) {
			ry = j += dj;
			rx += direction.x * (numery / denomy);
			rz += direction.z * (numery / denomy);
		} else {
			rz = k += dk;
			rx += direction.x * (numerz / denomz);
			ry += direction.y * (numerz / denomz);
        }
    }*/
    
    public Chunk get_chunk(int ci, int cj, int ck) {
		int idx = (ck * ydim + cj) * xdim + ci;
		return chunks [idx];
	}

	public Chunk writable_chunk(int ci, int cj, int ck) {
		int idx = (ck * ydim + cj) * xdim + ci;
		//Debug.Log("make_chunk " + ci + ", " + cj + ", " + ck);
		Chunk ch = chunks [idx];
		if (ch == null) {
			GameObject obj = new GameObject("ch_" + ci + "_" + cj + "_" + ck);
			ch = obj.AddComponent<Chunk>();
			obj.AddComponent<MeshFilter>();
			MeshRenderer mr = obj.AddComponent<MeshRenderer>();
			mr.material = world_material;
			ch.transform.position = new Vector3(
				(ci-xdim/2) * Chunk.xdim,
				(cj) * Chunk.ydim,
				(ck-zdim/2) * Chunk.zdim
				);
			ch.world = this;
			chunks[idx] = ch;
		}
		ch.init = true;
		return ch;
	}

	public void set_voxel(int i, int j, int k, int value) {
		//Debug.Log("set_voxel " + i + ", " + j + ", " + k);
		int ci = i >> Chunk.lg_xdim;
		int cj = j >> Chunk.lg_ydim;
		int ck = k >> Chunk.lg_zdim;
		int qi = i & (Chunk.xdim-1);
		int qj = j & (Chunk.ydim-1);
		int qk = k & (Chunk.zdim-1);
		Chunk ch = writable_chunk (ci, cj, ck);
		int qidx = (qk * Chunk.ydim + qj) * Chunk.xdim + qi;
		ch.values [qidx] = (byte)value;
	}

	// Use this for initialization
	void Start () {
		for (int i = -20; i <= 20; ++i) {
			for (int k = -20; k <= 20; ++k) {
				set_voxel(i + gxdim/2, 4, k + gzdim/2, 1);
				set_voxel(i + gxdim/2, 3, k + gzdim/2, 2);
				set_voxel(i + gxdim/2, 2, k + gzdim/2, 2);
				set_voxel(i + gxdim/2, 1, k + gzdim/2, 3);
				set_voxel(i + gxdim/2, 0, k + gzdim/2, 3);
			}
		}
		set_voxel(1 + gxdim/2, 5, 0 + gzdim/2, 3);
		set_voxel(1 + gxdim/2, 6, 0 + gzdim/2, 3);
        /*set_voxel(1 + gxdim/2, 8, 0 + gzdim/2, 4);
		set_voxel(0 + gxdim/2, 7, 0 + gzdim/2, 5);
		set_voxel(1 + gxdim/2, 7, 0 + gzdim/2, 5);
		set_voxel(2 + gxdim/2, 7, 0 + gzdim/2, 5);
		set_voxel(1 + gxdim/2, 6, 0 + gzdim/2, 5);*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
