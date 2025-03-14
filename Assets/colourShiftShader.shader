Shader "Custom/colourShiftShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Float) = 0.8
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        float _Speed;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos : WORLD_POS;
        };

        half RGBToWavelength(half3 rgb)
        {
            // Approximate conversion from RGB to wavelength (in nm)
            half wavelength = 0.0;
            if (rgb.r > rgb.g && rgb.r > rgb.b)
                wavelength = 620.0 + (rgb.r - rgb.g) * 60.0; // Red to Yellow
            else if (rgb.g > rgb.r && rgb.g > rgb.b)
                wavelength = 520.0 + (rgb.g - rgb.b) * 100.0; // Green to Cyan
            else
                wavelength = 440.0 + (rgb.b - rgb.r) * 80.0; // Blue to Violet
            return wavelength;
        }

        half3 WavelengthToRGB(half wavelength)
        {
            // Approximate conversion from wavelength (in nm) to RGB
            half3 rgb = half3(0.0, 0.0, 0.0);
            if (wavelength >= 620.0 && wavelength <= 700.0)
                rgb = half3(1.0, (700.0 - wavelength) / 80.0, 0.0); // Red to Yellow
            else if (wavelength >= 520.0 && wavelength < 620.0)
                rgb = half3((620.0 - wavelength) / 100.0, 1.0, 0.0); // Yellow to Green
            else if (wavelength >= 440.0 && wavelength < 520.0)
                rgb = half3(0.0, (wavelength - 440.0) / 80.0, 1.0); // Green to Blue
            else if (wavelength >= 380.0 && wavelength < 440.0)
                rgb = half3((440.0 - wavelength) / 60.0, 0.0, 1.0); // Blue to Violet
            return rgb;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);

            // Convert RGB to wavelength
            half wavelength = RGBToWavelength(c.rgb);

            // Calculate the angle theta between -x axis and the sample point
            half3 direction = normalize(IN.worldPos);
            half theta = acos(-direction.x);

            // Calculate beta and gamma
            half beta = _Speed;
            half gamma = 1.0 / sqrt(1.0 - beta * beta);

            // Apply relativistic Doppler effect
            half shiftedWavelength = wavelength * (1.0 - beta * cos(theta)) * gamma;

            // Convert shifted wavelength back to RGB
            half3 shiftedRGB = WavelengthToRGB(shiftedWavelength);

            o.Albedo = shiftedRGB;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}