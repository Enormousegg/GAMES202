#version 330 core

in vec3 Normal;
in vec3 FragPos;
in vec4 PosLightSpace;

out vec4 FragColor;

uniform vec3 floorcolor;
uniform float shininess;

struct Material {
    sampler2D shadow;
}; 
uniform Material material;

//定向光
struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};  
uniform DirLight dirLight;

//点光源
struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform PointLight Light;
uniform vec3 viewPos;

//函数定义
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
float ShadowCalculation(vec4 fragPosLightSpace);

void main()
{    
    // 属性
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    // 第一阶段：定向光照
    vec3 result = CalcDirLight(dirLight, norm, viewDir);

    //result += CalcPointLight(Light, norm, FragPos, viewDir); 

    //vec3 result = CalcPointLight(Light, norm, FragPos, viewDir); 

    FragColor = vec4(result, 1.0);
}

//平行光源着色计算
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    // 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // 镜面光着色
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    // 合并结果
    vec3 ambient  = light.ambient  * floorcolor;
    vec3 diffuse  = light.diffuse  * diff * floorcolor;
    vec3 specular = light.specular * spec * floorcolor;
    //计算阴影
    float shadow = ShadowCalculation(PosLightSpace);    

    return (ambient + (1.0f-shadow) * (diffuse + specular));
}

//点光源着色计算
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // 镜面光着色
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0),shininess);
    // 衰减
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + 
                 light.quadratic * (distance * distance));    
    // 合并结果
    vec3 ambient  = light.ambient  * floorcolor;
    vec3 diffuse  = light.diffuse  * diff * floorcolor;
    vec3 specular = light.specular * spec * floorcolor;
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    //计算阴影
    float shadow = ShadowCalculation(PosLightSpace);    

    return (ambient + (1.0f-shadow) * (diffuse + specular));
}

float ShadowCalculation(vec4 fragPosLightSpace)
{
    // 执行透视除法
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // 变换到[0,1]的范围
    projCoords = projCoords * 0.5 + 0.5;
    // 取得最近点的深度(使用[0,1]范围下的fragPosLight当坐标)
    float closestDepth = texture(material.shadow, projCoords.xy).r; 
    // 取得当前片段在光源视角下的深度
    float currentDepth = projCoords.z;
    // 检查当前片段是否在阴影中
    float shadow = currentDepth > closestDepth  ? 1.0 : 0.0;

    return shadow;
}