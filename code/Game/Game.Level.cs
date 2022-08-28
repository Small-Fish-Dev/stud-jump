namespace Stud;

partial class Game
{
	public static float StudToInch => 11f;
	private static Material defaultMat = Material.Load( "materials/stud.vmat" );
	private List<Material> materials = new();

	public ModelEntity Map { get; private set; }
	
	private Vector3 normalToTangent( Vector3 normal )
	{
		var t1 = Vector3.Cross( normal, Vector3.Forward );
		var t2 = Vector3.Cross( normal, Vector3.Up );

		return t1.Length > t2.Length ? t1 : t2;
	}

	private Vector2 planar( Vector3 pos, Vector3 uAxis, Vector3 vAxis )
	{
		return new Vector2()
		{
			x = Vector3.Dot( uAxis, pos ),
			y = Vector3.Dot( vAxis, pos )
		};
	}

	public void GenerateLevel()
	{
		var collisionVerts = new List<Vector3>();
		var collisionIndices = new List<int>();
		var offset = 0;
		var cubeCount = 0;

		var faceIndices = new int[]
		{
			0, 1, 2, 3,
			7, 6, 5, 4,
			0, 4, 5, 1,
			1, 5, 6, 2,
			2, 6, 7, 3,
			3, 7, 4, 0,
		};

		var uAxis = new Vector3[]
		{
			Vector3.Forward,
			Vector3.Left,
			Vector3.Left,
			Vector3.Forward,
			Vector3.Right,
			Vector3.Backward,
		};

		var vAxis = new Vector3[]
		{
			Vector3.Left,
			Vector3.Forward,
			Vector3.Down,
			Vector3.Down,
			Vector3.Down,
			Vector3.Down,
		};

		void createCube( Vector3 mins, Vector3 maxs, ref List<SimpleVertex> vertices, ref List<int> indices )
		{
			var verts = new Vector3[]
			{
				mins.WithZ( maxs.z ),
				maxs.WithX( mins.x ),
				maxs,
				maxs.WithY( mins.y ),
				mins,
				mins.WithY( maxs.y ),
				maxs.WithZ( mins.z ),
				mins.WithX( maxs.x )
			};

			for ( var i = 0; i < 6; ++i )
			{
				var tangent = uAxis[i];
				var binormal = vAxis[i];
				var normal = Vector3.Cross( tangent, binormal );

				for ( var j = 0; j < 4; ++j )
				{
					var index = faceIndices[(i * 4) + j];
					var pos = verts[index];

					vertices.Add( new SimpleVertex()
					{
						position = pos,
						normal = normal,
						tangent = tangent,
						texcoord = planar( (pos - mins) / StudToInch / 2f, uAxis[i], vAxis[i] )
					} );
				}

				var o = i * 4 + cubeCount * 24;
				indices.Add( o + 0 );
				indices.Add( o + 2 );
				indices.Add( o + 1 );
				indices.Add( o + 2 );
				indices.Add( o + 0 );
				indices.Add( o + 3 );
			}
			
			collisionVerts.AddRange( new Vector3[] { verts[0], verts[1], verts[2], verts[3], verts[4], verts[5] } );
			collisionIndices.AddRange( new int[] { 0 + offset, 1 + offset, 2 + offset, 0 + offset, 3 + offset, 2 + offset, offset + 0, offset + 4, offset + 1, offset + 1, offset + 4, offset + 5 } );
			offset += 6;
			cubeCount++;
		}
		
		var totalHeight = 0f;
		var meshes = new List<Mesh>();
		var multiplier = 1f;
		for ( int i = 1; i < 120; i++ )
		{
			cubeCount = 0;

			if ( i > 100 ) multiplier += (i - 100f) * 1.2f;

			var size = 15f * StudToInch;
			var heightNum = (i + multiplier - 1) * 0.5f;
			var height = heightNum * StudToInch;
			var pos = new Vector3( (i + 1) * (size + StudToInch) - StudToInch * 2f, 0, totalHeight + StudToInch / 4f );

			var mat = defaultMat.CreateCopy();
			if ( Host.IsClient )
			{
				var hsv = new ColorHsv( totalHeight, 1f, 1f );
				var col = hsv.ToColor().ToColor32();
				var tex = Texture.Create( 1, 1 )
					.WithData( new[] { col.r, col.g, col.b, (byte)255 } )
					.Finish();
				mat.OverrideTexture( "tint", tex );
				materials.Add( mat );
			}

			var vertices = new List<SimpleVertex>();
			var indices = new List<int>();
			var mesh = new Mesh( mat );
			var point = new Vector3( size / 2f, size / 2f, StudToInch / 2f );
			createCube( pos - point, pos + point, ref vertices, ref indices );
			createCube( pos - point.WithX( -point.x ), pos + new Vector3( point.x + StudToInch, point.y, height + StudToInch / 2f ), ref vertices, ref indices );
			
			mesh.CreateVertexBuffer( vertices.Count, SimpleVertex.Layout, vertices );
			mesh.CreateIndexBuffer( indices.Count, indices );

			meshes.Add( mesh );

			if ( Host.IsServer )
			{
				var checkpoint = new Checkpoint();
				checkpoint.Position = pos + point.z * Vector3.Up;
				checkpoint.Level = i;
				checkpoint.Transmit = TransmitType.Always;
			}
			else if ( i * 0.5f > 3f )
			{
				var h = 375f;
				var worldPanel = new WorldPanel()
				{
					Position = pos + Vector3.Up * height / 2f + point.WithY( 0 ) + Vector3.Backward * 1f,
					Rotation = Rotation.LookAt( Vector3.Backward ).RotateAroundAxis( Vector3.Forward, -90f ),
					PanelBounds = new Rect( -size * 10f, -h / 2f, size * 20f, h ),
					StyleSheet = HUD.Instance.StyleSheet
				};
				worldPanel.AddClass( "heightCount" );
				var label = worldPanel.AddChild<Label>();
				label.Text = $" {heightNum:F1} studs ";
			}

			totalHeight += height;
		}
		
		Map?.Delete();
		Map = new();
		Map.Model = Model.Builder
			.AddCollisionMesh( collisionVerts.ToArray(), collisionIndices.ToArray() )
			.AddMeshes( meshes.ToArray() )
			.Create();
		Map.SetupPhysicsFromModel( PhysicsMotionType.Static );
	}
}
