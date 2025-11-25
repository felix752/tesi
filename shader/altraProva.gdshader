shader_type canvas_item;

uniform float crt_curve : hint_range(0.0, 1.0) = 0.02;
uniform float aperture_grille_rate : hint_range(0.0, 1.0) = 0.4;
uniform float rf_switch_esque_blur : hint_range(0.0, 1.0) = 1.0;
uniform float white_noise_rate : hint_range(0.0, 1.0) = 0.1;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

float random(vec2 pos) {
	return fract(sin(dot(pos, vec2(12.9898,78.233))) * 43758.5453);
}

void fragment() {
	// Distorsione CRT (curvatura)
	vec2 crt_shift = (vec2(1.0) - sin(UV.yx * 3.1415926535)) * crt_curve;
	vec2 scale = vec2(1.0) + crt_shift * 2.0;
	vec2 distorted_uv = SCREEN_UV * scale - crt_shift;

	// Verifica se è dentro lo schermo distorto (evita pixel neri esterni)
	bool inside_bounds = all(greaterThanEqual(distorted_uv, vec2(0.0))) && all(lessThanEqual(distorted_uv, vec2(1.0)));

	vec3 base_color = texture(SCREEN_TEXTURE, distorted_uv).rgb;

	// RF blur su tutto lo schermo: calcolato rispetto a SCREEN_UV normale (non distorto)
	vec2 blur_offset = vec2(SCREEN_PIXEL_SIZE.x * 3.1, 0.0);
	vec3 blur_color =
		texture(SCREEN_TEXTURE, SCREEN_UV - blur_offset).rgb +
		texture(SCREEN_TEXTURE, SCREEN_UV + blur_offset).rgb;

	base_color = mix(base_color, blur_color * 0.5, rf_switch_esque_blur);

	// White noise
	vec2 pixel_pos = floor(SCREEN_UV / SCREEN_PIXEL_SIZE);
	float noise = random(pixel_pos + vec2(sin(TIME * 0.543254), cos(TIME * 0.254323563)));
	base_color = mix(base_color, vec3(noise), white_noise_rate);


	// Aperture grille (RGB stripe)
	float ag_point = mod(distorted_uv.x / SCREEN_PIXEL_SIZE.x, 3.0);
	float r = clamp(1.0 - ag_point, 0.0, 1.0) + clamp(ag_point - 2.0, 0.0, 1.0);
	float g = clamp(1.0 - abs(1.0 - ag_point), 0.0, 1.0);
	float b = 1.0 - r - g;

	base_color *= normalize(vec3(
		clamp(r, aperture_grille_rate, 1.0),
		clamp(g, aperture_grille_rate, 1.0),
		clamp(b, aperture_grille_rate, 1.0)
	));

	// Scanlines
	//float scanline_factor = float(int(distorted_uv.y / SCREEN_PIXEL_SIZE.y) % 2 == 0);
	//base_color = mix(base_color, vec3(0.0), scanline_factor * crt_scan_line_color);

	COLOR.rgb = base_color * float(inside_bounds);
	COLOR.a = 1.0;
}
