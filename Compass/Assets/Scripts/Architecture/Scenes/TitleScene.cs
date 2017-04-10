using UnityEngine;

public class TitleScene : Scene<TransitionData> {

	private void Update(){
		if (Input.anyKeyDown){
			Services.SceneManager.Swap<Level1Scene>();
		}
	}
}
