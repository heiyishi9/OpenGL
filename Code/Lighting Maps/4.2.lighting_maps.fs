#version 330 core
out vec4 FragColor;

struct Material {			// 材质属性：定义不透明类型(sampler2D)的漫反射、镜面光照，再添加一个反光度(Shininess)分量
    sampler2D diffuse;
    sampler2D specular;   
    float shininess;
}; 

struct Light {		// 光源属性：一个光源对它的ambient、diffuse和specular光照分量有着不同的强度
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

in vec3 Normal;  		// 法向量
in vec3 FragPos;  		// 世界坐标
in vec2 TexCoords;		// 纹理坐标

uniform vec3 viewPos; 		// 观察者的世界空间坐标(一般为摄像机)
uniform Material material;
uniform Light light;

void main()
{
    // ambient 环境光照
    vec3 ambient = light.ambient * texture(material.diffuse, TexCoords).rgb;
  	
    // diffuse 漫反射光照
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);		// 法向量和光源进行点乘，最小为0，不能为负数
    vec3 diffuse = light.diffuse * diff * texture(material.diffuse, TexCoords).rgb;  
    
    // specular 镜面光照
    vec3 viewDir = normalize(viewPos - FragPos);	// 视线方向向量
    vec3 reflectDir = reflect(-lightDir, norm);  
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * texture(material.specular, TexCoords).rgb;  
    
    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0);
} 