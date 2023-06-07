namespace Stud;

public class ExperienceBar : Panel
{

	Panel bar;
	Label currentRank;
	Label nextRank;
	float lerpValue;

	public ExperienceBar()
	{

		bar = Add.Panel( "bar" );
		bar.Add.Panel( "grid" );
		currentRank = Add.Label( "", "currentRank" );
		nextRank = Add.Label( "", "nextRank" );

	}

	public override void Tick()
	{

		if ( Sandbox.Game.LocalPawn is not Player pawn ) return;

		string rankColor = pawn.CurrentRank.Color.Hex;
		string darkColor = new Color( pawn.CurrentRank.Color.r - 0.3f, pawn.CurrentRank.Color.g - 0.3f, pawn.CurrentRank.Color.b - 0.3f ).Hex;

		bar.Style.Set( "background", $"linear-gradient( {rankColor} 0%, {darkColor} 60% )" );

		lerpValue = MathX.LerpTo( lerpValue, pawn.ExperienceProgress * 98f + 2f, 10f * Time.Delta );
		bar.Style.Width = Length.Percent( lerpValue );

		currentRank.Text = pawn.CurrentRank.Name;
		nextRank.Text = $"Next: {pawn.NextRank.Name.Split( ' ' )[0]}";

		currentRank.Style.FontColor = new Color( pawn.CurrentRank.Color.r + 0.2f, pawn.CurrentRank.Color.g + 0.2f, pawn.CurrentRank.Color.b + 0.2f );
		nextRank.Style.FontColor = new Color( pawn.NextRank.Color.r + 0.2f, pawn.NextRank.Color.g + 0.2f, pawn.NextRank.Color.b + 0.2f );

	}

	[Event("onExperience")]
	public void OnExperience( Player ply,  int experience )
	{

		for ( int i = 0; i < Math.Min( experience, 100 ); i++ )
		{

			float randLife = (float)(new Random().Next( 5, 10 )) / 10f;
			float randSize = (float)(new Random().Next( 3, 7 )) / 10f;
			float randSpeed = (float)(new Random().Next( 5, 20 )) / 10f;

			var star = new StarParticle( randLife, randSize, randSpeed, null );
			star.Style.Left = Length.Pixels( bar.Box.Rect.TopRight.x * Parent.ScaleFromScreen );
			star.Style.Top = Length.Pixels( ( bar.Box.Rect.TopRight.y + bar.Box.Rect.Height / 2 ) * Parent.ScaleFromScreen );
			Parent.AddChild( star );

		}

	}

	[Event("levelUp")]
	public void LevelUp( Player ply )
	{

		for ( int i = 0; i < 30; i++ )
		{

			float randLife = (float)(new Random().Next( 5, 10 )) / 10f;
			float randSize = (float)(new Random().Next( 3, 7 )) / 10f;
			float randSpeed = (float)(new Random().Next( 5, 20 )) / 10f;

			float randPos = Sandbox.Game.Random.Float( Box.Rect.Width );

			var star = new StarParticle( randLife, randSize, randSpeed, null, new Vector2( 0f, 2f ) );
			star.Style.Left = Length.Pixels( ( bar.Box.Rect.TopLeft.x  + randPos ) * Parent.ScaleFromScreen );
			star.Style.Top = Length.Pixels( (Box.Rect.TopRight.y + Box.Rect.Height / 2) * Parent.ScaleFromScreen );
			Parent.AddChild( star );

		}

	}

}
