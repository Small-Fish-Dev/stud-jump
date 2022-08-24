namespace Stud;

public struct Rank
{
	public static Rank[] All = new Rank[] 
	{ 
		new Rank { Name = "👨‍🦽 Impaired", Color = Color.Gray, Requirement = 0 },
		new Rank { Name = "🤓 Mingebag", Color = new Color( 1f, 0.6f, 0.6f ), Requirement = 20 },
		new Rank { Name = "🏳‍🌈 Swedish", Color = Color.Orange, Requirement = 300 },
		new Rank { Name = "🐱 Flopper", Color = Color.Red, Requirement = 1500 },
		new Rank { Name = "🧱 Bricka", Color = new Color( 0.6f, 1f, 0.6f ), Requirement = 4500 },
		new Rank { Name = "🕴 Mojangster", Color = Color.Green, Requirement = 14000 },
		new Rank { Name = "🏀 Baller", Color = new Color( 0.8f, 0.4f, 0.8f ), Requirement = 31000 },
		new Rank { Name = "📦 Bozer", Color = Color.Blue, Requirement = 50000 },
		new Rank { Name = "👑 King", Color = Color.Yellow, Requirement = 80000 },
		new Rank { Name = "🧠 Prodigy", Color = Color.Black, Requirement = 120000 },
		new Rank { Name = "🐟 Fish", Color = Color.Cyan, Requirement = 190000 },
		new Rank { Name = "😼 Mark", Color = Color.White, Requirement = 999999 },
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
	public Rank NextRank => Rank.All[Math.Min( RankIndex + 1, Rank.All.Length - 1 )];
	public float ExperienceProgress => (float)(Experience - CurrentRank.Requirement) / (float)( NextRank.Requirement - CurrentRank.Requirement );
}
