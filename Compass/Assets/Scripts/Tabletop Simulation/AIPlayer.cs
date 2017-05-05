namespace TabletopSimulator
{
	using BehaviorTree;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class AIPlayer : MonoBehaviour {


		////////////////////////////////////////////////////////////////////////
		/// This player's faction
		////////////////////////////////////////////////////////////////////////
		public enum Factions { Ultramarines, DeathGuard };
		public Factions myFaction;


		////////////////////////////////////////////////////////////////////////
		/// This player's toy soldiers
		////////////////////////////////////////////////////////////////////////
		private List<BaseMarine> myPieces = new List<BaseMarine>();
		private List<BaseMarine> piecesToLose = new List<BaseMarine>();


		////////////////////////////////////////////////////////////////////////
		/// The behavior tree that controls this player's behavior, along with
		/// associated variables to make it possible to prioritize the tree
		////////////////////////////////////////////////////////////////////////
		private Tree<AIPlayer> tree;

		//preferences the AI player can have
		public int attackDesire = 1;
		public int coverDesire = 2;
		public int takeCoverThreshold = 5; //how many losses must this player suffer before it gets worried?


		//orders the AI player can execute
		private Priority attack;
		private Sequence<AIPlayer> attackSequence;
		private Priority takeCover;
		private Sequence<AIPlayer> takeCoverSequence;


		////////////////////////////////////////////////////////////////////////
		/// Private class that contains the data needed to organize sequences
		////////////////////////////////////////////////////////////////////////
		private class Priority{


			public string Name { get; set; }
			public Node<AIPlayer> OrderToExecute { get; set; }
			public int CurrentPriority { get; set; }


			public Priority(string name, Node<AIPlayer> order, int priority){
				Name = name;
				OrderToExecute = order;
				CurrentPriority = priority;
			}


			public Priority(){
				Name = "Default";
				OrderToExecute = null;
				CurrentPriority = 0;
			}
		}


		////////////////////////////////////////////////////////////////////////
		/// BEHAVIOR TREE
		////////////////////////////////////////////////////////////////////////


		////////////////////////////////////////////////////////////////////////
		/// Conditions, and the functions they rely on
		////////////////////////////////////////////////////////////////////////


		private class IsLowOnPieces : Node<AIPlayer>{
			public override Result Tick(AIPlayer context){
				return context.myPieces.Count <= context.takeCoverThreshold ? Result.SUCCEED : Result.FAIL;
			}
		}


		////////////////////////////////////////////////////////////////////////
		/// Actions
		////////////////////////////////////////////////////////////////////////


		private class GeneralAdvance : Node<AIPlayer>{
			public override Result Tick(AIPlayer context){
				foreach (BaseMarine marine in context.myPieces){
					marine.MoveTowardEnemy();
				}

				return Result.SUCCEED;
			}
		}


		private class TakeCover : Node<AIPlayer>{
			public override Result Tick(AIPlayer context){
				foreach (BaseMarine marine in context.myPieces){
					marine.SeekCover();
				}

				return Result.SUCCEED;
			}
		}


		private class Attack : Node<AIPlayer>{
			public override Result Tick(AIPlayer context){
				foreach (BaseMarine marine in context.myPieces){
					marine.TryToAttack();
				}

				return Result.SUCCEED;
			}
		}


		////////////////////////////////////////////////////////////////////////
		/// Prioritization system that builds new trees
		////////////////////////////////////////////////////////////////////////


		public void ChooseAction(){
			//Debug.Log("ChooseNextTask() called");

			Priority priority1;
			Priority priority2;
			List<Priority> priorities = new List<Priority>() { attack, takeCover };

			priorities = PutPrioritiesInOrder(priorities);


			//Debug.Log(priorities);

			tree = new Tree<AIPlayer>(new Selector<AIPlayer>(priorities[0].OrderToExecute,
															 priorities[1].OrderToExecute));

			tree.Tick(this);
		}


		/// <summary>
		/// Orders a list of priorities by their CurrentPriority field, high to low
		/// </summary>
		/// <returns>A list of priorities, ordered high to low.</returns>
		/// <param name="priorityList">The list to be put in order.</param>
		private List<Priority> PutPrioritiesInOrder(List<Priority> priorityList){
			List<Priority> temp = new List<Priority>();

			int totalRuns = priorityList.Count;

			for (int i = 0; i < totalRuns; i++){
				int maxSoFar = -1; //nonsense initialization; all CurrentPriorities should always be higher than this
				Priority nextPriority = new Priority();

				for (int j = 0; j < priorityList.Count; j++){
					if (priorityList[j].CurrentPriority > maxSoFar){
						nextPriority = priorityList[j];
						maxSoFar = priorityList[j].CurrentPriority;
					}
				}

				Debug.Assert(nextPriority.Name != "Default");

				//Debug.Log("Adding " + nextPriority.Name);
				temp.Add(nextPriority);
				priorityList.Remove(nextPriority);
			}

			Debug.Assert(temp.Count > 0);

			return temp;
		}


		////////////////////////////////////////////////////////////////////////
		/// Toy soldiers--functions that manage them
		////////////////////////////////////////////////////////////////////////


		public void LoseToySoldier(GameObject toySoldier){
			if (!piecesToLose.Contains(toySoldier.GetComponent<BaseMarine>())){
				piecesToLose.Add(toySoldier.GetComponent<BaseMarine>());
				toySoldier.GetComponent<Renderer>().enabled = false;
			}
		}


		public void CleanUpBoard(){
			if (myPieces.Count > 0){
				for (int i = myPieces.Count - 1; i >=0; i--){
					foreach (BaseMarine piece in piecesToLose){
						if (myPieces[i].gameObject == piece.gameObject){
							GameObject temp = piece.gameObject;

							myPieces.RemoveAt(i);

							Destroy(temp);
						}
					}
				}


				piecesToLose.Clear();
				Debug.Assert(piecesToLose.Count == 0);
			}
		}


		////////////////////////////////////////////////////////////////////////
		/// Setup
		////////////////////////////////////////////////////////////////////////


		//initialize variables
		private void Start(){
			myPieces = GetMyPieces();


			attack = MakeAttackPriority();
			takeCover = MakeTakeCoverPriority();
		}


		/// <summary>
		/// Finds this player's toy soldiers, regardless of where they are in the hierarchy.
		/// </summary>
		/// <returns>A complete list of this player's pieces.</returns>
		private List<BaseMarine> GetMyPieces(){
			GameObject[] pieces = GameObject.FindGameObjectsWithTag(myFaction.ToString());
			List<BaseMarine> temp = new List<BaseMarine>();

			Debug.Assert(pieces.Length > 0);

			foreach (GameObject piece in pieces){
				temp.Add(piece.GetComponent<BaseMarine>());
			}

			return temp;
		}


		private Priority MakeAttackPriority(){
			attackSequence = new Sequence<AIPlayer>(new GeneralAdvance(),
													new Attack());

			return new Priority("Attack", attackSequence, attackDesire);
		}


		private Priority MakeTakeCoverPriority(){
			takeCoverSequence = new Sequence<AIPlayer>(new TakeCover(),
													   new Attack());

			return new Priority("TakeCover", takeCoverSequence, coverDesire);
		}
	}
}
