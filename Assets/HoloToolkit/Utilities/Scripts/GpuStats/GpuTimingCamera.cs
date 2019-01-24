﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Utilities
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.XR;

    /// <summary>
    /// Tracks the GPU time spent rendering a camera.
    /// For multi-pass stereo rendering, sampling is made from the beginning of the left eye to the end of the right eye.
    /// </summary>
    public class GpuTimingCamera : MonoBehaviour
    {
        public string TimingTag = "Frame";

        private Camera timingCamera;

        public event Action<float> NewGpuFrameTime;

        private void Start()
        {
            timingCamera = GetComponent<Camera>();
            Debug.Assert(timingCamera, "GpuTimingCamera component must be attached to a Camera.");
        }

        protected void OnPreRender()
        {
            if (timingCamera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Right)
            {
                var gpuFrameTime = GpuStats.GetSampleTime(TimingTag);
                if (gpuFrameTime >= 0.0)
                {
                    NewGpuFrameTime?.Invoke((float)gpuFrameTime);
                }

                GpuStats.BeginSample(TimingTag);
            }
        }

        protected void OnPostRender()
        {
            if (timingCamera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Left
                || (XRSettings.isDeviceActive
                    && (XRSettings.eyeTextureDesc.vrUsage == VRTextureUsage.TwoEyes
                        || XRSettings.eyeTextureDesc.dimension == TextureDimension.Tex2DArray)))
            {
                GpuStats.EndSample();
            }
        }
    }
}