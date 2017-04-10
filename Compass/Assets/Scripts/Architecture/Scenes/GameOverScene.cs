using UnityEngine;
using TMPro;

public class GameOverScene : Scene<TransitionData> {


	//the object that displays text
	private TextMeshProUGUI resultText;
	private const string CANVAS_OBJ = "Canvas";
	private const string TEXT_OBJ = "Result text";


	//the messages that can be displayed
	private const string SUCCESS = "Glorious victory!";
	private const string FAILURE = "Shameful defeat!";


	private void Update(){
		if (Input.anyKeyDown){
			Services.SceneManager.Swap<TitleScene>();
		}
	}


	internal override void OnEnter(TransitionData data){
		resultText = transform.Find(CANVAS_OBJ).Find(TEXT_OBJ).GetComponent<TextMeshProUGUI>();

		if (data.success){
			resultText.text = SUCCESS;
		} else {
			resultText.text = FAILURE;
		}
	}
}
