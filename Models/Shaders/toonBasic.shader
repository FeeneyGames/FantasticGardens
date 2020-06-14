shader_type spatial;
render_mode ambient_light_disabled;

uniform vec4 base_tint : hint_color = vec4(1.0);
uniform vec4 shade_tint : hint_color = vec4(.5, .5, .5, 1.0);
uniform sampler2D base_texture: hint_albedo;

void light() {
	float NdotL = dot(NORMAL, LIGHT);
	vec3 base = texture(base_texture, UV).rgb * base_tint.rgb;
	vec3 shade = texture(base_texture, UV).rgb * shade_tint.rgb;
	float shade_threshold = 0.;
	float shade_softness = 8./128.;
	//interpolates from 0 to 1 when NdotL would shade with RGB values 120 to 136
	float shade_value = smoothstep(shade_threshold - shade_softness, shade_threshold + shade_softness, NdotL);
	shade_value = max(NdotL, 0);
	vec3 diffuse = mix(shade, base, shade_value);
	DIFFUSE_LIGHT = diffuse;
}