namespace Stud;

public struct Rank
{
	public static Rank[] All = new Rank[] 
	{ 
		new Rank { Name = "👨‍🦽 Impaired", Color = Color.Gray, Requirement = 0 },
		new Rank { Name = "🤓 Mingebag", Color = new Color( 1f, 0.6f, 0.6f ), Requirement = 15 },
		new Rank { Name = "🏳‍🌈 Swedish", Color = Color.Orange, Requirement = 45 },
		new Rank { Name = "🐌​ Snail", Color = Color.Green, Requirement = 90 },
		new Rank { Name = "🤬 Squeaker", Color = new Color( 0.9f, 0.6f, 0.1f ), Requirement = 150 },
		new Rank { Name = "💩 Landlubber", Color = new Color( 0.6f, 0.6f, 0.1f ), Requirement = 225 },
		new Rank { Name = "🐱 Flopper", Color = Color.Yellow, Requirement = 315 },
		new Rank { Name = "🤡 Jester", Color = Color.White, Requirement = 420 },
		new Rank { Name = "​🧞​ Genie", Color = Color.Blue, Requirement = 540 },
		new Rank { Name = "🕴 Mojangster", Color = Color.Green, Requirement = 655 },
		new Rank { Name = "🧙​ Wizard", Color = new Color( 0.8f, 0.1f, 0.8f ), Requirement = 795 },
		new Rank { Name = "🐙 Octopus", Color =  new Color( 0.7f, 0.3f, 0.1f ), Requirement = 960 },
		new Rank { Name = "👨‍💻​​​ Hacker", Color = Color.Gray, Requirement = 1140 },
		new Rank { Name = "👽​ Bogos Binted", Color = Color.Green, Requirement = 1335 },
		new Rank { Name = "🎸 Rockstar", Color = new Color( 0.8f, 0.4f, 0.8f ), Requirement = 1545 },
		new Rank { Name = "🧱 Bricka", Color = new Color( 0.6f, 1f, 0.6f ), Requirement = 1770 },
		new Rank { Name = "🍄 Super Mario!", Color = Color.Red, Requirement = 2010 },
		new Rank { Name = "👑 King", Color = Color.Yellow, Requirement = 2265 },
		new Rank { Name = "🏀 Baller", Color = new Color( 0.8f, 0.4f, 0.8f ), Requirement = 2535 },
		new Rank { Name = "🐒 Lesser Ape", Color =  new Color( 0.7f, 0.5f, 0.1f ), Requirement = 2820 },
		new Rank { Name = "🦠​ Purple Capsule Creature", Color = new Color( 0.8f, 0.1f, 0.8f ), Requirement = 3120 },
		new Rank { Name = "📦 Bozer", Color = new Color( 0.9f, 0.6f, 0.3f ), Requirement = 3435 },
		new Rank { Name = "🔫 ​Wiseguy", Color = Color.Gray, Requirement = 3765 },
		new Rank { Name = "🦈 Big Fish", Color = Color.Blue, Requirement = 4110 },
		new Rank { Name = "🧠 Prodigy", Color = Color.Black, Requirement = 4485 },
		new Rank { Name = "​🦧​ Sigma Ape", Color = Color.Orange, Requirement = 4875 },
		new Rank { Name = "​🐳 Huge Fish", Color = Color.Cyan, Requirement = 5280 },
		new Rank { Name = "👤​ Gigachad", Color = Color.Gray, Requirement = 5700 },
		new Rank { Name = "🦅 Eagler", Color = new Color( 0.6f, 0.6f, 0.1f ), Requirement = 6135 },
		new Rank { Name = "🦍 Greater Ape", Color = new Color( 0.3f, 0.2f, 0.2f), Requirement = 6585 },
		new Rank { Name = "🐟 Small Fish", Color = Color.Cyan, Requirement = 7050 },
		new Rank { Name = "😻​ Mark", Color = Color.White, Requirement = 7530 },
		new Rank { Name = "🏴‍☠ CHEATER!!!", Color = Color.Black, Requirement = 999999 }
	};

	public string Name;
	public Color Color;
	public int Requirement;
}

partial class Player
{
	[Net, Change] private int experience { get; set; }
	[Net] private int previousRank { get; set; } = 0;
	internal int Experience 
	{ 
		get => experience; 
		set
		{
			Sandbox.Game.AssertServer();

			Event.Run( "onExperience", this, value - experience );
			experience = value;

			if ( !Client.IsBot )
			{

				if ( !Sandbox.Game.IsEditor )
				{

					Game.SubmitScore( "Experience", Client, experience );

				}
			}

			if ( previousRank < RankIndex )
			{
				Event.Run( "levelUp", this );
				previousRank = RankIndex;
			}
		}
	}


	[Net] private int lastExperience { get; set; } = -1;
	[Net] private int lastRank { get; set; }
	internal int RankIndex
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
	internal Rank CurrentRank => Rank.All[RankIndex];
	internal Rank NextRank => Rank.All[Math.Min( RankIndex + 1, Rank.All.Length - 1 )];
	internal float ExperienceProgress => (float)(Experience - CurrentRank.Requirement) / (float)( NextRank.Requirement - CurrentRank.Requirement );

	private void OnexperienceChanged( int oldValue, int newValue )
	{
		CallExperienceChange(oldValue, newValue);
	}

	[ClientRpc]
	private void CallExperienceChange(int oldValue, int newValue)
	{

		Event.Run( "onExperience", this, newValue - oldValue );

		if ( previousRank < RankIndex )
		{

			Event.Run( "levelUp", this );

		}
	}
}
