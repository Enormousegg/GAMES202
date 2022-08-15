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

//�����
struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};  
uniform DirLight dirLight;

//���Դ
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

//��������
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
float ShadowCalculation(vec4 fragPosLightSpace);

void main()
{    
    // ����
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    // ��һ�׶Σ��������
    vec3 result = CalcDirLight(dirLight, norm, viewDir);

    //result += CalcPointLight(Light, norm, FragPos, viewDir); 

    //vec3 result = CalcPointLight(Light, norm, FragPos, viewDir); 

    FragColor = vec4(result, 1.0);
}

//ƽ�й�Դ��ɫ����
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    // ��������ɫ
    float diff = max(dot(normal, lightDir), 0.0);
    // �������ɫ
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    // �ϲ����
    vec3 ambient  = light.ambient  * floorcolor;
    vec3 diffuse  = light.diffuse  * diff * floorcolor;
    vec3 specular = light.specular * spec * floorcolor;
    //������Ӱ
    float shadow = ShadowCalculation(PosLightSpace);    

    return (ambient + (1.0f-shadow) * (diffuse + specular));
}

//���Դ��ɫ����
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // ��������ɫ
    float diff = max(dot(normal, lightDir), 0.0);
    // �������ɫ
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0),shininess);
    // ˥��
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + 
                 light.quadratic * (distance * distance));    
    // �ϲ����
    vec3 ambient  = light.ambient  * floorcolor;
    vec3 diffuse  = light.diffuse  * diff * floorcolor;
    vec3 specular = light.specular * spec * floorcolor;
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    //������Ӱ
    float shadow = ShadowCalculation(PosLightSpace);    

    return (ambient + (1.0f-shadow) * (diffuse + specular));
}

float ShadowCalculation(vec4 fragPosLightSpace)
{
    // ִ��͸�ӳ���
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // �任��[0,1]�ķ�Χ
    projCoords = projCoords * 0.5 + 0.5;
    // ȡ�����������(ʹ��[0,1]��Χ�µ�fragPosLight������)
    float closestDepth = texture(material.shadow, projCoords.xy).r; 
    // ȡ�õ�ǰƬ���ڹ�Դ�ӽ��µ����
    float currentDepth = projCoords.z;
    // ��鵱ǰƬ���Ƿ�����Ӱ��
    float shadow = currentDepth > closestDepth  ? 1.0 : 0.0;

    return shadow;
}