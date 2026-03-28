/*
 * SPDX-FileCopyrightText: (C) 2025 dffdff2423 <dffdff2423@gmail.com>
 *
 * SPDX-License-Identifier: GPL-3.0-only
 */

#version 450

layout (location = 0) out vec3 o_color;

void main() {
	const vec2 triangle_vert[3] = vec2[3](
		vec2(-0.5, -0.5),
		vec2(0.0, 0.5),
		vec2(0.5, -0.5)
	);

    const vec3 triangle_color[3] = vec3[3](
        vec3(1.0, 0.0, 0.0),
        vec3(0.0, 1.0, 0.0),
        vec3(0.0, 0.0, 1.0)
    );
    o_color = triangle_color[gl_VertexIndex];

	gl_Position = vec4(triangle_vert[gl_VertexIndex], 0.0, 1.0);
}
