using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] private float shapeLifespanInSec;

		[SerializeField] private int TargetCount;
		[SerializeField] private float TransformPositionYExtend;
		[SerializeField] private float TransformPositionYBottom;

		[SerializeField] private Vector3 shapeScale;
		[SerializeField] private GameObject[] shapes;

		[SerializeField] private GameObject ContainersGameObject;

		[SerializeField] private TextMeshProUGUI countText0;
		[SerializeField] private TextMeshProUGUI countText1;
		[SerializeField] private TextMeshProUGUI countText2;
		[SerializeField] private TextMeshProUGUI countText3;
		[SerializeField] private TextMeshProUGUI countTextLeft;
		[SerializeField] private TextMeshProUGUI countTextRight;

		[SerializeField] private TextMeshProUGUI TextYouWin;
		[SerializeField] private Button ButtonRestart;

		private GameObject newShape;

		private int[] count;

		public int CountLeft => count.Length != 4 ? 0 : count[0] + count[1];
		public int CountRight => count.Length != 4 ? 0 : count[2] + count[3];
		public int CountDifference => CountRight - CountLeft;
		private bool InBalance => (CountDifference == 0);
		private bool TargetCountReached => InBalance && CountLeft == TargetCount && CountRight == TargetCount;

		// Start is called before the first frame update
		void Start()
		{
			Reset();
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void Reset()
		{
			DestroyAllShapes();
			ResetCounters();
			InstantiateNewShape();
		}

		public void IncrementOrSetCounter(int index, int? setValue)
		{
			if (index >= 0 && index <= 5)
			{
				if (setValue.HasValue)
				{
					count[index] = setValue.Value;
				}
				else
				{
					count[index] += 1;
				}
			}

			UpdateCountText();
		}

		private void UpdateCountText()
		{
			countText0.text = count[0].ToString();
			countText1.text = count[1].ToString();
			countText2.text = count[2].ToString();
			countText3.text = count[3].ToString();

			countText0.color = Color.white;
			countText1.color = Color.white;
			countText2.color = Color.white;
			countText3.color = Color.white;

			// Total count left and right
			countTextLeft.text = CountLeft.ToString();
			countTextRight.text = CountRight.ToString();
			// ! Show balance in some way
			float yLeft = TransformPositionYBottom + TransformPositionYExtend * ((float)CountLeft / TargetCount);
			float yRight = TransformPositionYBottom + TransformPositionYExtend * ((float)CountRight / TargetCount);
			countTextLeft.transform.position = new Vector3(countTextLeft.transform.position.x, yLeft, countTextLeft.transform.position.z);
			countTextRight.transform.position = new Vector3(countTextRight.transform.position.x, yRight, countTextRight.transform.position.z);

			// find the container count with the highest count
			var (number, index) = count.Select((n, i) => (n, i)).Max();
			if (index >= 0 && index <= 3)
			{
				switch (index)
				{
					case 0: countText0.color = Color.yellow; break;
					case 1: countText1.color = Color.yellow; break;
					case 2: countText2.color = Color.yellow; break;
					case 3: countText3.color = Color.yellow; break;
				}
			}
		}

		private void ResetCounters()
		{
			TextYouWin.enabled = false;
			ButtonRestart.gameObject.SetActive(false);
			count = new int[] { 0, 0, 0, 0 };
			UpdateCountText();
		}

		private void InstantiateNewShape()
		{
			// Target count reached?
			// The two sides must be in balance and at the target count exactly
			if (TargetCountReached)
			{
				TextYouWin.enabled = true;
				ButtonRestart.gameObject.SetActive(true);
				return;
			}

			// shape
			GameObject gameObject = shapes[Random.Range(0, shapes.Length)];
			newShape = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
			Shape shapeScript = gameObject.GetComponent<Shape>();

			// position
			newShape.transform.position = new Vector3(0f, 4f, 0f);
			newShape.transform.rotation = new Quaternion(30f, 30f, 30f, 0f);
			newShape.transform.localScale = shapeScale;

			// Set shape to not fall
			newShape.GetComponent<Rigidbody>().isKinematic = true;
		}

		internal void MoveShapeToContainer(ContainerFrontWall target)
		{
			// Get container that target (Frontwall) belongs to.
			Container container = target.ParentContainer;
			ShapeContainer shapeContainer = target.ShapeContainer;

			//if (target.shapeColor != ShapeColor.Any && target.shapeColor != shapeColor) { return; }
			//if (target.shapeType != ShapeType.Any && target.shapeType != shapeType) { return; }
			// Any shape and color is now allowed
			// If the new shape type and color conflicts with shapes already in the container
			//		then destroy the existing shapes and keep the new
			Shape shape = newShape.GetComponent<Shape>();
			DestroyConflictingShapes(shapeContainer, shape);

			// position
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 targetPos = target.transform.position;

			var newPosition = new Vector3(mouseWorldPos.x, mouseWorldPos.y, targetPos.z + 0.5f);
			//newShape.transform.position = target.transform.position + new Vector3(0f, -1f, 0.5f);
			newShape.transform.position = newPosition;

			// set container as parent
			newShape.transform.parent = shapeContainer.transform;

			int number = container.Number;
			if (number >= 0 && number <= 5)
			{
				Debug.Log(container.ShapeCount);
				IncrementOrSetCounter(number, container.ShapeCount);

				// destroy na 60 seconden
				StartCoroutine(DestroyShape(newShape, shapeLifespanInSec));
			}

			// Set shape to not fall
			newShape.GetComponent<Rigidbody>().isKinematic = false;

			InstantiateNewShape();
		}

		private void DestroyConflictingShapes(ShapeContainer shapeContainer, Shape newShape)
		{
			foreach (Transform shapeTransform in shapeContainer.GetComponentsInChildren<Transform>().Where(s => s.GetComponent<Shape>() != null))
			{
				GameObject shapeGameObject = shapeTransform.gameObject;
				Shape shape = shapeTransform.GetComponent<Shape>();
				if (shape != null)
				{
					if (shape.Type != newShape.Type && shape.Color != newShape.Color)
					{
						float secondsToDestroy = Random.Range(0.5f, 2f);
						// destroy
						StartCoroutine(DestroyShape(shapeGameObject, secondsToDestroy));
					}
				}
			}
		}

		private void DestroyAllShapes()
		{
			foreach (Shape shapeTransform in ContainersGameObject.GetComponentsInChildren<Shape>())
			{
				float secondsToDestroy = Random.Range(0.5f, 2f);
				StartCoroutine(DestroyShape(shapeTransform.gameObject, secondsToDestroy));
			}
		}

		public IEnumerator DestroyShape(GameObject shapeGameObject, float delay = 0.0f)
		{
			var shape = shapeGameObject.GetComponent<Shape>();
			var container = shapeGameObject.GetComponentInParent<Container>();

			if (delay != 0)
			{
				yield return new WaitForSeconds(delay);
			}

			if (!shape.IsDestroying)
			{
				shape.IsDestroying = true;

				int newCount = container.ShapeCount - 1;
				Destroy(shapeGameObject);
				//yield return null; // Allow shape to be destroyed so wait until next frame (it didn't work straight after)

				int number = container.Number;
				if (number >= 0 && number <= 3)
				{
					IncrementOrSetCounter(number, newCount);
				}
			}
		}
	}
}
