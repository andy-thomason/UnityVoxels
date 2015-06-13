using UnityEngine;
using System.Collections;

public class Chunk : MonoBehaviour {
	public const int lg_xdim = 4;
	public const int lg_ydim = 4;
	public const int lg_zdim = 4;
	public const int xdim = 1 << lg_xdim;
	public const int ydim = 1 << lg_ydim;
	public const int zdim = 1 << lg_zdim;
	public byte[] values = new byte[xdim*ydim*zdim];

	public bool init = true;

	public World world;

	public int gx;
	public int gy;
	public int gz;

	static byte[] textures = new byte[] {
		// left        front       right      back         top        bottom
		0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00, 
		0x01, 0x00,  0x01, 0x00,  0x01, 0x00,  0x01, 0x00,  0x02, 0x00,  0x02, 0x00, // grass
		0x02, 0x00,  0x02, 0x00,  0x02, 0x00,  0x02, 0x00,  0x02, 0x00,  0x02, 0x00, // dirt
		0x03, 0x00,  0x03, 0x00,  0x03, 0x00,  0x03, 0x00,  0x03, 0x00,  0x03, 0x00, // stone
		0x00, 0x03,  0x01, 0x03,  0x02, 0x03,  0x03, 0x03,  0x01, 0x02,  0x01, 0x04, // head
		0x03, 0x02,  0x03, 0x02,  0x03, 0x02,  0x03, 0x02,  0x03, 0x02,  0x03, 0x02, // blue
		0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00, 
		0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00, 
		0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00, 
		0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00, 
		0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00, 
		0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00,  0x00, 0x00, 
	};
	const int texture_stride = 12;

	void make_face(
		ref int ind, ref int id,
		Vector3[] vertices, Color[] colours, Vector2[] texcoord, int[] indices,
		int i, int j, int k,
		int ui, int uj, int uk,
		int vi, int vj, int vk,
		int value, int tex
	) {
		if (vertices == null) {
			id += 6;
			ind += 4;
			return;
		}
		// 0 1
		// 2 3
		indices[id++] = ind + 0;
		indices[id++] = ind + 1;
		indices[id++] = ind + 2;
		indices[id++] = ind + 1;
		indices[id++] = ind + 3;
		indices[id++] = ind + 2;


		int sx = 16, sy = 16;
		int tx = textures [value * texture_stride + tex * 2 + 0] * 16;
		int ty = textures [value * texture_stride + tex * 2 + 1] * 16;
		vertices[ind].x = i; vertices[ind].y = j; vertices[ind].z = k; 
		colours[ind].r = 1; colours[ind].g = 1; colours[ind].b = 1; colours[ind].a = 1;
		texcoord [ind].x = (tx + 0.25f) * (1.0f / 512); texcoord [ind].y = (512.0f/512) - (ty + 0.25f) * (1.0f / 512);
		ind++;
		vertices[ind].x = i+ui; vertices[ind].y = j+uj; vertices[ind].z = k+uk; 
		colours[ind].r = 1; colours[ind].g = 1; colours[ind].b = 1; colours[ind].a = 1;
		texcoord [ind].x = (tx+sx-0.25f) * (1.0f / 512); texcoord [ind].y = (512.0f/512) - (ty + 0.25f) * (1.0f / 512);
		ind++;
		vertices[ind].x = i+vi; vertices[ind].y = j+vj; vertices[ind].z = k+vk; 
		colours[ind].r = 1; colours[ind].g = 1; colours[ind].b = 1; colours[ind].a = 1;
		texcoord [ind].x = (tx + 0.25f) * (1.0f / 512); texcoord [ind].y = (512.0f/512) - (ty+sy - 0.25f) * (1.0f / 512);
		ind++;
		vertices[ind].x = i+ui+vi; vertices[ind].y = j+uj+vj; vertices[ind].z = k+uk+vk; 
		colours[ind].r = 1; colours[ind].g = 1; colours[ind].b = 1; colours[ind].a = 1;
		texcoord [ind].x = (tx+sx-0.25f) * (1.0f / 512); texcoord [ind].y = (512.0f/512) - (ty+sy - 0.25f) * (1.0f / 512);
		ind++;
	}

	void make_faces(ref int ind, ref int id, Vector3[] vertices, Vector2[] texcoord, Color[] colours, int[] indices) {
		for (int i = 0; i != xdim; ++i) {
			for (int j = 0; j != ydim; ++j) {
				for (int k = 0; k != zdim; ++k) {
					int idx = (k * ydim + j) * xdim + i;
					int value = values[idx];
					if (value != 0) {
						if (i == 0 || values[idx-1] == 0) {
							make_face(
								ref ind, ref id, vertices, colours, texcoord, indices,
								i, j+1, k+1, 0, 0, -1, 0, -1, 0, value, 0);
						}
						if (i == xdim-1 || values[idx+1] == 0) {
							make_face(
								ref ind, ref id, vertices, colours, texcoord, indices,
								i+1, j+1, k, 0, 0, 1, 0, -1, 0, value, 2);
						}
						if (j == 0 || values[idx-xdim] == 0) {
							make_face(
								ref ind, ref id, vertices, colours, texcoord, indices,
								i, j, k, 1, 0, 0, 0, 0, 1, value, 5);
						}
						if (j == ydim-1 || values[idx+xdim] == 0) {
							make_face(
								ref ind, ref id, vertices, colours, texcoord, indices,
								i, j+1, k+1, 1, 0, 0, 0, 0, -1, value, 4);
						}
						if (k == 0 || values[idx-ydim*xdim] == 0) {
							make_face(
								ref ind, ref id, vertices, colours, texcoord, indices,
								i, j+1, k, 1, 0, 0, 0, -1, 0, value, 1);
						}
						if (k == zdim-1 || values[idx+ydim*xdim] == 0) {
							make_face(
								ref ind, ref id, vertices, colours, texcoord, indices,
								i+1, j+1, k+1, -1, 0, 0, 0, -1, 0, value, 3);
						}
					}
				}
			}
		}
	}

	void rebuild() {
		int num_ind = 0;
		int num_id = 0;
		make_faces (ref num_ind, ref num_id, null, null, null, null);

		int ind = 0;
		int id = 0;
		Mesh mesh = new Mesh ();
		Vector3[] vertices = new Vector3[num_ind];
		Vector2[] texcoord = new Vector2[num_ind];
		Color[] colours = new Color[num_ind];
		int[] indices = new int[num_id];
		make_faces (ref ind, ref id, vertices, texcoord, colours, indices);

		if (num_ind != ind || num_id != id) {
			throw new System.Exception("ouch!");
		}

		mesh.vertices = vertices;
		mesh.uv = texcoord;
		mesh.colors = colours;
		mesh.triangles = indices;

		MeshFilter mf = GetComponent<MeshFilter>();
		mf.mesh = mesh;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (init) rebuild ();
		init = false;
	}
}
