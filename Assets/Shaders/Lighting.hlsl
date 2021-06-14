#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void CalculateMainLight_float(float3 WorldPos, out float3 Direction, out float3 Color,
	out half DistanceAtten, out half ShadowAtten) {
#ifdef SHADERGRAPH_PREVIEW //if the shader runs in the preview window set these default values
	Direction = float3(0.5, 0.5, 0);
	Color = 1;
	DistanceAtten = 1;
	ShadowAtten = 1;
#else //else get the main light
	#if SHADOWS_SCREEN
		half4 clipPos = TransformWorldToHClip(WorldPos);
		half4 shadowCoord = ComputeScreenPos(clipPos);
	#else
		half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
	#endif
	Light mainLight = GetMainLight(0);
	Direction = mainLight.direction;
	Color = mainLight.color;
	DistanceAtten = mainLight.distanceAttenuation;
	ShadowAtten = mainLight.shadowAttenuation;

#if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
	ShadowAtten = 1.0h;
#endif

#if SHADOWS_SCREEN
	ShadowAtten = SampleScreenSpaceShadowmap(shadowCoord);
#else
	ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
	half shadowStrength = GetMainLightShadowStrength();
	ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture,
		sampler_MainLightShadowmapTexture),
		shadowSamplingData, shadowStrength, false);
#endif

#endif
}

float GetLightIntensity(float3 color) {
	return max(color.r, max(color.g, color.b));
}

void AddAdditionalLights_float(float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView,
	float MainDiffuse, float MainSpecular, float3 MainColor,
	out float Diffuse, out float Specular, out float3 Color) {

	//float mainIntensity = GetLightIntensity(MainColor);
	Diffuse = MainDiffuse; //* mainIntensity;
	Specular = MainSpecular; //* mainIntensity;
	Color = MainColor * (MainDiffuse + MainSpecular);

#ifndef SHADERGRAPH_PREVIEW
	//float highestDiffuse = Diffuse;

	int pixelLightCount = GetAdditionalLightsCount();
	for (int i = 0; i < pixelLightCount; ++i) {
		Light light = GetAdditionalLight(i, WorldPosition);
		half NdotL = saturate(dot(WorldNormal, light.direction));
		half atten = light.distanceAttenuation * light.shadowAttenuation; //* GetLightIntensity(light.color);
		half thisDiffuse = atten * NdotL;
		half thisSpecular = LightingSpecular(thisDiffuse, light.direction, WorldNormal, WorldView, 1, Smoothness);
		Diffuse += thisDiffuse;
		Specular += thisSpecular;
		Color += light.color * (thisDiffuse + thisSpecular); //comment out for hard lighting
		/*
		if (thisDiffuse > highestDiffuse) {
			highestDiffuse = thisDiffuse;
			Color = light.color;
		}*/
	}
#endif

	half total = Diffuse + Specular;
	Color = total <= 0 ? MainColor : Color / total;
}

#endif