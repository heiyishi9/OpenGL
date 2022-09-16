#version 330 core
out vec4 FragColor;

struct Material {			// 材质属性：定义不透明类型(sampler2D)的漫反射、镜面光照，再添加一个反光度(Shininess)分量
    sampler2D diffuse;
    sampler2D specular;   
    float shininess;
}; 

struct DirLight {		// 定向光属性
    vec3 direction;
    // 每个光照分量有着不同的强度
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight {		// 点光源属性
    vec3 position;
    // 实现衰减的常数项、一次项和二次项
    float constant;
    float linear;
    float quadratic;
    // 每个光照分量有着不同的强度
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct SpotLight {		// 聚光源属性
    vec3 position;		// 光源位置向量
    vec3 direction;		// 光源方向向量
    float cutOff;		// 内切光角
    float outerCutOff;	// 外切光角

// 实现衰减的常数项、一次项和二次项
    float constant;
    float linear;
    float quadratic;

  // 每个光照分量有着不同的强度
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
  
};

#define NR_POINT_LIGHTS 4   

in vec3 Normal;  		// 法向量
in vec3 FragPos;  		// 世界坐标
in vec2 TexCoords;		// 纹理坐标

uniform vec3 viewPos; 		// 观察者的世界空间坐标(一般为摄像机)
uniform DirLight dirLight;                          
uniform PointLight pointLights[NR_POINT_LIGHTS];    // 数组：四个点光源
uniform SpotLight spotLight;                        
uniform Material material;

// function prototypes 函数声明
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
    // 属性
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    // 第一阶段：定向光照
    vec3 result = CalcDirLight(dirLight, norm, viewDir);
    // 第二阶段：点光源
    for(int i = 0; i < NR_POINT_LIGHTS; i++)
        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);    
    // 第三阶段：聚光
    result += CalcSpotLight(spotLight, norm, FragPos, viewDir);    

    FragColor = vec4(result, 1.0);
} 

// calculates the color when using a directional light.
// 定向光的函数
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    // diffuse shading 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading 镜面光着色
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // combine results 合并结果
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    return (ambient + diffuse + specular);
}

// calculates the color when using a point light.
// 点光源函数
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading 镜面光着色
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation 衰减
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    // combine results 合并结果
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

// calculates the color when using a spot light.
// 聚光源函数
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading 镜面光着色
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation 衰减
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    // spotlight intensity  聚光的强调，平滑/软化边缘
    float theta = dot(lightDir, normalize(-light.direction)); 
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    // combine results 合并结果
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    ambient *= attenuation * intensity;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
}



