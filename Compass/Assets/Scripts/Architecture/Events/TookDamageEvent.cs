public class TookDamageEvent : Event{


	public readonly SailingShip ship;


	public TookDamageEvent(SailingShip ship){
		this.ship = ship;
	}
}
