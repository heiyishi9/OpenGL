#version 330 core
out vec4 FragColor;

in vec3 Normal;  
in vec3 FragPos;  
  
uniform vec3 lightPos; 
uniform vec3 viewPos; 		// 观察者的世界空间坐标(一般为摄像机)
uniform vec3 lightColor;
uniform vec3 objectColor;

void main()
{
    // ambient 环境光照
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;
  	
    // diffuse 漫反射光照
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);		// 法向量和光源进行点乘，最小为0，不能为负数
    vec3 diffuse = diff * lightColor;
    
    // specular 镜面光照
    float specularStrength = 0.5;				// 镜面强度，0.5为中等亮度
    vec3 viewDir = normalize(viewPos - FragPos);	// 视线方向向量
    vec3 reflectDir = reflect(-lightDir, norm);  	// 对应的沿着法线轴的反射向量
    // 计算镜面分量
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);	
    vec3 specular = specularStrength * spec * lightColor;  
        
    vec3 result = (ambient + diffuse + specular) * objectColor;
    FragColor = vec4(result, 1.0);
} 