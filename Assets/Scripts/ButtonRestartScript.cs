using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class ButtonRestartScript : MonoBehaviour
	{
		public Button yourButton;

		// Start is called before the first frame update
		void Start()
		{
			Button btn = yourButton.GetComponent<Button>();
			btn.onClick.AddListener(TaskOnClick);
		}

		void TaskOnClick()
		{
			var game = (GameController)FindObjectsOfType(typeof(GameController)).SingleOrDefault();
			if (game != null)
			{
				game.Reset();
			}
		}
	}
}