using System.Text;

namespace Stud;

public partial class Player : AnimatedEntity
{
	public static readonly HashSet<string> BannedWords = new() { "simple", "complicated", "confusing" };

	[Net, Predicted] public PawnController Controller { get; set; }
	[Net, Predicted] public PawnAnimator Animator { get; set; }

	public IBaseInventory Inventory { get; protected set; }
	public HashSet<string> OwnedItems { get; set; } = new();

	public Checkpoint CheckpointReached { get; set; }

	public ClothingContainer Clothing = new();
	public bool IsAdmin => CheckpointReached.Level >= 119;

	public override void Spawn()
	{
		Inventory ??= new BaseInventory( this );
		Inventory.Add( new CocaColaEspuma() );
		Inventory.Add( new JumpCoil() );

		Controller ??= new PlayerController();
		Animator ??= new PlayerAnimator();

		SetModel( "models/citizenstud/citizenstud.vmdl" );

		var controller = Controller as PlayerController;
		
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, controller.CollisionBox.Mins, controller.CollisionBox.Maxs );

		UseAnimGraph = false;
		PlaybackRate = 1f;

		Tags.Add( "player" );

		CheckpointReached = Entity.All
			.OfType<Checkpoint>()
			.Where( x => x.Level == 0 )
			.First();

		Respawn();
	}

	public void Respawn()
	{
		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Position = Entity.All
			.OfType<Checkpoint>()
			.Where( x => x.Level == 0 )
			.First().Position;

		Rotation = Transform.Zero.Rotation;

		ResetInterpolation();
	}

	[Event.Tick.Client]
	public void AnimateOthers() // The PlayerAnimator doesn't play the jump animation?
	{

		if ( Local.Pawn == this ) return;

		ComputeAnimation();

	}

	TimeSince animationStart = 0f;
	string currentAnimation = "idle";

	public void ComputeAnimation()
	{

		var pawn = this;

		bool midair = pawn.GroundEntity == null;
		bool moving = pawn.Velocity.Length >= 5f;

		pawn.CurrentSequence.Name = midair ? "jump" : moving ? "run" : "idle";

		if ( pawn.CurrentSequence.Name != currentAnimation )
		{

			currentAnimation = pawn.CurrentSequence.Name;
			animationStart = 0f;

		}

		pawn.CurrentSequence.Time = midair ? Math.Min( animationStart, pawn.CurrentSequence.Duration ) : animationStart % pawn.CurrentSequence.Duration;

	}

	public void UpdateEyePosition()
	{
		if ( Controller is not PlayerController controller ) return;

		EyePosition = Position
			+ EyeLocalPosition;
	}

	public override void Simulate( Client cl )
	{
		Controller?.Simulate( cl, this, Animator );

		if ( Host.IsServer )
			UpdateEyePosition();

		if ( Position.z <= Game.StudToInch * -4 )
		{

			if ( CheckpointReached == null )
			{

				Position = Entity.All
			   .OfType<Checkpoint>()
			   .Where( x => x.Level == 0 )
			   .First().Position;

			}
			else
			{

				Velocity = Vector3.Zero;
				Position = CheckpointReached.Position;
				ResetInterpolation();

			}

		}

	}

	public async void LoadExperience()
	{

		var result = await Game.GetScore( "Experience", Client );

		if ( result != null )
		{

			Experience = result.Value.Score;
			Log.Info( $"{Client.Name}'s Experience ({Experience}) was fetched." );

		}

	}

	public async void LoadLevel()
	{

		var result = await Game.GetScore( "Studs", Client );

		if ( result != null )
		{

			int studs = result.Value.Score;

			Log.Info( $"{Client.Name}'s Studs ({studs}) was fetched." );

			Position = Entity.All
			.OfType<Checkpoint>()
			.Where( x => x.Level == studs )
			.First().Position;


		}

	}

	public override void FrameSimulate( Client cl )
	{
		Controller?.FrameSimulate( cl, this, Animator );
		UpdateEyePosition();
	}

	public override void BuildInput( InputBuilder input )
	{
		if ( input.Down( InputButton.SecondaryAttack ) )
		{
			input.ViewAngles += input.AnalogLook;
			input.ViewAngles.pitch = input.ViewAngles.pitch.Clamp( -89, 89 );
			input.ViewAngles.roll = 0;
		}

		input.InputDirection = input.AnalogMove;
	}

	TimeSince timeSinceLastFootstep = 0;

	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
		if ( !IsClient )
			return;

		if ( timeSinceLastFootstep < 0.2f )
			return;

		timeSinceLastFootstep = 0;

		var tr = Trace.Ray( pos, pos + Vector3.Down * 20f )
			.Radius( 1 )
			.Ignore( this )
			.IncludeClientside()
			.Run();

		if ( !tr.Hit ) return;

		volume *= Velocity.WithZ( 0 ).Length.LerpInverse( 0.0f, 200.0f );
		tr.Surface.DoFootstep( this, tr, foot, 1f );
	}

	[ClientRpc]
	public void BuyShit( BaseItem item )
	{
		Log.Info( $"TODO: Buy this {item.Name} for {item.Cost}!" );
	}

	[ConCmd.Server( "stud_inv", Help = "-1 to get rid of your items" )]
	public void GetInventorySlot( int number )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player ) return;

		if ( player.Inventory is not BaseInventory inventory ) return;

		if ( number > 0 )
			inventory.SetActiveSlot( number );
		else
			inventory.Active = null;
	}

	[ClientRpc]
	public static void sendChat( byte[] data )
	{
		List<(string text, Color col)> result = new();

		using ( var stream = new MemoryStream( data ) )
		{
			using ( var reader = new BinaryReader( stream ) )
			{
				var count = reader.ReadInt16();

				for ( int i = 0; i < count; i++ )
					result.Add( (reader.ReadString(), reader.ReadColor()) );
			}
		}

		Chat.Instance?.Append( result );
	}

	public static void SendChat( To target, List<(string text, Color col)> message )
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( (short)message.Count );

				for ( int i = 0; i < message.Count; i++ )
				{
					writer.Write( message[i].text );
					writer.Write( message[i].col );
				}
			}

			sendChat( target, stream.ToArray() );
		}
	}

	[ConCmd.Server( "say" )]
	public static void SayCommand( string text )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player pawn ) return;

		pawn.Say( text );

		new Hamster() { Position = pawn.Position };
	}

	public void Say( string text )
	{

		var sb = new StringBuilder( text );
		var i = 0;
		while ( i < text.Length )
		{
			var j = i;
			while ( j < text.Length && text[j] != ' ' )
				j++;

			if ( j != i )
			{
				if ( Rand.Int( 12 ) == 0 || BannedWords.Contains( text.Substring( i, j - i ) ) )
				{
					// 1984
					for ( var k = i; k < j; k++ )
						sb[k] = '#';
				}

				i = j;
			}
			else
				i++;
		}
		text = $"{sb}";

		var emoji = CurrentRank.Name.Split( " " );

		Player.SendChat( To.Everyone, new()
		{
			( $"{emoji.FirstOrDefault()} {Client.Name}: ", CurrentRank.Color ),
			( $"{text}", Color.White ),
		} );

	}

}
