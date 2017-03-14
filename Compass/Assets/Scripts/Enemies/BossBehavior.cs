using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : EnemyShip {


	public Vector3 destination = new Vector3(0.0f, 0.0f, 0.0f);


	//----------Internal variables----------
	private Image healthBar;
	private const string HEALTH_BAR_CANVAS = "Life meter canvas";
	private const string HEALTH_BAR = "Life meter";

	private TookDamageEvent.Handler damageFunc;

	protected override void Start(){
		base.Start();

		healthBar = transform.Find(HEALTH_BAR_CANVAS).Find(HEALTH_BAR).GetComponent<Image>();

		damageFunc = new TookDamageEvent.Handler(HandleDamage);
		EventManager.Instance.Register<TookDamageEvent>(damageFunc);

		TravelTask travelTask = new TravelTask(this, destination);
		SpawnTask spawnTask = new SpawnTask(transform,
											Resources.Load("Wind follower") as GameObject);
		travelTask.Then(spawnTask);
		//create and schedule all the tasks here
		RandomAttackTask randomAttackTask = new RandomAttackTask(this, GetComponent<RandomAttack>());
		spawnTask.Then(randomAttackTask);
		Services.TaskManager.AddTask(travelTask);
	}

	protected override void Turn(){ }


	//this Update() is intentionally limited, since this ship is run by tasks instead of by its own update loop
	protected override void Update(){ }


	private void HandleDamage(Event e){
		TookDamageEvent damageEvent = e as TookDamageEvent;

		if (damageEvent.ship == this){
			healthBar.fillAmount = damageEvent.damagePercent;

			if (damageEvent.damagePercent <= 0.0f){
				EventManager.Instance.Unregister<TookDamageEvent>(damageFunc);
			}
		}
	}
}
