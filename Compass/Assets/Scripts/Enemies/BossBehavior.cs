using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : EnemyShip {


	public Vector3 destination = new Vector3(0.0f, 0.0f, 0.0f);


	//----------Internal variables----------
	private Image healthBar;

	private TookDamageEvent.Handler damageFunc;

	protected override void Start(){
		base.Start();

		damageFunc = new TookDamageEvent.Handler(HandleDamage);
		EventManager.Instance.Register<TookDamageEvent>(damageFunc);

		TravelTask travelTask = new TravelTask(this, destination);
		//create and schedule all the tasks here
		RandomAttackTask randomAttackTask = new RandomAttackTask(this, GetComponent<RandomAttack>());
		Services.TaskManager.AddTask(travelTask);
	}

	protected override void Turn(){ }


	//this Update() is intentionally limited, since this ship is run by tasks instead of by its own update loop
	protected override void Update(){ }


	private void HandleDamage(Event e){
		TookDamageEvent damageEvent = e as TookDamageEvent;

		if (damageEvent.ship == this){
			healthBar.fillAmount = GetHealthPercentage();

			if (healthBar.fillAmount <= 0.0f){
				EventManager.Instance.Unregister<TookDamageEvent>(damageFunc);
			}
		}
	}
}
