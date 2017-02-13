public class ShipSinkEvent : Event {

	public readonly SailingShip ship;


	public ShipSinkEvent(SailingShip ship){
		this.ship = ship;
	}
}
