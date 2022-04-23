using System;

/*
	https://research.ncl.ac.uk/game/mastersdegree/gametechnologies/physicstutorials/5collisionresponse/Physics%20-%20Collision%20Response.pdf
	https://pt.slideshare.net/ohyecloudy/ndc12-12668524


	e = Bouncing Const Scalar
	Inv_Mass = 1 / Mass
	Inv_Inertia = 1 / Inertia
	Inv_Inertia = Inv_Mass / (relative_AToP.Length ^ 2)
	
	velocity_AToB = B's Velocity - A's Velocity
	relative_AToP = Contact Point - A's Center
*/



public static class Mechanics
{
	public enum RelativeState
	{
		Leaving,
		Smashing,
		Stop
	}
	
	public static RelativeState GetRelativeStateOfBodies(Vector2 velocity_AToB, Vector2 normal)
	{
		float d = Vector2.Dot(velocity_AToB,normal);
		if(d == 0.0f)
			return RelativeState.Stop;
		else if(d < 0.0f)
			return RelativeState.Smashing;
		// d > 0.0f
		return RelativeState.Leaving;
	}
	
	public static float GetInverseInertia(Vector2 rotate_axis, Vector2 mass_center, float inv_mass)
	{
		float r = Vector2.Distance(rotate_axis, mass_center);
		return inv_mass / (r * r);
	}
	
	/*
	public static Vector2 TripleProduct(Vector2 a, Vector2 b, Vector2 c) //	(A X B) X C
	{
		float ac = Vector2.Dot(a,c); (a.x * c.x) + (a.y * c.y)
		float bc = Vector2.Dot(b,c); (b.x * c.x) + (b.y * c.y)
		return b * ac - a * bc;
	}
	*/
	
	//	This is the formula for the sphere used, But I wanted simplification. :3
	public static float CalcInverseInertiaAtContactPoint(float inv_mass, Vector2 relative)
	{
		return 2.0f * inv_mass / relative.LengthSquared();
	}
	
	public static Vector2 CalcTotalVelocityAtContactPoint(Vector2 velocity, float angular_velocity, Vector2 relative)
	{
		return velocity + Vector2.Cross(angular_velocity, relative);
	}
	
	/*
	 real raCrossN = Cross( ra, normal );
    real rbCrossN = Cross( rb, normal );
    real invMassSum = A->im + B->im + Sqr( raCrossN ) * A->iI + Sqr( rbCrossN ) * B->iI;
	
	*/
	
	
	//	Scalar J
	public static float CalcImpulseScalar(float e, float aInv_mass, float aInv_inertia, float bInv_mass, float bInv_inertia,
										  Vector2 normal, Vector2 velocity_AToB, Vector2 r_AToP, Vector2 r_BToP, float contact_count = 1)
	{
		float temp = Vector2.Dot(velocity_AToB, normal);
		if(temp > 0.0f)
			return 0.0f;
		float up = -(1.0f + e) * temp;
		
		temp = Vector2.Cross(r_AToP, normal);
		float a_down = UMath.Pow2(temp) * aInv_inertia;
		
		temp = Vector2.Cross(r_BToP, normal);
		float b_down = UMath.Pow2(temp) * bInv_inertia;
		
		float down = aInv_mass + bInv_mass + a_down + b_down;
		
		return (up / down) / contact_count;
	}
	
	public static Vector2 CalcLinearVelocity(Vector2 impulse_vector, float inv_mass)
	{
		return impulse_vector * inv_mass;
	}
	
	public static float CalcAngularVelocity(Vector2 impulse_vector, float inv_inertia, Vector2 r_CToP)
	{
		float d = Vector2.Cross(r_CToP, impulse_vector);
		return d * inv_inertia;
	}
	
	public static Vector2 CalcFriction(float e, float impulse_scalar, float aInv_mass, float aStatic_friction, float aDynamic_friction, float bInv_mass, float bStatic_friction, float bDynamic_friction, Vector2 normal, Vector2 velocity_AToB, float contact_count = 1)
	{
		Vector2 tangent = velocity_AToB - normal * Vector2.Dot(velocity_AToB,normal);
		tangent.SetNormalize();
		float jt = -(1.0f + e) * Vector2.Dot(velocity_AToB,tangent);
		jt /= (aInv_mass + bInv_mass);
		jt /= contact_count;
		
		float mu = aStatic_friction * aStatic_friction + bStatic_friction * bStatic_friction;
		Vector2 frictionImpulse;
		if(Math.Abs(jt) < impulse_scalar * mu)
			frictionImpulse = tangent * jt;
		else
			frictionImpulse = tangent * (-impulse_scalar * (aDynamic_friction * aDynamic_friction + bDynamic_friction * bDynamic_friction));
		
		return frictionImpulse;
	}
	
	public static Vector2 CalcDrag(float drag_factor, Vector2 velocity, float face = 1.0f, float pop = 1.0f)
	{
		float vsquared = velocity.lengthSquared;
		return velocity.normalize * (pop * vsquared * face * drag_factor * -0.5f);
	}
	
	public static float CalcAngularDrag(float drag_factor, float angular_velocity, float face = 1.0f, float pop = 1.0f)
	{
		float sign = 0.0f;
		
		if(angular_velocity > 0.0f)
			sign = 1.0f;
		else if(angular_velocity < 0.0f)
			sign = -1.0f;
		
		return sign * (pop * angular_velocity * angular_velocity * face * drag_factor * -0.5f);
	}
}