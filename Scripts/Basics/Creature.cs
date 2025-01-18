
using Godot;

/// <summary>
/// The basic functions that every 'creature' must be able to perform
/// Anything that is a thing and does things must follow these guidelines
/// </summary>
public interface Creature
{
	int GetHP();
	void ChangeHP(int change, DamageSource source);
	void Stun(float time);
	void Push(Vector3 force);
	CreatureState GetState();
}

//it's probably weird to shove something like this in the global scope but who care, this is so convenient
//also this enumerator describes every possible state a creature can be in, but not every creature needs to implement all of them
public enum CreatureState {Grounded, OpenAir, WallSlide, Dive, Bonk, Stun, StunAir, Attack, AttackAir, AttackPoke, PokeStuck, Dead, DeadAir}

//source of damage, can be different depending on where it's used
//for example, ChangeHP with a source of "Fall" is landing too hard while Death with a source of "Fall" is falling into the void
//largely the point is so that I can decide additional things that happen to each character
public enum DamageSource {Fall, Bonk}