﻿namespace TabletopSimulator
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class BaseMarine : MonoBehaviour {


		////////////////////////////////////////////////////////////////////////
		/// Stats
		////////////////////////////////////////////////////////////////////////
		public int Movement { get; private set; }
		public int BallisticSkill { get; private set; }
		public int Toughness { get; private set; }
		public int Wounds { get; private set; }
		public int Save { get; private set; }
		public int CoverSaveMod { get; set; }
		public float WeaponRange { get; private set; }
		public int WeaponDamage { get; private set; }
		public int WeaponAP { get; private set; }
		public AIPlayer.Factions MyFaction { get; private set; }


		////////////////////////////////////////////////////////////////////////
		/// Miscellany
		////////////////////////////////////////////////////////////////////////
		protected const string COVER_TAG = "Cover";
		protected AIPlayer.Factions enemyFaction;
		protected bool inCover = false;
		protected Rigidbody2D body;
		protected AIPlayer myPlayer;



		protected void Start(){
			Movement = 6;
			BallisticSkill = 3;
			Toughness = 4;
			Wounds = 1;
			Save = 3;
			CoverSaveMod = 0;
			WeaponRange = 24.0f;
			WeaponDamage = 1;
			WeaponAP = 0;
			MyFaction = transform.parent.GetComponent<AIPlayer>().myFaction;
			enemyFaction = GetOtherFaction();
			body = GetComponent<Rigidbody2D>();
			myPlayer = transform.parent.GetComponent<AIPlayer>();
		}


		protected AIPlayer.Factions GetOtherFaction(){
			if (MyFaction == AIPlayer.Factions.Ultramarines){
				return AIPlayer.Factions.DeathGuard;
			} else {
				return AIPlayer.Factions.Ultramarines;
			}
		}


		////////////////////////////////////////////////////////////////////////
		/// Movement
		////////////////////////////////////////////////////////////////////////


		public void MoveTowardEnemy(){
			Transform closestEnemy = FindClosestEnemy();

			Advance(closestEnemy);
		}


		protected void Advance (Transform enemy){
			body.MovePosition(transform.position + (enemy.position - transform.position).normalized * Movement);
		}


		protected void Retreat(Transform enemy){
			body.MovePosition(transform.position + (transform.position - enemy.position).normalized * Movement);
		}


		public void SeekCover(){
			if (inCover) { return; }

			GameObject[] coverObjs = GameObject.FindGameObjectsWithTag(COVER_TAG);

			float minDist = 100000.0f; //nonsense initialization sure to be larger than at least one actual distance
			Transform closestCover = transform; //nonsense initialization for debugging; should always be overridden

			foreach (GameObject cover in coverObjs){
				float dist = Vector3.Distance(transform.position, cover.transform.position);

				if (dist < minDist){
					closestCover = cover.transform;
					minDist = dist;
				}
			}

			Debug.Assert(closestCover != transform);

			if (minDist <= Movement){
				body.MovePosition(closestCover.position);
			} else {
				body.MovePosition(transform.position + 
					(closestCover.position - transform.position).normalized * Movement);
			}
		}


		////////////////////////////////////////////////////////////////////////
		/// Attacking
		////////////////////////////////////////////////////////////////////////


		public void TryToAttack(){
			Transform closestEnemy = FindClosestEnemy();

			if (Vector3.Distance(closestEnemy.position, transform.position) <= WeaponRange){
				AttackRoll(closestEnemy);
			}
		}


		protected Transform FindClosestEnemy(){
			GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyFaction.ToString());

			float minDist = 100000.0f; //nonsense initialization sure to be larger than at least one actual distance
			Transform closestEnemy = transform; //nonsense initialization for debugging; should always be overridden

			foreach (GameObject enemy in enemies){
				float dist = Vector3.Distance(transform.position, enemy.transform.position);

				if (dist < minDist){
					closestEnemy = enemy.transform;
					minDist = dist;
				}
			}

			Debug.Assert(closestEnemy != transform);

			return closestEnemy;
		}


		protected void AttackRoll(Transform enemy){
			if (Random.Range(1, 7) >= BallisticSkill){
				WoundRoll(enemy);
			}
		}


		protected void WoundRoll(Transform enemy){
			if (Random.Range(1, 7) >= 4){
				enemy.GetComponent<BaseMarine>().GetHit(transform);
			}
		}


		////////////////////////////////////////////////////////////////////////
		/// Getting hit and taking damage
		////////////////////////////////////////////////////////////////////////


		public void GetHit(Transform enemy){
			int targetNum = Save + CoverSaveMod - enemy.GetComponent<BaseMarine>().WeaponAP;

			if (Random.Range(1, 7) >= targetNum){
				return;
			} else {
				TakeDamage(enemy.GetComponent<BaseMarine>().WeaponDamage);
			}
		}


		protected void TakeDamage(int amount){
			Wounds -= amount;

			if (Wounds <= 0){
				myPlayer.LoseToySoldier(gameObject);
			}
		}


		////////////////////////////////////////////////////////////////////////
		/// Cover system
		////////////////////////////////////////////////////////////////////////


		protected virtual void OnTriggerEnter2D(Collider2D other){
			if (other.tag == COVER_TAG){
				inCover = true;
				CoverSaveMod = 1;
			}
		}


		protected virtual void OnTriggerExit2D(Collider2D other){
			if (other.tag == COVER_TAG){
				inCover = false;
				CoverSaveMod = 0;
			}
		}
	}
}
