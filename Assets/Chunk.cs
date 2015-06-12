using UnityEngine;
using System.Collections;

public class Chunk : MonoBehaviour {
	const int dim = 16;
	byte[] values = new byte[dim*dim*dim];
	bool init = true;

	void make_face(
		ref int ind, ref int id,
		Vector3[] vertices, Color[] colours, int[] indices,
		int i, int j, int k,
		int ui, int uj, int uk,
		int vi, int vj, int vk
	) {
		// 0 1
		// 2 3
		indices[id++] = ind + 0;
		indices[id++] = ind + 1;
		indices[id++] = ind + 2;
		indices[id++] = ind + 1;
		indices[id++] = ind + 3;
		indices[id++] = ind + 2;

		vertices[ind].x = i; vertices[ind].y = j; vertices[ind].z = k; 
		colours[ind].r = 1; colours[ind].g = 0; colours[ind].b = 0; colours[ind].a = 1;
		ind++;
		vertices[ind].x = i+ui; vertices[ind].y = j+uj; vertices[ind].z = k+uk; 
		colours[ind].r = 1; colours[ind].g = 0; colours[ind].b = 0; colours[ind].a = 1;
		ind++;
		vertices[ind].x = i+vi; vertices[ind].y = j+vj; vertices[ind].z = k+vk; 
		colours[ind].r = 1; colours[ind].g = 0; colours[ind].b = 0; colours[ind].a = 1;
		ind++;
		vertices[ind].x = i+ui+vi; vertices[ind].y = j+uj+vj; vertices[ind].z = k+uk+vk; 
		colours[ind].r = 1; colours[ind].g = 0; colours[ind].b = 0; colours[ind].a = 1;
		ind++;
	}

	void rebuild() {
		int num_faces = 0;
		for (int i = 0; i != dim; ++i) {
			for (int j = 0; j != dim; ++j) {
				for (int k = 0; k != dim; ++k) {
					int idx = (k * dim + j) * dim + i;
					if (values[idx] != 0) {
						if (i == 0 || values[idx-1] == 0) {
							num_faces++;
						}
						if (i == dim-1 || values[idx+1] == 0) {
							num_faces++;
						}
						if (j == 0 || values[idx-dim] == 0) {
							num_faces++;
						}
						if (j == dim-1 || values[idx+dim] == 0) {
							num_faces++;
						}
						if (k == 0 || values[idx-dim*dim] == 0) {
							num_faces++;
						}
						if (k == dim-1 || values[idx+dim*dim] == 0) {
							num_faces++;
						}
					}
				}
			}
		}
		
		Mesh mesh = new Mesh ();
		Vector3[] vertices = new Vector3[num_faces * 4];
		Color[] colours = new Color[num_faces * 4];
		int[] indices = new int[num_faces * 6];
		int ind = 0;
		int id = 0;
		for (int i = 0; i != dim; ++i) {
			for (int j = 0; j != dim; ++j) {
				for (int k = 0; k != dim; ++k) {
					int idx = (k * dim + j) * dim + i;
					if (values[idx] != 0) {
						if (i == 0 || values[idx-1] == 0) {
							make_face(ref ind, ref id, vertices, colours, indices, i, j, k, 0, 0, 1, 0, 1, 0);
						}
						if (i == dim-1 || values[idx+1] == 0) {
							make_face(ref ind, ref id, vertices, colours, indices, i+1, j+1, k, 0, 0, 1, 0, -1, 0);
						}
						if (j == 0 || values[idx-dim] == 0) {
							make_face(ref ind, ref id, vertices, colours, indices, i, j, k, 1, 0, 0, 0, 0, 1);
						}
						if (j == dim-1 || values[idx+dim] == 0) {
							make_face(ref ind, ref id, vertices, colours, indices, i, j+1, k+1, 1, 0, 0, 0, 0, -1);
						}
						if (k == 0 || values[idx-dim*dim] == 0) {
							make_face(ref ind, ref id, vertices, colours, indices, i, j+1, k, 1, 0, 0, 0, -1, 0);
						}
						if (k == dim-1 || values[idx+dim*dim] == 0) {
							make_face(ref ind, ref id, vertices, colours, indices, i, j, k+1, 1, 0, 0, 0, 1, 0);
						}
					}
				}
			}
		}
		mesh.vertices = vertices;
		mesh.colors = colours;
		mesh.triangles = indices;

		MeshFilter mf = GetComponent<MeshFilter>();
		mf.mesh = mesh;
	}

	// Use this for initialization
	void Start () {
		Vector3 centre = new Vector3(dim*0.5f, dim*0.5f, dim*0.5f);
		for (int i = 0; i != dim; ++i) {
			for (int j = 0; j != dim; ++j) {
				for (int k = 0; k != dim; ++k) {
					Vector3 pos = new Vector3(i, j, k) - centre;
					int idx = (k * dim + j) * dim + i;
					values[idx] = (pos.sqrMagnitude >= 7*7) ? (byte)0 : (byte)1;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (init) rebuild ();
		init = false;
	}
}
