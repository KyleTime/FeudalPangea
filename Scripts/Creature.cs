
/// <summary>
/// The basic functions that every 'creature' must be able to perform
/// Anything that is a thing and does things must follow these guidelines
/// </summary>
public interface Creature
{
	int GetHP();
	void ChangeHP(int change);
	void Stun(float time);
}
