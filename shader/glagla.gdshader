shader_type canvas_item;

uniform sampler2D _MainTex;

void fragment() {
    // Affine UV mapping
    vec2 uv = UV;
    vec4 col = texture(_MainTex, uv);
    COLOR = col;
}

void vertex() {
    // Transform the vertex to clip space
    VERTEX = (PROJECTION_MATRIX * WORLD_MATRIX * vec4(VERTEX, 1.0)).xyz;
}
