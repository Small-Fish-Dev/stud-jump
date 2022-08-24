using Sandbox;

namespace Stud;

public partial class Player : AnimatedEntity
{
	[Net, Predicted] public PawnController Controller { get; set; }
	[Net, Predicted] public PawnAnimator Animator { get; set; }

	public IBaseInventory Inventory { get; protected set; }
	public HashSet<string> OwnedItems { get; set; } = new();

	public Checkpoint CheckpointReached { get; set; }

	public ClothingContainer Clothing = new();

	public override void Spawn()
	{
		Inventory ??= new BaseInventory( this );
		Inventory.Add( new CocaColaEspuma() );
		Inventory.Add( new JumpCoil() );

		Controller ??= new PlayerController();
		Animator ??= new PlayerAnimator();

		var controller = Controller as PlayerController;

		SetModel( "models/citizenstud/citizenstud.vmdl" );
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, controller.CollisionBox.Mins, controller.CollisionBox.Maxs );

		UseAnimGraph = false;
		PlaybackRate = 1f;

		Tags.Add( "player" );

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
}
