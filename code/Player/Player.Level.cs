namespace Stud;

public struct Rank
{
	public static Rank[] All = new Rank[] 
	{ 
		new Rank { Name = "👨‍🦽 Impaired", Color = Color.Gray, Requirement = 0 },
		new Rank { Name = "🏳‍🌈 Swedish", Color = Color.Orange, Requirement = 100 },
		new Rank { Name = "🐟 Fish", Color = Color.Cyan, Requirement = 15000 }
	};

	public string Name;
	public Color Color;
	public int Requirement;
}

partial class Player
{
	[Net] private int experience { get; set; }
	private int previousRank = 0;
	public int Experience 
	{ 
		get => experience; 
		set
		{
			Host.AssertServer();

			Event.Run( "onExperience", this, value - experience );
			experience = value;
			
			if ( previousRank < RankIndex )
			{
				Event.Run( "levelUp", this );
				previousRank = RankIndex;
			}
		}
	}

	private int lastExperience = -1;
	private int lastRank;
	public int RankIndex
	{
		get
		{
			if ( lastExperience == Experience ) return lastRank;

			foreach ( var rank in Rank.All )
				if ( Experience >= rank.Requirement ) 
					lastRank = Array.IndexOf( Rank.All, rank );
			lastExperience = Experience;

			return lastRank;
		}
	}
	public Rank CurrentRank => Rank.All[RankIndex];

	[Event.Tick.Client]
	void clientTick()
	{
		var nextRank = Rank.All[Math.Min( RankIndex + 1, Rank.All.Length - 1 )];
		DebugOverlay.ScreenText( $"Total EXP: {Experience}" );
		DebugOverlay.ScreenText( $"EXP till next: {Experience - CurrentRank.Requirement} / {nextRank.Requirement} -> {nextRank.Name}", 1 );
		DebugOverlay.ScreenText( $"Title: {CurrentRank.Name}", 2 );
	}
}
