﻿using System;
using HTC.UnityPlugin.FoveatedRendering;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class DofAdaptiveRecursiveFiltering : MonoBehaviour
{
    //A is a scaling paramter for blurring degree controll
    //D is the corresponding depth map from the rendered color image
    //Df ist the focal length -> Distance between the eye and the object
    //D(p) ist the value from pixel p from the depth map
    //Alpha ist the weight

    [SerializeField] private float scalingFactor = 1f;
    [Range(0, 65)] [SerializeField] private float focalLength = 36f;
    [SerializeField] private float cocMinThreshold = 0.03f;

    [SerializeField] private Camera camera;

    [SerializeField] private bool isOptimized;

    public float FocalLength
    {
        set => focalLength = value;
    }
    
    public bool IsOptimized
    {
        set => isOptimized = value;
    }

    [HideInInspector] public Shader dofShader;
    [HideInInspector] public Shader optimizedDofShader;
    [NonSerialized] private Material dofMaterial;
    [NonSerialized] private Material optimizedDofMaterial;

    private float minimumFocalLength;
    private float maximumFocalLength;

    [SerializeField] private DebugType debugMode = DebugType.None;

    private const int circleOfConfusionPass = 0;
    private const int regionPass = 1;
    private const int weightLeftRightPass = 2;
    private const int weightBotTopPass = 3;
    private const int filterPass = 4;

    //Debug Passes Opti:
    private const int debugInnerRegion = 5;
    private const int debugMiddleRegion = 6;
    private const int debugInnerCoc = 7;
    private const int debugMiddleCoc = 8;
    private const int debugInnerWeight = 9;
    private const int debugMiddleWeight = 10;

    //Debug Passes Normi:
    private const int debugRegion = 5;
    private const int debugCoc = 6;
    private const int debugWeight = 7;
    
    //For radii calculation
    Vector2 eyeResolution = Vector2.one;
    Vector3 normalizedGazeDirection = new Vector3(0.0f, 0.0f, 1.0f);
    [SerializeField] private ViveFoveatedRendering viveFoveatedRendering;

    void Start()
    {
        camera.depthTextureMode = DepthTextureMode.Depth;

        //========= Copyright 2020, HTC Corporation. All rights reserved. ===========
        if (XRSettings.enabled)
        {
            eyeResolution.x = XRSettings.eyeTextureWidth;
            eyeResolution.y = XRSettings.eyeTextureHeight;
        }
        else
        {
            eyeResolution.x = Screen.width;
            eyeResolution.y = Screen.height;
        }

        //===========================================================================
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (dofMaterial == null)
        {
            dofMaterial = new Material(dofShader);
            dofMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        if (optimizedDofMaterial == null)
        {
            optimizedDofMaterial = new Material(optimizedDofShader);
            optimizedDofMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        //Far Plain Distance as max value for both
        minimumFocalLength = Math.Min((scalingFactor * focalLength) / (scalingFactor + cocMinThreshold),
            camera.farClipPlane);
        maximumFocalLength = Math.Min((scalingFactor * focalLength) / (scalingFactor - cocMinThreshold),
            camera.farClipPlane);

        var coc = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear
        );

        var region = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear
        );

        var weightLeftRight = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear
        );

        var weightTopBot = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear
        );

        if (isOptimized)
        {
            optimizedDofMaterial.SetFloat("_FocalLength", focalLength);
            optimizedDofMaterial.SetFloat("_MinimumFocalLength", minimumFocalLength);
            optimizedDofMaterial.SetFloat("_MaximumFocalLength", maximumFocalLength);
            optimizedDofMaterial.SetFloat("_ScalingFactor", scalingFactor);
            optimizedDofMaterial.SetFloat("_CocTreshhold", cocMinThreshold);

            optimizedDofMaterial.SetTexture("_CoCTex", coc);
            optimizedDofMaterial.SetTexture("_RegionTex", region);
            optimizedDofMaterial.SetTexture("_WeightLeftRightTex", weightLeftRight);
            optimizedDofMaterial.SetTexture("_WeightTopBotTex", weightTopBot);

            //========= Copyright 2020, HTC Corporation. All rights reserved. ===========
            var tanHalfVerticalFov = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView / 2.0f);
            var tanHalfHorizontalFov = tanHalfVerticalFov * camera.aspect;

            Vector2 gazeData = Vector2.zero;
            gazeData.x = (normalizedGazeDirection.x / normalizedGazeDirection.z) / tanHalfHorizontalFov;
            gazeData.y = (normalizedGazeDirection.y / normalizedGazeDirection.z) / tanHalfVerticalFov;
            gazeData.x = -gazeData.x;

            gazeData = (gazeData + Vector2.one) / 2.0f;
            optimizedDofMaterial.SetVector("_GazeData", new Vector4(gazeData.x, gazeData.y, 0, 0));

            var innerRadii = viveFoveatedRendering.GetRegionRadii(TargetArea.INNER) * 0.5f;
            var middleRadii = (viveFoveatedRendering.GetRegionRadii(TargetArea.INNER) * 1.15f) * 0.5f;
            //var middleRadii = viveFoveatedRendering.GetRegionRadii(TargetArea.MIDDLE)  * 0.5f;

            //  To keep the shape of given region, invert the aspect ratio.
            //  Align short side.
            if (eyeResolution.x > eyeResolution.y)
            {
                var ratio = eyeResolution.y / eyeResolution.x;
                innerRadii.x *= ratio;
                middleRadii.x *= ratio;
            }
            else
            {
                var ratio = eyeResolution.x / eyeResolution.y;
                innerRadii.y *= ratio;
                middleRadii.y *= ratio;
            }

            //  For calculation convenience with single pass stereo mode, the x range in UV space is divided by 2.
            Vector4 gazePointRadii = new Vector4(0.005f, 0.005f, 0.0f, 0.0f);
            if (XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.SinglePass)
            {
                gazePointRadii.x *= 0.5f;
                innerRadii.x *= 0.5f;
                middleRadii.x *= 0.5f;
            }

            optimizedDofMaterial.SetVector("_GazePointRadii", gazePointRadii);
            optimizedDofMaterial.SetVector("_InnerRadii", new Vector4(innerRadii.x, innerRadii.y, 0, 0));
            optimizedDofMaterial.SetVector("_MiddleRadii", new Vector4(middleRadii.x, middleRadii.y, 0, 0));
            //===============================================================

            switch (debugMode)
            {
                case DebugType.None:
                    Graphics.Blit(source, coc, optimizedDofMaterial, circleOfConfusionPass);
                    Graphics.Blit(source, region, optimizedDofMaterial, regionPass);
                    Graphics.Blit(source, weightLeftRight, optimizedDofMaterial, weightLeftRightPass);
                    Graphics.Blit(source, weightTopBot, optimizedDofMaterial, weightBotTopPass);
                    Graphics.Blit(source, destination, optimizedDofMaterial, filterPass);
                    break;
                case DebugType.InnerRegion:
                    Graphics.Blit(source, region, optimizedDofMaterial, regionPass);
                    Graphics.Blit(region, destination, optimizedDofMaterial, debugInnerRegion);
                    break;
                case DebugType.MiddleRegion:
                    Graphics.Blit(source, region, optimizedDofMaterial, regionPass);
                    Graphics.Blit(region, destination, optimizedDofMaterial, debugMiddleRegion);
                    break;
                case DebugType.InnerCoc:
                    Graphics.Blit(source, destination, optimizedDofMaterial, circleOfConfusionPass);
                    break;
                case DebugType.MiddleCoc:
                    Graphics.Blit(source, destination, optimizedDofMaterial, circleOfConfusionPass);
                    break;
                case DebugType.InnerWeight:
                    Graphics.Blit(source, region, optimizedDofMaterial, regionPass);
                    Graphics.Blit(source, coc, optimizedDofMaterial, circleOfConfusionPass);
                    Graphics.Blit(source, destination, optimizedDofMaterial, weightLeftRightPass);
                    break;
                case DebugType.MiddleWeight:
                    Graphics.Blit(source, region, optimizedDofMaterial, regionPass);
                    Graphics.Blit(source, coc, optimizedDofMaterial, circleOfConfusionPass);
                    Graphics.Blit(source, destination, optimizedDofMaterial, weightLeftRightPass);
                    break;
            }
        }
        else
        {
            dofMaterial.SetFloat("_FocalLength", focalLength);
            dofMaterial.SetFloat("_MinimumFocalLength", minimumFocalLength);
            dofMaterial.SetFloat("_MaximumFocalLength", maximumFocalLength);
            dofMaterial.SetFloat("_ScalingFactor", scalingFactor);
            dofMaterial.SetFloat("_CocTreshhold", cocMinThreshold);

            dofMaterial.SetTexture("_CoCTex", coc);
            dofMaterial.SetTexture("_RegionTex", region);
            dofMaterial.SetTexture("_WeightLeftRightTex", weightLeftRight);
            dofMaterial.SetTexture("_WeightTopBotTex", weightTopBot);

            switch (debugMode)
            {
                case DebugType.None:
                    Graphics.Blit(source, coc, dofMaterial, circleOfConfusionPass);
                    Graphics.Blit(source, region, dofMaterial, regionPass);
                    Graphics.Blit(source, weightLeftRight, dofMaterial, weightLeftRightPass);
                    Graphics.Blit(source, weightTopBot, dofMaterial, weightBotTopPass);
                    Graphics.Blit(source, destination, dofMaterial, filterPass);
                    break;
                case DebugType.Region:
                    Graphics.Blit(source, region, dofMaterial, regionPass);
                    Graphics.Blit(region, destination, dofMaterial, debugRegion);
                    break;
                case DebugType.Coc:
                    Graphics.Blit(source, destination, dofMaterial, circleOfConfusionPass);
                    break;
                case DebugType.Weight:
                    Graphics.Blit(source, region, dofMaterial, regionPass);
                    Graphics.Blit(source, coc, dofMaterial, circleOfConfusionPass);
                    Graphics.Blit(source, destination, dofMaterial, weightLeftRightPass);
                    break;
            }

            
        }

        RenderTexture.ReleaseTemporary(coc);
        RenderTexture.ReleaseTemporary(region);
        RenderTexture.ReleaseTemporary(weightLeftRight);
        RenderTexture.ReleaseTemporary(weightTopBot);
    }
}

enum DebugType
{
    None,
    Region,
    InnerRegion,
    MiddleRegion,
    Coc,
    InnerCoc,
    MiddleCoc,
    Weight,
    InnerWeight,
    MiddleWeight
}