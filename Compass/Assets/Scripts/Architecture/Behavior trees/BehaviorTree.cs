namespace BehaviorTree{

	using System;

	#region building blocks

	//nodes of the tree
	public abstract class Node<T> {
		public enum Result { SUCCEED, FAIL, RUNNING, ERROR };


		public abstract Result Tick(T context);
	}


	//the tree
	public class Tree<T> : Node<T>{
		private readonly Node<T> root;


		public Tree(Node<T> root){
			this.root = root;
		}


		public override Result Tick(T context){
			return root.Tick(context);
		}
	}

	#endregion


	#region node types


	//ACTION NODE: do something
	public class Action<T> : Node<T>{
		public delegate Result NodeAction(T context);


		private readonly NodeAction action;


		public Action(NodeAction action){
			this.action = action;
		}


		public override Result Tick(T context){
			return action(context);
		}



	}


	//CONDITION NODE: get information a composite node needs to decide what to do next
	//IMPORTANT: this does not allow for the RUNNING or ERROR states; those will have to wait
	//for a more complex implementation
	public class Condition<T> : Node<T>{
		private readonly Predicate<T> condition;


		public Condition(Predicate<T> condition){
			this.condition = condition;
		}


		public override Result Tick(T context){
			if (condition(context)){
				return Result.SUCCEED;
			} else {
				return Result.FAIL;
			}
		}
	}


	//decision nodes

	//base class for decision nodes
	public abstract class BranchNode<T> : Node<T>{
		protected Node<T>[] Children { get; private set; }


		protected BranchNode(params Node<T>[] children){
			Children = children;
		}
	}


	//SELECTOR NODE: returns success after the first child succeeds.
	//if no child succeeds, returns fail
	public class Selector<T> : BranchNode<T>{
		public Selector(params Node<T>[] children) : base(children) { }


		public override Result Tick(T context){
			foreach (var child in Children){
				if (child.Tick(context) == Result.SUCCEED) return Result.SUCCEED; 
			}

			return Result.FAIL;
		}
	}


	//SEQUENCE NODE: returns success if all children return success
	//returns fail immediately upon any child returning fail
	public class Sequence<T> : BranchNode<T>{
		public Sequence(params Node<T>[] children) : base(children) { }

		public override Result Tick(T context){
			foreach (var child in Children){
				if (child.Tick(context) != Result.SUCCEED) return Result.FAIL;
			}

			return Result.SUCCEED;
		}
	}


	//DECORATOR NODE: base class for nodes that modify the return of another node
	//subclasses must determine how the return is modified
	public abstract class Decorator<T> : Node<T>{
		//the node to be modified
		protected Node<T> Child { get; private set; }


		protected Decorator(Node<T> child){
			Child = child;
		}
	}


	#region decorators


	//decorator that inverts the return of the decorated node, from success to fail or vice-versa
	public class Not<T> : Decorator<T>{
		public Not(Node<T> child) : base(child) { }


		public override Result Tick(T context){
			if(Child.Tick(context) == Result.SUCCEED){
				return Result.FAIL;
			} else if (Child.Tick(context) == Result.FAIL){
				return Result.SUCCEED;
			} else {
				return Result.ERROR;
			}
		}
	}

	#endregion

	#endregion
}
