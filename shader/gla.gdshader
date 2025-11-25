shader_type spatial;

render_mode unshaded, cull_disabled, depth_prepass_alpha;

uniform float affineRadius = 4.0;
uniform float affineIntensity = 400;

uniform sampler2D image : source_color, repeat_enable;

uniform vec2 uv_scale = vec2(1.0,1.0);
varying float vertex_force;

void vertex() {
	vec3 vertex_world_pos = (MODEL_MATRIX*vec4(VERTEX,1.0f)).xyz;
	float true_dist = distance(CAMERA_POSITION_WORLD, vertex_world_pos);

	if (true_dist <= 0.001){
		vertex_force = 1.0/affineIntensity*length(UV)+1.0;

	}
	else {
		vertex_force = affineRadius/true_dist/affineIntensity*length(UV)+1.0;
	}

}

void fragment() {
	vec2 new_uv = UV;
	if (uv_scale[0] > 0.0f || uv_scale[1] > 0.0f){

		new_uv[0] *= uv_scale[0];
		new_uv[1] *= uv_scale[1];

	}

	vec4 new_image;
	new_image = texture( image, new_uv*vertex_force);

	ALBEDO = new_image.rgb;
	ALPHA = new_image.a;


}
