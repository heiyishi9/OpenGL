
#include <glad/glad.h>
#include <GLFW/glfw3.h>
// ͨ������STB_IMAGE_IMPLEMENTATION��Ԥ���������޸�ͷ�ļ�������ֻ������صĺ�������Դ�룬�����ǽ����ͷ�ļ���Ϊһ�� .cpp �ļ��ˡ�
#define STB_IMAGE_IMPLEMENTATION
#include "learnopengl/stb_image.h"
#include "learnopengl/shader_s.h"
#include "learnopengl/camera.h"
#include "learnopengl/model.h"

// ����任�����
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include <iostream>

void framebuffer_size_callback(GLFWwindow* window, int width, int height);
void mouse_callback(GLFWwindow* window, double xpos, double ypos);
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset);
void processInput(GLFWwindow* window);

// settings
const unsigned int SCR_WIDTH = 800;
const unsigned int SCR_HEIGHT = 600;

// camera
Camera camera(glm::vec3(0.0f, 0.0f, 3.0f));
float lastX = SCR_WIDTH / 2.0f;
float lastY = SCR_HEIGHT / 2.0f;
bool firstMouse = true;

// timing
float deltaTime = 0.0f;	// ��ǰ֡����һ֡��ʱ���
float lastFrame = 0.0f; // ��һ֡��ʱ��

// lighting
// ��Դ������ռ��λ��
glm::vec3 lightPos(1.2f, 1.0f, 2.0f);

int main()
{
    glfwInit();
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
    //glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);

    // ����һ�����ڶ���
    GLFWwindow* window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, "LearnOpenGL", NULL, NULL);  // ���������ߣ���������
    if (window == NULL)
    {
        std::cout << "Failed to create GLFW window" << std::endl;
        glfwTerminate();
        return -1;
    }
    glfwMakeContextCurrent(window);
    // ע�����framebuffer_size_callback()����������GLFW����ϣ��ÿ�����ڵ�����С��ʱ������������
    glfwSetFramebufferSizeCallback(window, framebuffer_size_callback);
    // ע������ƶ��������ֵĻص�����
    glfwSetCursorPosCallback(window, mouse_callback);
    glfwSetScrollCallback(window, scroll_callback);

    // ��ʼ��GLAD
    if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
    {
        std::cout << "Failed to initialize GLAD" << std::endl;
        return -1;
    }

    // tell stb_image.h to flip loaded texture's on the y-axis (before loading model).
    // ����stb_image.h��y���Ϸ�ת���ص�����(�ڼ���ģ��֮ǰ)��
    stbi_set_flip_vertically_on_load(true);

    // ����ȫ��opengl״̬
    // ������Ȳ��ԣ�Z����: ������ʱ����һ�����ض���ʱ�����ǣ�
    glEnable(GL_DEPTH_TEST);

    // �������������ǵ���ɫ������
    Shader ourShader("1.model_loading.vs", "1.model_loading.fs"); 

    // load models 
    // ����ģ��
    Model ourModel("model/nanosuit/nanosuit.obj");
   

    // ѭ����Ⱦ
    // ÿ��ѭ���Ŀ�ʼǰ���һ��GLFW�Ƿ�Ҫ���˳�������ǵĻ��ú�������trueȻ����Ⱦѭ���������
    while (!glfwWindowShouldClose(window))
    {
        // ��ǰ֡����ÿһ֡�����Ǽ�����µ� deltaTime �Ա�����
        float currentFrame = static_cast<float>(glfwGetTime());
        deltaTime = currentFrame - lastFrame;
        lastFrame = currentFrame;

        // ��������
        processInput(window);

        // ��Ⱦָ��
        // ������ɫ���嶼�ᱻ���ΪglClearColor�������õ���ɫ
        glClearColor(0.05f, 0.05f, 0.05f, 1.0f);
        // �����Ļ���õ���ɫ����Ȼ�����
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        // ������ɫ��
        ourShader.use();

        // �۲졢ͶӰ����
        glm::mat4 projection = glm::perspective(glm::radians(camera.Zoom), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
        glm::mat4 view = camera.GetViewMatrix();
        ourShader.setMat4("projection", projection);
        ourShader.setMat4("view", view);
        // ģ�;���
        glm::mat4 model = glm::mat4(1.0f);
        model = glm::translate(model, glm::vec3(0.0f, 0.0f, 0.0f)); // �����ƶ����������ڳ���������
        model = glm::scale(model, glm::vec3(0.2f, 0.2f, 0.2f));	    // �����ǵĳ�����˵�е�̫���ˣ�������Сһ��
        ourShader.setMat4("model", model);
        ourModel.Draw(ourShader);


        // �����ύ����ɫ���壨����һ��������GLFW����ÿһ��������ɫֵ�Ĵ󻺳壩��������һ�����б��������ƣ����ҽ�����Ϊ�����ʾ����Ļ��
        glfwSwapBuffers(window);
        // ���������û�д���ʲô�¼�������������롢����ƶ��ȣ������´���״̬�������ö�Ӧ�Ļص�����������ͨ���ص������ֶ����ã�
        glfwPollEvents();
    }

    // �ͷ�/ɾ��֮ǰ�ķ����������Դ
    glfwTerminate();
    return 0;
}

// ������������Ƿ����ڱ�����
void processInput(GLFWwindow* window)
{
    // ����û��Ƿ����˷��ؼ�(Esc)������ر�GLFW
    if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
        glfwSetWindowShouldClose(window, true);
    // wasd�ƶ������
    if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
        camera.ProcessKeyboard(FORWARD, deltaTime);
    if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
        camera.ProcessKeyboard(BACKWARD, deltaTime);
    if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
        camera.ProcessKeyboard(LEFT, deltaTime);
    if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
        camera.ProcessKeyboard(RIGHT, deltaTime);
}

// ÿ�δ��ڴ�С��������ʱ�򱻵���
void framebuffer_size_callback(GLFWwindow* window, int width, int height)
{
    // ��Ⱦ���ڵĳߴ��С
    // ǰ�����������ƴ������½ǵ�λ�á��������͵��ĸ�����������Ⱦ���ڵĿ�Ⱥ͸߶ȣ����أ���
    glViewport(0, 0, width, height);
}

// GLFW��������ƶ��¼�
// -------------------------------------------------------
void mouse_callback(GLFWwindow* window, double xposIn, double yposIn)
{
    // ������һ֡�����λ�ã���ʼ��
    float xpos = static_cast<float>(xposIn);
    float ypos = static_cast<float>(yposIn);

    // ��һ�λ�ȡ�����ʱ���������ͻȻ��һ�£�Ϊ�����������������Ĵ���
    // �����Ƿ��һ�λ�ȡ�������
    if (firstMouse)
    {
        lastX = xpos;
        lastY = ypos;
        firstMouse = false;
    }

    // ���㵱ǰ֡����һ֡���λ�õ�ƫ����
    float xoffset = xpos - lastX;
    float yoffset = lastY - ypos; // ע���������෴�ģ���Ϊy�����Ǵӵײ����������������

    lastX = xpos;
    lastY = ypos;

    camera.ProcessMouseMovement(xoffset, yoffset);
}

// glfw: whenever the mouse scroll wheel scrolls, this callback is called
// �����������¼�
// ----------------------------------------------------------------------
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset)
{
    camera.ProcessMouseScroll(static_cast<float>(yoffset));
}




