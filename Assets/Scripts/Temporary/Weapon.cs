using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class Weapon : MonoBehaviour
    {
        public float m_BulletDamage;
        public GameObject m_HitDecal;
        public Vector3 m_MidPoint { get; private set; }

        public AnimationCurve m_VerticalRecoil;

        // ============= INTERNAL BOOKKEEPING ============= //
        [HideInInspector]
        public float m_VerticalProgress = 0.0f; // To track position on the recoil curve.

        // ================================================ //


        // Start is called before the first frame update
        void Start()
        {
            m_MidPoint = transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Shoot is a temporary function to allow testing of weapon recoil.
        /// </summary>
        /// <param name="objCamera">The transform that will be receiving the vertical recoil.</param>
        public float ShootRecoil(Transform objCamera, float timeHeld)
        {
            Debug.Log(timeHeld);
            return m_VerticalRecoil.Evaluate(timeHeld);
        }

     
    }
