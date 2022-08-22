namespace Stud;

partial class Game
{
	private static float studToInch => 11.023622f;
	private static Material defaultMat = Material.Load( "materials/stud.vmat" );

	public ModelEntity Map { get; private set; }
	
	private Vector3 normalToTangent( Vector3 normal )
	{
		var t1 = Vector3.Cross( normal, Vector3.Forward );
		var t2 = Vector3.Cross( normal, Vector3.Up );

		return t1.Length > t2.Length ? t1 : t2;
	}

	public void GenerateLevel()
	{
		var collisionVerts = new List<Vector3>();
		var indices = new List<int>();
		var offset = 0;
		
		Mesh createStep( Vector3 pos, float size, float height, float scale = 1f )
		{
			var vertices = new List<SimpleVertex>();
			var mat = defaultMat.CreateCopy();
			if ( Host.IsClient )
			{
				var col = new Color( Rand.Float( 1 ), Rand.Float( 1 ), Rand.Float( 1 ) ).ToColor32();
				var tex = Texture.Create( 2, 2 )
					.WithData( new[] { col.r, col.g, col.b, (byte)255, col.r, col.g, col.b, (byte)255, col.r, col.g, col.b, (byte)255, col.r, col.g, col.b, (byte)255 } )
					.Finish();
				mat.OverrideTexture( "tint", tex );
			}
			var mesh = new Mesh( mat );

			var v1 = pos + new Vector3( 0f, 0f, 0f ) * scale;
			var v2 = pos + new Vector3( 0f, 0f, height ) * scale;
			var v3 = pos + new Vector3( 0f, size, 0f ) * scale;
			var v4 = pos + new Vector3( 0f, size, height ) * scale;
			var v5 = pos + new Vector3( size, 0f, height ) * scale;
			var v6 = pos + new Vector3( size, size, height ) * scale;

			collisionVerts.AddRange( new List<Vector3>() { v1, v2, v3, v4, v5, v6 } );

			if ( Host.IsClient )
			{
				vertices.Add( new SimpleVertex()
				{
					position = v1,
					normal = Vector3.Backward,
					tangent = normalToTangent( Vector3.Backward ),
					texcoord = new Vector3( 0f, 0f )
				} );

				vertices.Add( new SimpleVertex()
				{
					position = v2,
					normal = Vector3.Backward,
					tangent = normalToTangent( Vector3.Backward ),
					texcoord = new Vector3( 0f, 1f )
				} );

				vertices.Add( new SimpleVertex()
				{
					position = v3,
					normal = Vector3.Backward,
					tangent = normalToTangent( Vector3.Backward ),
					texcoord = new Vector3( 1f, 0f )
				} );

				vertices.Add( new SimpleVertex()
				{
					position = v4,
					normal = Vector3.Backward,
					tangent = normalToTangent( Vector3.Backward ),
					texcoord = new Vector3( 1f, 1f )
				} );

				vertices.Add( new SimpleVertex()
				{
					position = v5,
					normal = Vector3.Up,
					tangent = normalToTangent( Vector3.Backward ),
					texcoord = new Vector3( 0f, 0f )
				} );

				vertices.Add( new SimpleVertex()
				{
					position = v6,
					normal = Vector3.Up,
					tangent = normalToTangent( Vector3.Up ),
					texcoord = new Vector3( 1f, 0f )
				} );
			}

			var meshIndices = new List<int> { 0, 1, 2, 2, 1, 3, 1, 4, 3, 3, 4, 5 };

			var i = offset;
			indices.AddRange( new List<int> { i + 0, i + 1, i + 2, i + 1, i + 2, i + 3, i + 1, i + 3, i + 4, i + 5, i + 3, i + 4 } );
			offset += 6;

			mesh.CreateVertexBuffer( vertices.Count, SimpleVertex.Layout, vertices );
			mesh.CreateIndexBuffer( meshIndices.Count, meshIndices );

			return mesh;
		}
		
		var totalHeight = 0f;
		var meshes = new List<Mesh>();

		for ( int i = 0; i < 17 * 2; i++ )
		{
			var size = studToInch * 12.5f;
			var height = (i + 1) * 0.5f * studToInch;
			meshes.Add( createStep( new Vector3( i * size, 0, totalHeight ), size, height ) );
			totalHeight += height;
		}

		Map = new();
		Map.Model = Model.Builder
			.AddCollisionMesh( collisionVerts.ToArray(), indices.ToArray() )
			.AddMeshes( meshes.ToArray() )
			.Create();
		Map.SetupPhysicsFromModel( PhysicsMotionType.Static );
	}
}
