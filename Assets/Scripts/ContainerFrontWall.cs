using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ContainerFrontWall : MonoBehaviour
    {
        [SerializeField] private GameController gameController;
        //public GameController.ShapeType shapeType;
        //public GameController.ShapeColor shapeColor;

        private Renderer objectRenderer;
        private Color orgColor;
        private Color tempColor;
        public Container ParentContainer => this.transform.parent.parent.GetComponent<Container>();
        public ShapeContainer ShapeContainer => ParentContainer.GetComponentInChildren<ShapeContainer>();

        // Start is called before the first frame update
        void Start()
        {
            objectRenderer = GetComponent<Renderer>();

            orgColor = Color.white;
            orgColor.a = 0.0f;

            tempColor = Color.red;
            tempColor.a = 0.3f;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnMouseEnter()
        {
            objectRenderer.material.color = tempColor;
        }

        private void OnMouseExit()
        {
            objectRenderer.material.color = orgColor;
        }

        private void OnMouseOver()
        {
            //gameController.MoveShapeToContainer(this);
        }

        private void OnMouseDown()
        {
            //gameController.MoveShapeToContainer(this);
        }

        private void OnMouseUpAsButton()
        {
            gameController.MoveShapeToContainer(this);
        }
    }
}