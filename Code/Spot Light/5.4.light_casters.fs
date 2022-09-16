#version 330 core
out vec4 FragColor;

struct Material {			// 材质属性：定义不透明类型(sampler2D)的漫反射、镜面光照，再添加一个反光度(Shininess)分量
    sampler2D diffuse;
    sampler2D specular;   
    float shininess;
}; 

struct Light {		// 光源属性
    vec3 position;		// 光源位置向量
    vec3 direction;		// 光源方向向量
    float cutOff;		// 内切光角
    float outerCutOff;	// 外切光角
  // 每个光照分量有着不同的强度
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
  // 实现衰减的常数项、一次项和二次项
    float constant;
    float linear;
    float quadratic;
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
    
    // spotlight (soft edges)  聚光计算
    float theta = dot(lightDir, normalize(-light.direction)); 
    float epsilon = (light.cutOff - light.outerCutOff);
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    // 将不对环境光做出影响，让它总是能有一点光
    diffuse  *= intensity;
    specular *= intensity;

    // attenuation 光照衰减
    float distance    = length(light.position - FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    // 衰减值到光照计算中，将它分别乘以环境光、漫反射和镜面光颜色
    ambient  *= attenuation; 
    diffuse   *= attenuation;
    specular *= attenuation; 

    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0);
} 




