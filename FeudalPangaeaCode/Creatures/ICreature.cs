
using Godot;

/// <summary>
/// The basic functions that every 'creature' must be able to perform
/// Anything that is a thing and does things must follow these guidelines
/// </summary>
public interface ICreature
{
	int GetHP();
	void ChangeHP(int change, DamageSource source);

	/// <summary>
	/// Returns the position at the feet of the creature.
	/// </summary>
	Vector3 GetCreaturePosition();

	/// <summary>
	/// Returns the position at the center of the creature.
	/// </summary>
	/// <returns></returns>
	Vector3 GetCreatureCenter();
	Vector3 GetCreatureVelocity();
	bool IsProtectedUnlessStunned();
	void Stun(float time);
	void Push(Vector3 force);
	CreatureState GetState();
}

//it's probably weird to shove something like this in the global scope but who care, this is so convenient
//also this enumerator describes every possible state a creature can be in, but not every creature needs to implement all of them
public enum CreatureState {Grounded, OpenAir, WallSlide, LedgeHang, Dive, Bonk, Stun, 
StunAir, Dead, DeadAir, Casting, Attack}

/// <summary>
/// DAMAGE SOURCES:
/// [FALL] - Typically damage from falling a long way or into a death plane
/// [BONK] - Blunt force trauma, typically causes stun
/// [MAGIC] - Damage from spells, will probably be split into elemental types later. Goes through defense.
/// </summary>
public enum DamageSource {Fall, Bonk, Magic}