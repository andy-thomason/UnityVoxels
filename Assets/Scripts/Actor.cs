using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {
	public Material material;

	void make_face(
		ref int ind, ref int id,
		Vector3[] vertices, Color[] colours, Vector2[] texcoord, int[] indices,
		float i, float j, float k,
		float ui, float uj, float uk,
		float vi, float vj, float vk,
		int tx, int ty, int sx, int sy
	) {
		indices[id++] = ind + 0;
		indices[id++] = ind + 1;
		indices[id++] = ind + 2;
		indices[id++] = ind + 1;
		indices[id++] = ind + 3;
		indices[id++] = ind + 2;
		
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

	GameObject create_part(Transform parent, string name, float i, float j, float k, float scale, int u, int v, int w, int h, int d) {
		GameObject part = new GameObject ();
		part.name = name;
		part.transform.parent = parent;
		part.transform.position = parent.position;
		MeshFilter mf = part.AddComponent<MeshFilter> ();
		MeshRenderer mr = part.AddComponent<MeshRenderer> ();
		mr.material = material;

		Mesh mesh = new Mesh ();
		mesh.name = "part";
		Vector3[] vertices = new Vector3[6*4];
		Vector2[] texcoord = new Vector2[6*4];
		Color[] colours = new Color[6*4];
		int[] indices = new int[6*6];

		int id = 0, ind = 0;
		float di = w * scale;
		float dj = h * scale;
		float dk = d * scale;

		// left
		make_face(
			ref ind, ref id, vertices, colours, texcoord, indices,
			i, j+dj, k+dk, 0, 0, -dk, 0, -dj, 0, u-d, v, d, h);
		// right
		make_face(
			ref ind, ref id, vertices, colours, texcoord, indices,
			i+di, j+dj, k, 0, 0, dk, 0, -dj, 0, u+w, v, d, h);
		// bottom
		make_face(
			ref ind, ref id, vertices, colours, texcoord, indices,
			i, j, k, di, 0, 0, 0, 0, dk, u, v+h, w, d);
		// top
		make_face(
			ref ind, ref id, vertices, colours, texcoord, indices,
			i, j+dj, k+dk, di, 0, 0, 0, 0, -dk, u, v-d, w, d);
		// front
		make_face(
			ref ind, ref id, vertices, colours, texcoord, indices,
			i, j+dj, k, di, 0, 0, 0, -dj, 0, u, v, w, h);
		// back
		make_face(
			ref ind, ref id, vertices, colours, texcoord, indices,
			i+di, j+dj, k+dk, -di, 0, 0, 0, -dj, 0, u+w+d, v, w, h);

		mesh.vertices = vertices;
		mesh.uv = texcoord;
		mesh.colors = colours;
		mesh.triangles = indices;

		mf.mesh = mesh;
		return part;
	}

	GameObject head;
	GameObject body;
	GameObject larm;
	GameObject rarm;
	GameObject lleg;
	GameObject rleg;
	public GameObject camera;
	int anim_time = 0;

	public float speed = 0;
	public float strafe = 0;
	public float updown_look = 0;
	public float leftright_look = 0;
	public float y_velocity;


	// Use this for initialization
	void Start () {
		float scale = 1.0f / 40;
		body = create_part (transform, "body", -8.0f* scale, 0.0f* scale, -4.0f* scale, scale, 40, 88, 16, 32, 8);
		body.transform.Translate (new Vector3 (0, 24.0f * scale, 0));

		head = create_part (body.transform, "head", -8.0f * scale, -8.0f * scale, -8.0f * scale, scale, 16, 48, 16, 16, 16);
		head.transform.Translate (new Vector3 (0, 40.0f * scale, 0));
		larm = create_part (body.transform, "larm", -4.0f * scale, -20.0f* scale, -4.0f* scale, scale, 88, 88, 8, 24, 8);
		larm.transform.Translate (new Vector3 (-12.0f * scale, 24.0f * scale, 0));
		rarm = create_part (body.transform, "rarm", -4.0f * scale, -20.0f * scale, -4.0f * scale, scale, 88, 88, 8, 24, 8);
		rarm.transform.Translate (new Vector3 (12.0f * scale, 24.0f * scale, 0));

		lleg = create_part (body.transform, "lleg", -4.0f * scale, -32.0f* scale, -4.0f* scale, scale, 8, 88, 8, 32, 8);
		lleg.transform.Translate (new Vector3 (-5.0f * scale, 8.0f * scale, 0));
		rleg = create_part (body.transform, "rleg", -4.0f * scale, -32.0f * scale, -4.0f * scale, scale, 8, 88, 8, 32, 8);
		rleg.transform.Translate (new Vector3 (5.0f * scale, 8.0f * scale, 0));

		body.transform.Rotate (new Vector3 (0, 180, 0));

		// disable the cursor
		Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		float tdt = Time.deltaTime;
		if (Input.GetKey (KeyCode.W)) {
			speed += tdt * 2;
		} else if (Input.GetKey (KeyCode.S)) {
			speed -= tdt * 2;
		} else {
			speed *= Mathf.Exp(-tdt*5);
		}
		if (Input.GetKey (KeyCode.A)) {
			strafe -= tdt * 2;
		} else if (Input.GetKey (KeyCode.D)) {
			strafe += tdt * 2;
		} else {
			strafe *= Mathf.Exp(-tdt*5);
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			y_velocity = 4;
		} else {
			y_velocity -= 9.8f * tdt;
		}

		speed = Mathf.Clamp(speed, -1, 1);
		strafe = Mathf.Clamp(strafe, -1, 1);

		leftright_look += Input.GetAxis ("Mouse X") * 10;
		updown_look -= Input.GetAxis ("Mouse Y") * 10;

		// track mouse movment
		transform.rotation = new Quaternion (0, Mathf.Sin (leftright_look*(Mathf.PI/360)), 0, Mathf.Cos (leftright_look*(Mathf.PI/360)));
		updown_look = Mathf.Clamp (updown_look, -90, 90);

		Vector3 fwd = new Vector3 (Mathf.Sin (leftright_look * (Mathf.PI / 180)), 0, Mathf.Cos (leftright_look * (Mathf.PI / 180)));
		Vector3 right = new Vector3 (Mathf.Cos (leftright_look * (Mathf.PI / 180)), 0, -Mathf.Sin (leftright_look * (Mathf.PI / 180)));
		Vector3 up = new Vector3 (0, 1, 0);

		transform.position += fwd * speed * tdt * 10.0f + right * strafe * tdt * 10.0f + up * y_velocity * tdt;
		if (transform.position.y < 5) {
			transform.position = new Vector3(transform.position.x, 5, transform.position.z);
			y_velocity = 0;
		}

		if (camera != null) {
			//camera.transform.position = transform.position + new Vector3(0, 1.75f, 0) - fwd * 2.0f;
			camera.transform.position = transform.position + new Vector3(0, 1.75f, 0) + fwd * 0.3f;
			camera.transform.rotation = transform.rotation;
			//Debug.Log(transform.rotation + " " + fwd);
			camera.transform.Rotate(new Vector3(updown_look, 0, 0));
		}

		// animation
		float val = Mathf.Sin (anim_time * 0.1f) * (speed + strafe);
		larm.transform.localRotation = new Quaternion (Mathf.Sin (val), 0, 0, Mathf.Cos (val));
		rarm.transform.localRotation = new Quaternion (-Mathf.Sin (val), 0, 0, Mathf.Cos (val));
		lleg.transform.localRotation = new Quaternion (-Mathf.Sin (val*0.5f), 0, 0, Mathf.Cos (val*0.5f));
		rleg.transform.localRotation = new Quaternion (Mathf.Sin (val*0.5f), 0, 0, Mathf.Cos (val*0.5f));
		head.transform.localRotation = new Quaternion (-Mathf.Sin (updown_look*(Mathf.PI/360)), 0, 0, Mathf.Cos (updown_look*(Mathf.PI/360)));
		anim_time++;
	}
}
