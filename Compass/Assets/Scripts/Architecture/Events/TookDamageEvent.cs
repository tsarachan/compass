public class TookDamageEvent : Event{


	public readonly SailingShip ship;
	public readonly float damagePercent;


	public TookDamageEvent(SailingShip ship, float damagePercent){
		this.ship = ship;
		this.damagePercent = damagePercent;
	}
}
