#version 330 core
out vec4 FragColor;

struct Material {			// 材质属性：为三个光照分量定义一个材质颜色，再添加一个反光度(Shininess)分量
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;    
    float shininess;
}; 

struct Light {		// 光源属性：一个光源对它的ambient、diffuse和specular光照分量有着不同的强度
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

in vec3 Normal;  
in vec3 FragPos;  

uniform vec3 viewPos; 		// 观察者的世界空间坐标(一般为摄像机)
uniform Material material;
uniform Light light;

void main()
{
    // ambient 环境光照
    vec3 ambient = light.ambient * material.ambient;
  	
    // diffuse 漫反射光照
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);		// 法向量和光源进行点乘，最小为0，不能为负数
    vec3 diffuse = light.diffuse * (diff * material.diffuse);
    
    // specular 镜面光照
    vec3 viewDir = normalize(viewPos - FragPos);	// 视线方向向量
    vec3 reflectDir = reflect(-lightDir, norm);  
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * (spec * material.specular);  
    
    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0);
} 