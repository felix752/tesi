shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;

void fragment() {
    vec2 uv = SCREEN_UV;
    uv.y += sin(uv.x * 10.0) * 0.02; // distorsione
    vec4 color = texture(SCREEN_TEXTURE, uv);
    COLOR = vec4(color.rgb * vec3(1.2, 0.9, 0.9), color.a); // tinta
}
