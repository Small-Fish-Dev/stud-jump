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

		if ( Local.Pawn is not Player pawn ) return;

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

}
