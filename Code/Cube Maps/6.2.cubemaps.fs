#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec3 Position;

uniform vec3 cameraPos;
uniform samplerCube skybox;

void main()
{    
	// 反射的实现
    //vec3 I = normalize(Position - cameraPos); 		// 观察/摄像机方向向量I
    //vec3 R = reflect(I, normalize(Normal));			// 计算反射向量R
    //FragColor = vec4(texture(skybox, R).rgb, 1.0);

	//折射的实现
    float ratio = 1.00 / 1.52;
    vec3 I = normalize(Position - cameraPos);
    vec3 R = refract(I, normalize(Normal), ratio);
    FragColor = vec4(texture(skybox, R).rgb, 1.0);
}