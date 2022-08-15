#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

uniform mat4 lightSpaceMatrix;
uniform mat4 model;

void main()
{
    vec3 Position = aPos;
    gl_Position = lightSpaceMatrix * model * vec4(Position, 1.0f);
}