using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DofAdaptiveRecursiveFiltering : MonoBehaviour
{
    //A is a scaling paramter for blurring degree controll
    //D is the corresponding depth map from the rendered color image
    //Df ist the focal length -> Distance between the eye and the object
    //D(p) ist the value from pixel p from the depth map
    //Alpha ist the weight
    
    public float scalingFactor;
    [Range(0,1)]
    public float focalLength = 0.2f;
    public float cocMinTreshold; //CoCMin cant be bigger than scalingFactor

    [SerializeField] private Controlls controllScript;

    [HideInInspector] public Shader dofShader;
    [NonSerialized] private Material dofMaterial;
    private float minimumFocalLength;
    private float maximumFocalLength;

 
    private const int circleOfConfusionPass = 0;
    private const int regionPass = 1;
    private const int weightPass = 2;
    private const int filterPass = 3;


    void OnRenderImage (RenderTexture source, RenderTexture destination) {
        if (dofMaterial == null) {
            dofMaterial = new Material(dofShader);
            dofMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        
        focalLength = controllScript.FocalLength;
        
        //Minimum/maximum focal length calculation 
        //Note: Unity depth map is between 0 and 1
        minimumFocalLength = Math.Min((scalingFactor * focalLength) / (scalingFactor + cocMinTreshold),1);
        maximumFocalLength = Math.Min((scalingFactor * focalLength) / (scalingFactor - cocMinTreshold),1);
        
        //Texture Maps
        RenderTexture coc = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear
        );
        
        RenderTexture region = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear
        );
        
        RenderTexture weight = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear
        );

        //Variabel for shader
        dofMaterial.SetFloat("_FocalLength", focalLength);
        dofMaterial.SetFloat("_MinimumFocalLength", minimumFocalLength);
        dofMaterial.SetFloat("_MaximumFocalLength", maximumFocalLength);
        
        dofMaterial.SetFloat("_ScalingFactor",scalingFactor);

        dofMaterial.SetTexture("_CoCTex", coc);
        dofMaterial.SetTexture("_RegionTex", region);
        dofMaterial.SetTexture("_WeightTex", weight);
        
        //rendering
        Graphics.Blit(source, coc, dofMaterial, circleOfConfusionPass);
        Graphics.Blit(source,region, dofMaterial, regionPass);
        Graphics.Blit(source,weight, dofMaterial, weightPass);
        Graphics.Blit(source,destination, dofMaterial, filterPass);
        
        RenderTexture.ReleaseTemporary(coc);
        RenderTexture.ReleaseTemporary(region);
        RenderTexture.ReleaseTemporary(weight);
    }
}
