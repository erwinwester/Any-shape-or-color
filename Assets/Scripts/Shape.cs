using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Shape : MonoBehaviour
    {
        private float bottom;
        public bool IsDestroying { get; set; }

        public ShapeType Type;
        public ShapeColor Color;

        // Start is called before the first frame update
        void Start()
        {
            Vector3 worldBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
            bottom = worldBottom.y;
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsDestroying)
            {
                // if below bottom of screen, destroy (this) instance
                if (this != null && this.transform.position.y < bottom)
                {
                    // destroy shape
                    var game = (GameController)FindObjectsOfType(typeof(GameController)).SingleOrDefault();
                    if (game != null)
                    {
                        StartCoroutine(game.DestroyShape(gameObject));
                    }
                }
            }
        }
    }
}