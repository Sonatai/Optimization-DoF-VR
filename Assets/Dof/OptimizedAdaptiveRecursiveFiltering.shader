Shader "Hidden/OptimizedDoFAlgorithmus"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GazeData("GazeData", Vector) = (0, 0, 0, 0)
		_GazePointRadii("GazePointRadii", Vector) = (0, 0, 0, 0)
		_InnerRadii("InnerRadii", Vector) = (0, 0, 0, 0)
		_MiddleRadii("MiddleRadii", Vector) = (0, 0, 0, 0)
    }
    CGINCLUDE
        #include "UnityCG.cginc"
        
        sampler _LastCameraDepthTexture, _CoCTex, _RegionTex, _WeightLeftRightTex, _WeightTopBotTex;
        float4 _MainTex_TexelSize;
        float _FocalLength, _ScalingFactor, _MinimumFocalLength, _MaximumFocalLength, _CocTreshhold, _Delta = pow(10,-5);
        
        UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
		half4 _MainTex_ST;
		
	    float4 _GazeData, _GazePointRadii, _InnerRadii, _MiddleRadii;
              
        struct VertexData {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        
        struct Interpolators {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			
			UNITY_VERTEX_OUTPUT_STEREO
		};
		
		Interpolators VertexProgram (VertexData v) {
			Interpolators i;
			
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_OUTPUT(Interpolators, i);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(i);
				
			i.pos = UnityObjectToClipPos(v.vertex);
			i.uv = v.uv;
			return i;
		}
		
		//========= Copyright 2020, HTC Corporation. All rights reserved. ===========
		bool InsideEllipse(Interpolators i)
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

            float2 pixelPos = i.uv;
            float2 center = _GazeData.xy;

            #if UNITY_SINGLE_PASS_STEREO
                pixelPos = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
                center = UnityStereoScreenSpaceUVAdjust(_GazeData.xy, _MainTex_ST);
                center.x += lerp(0.0125f, -0.0125f, step(0.5f, center.x));
            #else
                //	For multi pass and single pass stereo instanced.
                center.x += lerp(0.025, -0.025, unity_StereoEyeIndex);
            #endif 

            float2 calcMiddle = pow((pixelPos - center) / _MiddleRadii.xy, 2);
            return (calcMiddle.x + calcMiddle.y) <= 1;
        }
        
        bool InsideInInner(Interpolators i){
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

            float2 pixelPos = i.uv;
            float2 center = _GazeData.xy;

            #if UNITY_SINGLE_PASS_STEREO
                pixelPos = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
                center = UnityStereoScreenSpaceUVAdjust(_GazeData.xy, _MainTex_ST);
                center.x += lerp(0.0125f, -0.0125f, step(0.5f, center.x));
            #else
                //	For multi pass and single pass stereo instanced.
                center.x += lerp(0.025, -0.025, unity_StereoEyeIndex);
            #endif 
            
            float2 calcInner = pow((pixelPos - center) / _InnerRadii.xy, 2);
            return (calcInner.x + calcInner.y) <= 1;
        }
        //===============================================================
        
        
			
		float2 GetCocs(Interpolators i, fixed2 texelShift) {
            //Bilineare Interpolation
            float4 texel = _MainTex_TexelSize.xyxy * float2(-0.5, 0.5).xxyy;
            float coc0 = tex2D(_CoCTex, i.uv + texel.xy).r;
            float coc1 = tex2D(_CoCTex, i.uv + texel.zy).r;
            float coc2 = tex2D(_CoCTex, i.uv + texel.xw).r;
            float coc3 = tex2D(_CoCTex, i.uv + texel.zw).r;
            
            float cocP = (coc0 + coc1 + coc2 + coc3) * 0.25;
            fixed2 uvOfNeightbour= (i.uv + texelShift);
            
            coc0 = tex2D(_CoCTex, uvOfNeightbour + texel.xy).r;
            coc1 = tex2D(_CoCTex, uvOfNeightbour + texel.zy).r;
            coc2 = tex2D(_CoCTex, uvOfNeightbour + texel.xw).r;
            coc3 = tex2D(_CoCTex, uvOfNeightbour + texel.zw).r;
            
            float cocQ = (coc0 + coc1 + coc2 + coc3) * 0.25;
            
            return float2(cocP, cocQ);
	    }
		
		half GetWeight(Interpolators i, fixed2 texelShift) {
		    int regionP = tex2D(_RegionTex, i.uv).r;
			int regionQ = tex2D(_RegionTex, i.uv + texelShift).r;
			
		    int FOR = 10;
            int IR = 50;
            int BOR = 100;
            int ERROR = 0;
            
            if(regionP == ERROR || regionQ == ERROR)
            {
                return 999999;
            }
            
            //1: p,q are equal segment
            if(regionP == FOR && regionQ == FOR
                || regionP == IR && regionQ == IR
                || regionP == BOR && regionQ == BOR) 
            {
                
                float cocP = tex2D(_CoCTex, i.uv).r;
                float cocQ = tex2D(_CoCTex, i.uv + texelShift).r;
                
                if((0.5 * (cocP + cocQ)) > _Delta) 
                {
                    return exp(-(1/(0.5 * (cocP + cocQ))));
                }
                
                return 0;
            }
            
            //2: p element IR, q element FOR or p element FOR, q element IR
            if(regionP == IR && regionQ == FOR 
                || regionP == FOR && regionQ == IR) 
            {
                
                float2 cocs = GetCocs(i, texelShift);
                if(max(cocs[0],cocs[1]) > _Delta) 
                {
                    return exp(-(1/max(cocs[0],cocs[1])));
                }
                
                return 0;
            }
            
            //3: p element IR, q element BOR or p element BOR, q element IR
            if(regionP == IR && regionQ == BOR
                || regionP == BOR && regionQ == IR) 
            {
                
                float cocP = tex2D(_CoCTex, i.uv).r;
                float cocQ = tex2D(_CoCTex, i.uv + texelShift).r;
                
                if(min(cocP,cocQ) > _Delta) 
                {
                    return exp(-(1/min(cocP,cocQ)));
                }
                
                return 0;
            }
            
            //4: p element FOR, q element BOR or p element BOR, q element FOR
            if(regionP == FOR && regionQ == BOR
                || regionP == BOR && regionQ == FOR) 
            {
                
                float2 cocs = GetCocs(i, texelShift);
                
                if(max(cocs[0],cocs[1]) > _Delta) 
                {
                    return exp(-(1/max(cocs[0],cocs[1])));
                }
                
                return 0;
            }
            
            //ERROR
            return 999999;
		}	
    ENDCG
    
    SubShader
    {
         ZWrite Off
         ZTest Always
         Blend Off
         Cull Off
         
        Pass //0 Create Circle of Confusion (CoC)
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram

                half4 FragmentProgram (Interpolators i) : SV_Target {
					half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_LastCameraDepthTexture, i.uv));
					float coc = _ScalingFactor * abs(1 - (_FocalLength / depth));
					
					return  coc;
				}
            ENDCG
        }
        
        Pass //1 Create Regions
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram

                half FragmentProgram (Interpolators i) : SV_Target {
                
                    if(!InsideEllipse(i)){
                        return 0;
                    }
					half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_LastCameraDepthTexture   , i.uv));
					
					//In focus
					if(depth > _MinimumFocalLength && depth < _MaximumFocalLength) {
					    return 50;
					}
					
					//FOR
					if(depth <= _MinimumFocalLength) {
					    return 10;
					}
					
					//BOR
					if(depth >= _MaximumFocalLength) {
					    return 100;
					}
					
					//ERROR
					return 0;
				}
            ENDCG
        }
        
        Pass //2 Calculate Weight Left Right
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram
				
                half FragmentProgram (Interpolators i) : SV_Target {  
                    if(!InsideEllipse(i)){
                        return 0;
                    }                  
					return GetWeight(i, fixed2(_MainTex_TexelSize.x, 0));
				}
            ENDCG
        }
        
        Pass //3 Calculate Weight Top Bot
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram
				
				
                half FragmentProgram (Interpolators i) : SV_Target {
                    if(!InsideEllipse(i)){
                        return 0;
                    }
					return GetWeight(i, fixed2(0, _MainTex_TexelSize.y));
				}
            ENDCG
        }
        
        Pass //4 Filter image => GLSL does not support recursion
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram
				
				fixed GetColor(fixed color, float weight) {
				     return (1 - weight) * color;
				}
				
				fixed4 CreateNewColor(fixed4 currentColor, fixed red, fixed green, fixed blue){
				    currentColor.r = red;
				    currentColor.g = green;
				    currentColor.b = blue;
				    
				    return currentColor;
				}
				
                fixed4  FragmentProgram (Interpolators i) : SV_Target {
                    const static int width = 10;
                    
                    fixed4 currentColor = tex2D(_MainTex, i.uv);
                    fixed4 colors[width];
                    int colorsTotalLength = 0;
                    
                    float2  uvCoordinates = i.uv;
                    
                    int index = 0;
                    
                    if(!InsideInInner(i)){
                    //TODO: InterPolation?
                        return currentColor;
                    }
                    
                    //Start from the back of the recursion
                    fixed red = currentColor.r;
                    fixed blue = currentColor.b;
                    fixed green = currentColor.g;
                   
                    //... Filter right to left ..................                   
                    [unroll(width)] //optimization of the for loop
                    while(index < width || (0 <= uvCoordinates[0] && 0 <= uvCoordinates[1] && uvCoordinates[0] <= 1 && uvCoordinates[1] <= 1)) 
                    {
                        //Get the new colors
                        red = GetColor(tex2D(_MainTex, uvCoordinates).r, tex2D(_WeightLeftRightTex, uvCoordinates).r);
                        blue = GetColor(tex2D(_MainTex, uvCoordinates).b, tex2D(_WeightLeftRightTex, uvCoordinates).r);
                        green = GetColor(tex2D(_MainTex, uvCoordinates).g, tex2D(_WeightLeftRightTex, uvCoordinates).r);
                        
                        colors[index] = CreateNewColor(currentColor,red,green,blue); //Save new colors at the index a
                        uvCoordinates = uvCoordinates - fixed2(_MainTex_TexelSize.x, 0); //shift the uv coordinate to the left
                        
                        colorsTotalLength++; //count how many colors we are calculated
                        index++;
                    }
                    
                    index = colorsTotalLength-1;
                    [unroll(width)] //optimization of the for loop
                    while(index >= 0)
                    {
                        // calulate the colors values for the pixel => in the first round red has the last value of the recursion
                        // than we start to calculate the recursion backwards
                        uvCoordinates = uvCoordinates + fixed2(_MainTex_TexelSize.x, 0); 
                        
                        red = colors[index][0] + tex2D(_WeightLeftRightTex, uvCoordinates) * red;
                        green = colors[index][1] + tex2D(_WeightLeftRightTex, uvCoordinates) * green;  
                        blue = colors[index][2] + tex2D(_WeightLeftRightTex, uvCoordinates) * blue;
                        
                        index--;
                    }
                    currentColor = CreateNewColor(currentColor,red,green,blue);
                    
                    //... Filter left to right ..................
                    uvCoordinates = i.uv; 
                    colorsTotalLength = 0; 
                    
                    index = 0;
                    [unroll(width)]
                    while(index < width || (0 <= uvCoordinates[0] && 0 <= uvCoordinates[1] && uvCoordinates[0] <= 1 && uvCoordinates[1] <= 1))
                    {
                        red = GetColor(tex2D(_MainTex, uvCoordinates).r,tex2D(_WeightLeftRightTex, uvCoordinates).r);
                        blue = GetColor(tex2D(_MainTex, uvCoordinates).b,tex2D(_WeightLeftRightTex, uvCoordinates).r);
                        green = GetColor(tex2D(_MainTex, uvCoordinates).g,tex2D(_WeightLeftRightTex, uvCoordinates).r);
                        
                        colors[index] = CreateNewColor(currentColor,red,green,blue);
                        uvCoordinates = uvCoordinates + fixed2(_MainTex_TexelSize.x, 0);
                        
                        colorsTotalLength++;
                        index++;
                    }
                    
                    index = colorsTotalLength-1;
                    [unroll(width)]
                    while(index >= 0)
                    {
                        uvCoordinates = uvCoordinates - fixed2(_MainTex_TexelSize.x, 0); 
                        
                        red = colors[index][0] + tex2D(_WeightLeftRightTex, uvCoordinates) * red;
                        green = colors[index][1] + tex2D(_WeightLeftRightTex, uvCoordinates) * green; 
                        blue = colors[index][2] + tex2D(_WeightLeftRightTex, uvCoordinates) * blue;
                        
                        index--;
                    }
                    currentColor = CreateNewColor(currentColor,red,green,blue);
                    
                    
                    //... Filter top to bottom ..................
                    uvCoordinates = i.uv;
                    colorsTotalLength = 0;
                    
                    index = 0;
                    [unroll(width)]
                    while(index < width || (0 <= uvCoordinates[0] && 0 <= uvCoordinates[1] && uvCoordinates[0] <= 1 && uvCoordinates[1] <= 1))
                    {
                        red = GetColor(tex2D(_MainTex, uvCoordinates).r, tex2D(_WeightTopBotTex, uvCoordinates).r);
                        blue = GetColor(tex2D(_MainTex, uvCoordinates).b, tex2D(_WeightTopBotTex, uvCoordinates).r);
                        green = GetColor(tex2D(_MainTex, uvCoordinates).g, tex2D(_WeightTopBotTex, uvCoordinates).r);
                        
                        colors[index] = CreateNewColor(currentColor,red,green,blue);
                        uvCoordinates = uvCoordinates - fixed2( 0, _MainTex_TexelSize.y);
                        
                        colorsTotalLength++;
                        index++;
                    }
                            
                    index = colorsTotalLength-1;            
                    [unroll(width)]
                    while(index >= 0)
                    {
                        uvCoordinates = uvCoordinates + fixed2( 0, _MainTex_TexelSize.y); 
                        
                        red = colors[index][0] + tex2D(_WeightTopBotTex, uvCoordinates) * red;
                        green = colors[index][1] + tex2D(_WeightTopBotTex, uvCoordinates) * green; 
                        blue = colors[index][2] + tex2D(_WeightTopBotTex, uvCoordinates) * blue;
                        
                        index--;
                    }
                    currentColor = CreateNewColor(currentColor,red,green,blue);
                    
                    //... Filter bottom to top ..................
                    uvCoordinates = i.uv;
                    colorsTotalLength = 0;
                    
                    index = 0;
                    [unroll(width)]
                    while(index < width || (0 <= uvCoordinates[0] && 0 <= uvCoordinates[1] && uvCoordinates[0] <= 1 && uvCoordinates[1] <= 1))
                    {
                        red = GetColor(tex2D(_MainTex, uvCoordinates).r, tex2D(_WeightTopBotTex, uvCoordinates).r);
                        blue = GetColor(tex2D(_MainTex, uvCoordinates).b, tex2D(_WeightTopBotTex, uvCoordinates).r);
                        green = GetColor(tex2D(_MainTex, uvCoordinates).g, tex2D(_WeightTopBotTex, uvCoordinates).r);
                        
                        colors[index] = CreateNewColor(currentColor,red,green,blue);
                        uvCoordinates = uvCoordinates + fixed2( 0,_MainTex_TexelSize.y);
                        
                        colorsTotalLength++;
                        index++;
                    }
                    
                    index = colorsTotalLength-1;
                    [unroll(width)]
                    while(index >= 0)
                    {
                        uvCoordinates = uvCoordinates - fixed2(0, _MainTex_TexelSize.y); 
                        
                        red = colors[index][0] + tex2D(_WeightTopBotTex, uvCoordinates) * red;
                        green = colors[index][1] + tex2D(_WeightTopBotTex, uvCoordinates) * green;  
                        blue = colors[index][2] + tex2D(_WeightTopBotTex, uvCoordinates) * blue;
                        
                        index--;
                    }
                    currentColor = CreateNewColor(currentColor,red,green,blue);
                    
					return currentColor;
				}
            ENDCG
        }
        
         Pass //5 Debug Region
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram

                half4 FragmentProgram (Interpolators i) : SV_Target {
                    fixed4 currentColor = tex2D(_MainTex, i.uv);
					int regionP = tex2D(_RegionTex, i.uv).r;
					
                    if(regionP == 50){
                        currentColor.r = 1;
                        currentColor.b = 0; 
                        currentColor.g = 0;
                    } 
                    if(regionP == 10){
                        currentColor.r = 0;
                        currentColor.b = 1; 
                        currentColor.g = 0;
                    }
                    
                    if(regionP == 100){
                        currentColor.r = 0;
                        currentColor.b = 0; 
                        currentColor.g = 1;
                    }
                    
                    return currentColor;
				}
            ENDCG
        }
        
        Pass //6 Debug CoC
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram

                half4 FragmentProgram (Interpolators i) : SV_Target {
                    fixed4 currentColor = tex2D(_MainTex, i.uv);
					float cocP = tex2D(_CoCTex, i.uv).r;
					
                    if(cocP <= 0.03){
                        currentColor.r = 0;
                        currentColor.b = 0; 
                        currentColor.g = 0;
                    } else {
                        currentColor.r = 1;
                        currentColor.b = 1; 
                        currentColor.g = 1;
                    }
                    
                    return currentColor;
				}
            ENDCG
        }
        
        Pass //7 Debug pure CoC
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram

                half4 FragmentProgram (Interpolators i) : SV_Target {
					float cocP = tex2D(_CoCTex, i.uv).r;
					
                    return cocP;
				}
            ENDCG
        }
        
        Pass //8 Debug colored CoC
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram

                half4 FragmentProgram (Interpolators i) : SV_Target {
                
                    fixed4 currentColor = tex2D(_MainTex, i.uv);
					float cocP = tex2D(_CoCTex, i.uv).r;
					float schwellwert = 1;
					
					if(cocP == schwellwert){
					    currentColor.r = 0;
					    currentColor.b = 1;
					    currentColor.g = 0;
					}
					
					else if(cocP < schwellwert){
					    currentColor.r = 1;
					    currentColor.b = 0;
					    currentColor.g = 0;
					}
					
					else if(cocP > schwellwert){
					currentColor.r = 0;
					    currentColor.b = 0;
					    currentColor.g = 1;
					} else {
					currentColor.r = 1;
					    currentColor.b = 1;
					    currentColor.g = 1;
					}
                    return currentColor;
				}
            ENDCG
        }
        
        Pass //9 Debug depth
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram

                half4 FragmentProgram (Interpolators i) : SV_Target {
					half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_LastCameraDepthTexture   , i.uv));
					return  depth;
				}
            ENDCG
        }
    }
}
