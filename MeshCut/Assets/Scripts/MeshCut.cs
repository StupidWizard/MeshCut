using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class MeshCut{
	public class MeshCutSide{

		public List<Vector3>  vertices  = new List<Vector3>();
		public List<Vector3>  normals   = new List<Vector3>();
		public List<Vector2>  uvs       = new List<Vector2>();
		public List<int>      triangles = new List<int>();
		public List<List<int>> subIndices = new List<List<int>>();


		public void ClearAll(int length) {
			if (vertices.Capacity < length) {
				vertices = new List<Vector3>(length);
			} else {
				vertices.Clear();
			}

			if (normals.Capacity < length) {
				normals = new List<Vector3>(length);
			} else {
				normals.Clear();
			}

			if (uvs.Capacity < length) {
				uvs = new List<Vector2>(length);
			} else {
				uvs.Clear();
			}

			if (triangles.Capacity < length) {
				triangles = new List<int>(length);
			} else {
				triangles.Clear();
			}

			subIndices.Clear();
		}

		int base_index;
		int base_index1;
		int base_index2;
		public void AddTriangle( int p1, int p2, int p3, int submesh){

			// triangle index order goes 1,2,3,4....

			base_index = vertices.Count;
			base_index1 = vertices.Count+1;
			base_index2 = vertices.Count+2;

			subIndices[submesh].Add(base_index);
			subIndices[submesh].Add(base_index1);
			subIndices[submesh].Add(base_index2);

			triangles.Add(base_index);
			triangles.Add(base_index1);
			triangles.Add(base_index2);

			vertices.Add(victim_mesh.vertices[p1]);
			vertices.Add(victim_mesh.vertices[p2]);
			vertices.Add(victim_mesh.vertices[p3]);

			if (useNormals) {
				normals.Add(victim_mesh.normals[p1]);
				normals.Add(victim_mesh.normals[p2]);
				normals.Add(victim_mesh.normals[p3]);
			}

			uvs.Add(victim_mesh.uv[p1]);
			uvs.Add(victim_mesh.uv[p2]);
			uvs.Add(victim_mesh.uv[p3]);

		}

		public void AddTriangle(Vector3[] points3, Vector3[] normals3, Vector2[] uvs3, Vector3 faceNormal, int submesh){

			Vector3 calculated_normal = Vector3.Cross((points3[1] - points3[0]).normalized, (points3[2] - points3[0]).normalized);

			int p1 = 0;
			int p2 = 1;
			int p3 = 2;

			if(Vector3.Dot(calculated_normal, faceNormal) < 0){

				p1 = 2;
				p2 = 1;
				p3 = 0;
			}

			int base_index = vertices.Count;

			subIndices[submesh].Add(base_index);
			subIndices[submesh].Add(base_index+1);
			subIndices[submesh].Add(base_index+2);

			triangles.Add(base_index);
			triangles.Add(base_index+1);
			triangles.Add(base_index+2);

			vertices.Add(points3[p1]);
			vertices.Add(points3[p2]);
			vertices.Add(points3[p3]);

			if (useNormals) {
				normals.Add(normals3[p1]);
				normals.Add(normals3[p2]);
				normals.Add(normals3[p3]);
			}

			uvs.Add(uvs3[p1]);
			uvs.Add(uvs3[p2]);
			uvs.Add(uvs3[p3]);
		}

	}

	//		private static bool useMeshTangents;
	//		private static bool useVertexColors;
	private static bool useNormals;

	//		private static MeshCutSide left_side = new MeshCutSide();
	//		private static MeshCutSide right_side = new MeshCutSide();

	private static Plane blade;
	private static Mesh victim_mesh;

	// capping stuff
	private static List<Vector3> new_vertices = new List<Vector3>();



	public static List<Vector3>[]  vertices;//  = new List<Vector3>();
	public static List<Vector3>[]  normals;//   = new List<Vector3>();
	public static List<Vector2>[]  uvs;//       = new List<Vector2>();
	public static List<int>[]      triangles;// = new List<int>();
	public static List<List<int>>[] subIndices;// = new List<List<int>>();


	private static void InitBuffer(int trianglesNum, int verticesNum, int nSubMeshCount) {
		if (triangles == null || triangles[0].Capacity < trianglesNum) {
			triangles = new[] { new List<int>(trianglesNum), new List<int>(trianglesNum) };
		}
		else {
			triangles[0].Clear();
			triangles[1].Clear();
		}

		if (vertices == null || vertices[0].Capacity < verticesNum) {
			vertices = new[] { new List<Vector3>(verticesNum), new List<Vector3>(verticesNum) };
			normals = new[] { new List<Vector3>(verticesNum), new List<Vector3>(verticesNum) };
			uvs = new[] { new List<Vector2>(verticesNum), new List<Vector2>(verticesNum) };
		}
		else {
			for (int i = 0; i < 2; i++) {
				vertices[i].Clear();
				normals[i].Clear();
				uvs[i].Clear();
			}
		}

		subIndices = new List<List<int>>[2];
		subIndices[0] = new List<List<int>>();
		subIndices[1] = new List<List<int>>();
		//			for (int i = 0; i < nSubMeshCount; i++) {
		//				
		//			}

	}


	public static Vector2[] meshUV;// = victim_mesh.uv;
	public static Vector3[] meshVertices;
	public static Vector3[] meshNormals;
	/// <summary>
	/// Cut the specified victim, blade_plane and capMaterial.
	/// </summary>
	/// <param name="victim">Victim.</param>
	/// <param name="blade_plane">Blade plane.</param>
	/// <param name="capMaterial">Cap material.</param>
	public static GameObject[] Cut(GameObject victim, Vector3 anchorPoint, Vector3 normalDirection, Material capMaterial){
		var stopWatch = new Stopwatch();
		stopWatch.Start();

		// set the blade relative to victim
		blade = new Plane(victim.transform.InverseTransformDirection(-normalDirection),
			victim.transform.InverseTransformPoint(anchorPoint));

		// get the victims mesh
		victim_mesh = victim.GetComponent<MeshFilter>().mesh;

		//			var meshTangents = victim_mesh.tangents;
		//			var meshColors = victim_mesh.colors32;
		meshNormals = victim_mesh.normals;
		meshUV = victim_mesh.uv;
		meshVertices = victim_mesh.vertices;
		var meshTriangles = victim_mesh.triangles;


		//			useMeshTangents = meshTangents != null && meshTangents.Length > 0;
		//			useVertexColors = meshColors != null && meshColors.Length > 0;
		useNormals = meshNormals != null && meshNormals.Length > 0;

		// reset values
		new_vertices.Clear();
		int sum = 0;
		for (int sub = 0; sub < victim_mesh.subMeshCount; sub++) {
			sum += victim_mesh.GetIndices(sub).Length;
		}
		//			left_side.ClearAll(sum);
		//			right_side.ClearAll(sum);


		InitBuffer(sum, sum, victim_mesh.subMeshCount);


		bool[] sides = new bool[3];
		int[] indices;
		int   p1,p2,p3;

		// go throught the submeshes
		for(int sub=0; sub<victim_mesh.subMeshCount; sub++){
			//			for(int sub=0; sub<1; sub++){
			indices = victim_mesh.GetIndices(sub);

			//				left_side.subIndices.Add(new List<int>());
			//				right_side.subIndices.Add(new List<int>());
			subIndices[0].Add(new List<int>());
			subIndices[1].Add(new List<int>());

			UnityEngine.Debug.LogError("Indices length = " + indices.Length);

			for(int i=0; i<indices.Length; i+=3){

				p1 = indices[i];
				p2 = indices[i+1];
				p3 = indices[i+2];

				sides[0] = blade.GetSide(meshVertices[p1]);
				sides[1] = blade.GetSide(meshVertices[p2]);
				sides[2] = blade.GetSide(meshVertices[p3]);

				// whole triangle
				if(sides[0] == sides[1] && sides[0] == sides[2]){
					//						if(sides[0]){ // left side
					//							left_side.AddTriangle(p1,p2,p3,sub);
					//						}else{
					//							right_side.AddTriangle(p1,p2,p3,sub);
					//						}

					int idx = sides[0]? 0 : 1;

					subIndices[idx][sub].Add(vertices[idx].Count);
					subIndices[idx][sub].Add(vertices[idx].Count + 1);
					subIndices[idx][sub].Add(vertices[idx].Count + 2);

					triangles[idx].Add(vertices[idx].Count);
					triangles[idx].Add(vertices[idx].Count + 1);
					triangles[idx].Add(vertices[idx].Count + 2);

					vertices[idx].Add(meshVertices[p1]);
					vertices[idx].Add(meshVertices[p2]);
					vertices[idx].Add(meshVertices[p3]);

					if (useNormals) {
						normals[idx].Add(meshNormals[p1]);
						normals[idx].Add(meshNormals[p2]);
						normals[idx].Add(meshNormals[p3]);
					}

					uvs[idx].Add(meshUV[p1]);
					uvs[idx].Add(meshUV[p2]);
					uvs[idx].Add(meshUV[p3]);

				}else{ // cut the triangle
					Cut_this_Face(sub, sides, p1, p2, p3);
				}
			}
		}


		Material[] mats = victim.GetComponent<MeshRenderer>().sharedMaterials;

		if(mats[mats.Length-1].name != capMaterial.name){ // add cap indices

			//				left_side.subIndices.Add(new List<int>());
			//				right_side.subIndices.Add(new List<int>());
			subIndices[0].Add(new List<int>());
			subIndices[1].Add(new List<int>());

			Material[] newMats = new Material[mats.Length+1];
			mats.CopyTo(newMats, 0);
			newMats[mats.Length] = capMaterial;
			mats = newMats;
		}




		// cap the opennings
		Capping();


		// Left Mesh

		Mesh left_HalfMesh = new Mesh();
		left_HalfMesh.name =  "Split Mesh Left";
		//			left_HalfMesh.vertices  = left_side.vertices.ToArray();
		//			left_HalfMesh.triangles = left_side.triangles.ToArray();
		//			if (useNormals) {
		//				left_HalfMesh.normals   = left_side.normals.ToArray();
		//			}
		//			left_HalfMesh.uv        = left_side.uvs.ToArray();
		//
		//			left_HalfMesh.subMeshCount = left_side.subIndices.Count;
		//			for(int i=0; i<left_side.subIndices.Count; i++)
		//				left_HalfMesh.SetIndices(left_side.subIndices[i].ToArray(), MeshTopology.Triangles, i);	

		left_HalfMesh.vertices  = vertices[0].ToArray();
		left_HalfMesh.triangles = triangles[0].ToArray();
		if (useNormals) {
			left_HalfMesh.normals   = normals[0].ToArray();
		}

		left_HalfMesh.uv        = uvs[0].ToArray();

		left_HalfMesh.subMeshCount = subIndices[0].Count;
		for(int i=0; i< subIndices[0].Count; i++)
			left_HalfMesh.SetIndices(subIndices[0][i].ToArray(), MeshTopology.Triangles, i);	

		// Right Mesh

		Mesh right_HalfMesh = new Mesh();
		right_HalfMesh.name = "Split Mesh Right";
		//			right_HalfMesh.vertices  = right_side.vertices.ToArray();
		//			right_HalfMesh.triangles = right_side.triangles.ToArray();
		//			if (useNormals) {
		//				right_HalfMesh.normals   = right_side.normals.ToArray();
		//			}
		//
		//			right_HalfMesh.uv        = right_side.uvs.ToArray();
		//
		//			right_HalfMesh.subMeshCount = right_side.subIndices.Count;
		//			for(int i=0; i<right_side.subIndices.Count; i++)
		//				right_HalfMesh.SetIndices(right_side.subIndices[i].ToArray(), MeshTopology.Triangles, i);

		right_HalfMesh.vertices  = vertices[1].ToArray();
		right_HalfMesh.triangles = triangles[1].ToArray();
		if (useNormals) {
			right_HalfMesh.normals   = normals[1].ToArray();
		}

		right_HalfMesh.uv        = uvs[1].ToArray();

		right_HalfMesh.subMeshCount = subIndices[1].Count;
		for(int i=0; i< subIndices[1].Count; i++)
			right_HalfMesh.SetIndices(subIndices[1][i].ToArray(), MeshTopology.Triangles, i);

		// assign the game objects

		victim.name = "left side";
		victim.GetComponent<MeshFilter>().mesh = left_HalfMesh;

		GameObject leftSideObj = victim;

		GameObject rightSideObj = new GameObject("right side", typeof(MeshFilter), typeof(MeshRenderer));
		rightSideObj.transform.position = victim.transform.position;
		rightSideObj.transform.rotation = victim.transform.rotation;
		rightSideObj.GetComponent<MeshFilter>().mesh = right_HalfMesh;


		// assign mats
		leftSideObj.GetComponent<MeshRenderer>().materials = mats;
		rightSideObj.GetComponent<MeshRenderer>().materials = mats;

		stopWatch.Stop();

		return new GameObject[]{ leftSideObj, rightSideObj };

	}


	static void Cut_this_Face(int submesh, bool[] sides, int index1, int index2, int index3){


		Vector3[] leftPoints = new Vector3[2];
		Vector3[] leftNormals = new Vector3[2];
		Vector2[] leftUvs = new Vector2[2];
		Vector3[] rightPoints = new Vector3[2];
		Vector3[] rightNormals = new Vector3[2];
		Vector2[] rightUvs = new Vector2[2];

		bool didset_left = false;
		bool didset_right = false;

		int p = index1;
		for(int side=0; side<3; side++){

			switch(side){
			case 0: p = index1;
				break;
			case 1: p = index2;
				break;
			case 2: p = index3;
				break;

			}

			if(sides[side]){
				if(!didset_left){
					didset_left = true;

					//						leftPoints[0]   = victim_mesh.vertices[p];
					leftPoints[0]   = meshVertices[p];
					leftPoints[1]   = leftPoints[0];
					//						leftUvs[0]     = victim_mesh.uv[p];
					leftUvs[0]     = meshUV[p];
					leftUvs[1]     = leftUvs[0];
					//						leftNormals[0] = victim_mesh.normals[p];
					leftNormals[0] = meshNormals[p];
					leftNormals[1] = leftNormals[0];

				}else{

					//						leftPoints[1]    = victim_mesh.vertices[p];
					leftPoints[1]    = meshVertices[p];
					//						leftUvs[1]      = victim_mesh.uv[p];
					leftUvs[1]      = meshUV[p];
					//						leftNormals[1]  = victim_mesh.normals[p];
					leftNormals[1]  = meshNormals[p];

				}
			}else{
				if(!didset_right){
					didset_right = true;

					//						rightPoints[0]   = victim_mesh.vertices[p];
					rightPoints[0]   = meshVertices[p];
					rightPoints[1]   = rightPoints[0];
					//						rightUvs[0]     = victim_mesh.uv[p];
					rightUvs[0]     = meshUV[p];
					rightUvs[1]     = rightUvs[0];
					//						rightNormals[0] = victim_mesh.normals[p];
					rightNormals[0] = meshNormals[p];
					rightNormals[1] = rightNormals[0];

				}else{
					//						rightPoints[1]   = victim_mesh.vertices[p];
					rightPoints[1]   = meshVertices[p];
					//						rightUvs[1]     = victim_mesh.uv[p];
					rightUvs[1]     = meshUV[p];
					//						rightNormals[1] = victim_mesh.normals[p];
					rightNormals[1] = meshNormals[p];
				}
			}
		}


		float normalizedDistance = 0.0f;
		float distance = 0;
		blade.Raycast(new Ray(leftPoints[0], (rightPoints[0] - leftPoints[0]).normalized), out distance);

		normalizedDistance =  distance/(rightPoints[0] - leftPoints[0]).magnitude;
		Vector3 newVertex1 = Vector3.Lerp(leftPoints[0], rightPoints[0], normalizedDistance);
		Vector2 newUv1     = Vector2.Lerp(leftUvs[0], rightUvs[0], normalizedDistance);
		Vector3 newNormal1 = Vector3.Lerp(leftNormals[0] , rightNormals[0], normalizedDistance);

		new_vertices.Add(newVertex1);

		blade.Raycast(new Ray(leftPoints[1], (rightPoints[1] - leftPoints[1]).normalized), out distance);

		normalizedDistance =  distance/(rightPoints[1] - leftPoints[1]).magnitude;
		Vector3 newVertex2 = Vector3.Lerp(leftPoints[1], rightPoints[1], normalizedDistance);
		Vector2 newUv2     = Vector2.Lerp(leftUvs[1], rightUvs[1], normalizedDistance);
		Vector3 newNormal2 = Vector3.Lerp(leftNormals[1] , rightNormals[1], normalizedDistance);

		new_vertices.Add(newVertex2);


		//			left_side.AddTriangle(new Vector3[]{leftPoints[0], newVertex1, newVertex2},
		//				new Vector3[]{leftNormals[0], newNormal1, newNormal2 },
		//				new Vector2[]{leftUvs[0], newUv1, newUv2}, newNormal1,
		//				submesh);

		AddTriangle(new Vector3[]{leftPoints[0], newVertex1, newVertex2},
			new Vector3[]{leftNormals[0], newNormal1, newNormal2 },
			new Vector2[]{leftUvs[0], newUv1, newUv2}, newNormal1,
			submesh, 0);

		//			left_side.AddTriangle(new Vector3[]{leftPoints[0], leftPoints[1], newVertex2},
		//				new Vector3[]{leftNormals[0], leftNormals[1], newNormal2},
		//				new Vector2[]{leftUvs[0], leftUvs[1], newUv2}, newNormal2,
		//				submesh);

		AddTriangle(new Vector3[]{leftPoints[0], leftPoints[1], newVertex2},
			new Vector3[]{leftNormals[0], leftNormals[1], newNormal2},
			new Vector2[]{leftUvs[0], leftUvs[1], newUv2}, newNormal2,
			submesh, 0);

		//			right_side.AddTriangle(new Vector3[]{rightPoints[0], newVertex1, newVertex2},
		//				new Vector3[]{rightNormals[0], newNormal1, newNormal2},
		//				new Vector2[]{rightUvs[0], newUv1, newUv2}, newNormal1,
		//				submesh);
		AddTriangle(new Vector3[]{rightPoints[0], newVertex1, newVertex2},
			new Vector3[]{rightNormals[0], newNormal1, newNormal2},
			new Vector2[]{rightUvs[0], newUv1, newUv2}, newNormal1,
			submesh, 1);

		//			right_side.AddTriangle(new Vector3[]{rightPoints[0], rightPoints[1], newVertex2},
		//				new Vector3[]{rightNormals[0], rightNormals[1], newNormal2},
		//				new Vector2[]{rightUvs[0], rightUvs[1], newUv2}, newNormal2,
		//				submesh);

		AddTriangle(new Vector3[]{rightPoints[0], rightPoints[1], newVertex2},
			new Vector3[]{rightNormals[0], rightNormals[1], newNormal2},
			new Vector2[]{rightUvs[0], rightUvs[1], newUv2}, newNormal2,
			submesh, 1);

	}

	private static void AddTriangle(Vector3[] points3, Vector3[] normals3, Vector2[] uvs3, Vector3 faceNormal, int submesh, int idx){

		Vector3 calculated_normal = Vector3.Cross((points3[1] - points3[0]).normalized, (points3[2] - points3[0]).normalized);

		int p1 = 0;
		int p2 = 1;
		int p3 = 2;

		if(Vector3.Dot(calculated_normal, faceNormal) < 0){

			p1 = 2;
			p2 = 1;
			p3 = 0;
		}

		int base_index = vertices[idx].Count;

		subIndices[idx][submesh].Add(base_index);
		subIndices[idx][submesh].Add(base_index+1);
		subIndices[idx][submesh].Add(base_index+2);

		triangles[idx].Add(base_index);
		triangles[idx].Add(base_index+1);
		triangles[idx].Add(base_index+2);

		vertices[idx].Add(points3[p1]);
		vertices[idx].Add(points3[p2]);
		vertices[idx].Add(points3[p3]);

		if (useNormals) {
			normals[idx].Add(normals3[p1]);
			normals[idx].Add(normals3[p2]);
			normals[idx].Add(normals3[p3]);
		}

		uvs[idx].Add(uvs3[p1]);
		uvs[idx].Add(uvs3[p2]);
		uvs[idx].Add(uvs3[p3]);
	}

	private static List<Vector3> capVertTracker = new List<Vector3>();
	private static List<Vector3> capVertpolygon = new List<Vector3>();

	static void Capping(){

		capVertTracker.Clear();

		for(int i=0; i<new_vertices.Count; i++)
			if(!capVertTracker.Contains(new_vertices[i]))
			{
				capVertpolygon.Clear();
				capVertpolygon.Add(new_vertices[i]);
				capVertpolygon.Add(new_vertices[i+1]);

				capVertTracker.Add(new_vertices[i]);
				capVertTracker.Add(new_vertices[i+1]);


				bool isDone = false;
				while(!isDone){
					isDone = true;

					for(int k=0; k<new_vertices.Count; k+=2){ // go through the pairs

						if(new_vertices[k] == capVertpolygon[capVertpolygon.Count-1] && !capVertTracker.Contains(new_vertices[k+1])){ // if so add the other

							isDone = false;
							capVertpolygon.Add(new_vertices[k+1]);
							capVertTracker.Add(new_vertices[k+1]);

						}else if(new_vertices[k+1] == capVertpolygon[capVertpolygon.Count-1] && !capVertTracker.Contains(new_vertices[k])){// if so add the other

							isDone = false;
							capVertpolygon.Add(new_vertices[k]);
							capVertTracker.Add(new_vertices[k]);
						}
					}
				}

				FillCap(capVertpolygon);

			}

	}

	static void FillCap(List<Vector3> vertices){


		// center of the cap
		Vector3 center = Vector3.zero;
		foreach(Vector3 point in vertices)
			center += point;

		center = center/vertices.Count;

		// you need an axis based on the cap
		Vector3 upward = Vector3.zero;
		// 90 degree turn
		upward.x = blade.normal.y;
		upward.y = -blade.normal.x;
		upward.z = blade.normal.z;
		Vector3 left = Vector3.Cross(blade.normal, upward);

		Vector3 displacement = Vector3.zero;
		Vector3 newUV1 = Vector3.zero;
		Vector3 newUV2 = Vector3.zero;

		for(int i=0; i<vertices.Count; i++){

			displacement = vertices[i] - center;
			newUV1 = Vector3.zero;
			newUV1.x = 0.5f + Vector3.Dot(displacement, left);
			newUV1.y = 0.5f + Vector3.Dot(displacement, upward);
			newUV1.z = 0.5f + Vector3.Dot(displacement, blade.normal);

			displacement = vertices[(i+1) % vertices.Count] - center;
			newUV2 = Vector3.zero;
			newUV2.x = 0.5f + Vector3.Dot(displacement, left);
			newUV2.y = 0.5f + Vector3.Dot(displacement, upward);
			newUV2.z = 0.5f + Vector3.Dot(displacement, blade.normal);

			//	uvs.Add(new Vector2(relativePosition.x, relativePosition.y));
			//	normals.Add(blade.normal);

			//				left_side.AddTriangle( new Vector3[]{vertices[i], vertices[(i+1) % vertices.Count], center},
			//					new Vector3[]{-blade.normal, -blade.normal, -blade.normal},
			//					new Vector2[]{newUV1, newUV2, new Vector2(0.5f, 0.5f)}, -blade.normal, left_side.subIndices.Count-1);

			AddTriangle( new Vector3[]{vertices[i], vertices[(i+1) % vertices.Count], center},
				new Vector3[]{-blade.normal, -blade.normal, -blade.normal},
				new Vector2[]{newUV1, newUV2, new Vector2(0.5f, 0.5f)},
				-blade.normal, subIndices[0].Count-1, 0);


			//				right_side.AddTriangle( new Vector3[]{vertices[i], vertices[(i+1) % vertices.Count], center},
			//					new Vector3[]{blade.normal, blade.normal, blade.normal},
			//					new Vector2[]{newUV1, newUV2, new Vector2(0.5f, 0.5f)},
			//					blade.normal, right_side.subIndices.Count-1);

			AddTriangle( new Vector3[]{vertices[i], vertices[(i+1) % vertices.Count], center},
				new Vector3[]{blade.normal, blade.normal, blade.normal},
				new Vector2[]{newUV1, newUV2, new Vector2(0.5f, 0.5f)},
				blade.normal, subIndices[1].Count-1, 1);

		}


	}

}