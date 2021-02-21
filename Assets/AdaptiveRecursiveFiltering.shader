Shader "Hidden/AdaptiveRecursiveFiltering"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    CGINCLUDE
        #include "UnityCG.cginc"
        
        sampler _MainTex,  _LastCameraDepthTexture   , _CoCTex, _RegionTex, _WeightLeftRightTex, _WeightTopBotTex;
        float4 _MainTex_TexelSize;
        float _FocalLength, _ScalingFactor, _MinimumFocalLength, _MaximumFocalLength, _CocTreshhold, _Delta = pow(10,-5);
        
        
        struct VertexData {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };
        
        struct Interpolators {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};
		
		Interpolators VertexProgram (VertexData v) {
			Interpolators i;
			i.pos = UnityObjectToClipPos(v.vertex);
			i.uv = v.uv;
			return i;
		}
		
		float2 GetCocs(Interpolators i, fixed2 whatever) {
            //Bilineare Interpolation
            float4 texel = _MainTex_TexelSize.xyxy * float2(-0.5, 0.5).xxyy;
            float coc0 = tex2D(_CoCTex, i.uv + texel.xy).r;
            float coc1 = tex2D(_CoCTex, i.uv + texel.zy).r;
            float coc2 = tex2D(_CoCTex, i.uv + texel.xw).r;
            float coc3 = tex2D(_CoCTex, i.uv + texel.zw).r;
            
            float cocP = (coc0 + coc1 + coc2 + coc3) * 0.25;
            
            coc0 = tex2D(_CoCTex, i.uv + texel.xy + whatever).r;
            coc1 = tex2D(_CoCTex, i.uv + texel.zy + whatever).r;
            coc2 = tex2D(_CoCTex, i.uv + texel.xw + whatever).r;
            coc3 = tex2D(_CoCTex, i.uv + texel.zw + whatever).r;
            
            float cocQ = (coc0 + coc1 + coc2 + coc3) * 0.25;
            
            return float2(cocP, cocQ);
	    }
		
		half getWeight(Interpolators i, fixed2 whatever) {
		    int regionP = tex2D(_RegionTex, i.uv).r;
			int regionQ = tex2D(_RegionTex, i.uv + whatever).r;
			
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
                float cocQ = tex2D(_CoCTex, i.uv + whatever).r;
                
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
                
                float2 cocs = GetCocs(i, whatever);
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
                float cocQ = tex2D(_CoCTex, i.uv + whatever).r;
                
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
                
                float2 cocs = GetCocs(i, whatever);
                
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
					return getWeight(i, fixed2(_MainTex_TexelSize.x, 0));
				}
            ENDCG
        }
        
        Pass //3 Calculate Weight Top Bot
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram
				
				
                half FragmentProgram (Interpolators i) : SV_Target {
					return getWeight(i, fixed2(0, _MainTex_TexelSize.y));
				}
            ENDCG
        }
        
        Pass //4 Filter image => GLSL does not support recursion
        { 
            CGPROGRAM
                #pragma vertex VertexProgram
				#pragma fragment FragmentProgram
				
				fixed GetRed(float2 i, fixed color, sampler weightTexture) {
				     return (1-tex2D(weightTexture, i).r) * color;
				}
				
				fixed GetBlue(float2 i, fixed color, sampler weightTexture) {
				     return (1-tex2D(weightTexture, i).r) * color;
				}
				
				fixed GetGreen(float2 i, fixed color, sampler weightTexture) {
				     return (1-tex2D(weightTexture, i).r) * color;
				}
				
				fixed4 CreateNewColor(fixed4 currentColor, fixed red, fixed blue, fixed green){
				    currentColor.r = red;
				    currentColor.g = green;
				    currentColor.b = blue;
				    
				    return currentColor;
				}

                fixed4  FragmentProgram (Interpolators i) : SV_Target {
                    const static int width = 20;
                    
                    fixed4 currentColor = tex2D(_MainTex, i.uv);
                    
                    fixed4 colors[width];
                    
                    float2  uvCoordinates = i.uv;
                    
                    int colorsTotalLength = 0;
                    
                    int index = 0;
                    
                    //Start from the back of the recursion
                    fixed red = currentColor.r;
                    fixed blue = currentColor.b;
                    fixed green = currentColor.g;
                   
                   //Filter right to left ..................
                    [unroll(width)] //optimization of the for loop
                    for(index; 
                        index < width || (uvCoordinates[0] < 0 || uvCoordinates[1] < 0
                            || uvCoordinates[0] > 1 || uvCoordinates[1] > 1); 
                        index++) 
                    {                       
                        //Get the new colors
                        red = GetRed(uvCoordinates, tex2D(_MainTex, uvCoordinates).r, _WeightLeftRightTex);
                        blue = GetBlue(uvCoordinates, tex2D(_MainTex, uvCoordinates).b, _WeightLeftRightTex);
                        green = GetGreen(uvCoordinates, tex2D(_MainTex, uvCoordinates).g, _WeightLeftRightTex);
                        
                        colors[index] = fixed4(red,blue,green,tex2D(_MainTex, uvCoordinates).a); //Save new colors at the index a
                        uvCoordinates = uvCoordinates - fixed2(_MainTex_TexelSize.x, 0); //shift the uv coordinate to the left
                        colorsTotalLength++; //count how many colors we are calculated
                    }
                    uvCoordinates = uvCoordinates + fixed2(_MainTex_TexelSize.x, 0); //shift one back because we shifted one to much
                    colorsTotalLength--; //minus 1 because we add one to many
                    
                    [unroll(width)] //optimization of the for loop
                    for(index = colorsTotalLength-1; index >= 0; index--) 
                    {
                        // calulate the colors values for the pixel => in the first round red has the last value of the recursion
                        // than we start to calculate the recursion backwards
                        uvCoordinates = uvCoordinates + fixed2(_MainTex_TexelSize.x, 0); 
                        red = colors[index][0] + tex2D(_WeightLeftRightTex, uvCoordinates) * red;
                        blue = colors[index][1] + tex2D(_WeightLeftRightTex, uvCoordinates) * blue;
                        green = colors[index][2] + tex2D(_WeightLeftRightTex, uvCoordinates) * green;  
                    }
                    
                    currentColor = CreateNewColor(currentColor,red,blue,green);
                    
                    //Filter left to right ..................
                    uvCoordinates = i.uv; //reset uv coordinate
                    colorsTotalLength = 0; //reset the currentColor total length to 0
                    
                    //if we did not reach the borders, calculate the next red/blue/green
                    //Out of the loop, because we just need to check once here
                    if(uvCoordinates[0] > 0 || uvCoordinates[1] > 0
                        || uvCoordinates[0] < 1 || uvCoordinates[1] < 1) 
                    {  
                        red = GetRed(uvCoordinates, currentColor.r,_WeightLeftRightTex);    
                        blue = GetBlue(uvCoordinates, currentColor.b,_WeightLeftRightTex);
                        green = GetGreen(uvCoordinates, currentColor.g,_WeightLeftRightTex);
                        colors[0] = fixed4(red,blue,green,currentColor.a);
                        uvCoordinates = uvCoordinates + fixed2(_MainTex_TexelSize.x, 0);
                        colorsTotalLength++;
                    }
                    
                        
                    [unroll(width)]
                    for(index = 1; 
                        index < width || (uvCoordinates[0] < 0 || uvCoordinates[1] < 0 
                            || uvCoordinates[0] > 1 || uvCoordinates[1] > 1); 
                        index++) 
                    {
                        red = GetRed(uvCoordinates, tex2D(_MainTex, uvCoordinates).r,_WeightLeftRightTex);
                        blue = GetBlue(uvCoordinates, tex2D(_MainTex, uvCoordinates).b,_WeightLeftRightTex);
                        green = GetGreen(uvCoordinates, tex2D(_MainTex, uvCoordinates).g,_WeightLeftRightTex);
                        colors[index] = fixed4(red,blue,green,tex2D(_MainTex, uvCoordinates).a);
                        uvCoordinates = uvCoordinates + fixed2(_MainTex_TexelSize.x, 0);
                        colorsTotalLength++;
                    }
                    uvCoordinates = uvCoordinates - fixed2(_MainTex_TexelSize.x, 0);
                    colorsTotalLength--;
                    
                    [unroll(width)]
                    for(index = colorsTotalLength-1; index >= 0; index--) 
                    {
                        uvCoordinates = uvCoordinates - fixed2(_MainTex_TexelSize.x, 0); 
                        red = colors[index][0] + tex2D(_WeightLeftRightTex, uvCoordinates) * red;
                        blue = colors[index][1] + tex2D(_WeightLeftRightTex, uvCoordinates) * blue;
                        green = colors[index][2] + tex2D(_WeightLeftRightTex, uvCoordinates) * green;  
                    }
                    
                    currentColor = CreateNewColor(currentColor,red,blue,green);
                    
                    
                    //Filter top to bottom ..................
                    uvCoordinates = i.uv;
                    colorsTotalLength = 0;
                    
                    if(uvCoordinates[0] > 0 || uvCoordinates[1] > 0
                        || uvCoordinates[0] < 1 || uvCoordinates[1] < 1) 
                    {
                        red = GetRed(uvCoordinates, currentColor.r,_WeightTopBotTex);
                        blue = GetBlue(uvCoordinates, currentColor.b,_WeightTopBotTex);
                        green = GetGreen(uvCoordinates, currentColor.g,_WeightTopBotTex);
                        colors[0] = fixed4(red,blue,green,currentColor.a);
                        uvCoordinates = uvCoordinates - fixed2( 0, _MainTex_TexelSize.y);
                        colorsTotalLength++;
                    }
                    
                    [unroll(width)] // While do? Oder just while?
                    for(index = 1; 
                        index < width || (uvCoordinates[0] < 0 || uvCoordinates[1] < 0
                            || uvCoordinates[0] > 1 || uvCoordinates[1] > 1); 
                        index++) 
                    {
                        red = GetRed(uvCoordinates, tex2D(_MainTex, uvCoordinates).r, _WeightTopBotTex);
                        blue = GetBlue(uvCoordinates, tex2D(_MainTex, uvCoordinates).b,_WeightTopBotTex);
                        green = GetGreen(uvCoordinates, tex2D(_MainTex, uvCoordinates).g,_WeightTopBotTex);
                        colors[index] = fixed4(red,blue,green,tex2D(_MainTex, uvCoordinates).a);
                        uvCoordinates = uvCoordinates - fixed2( 0, _MainTex_TexelSize.y);
                        colorsTotalLength++;
                    }
                    uvCoordinates = uvCoordinates + fixed2( 0,_MainTex_TexelSize.y);
                    colorsTotalLength--;
                    
                    [unroll(width)]
                    for(index = colorsTotalLength-1; index >= 0; index--)
                    {
                        uvCoordinates = uvCoordinates + fixed2( 0, _MainTex_TexelSize.y); 
                        red = colors[index][0] + tex2D(_WeightTopBotTex, uvCoordinates) * red;
                        blue = colors[index][1] + tex2D(_WeightTopBotTex, uvCoordinates) * blue;
                        green = colors[index][2] + tex2D(_WeightTopBotTex, uvCoordinates) * green;  
                    }
                    
                    currentColor = CreateNewColor(currentColor,red,blue,green);
                    
                    //Filter bottom to top ..................
                    uvCoordinates = i.uv;
                    colorsTotalLength = 0;
                    
                    if(uvCoordinates[0] > 0 || uvCoordinates[1] > 0
                            || uvCoordinates[0] < 1 || uvCoordinates[1] < 1) 
                    {
                        red = GetRed(uvCoordinates, currentColor.r,_WeightTopBotTex);
                        blue = GetBlue(uvCoordinates, currentColor.b,_WeightTopBotTex);
                        green = GetGreen(uvCoordinates, currentColor.g,_WeightTopBotTex);
                        colors[0] = fixed4(red,blue,green,currentColor.a);
                        uvCoordinates = uvCoordinates + fixed2( 0, _MainTex_TexelSize.y);
                        colorsTotalLength++;
                    }
                    
                    [unroll(width)]
                    for(index = 1; 
                        index < width || (uvCoordinates[0] < 0 || uvCoordinates[1] < 0
                            || uvCoordinates[0] > 1 || uvCoordinates[1] > 1); 
                        index++) 
                    {
                        red = GetRed(uvCoordinates, tex2D(_MainTex, uvCoordinates).r,_WeightTopBotTex);
                        blue = GetBlue(uvCoordinates, tex2D(_MainTex, uvCoordinates).b,_WeightTopBotTex);
                        green = GetGreen(uvCoordinates, tex2D(_MainTex, uvCoordinates).g,_WeightTopBotTex);
                        colors[index] = fixed4(red,blue,green,tex2D(_MainTex, uvCoordinates).a);
                        uvCoordinates = uvCoordinates + fixed2( 0,_MainTex_TexelSize.y);
                        colorsTotalLength++;
                    }
                    uvCoordinates = uvCoordinates - fixed2(0, _MainTex_TexelSize.y);
                    colorsTotalLength--;
                    
                    [unroll(width)]
                    for(index = colorsTotalLength-1; index >= 0; index--) 
                    {
                        uvCoordinates = uvCoordinates - fixed2(0, _MainTex_TexelSize.y); 
                        red = colors[index][0] + tex2D(_WeightTopBotTex, uvCoordinates) * red;
                        blue = colors[index][1] + tex2D(_WeightTopBotTex, uvCoordinates) * blue;
                        green = colors[index][2] + tex2D(_WeightTopBotTex, uvCoordinates) * green;  
                    }
                    
                     currentColor = CreateNewColor(currentColor,red,blue,green);
                    
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
