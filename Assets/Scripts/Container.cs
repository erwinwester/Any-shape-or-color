using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Container : MonoBehaviour
    {
        public int ShapeCount => this.GetComponentInChildren<ShapeContainer>().transform.childCount;
        public int Number
        {
            get
            {
                char numberAsChar = this.name.Last();
                return (int)char.GetNumericValue(numberAsChar);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}