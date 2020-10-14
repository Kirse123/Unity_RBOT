using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBOT_UnityPlugin
{
    public class Object3D
    {
        private string _fullFileName;
        public string FullFileName
        {
            get
            {
                return _fullFileName;

            }
        }

        private float _tx;
        public float tx
        {
            get
            {
                return _tx;

            }
        }

        private float _ty;
        public float ty
        {
            get
            {
                return _ty;
            }
        }

        private float _tz;
        public float tz
        {
            get
            {
                return _tz;
            }
        }

        private float _alpha;
        public float alpha
        {
            get
            {
                return _alpha;
            }
        }

        private float _beta;
        public float beta
        {
            get
            {
                return _beta;
            }
        }

        private float _gamma;
        public float gamma
        {
            get
            {
                return _gamma;
            }
        }

        private float _scale;
        public float scale
        {
            get
            {
                return _scale;
            }
        }

        public bool isTracking;

        public float[,] TRS;

        public Object3D(string fullFileName, float tx, float ty, float tz, float alpha, float beta, float gamma, float scale)
        {
            _fullFileName = fullFileName;
            _tx = tx;
            _ty = ty;
            _tz = tz;
            _alpha = alpha;
            _beta = beta;
            _gamma = gamma;
            _scale = scale;

            isTracking = false;

            TRS = new float[4, 4];
        }
    }
}


