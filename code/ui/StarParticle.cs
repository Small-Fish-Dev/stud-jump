using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

public class HeartParticle : Panel
{

	TimeSince lifeTime = 0f;
	float deathTime;
	float transitions;
	float particleSize;
	float particleRotation;
	float particleSpeed;
	Vector2 particleGravity;
	Vector2 particleVelocity;

	public HeartParticle( float duration = 1f, float size = 1f, float speed = 1f, Vector2? direction = null, Vector2? gravity = null )
	{

		deathTime = duration;
		transitions = duration / 2;
		particleSpeed = speed;
		particleSize = Rand.Float( 20, 50 ) * size;

		Style.Height = 0;
		Style.BackgroundAngle = Length.Percent( particleRotation );
		Style.ZIndex = (int)( Time.Now * 100 );

		particleVelocity = direction.GetValueOrDefault( Vector2.Random.Normal ) * Rand.Float( 5f, 10f );
		particleGravity = gravity.GetValueOrDefault( new Vector2( 0f, 1f ) );

	}

	public override void Tick()
	{
		
		float velocityStrength = 10f * particleSpeed;
		float gravityStrength = 1f;
		particleVelocity = new Vector2( particleVelocity.x + (float)Math.Pow( lifeTime * gravityStrength, 2f ) * particleGravity.x, particleVelocity.y + (float)Math.Pow( lifeTime * gravityStrength, 2f ) * particleGravity.y );
		Style.Left = Length.Pixels( Style.Left.Value.GetPixels( Screen.Width ) + particleVelocity.x * Time.Delta * velocityStrength );
		Style.Top = Length.Pixels( Style.Top.Value.GetPixels( Screen.Height ) + particleVelocity.y * Time.Delta * velocityStrength );


		Style.Height = Length.Pixels( Math.Min( lifeTime, transitions ) / transitions * particleSize );
		Style.Opacity = Math.Min( deathTime - lifeTime , transitions ) / transitions;

		if ( lifeTime > deathTime )
		{

			Delete();

		}

	}

}
