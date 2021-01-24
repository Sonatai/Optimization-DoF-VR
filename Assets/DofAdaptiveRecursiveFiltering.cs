using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DofAdaptiveRecursiveFiltering : MonoBehaviour
{
    //A is a scaling paramter for blurring degree controll
    //D is the corresponding depth map from the rendered color image
    //Df ist the focal length -> with eyetracking the distance to the focus point
    //D(p) ist the value from pixel p from the depth map
    //Alpha ist the weight
    
    public float scalingFactor;
    public float focalLength = 0.2f;
    public float cocMinTreshold;

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
        minimumFocalLength = (scalingFactor * focalLength) / (scalingFactor + cocMinTreshold);
        maximumFocalLength = (scalingFactor * focalLength) / (scalingFactor - cocMinTreshold);
        
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
        
        




        //Variabel für Shader übergeben
        dofMaterial.SetFloat("_FocalLength", focalLength);
        dofMaterial.SetVector("_FocusPoint", new Vector4(controllScript.HitPoint.x,controllScript.HitPoint.y,controllScript.HitPoint.z,0));
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
