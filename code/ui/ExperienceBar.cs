namespace Stud;

public class ExperienceBar : Panel
{

	Panel bar;

	public ExperienceBar()
	{

		bar = Add.Panel( "bar" );

		var firstColor = "#2f3330";
		var secondColor = "#aed6b8";
		var stops = 4f;

		var percent = 100f / stops;
		var result = $"{firstColor} 0%, {firstColor} {percent}%";

		for ( int i = 1; i < stops; i++ )
		{
			var col = i % 2 == 0 ? firstColor : secondColor;
			result += $", {col} {i * percent}%, {col} {(i + 1) * percent}%";
		}

		Style.Set( "background", $"linear-gradient({result})" );
		Style.Dirty();

	}

	public override void Tick()
	{
	}

}
