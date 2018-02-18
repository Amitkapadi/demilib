﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/02/13 11:17
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEngine;

namespace DG.De2D
{
    /// <summary>
    /// Sets up the 2D orthographic camera it's attached to
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class De2DCamera : MonoBehaviour
    {
        public enum OrthoMode
        {
            FixedHeight, // Default ortho behaviour
            FixedWidth
        }

        public int targetWidth = 960;
        public int targetHeight = 960;
        public int ppu = 100; // Pixels per unit
        public OrthoMode orthoMode = OrthoMode.FixedHeight;

        Camera _thisCam;

        #region Unity Methods

        void Start()
        {
            Adapt();
            if (Application.isPlaying) this.enabled = false;
        }

        // Here to be executed in edit mode and then disabled at runtime
        void Update() // EDIT MODE only
        {
            Adapt();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the camera to center and fit on the given target.<para/>
        /// Uses code from satchel82: https://answers.unity.com/questions/1231701/fitting-bounds-into-orthographic-2d-camera.html
        /// </summary>
        /// <param name="targetRenderer">Target renderer to use for fitting</param>
        public void FitToTarget(Renderer targetRenderer)
        {
            Bounds bounds = targetRenderer.bounds;
            float camRatio = _thisCam.aspect;
            float targetRatio = bounds.size.x / bounds.size.y;

            this.transform.position = new Vector3(bounds.center.x, bounds.center.y, _thisCam.transform.position.z);
            if (camRatio >= targetRatio) _thisCam.orthographicSize = bounds.size.y * 0.5f;
            else {
                float diff = targetRatio / camRatio;
                _thisCam.orthographicSize = bounds.size.y * 0.5f * diff;
            }
        }

        public void Adapt()
        {
            if (targetWidth <= 0 || targetHeight <= 0 || ppu <= 0) {
                Debug.LogWarning("De2DCamera ::: Values can't be 0 or less", this);
                return;
            }

            // Store the cam
            if (_thisCam == null) {
                _thisCam = this.GetComponent<Camera>();
                if (_thisCam == null) {
                    Debug.LogWarning(string.Format("De2DCamera ::: No Camera found on \"{0}\"", this.name), this);
                    return;
                }
            }

            // Assign ortho value
            if (!_thisCam.orthographic) {
                _thisCam.orthographic = true;
                Debug.Log(string.Format("De2DCamera ::: Setting \"{0}\" camera to orthographic", this.name), this);
            }
            _thisCam.orthographicSize = ((orthoMode == OrthoMode.FixedWidth ? targetWidth : targetHeight) / (float)ppu) * 0.5f;
            if (orthoMode == OrthoMode.FixedWidth) {
                float ratio = (float)Screen.height / Screen.width;
                _thisCam.orthographicSize *= ratio;
            }
        }

#endregion
    }
}