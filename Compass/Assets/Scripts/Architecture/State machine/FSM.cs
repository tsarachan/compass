using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<TContext> {


	//this state machine's context
	private readonly TContext context;


	//dictionary of states
	private readonly Dictionary<System.Type, State> stateCache = new Dictionary<System.Type, State>();


	//the current state
	public State CurrentState { get; private set; }


	//this variable holds the next state when there's to be a transition, so that transitions don't have to
	//interrupt a state in progress
	private State pendingState;


	//constructor to set the readonly context
	public FSM(TContext context){
		this.context = context;
	}


	public void Update(){
		//handle pending transitions started outside this FSM
		PerformPendingTransition();

		//update the current state
		Debug.Assert(CurrentState != null, "No current state");
		CurrentState.Update();

		//handle pending transitions started during the update
		PerformPendingTransition();
	}

	//queues up a transition to a new state
	public void TransitionTo<TState>() where TState : State {
		pendingState = GetOrCreateState<TState>();
	}


	//complete a transition to a pending state
	private void PerformPendingTransition(){
		if (pendingState != null){
			if (CurrentState != null) CurrentState.OnExit();
			CurrentState = pendingState;
			CurrentState.OnEnter();
			pendingState = null;
		}
	}


	private TState GetOrCreateState<TState>() where TState : State {
		State state;

		if (stateCache.TryGetValue(typeof(TState), out state)){
			return (TState)state;
		} else {
			var newState = Activator.CreateInstance<TState>();
			newState.Parent = this;
			newState.Init();
			stateCache[typeof(TState)] = newState;

			return newState;
		}
	}


	public abstract class State {


		//the state machine for this state
		internal FSM<TContext> Parent { get; set; }


		//easy access to this state's context
		protected TContext Context { get { return Parent.context; } }


		//used to have this state call for a transition to a different state
		protected void TransitionTo<TState>() where TState : State{
			Parent.TransitionTo<TState>();
		}


		//called when state is first created
		public virtual void Init() { }


		//called when state becomes active
		public virtual void OnEnter() { }


		//called when state becomes inactive
		public virtual void OnExit() { }


		//update method
		public virtual void Update() { }


		//called when state machine in cleared
		public virtual void Cleanup() { }
	}
}
