using System.Collections;
using UnityEngine;

public abstract class Services {

	//the task manager
	private static TaskManager taskManager;
	public static TaskManager TaskManager{
		get {  
			Debug.Assert(taskManager != null);
			return taskManager;
		}
		set { taskManager = value; }
	}


	private static LevelManager levelManager;
	public static LevelManager LevelManager{
		get {
			Debug.Assert(levelManager != null);
			return levelManager;
		}
		set { levelManager = value; }
	}


	private static SceneManager<TransitionData> sceneManager;
	public static SceneManager<TransitionData> SceneManager{
		get {
			Debug.Assert(sceneManager != null);
			return sceneManager;
		}
		set { sceneManager = value; }
	}

	private static PrefabDatabase prefabDatabase;
	public static PrefabDatabase PrefabDatabase{
		get {
			Debug.Assert(prefabDatabase != null);
			return prefabDatabase;
		}
		set { prefabDatabase = value; }
	}


	private static TabletopSimulator.VictoryChecker victoryChecker;
	public static TabletopSimulator.VictoryChecker VictoryChecker{
		get {
			Debug.Assert(victoryChecker != null);
			return victoryChecker;
		}
		set { victoryChecker = value; }
	}


	private static TabletopSimulator.TurnManager turnManager;
	public static TabletopSimulator.TurnManager TurnManager{
		get {
			Debug.Assert(turnManager != null);
			return turnManager;
		} set { turnManager = value; }
	}


	private static TabletopSimulator.BoardManager boardManager;
	public static TabletopSimulator.BoardManager BoardManager{
		get {
			Debug.Assert(boardManager != null);
			return boardManager;
		} set { boardManager = value; }
	}
}
