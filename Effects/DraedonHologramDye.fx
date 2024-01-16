sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;
float2 uTargetPosition;
//这一部分定义了一些全局变量，包括纹理采样器（sampler）、颜色、透明度、旋转角度、时间等。这些变量在后续的着色器代码中被使用。

float4 DraedonHologramDye(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //主要着色器函数 DraedonHologramDye：
    //这是一个主要的着色器函数。它接收两个参数，sampleColor 表示从纹理中采样的颜色，coords 是当前像素的纹理坐标。
    //着色器主要通过对坐标的操作和一些数学函数来实现特定的图形效果。
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    float mySin3 = sin(uTime * 1.8);
    float mySin2 = clamp(sin(uTime * 3), 0, 0.25);
    float mySin2_1 = clamp(sin(uTime * 3), 0, 0.05);
    float mySin1 = sin(uTime * 2);
    float mySin4 = sin(uTime) * frameY;
    
    if (frameY > 0.10 + mySin4 && frameY < 0.20 + mySin4)
    {
        if (mySin3 > 0.25 && mySin3 < 0.45)
        {//坐标操作：这些操作在纹理坐标上进行，影响着色器在纹理上采样的位置。
            coords.x = coords.x + 0.2;
        }
    }
    else if (frameY > 0.40 + mySin4 && frameY < 0.60 + mySin4)
    {
        if (mySin3 > 0.25 && mySin3 < 0.45)
        {
            coords.x = coords.x - 0.3;
        }
        
        if (frameY > 0.50 + mySin4 && frameY < 0.55 + mySin4)
        {
            if (mySin3 > 0.25 && mySin3 < 0.45)
            {
                coords.x = coords.x + 0.6;
            }
        }
    }
    float4 color = tex2D(uImage0, coords);
    float luminosity = (color.r + color.g + color.b) / 3;
    //色彩调整：这里根据一些数学计算来调整采样到的颜色，实现一些特定的效果。
    color.r = (0.121568627 + mySin2_1) * luminosity;
    color.g = (0.734313725 + mySin2) * luminosity;
    color.b = (0.75 + mySin2) * luminosity;
    
    if (frameY > 0.1 + mySin1 && frameY < 0.15 + mySin1)
    {//条件判断：这是一个条件语句，根据 frameY 的值来判断是否应用某种颜色调整。
        color.rgb = 2 * luminosity;
    }
    
    if (frameY > 0.2 + mySin1 && frameY < 0.30 + mySin1)
    {
        color.rgb = 2 * luminosity;
    }
    
    if (frameY > 0.37 + mySin1 && frameY < 0.42 + mySin1)
    {
        color.rgb = 2 * luminosity;
    }
    return color * sampleColor;
}

technique Technique1
{//Technique 和 Pass：这部分定义了一种技术（technique）和一个渲染通道（pass）。在这个例子中，只有一个渲染通道，使用了前面定义的着色器函数 DraedonHologramDye。
    pass DraedonHologramDyePass
    {
        PixelShader = compile ps_2_0 DraedonHologramDye();
    }
}
//总体而言，这段 HLSL 着色器代码的作用是在纹理渲染过程中通过一些数学和条件逻辑来实现一些动态的颜色和坐标调整，以达到一种特定的视觉效果。