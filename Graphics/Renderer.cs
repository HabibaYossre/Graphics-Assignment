using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Tao.OpenGl;
using GlmNet;

namespace Graphics
{
    class Renderer
    {
        Shader sh;

        uint cubeBufferID;
        uint pyramidBufferID;
        uint xyzAxesBufferID;

        // 3D Drawing
        mat4 ModelMatrix;
        mat4 ViewMatrix;
        mat4 ProjectionMatrix;

        int ShaderModelMatrixID;
        int ShaderViewMatrixID;
        int ShaderProjectionMatrixID;

        const float rotationSpeed = 1f;
        float rotationAngle = 0;

        public float translationX = 0,
                     translationY = 0,
                     translationZ = 0;

        Stopwatch timer = Stopwatch.StartNew();

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            Gl.glClearColor(0, 0, 0.4f, 1);

            // Define cube vertices (blue)
            float[] cubeVertices = {
                // Front face
                -1.0f, -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f, -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f,  1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                -1.0f,  1.0f,  1.0f, 0.0f, 0.0f, 1.0f,

                // Back face
                -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f,  1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
                -1.0f,  1.0f, -1.0f, 0.0f, 0.0f, 1.0f,

                // Left face
                -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
                -1.0f, -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                -1.0f,  1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                -1.0f,  1.0f, -1.0f, 0.0f, 0.0f, 1.0f,

                // Right face
                 1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f, -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f,  1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f,  1.0f, -1.0f, 0.0f, 0.0f, 1.0f,

                // Top face
                -1.0f,  1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f,  1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f,  1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
                -1.0f,  1.0f, -1.0f, 0.0f, 0.0f, 1.0f,

                // Bottom face
                -1.0f, -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f, -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
                 1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
                -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 1.0f
            };

            // Define pyramid vertices (red)
            float[] pyramidVertices = {
                // Base
                -1.0f,  1.0f, -1.0f, 1.0f, 0.0f, 0.0f, // Bottom-left
                 1.0f,  1.0f, -1.0f, 1.0f, 0.0f, 0.0f, // Bottom-right
                 1.0f,  1.0f,  1.0f, 1.0f, 0.0f, 0.0f, // Top-right
                -1.0f,  1.0f,  1.0f, 1.0f, 0.0f, 0.0f, // Top-left

                // Apex
                 0.0f,  2.0f,  0.0f, 1.0f, 0.0f, 0.0f  // Apex
            };

            // Define XYZ axes vertices
            float[] xyzAxesVertices = {
                // X axis
                0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
                100.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,

                // Y axis
                0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 100.0f, 0.0f, 0.0f, 1.0f, 0.0f,

                // Z axis
                0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, -100.0f, 0.0f, 0.0f, 1.0f
            };

            // Generate buffers
            cubeBufferID = GPU.GenerateBuffer(cubeVertices);
            pyramidBufferID = GPU.GenerateBuffer(pyramidVertices);
            xyzAxesBufferID = GPU.GenerateBuffer(xyzAxesVertices);

            // View matrix
            ViewMatrix = glm.lookAt(
                new vec3(5, 5, 5), // Camera position
                new vec3(0, 0, 0), // Look at point
                new vec3(0, 1, 0)  // Up vector
            );

            // Model matrix
            ModelMatrix = new mat4(1);

            // Projection matrix
            ProjectionMatrix = glm.perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);

            // Get uniform locations
            sh.UseShader();
            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");

            // Set view and projection matrices
            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

            timer.Start();
        }

        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            #region XYZ Axes
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, xyzAxesBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, new mat4(1).to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_LINES, 0, 6);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region Draw Cube
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, cubeBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_QUADS, 0, 24);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region Draw Pyramid
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, pyramidBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_TRIANGLE_FAN, 0, 5);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion
        }

        public void Update()
        {
            timer.Stop();
            var deltaTime = timer.ElapsedMilliseconds / 1000.0f;
            rotationAngle += deltaTime * rotationSpeed;

            List<mat4> transformations = new List<mat4>();
            transformations.Add(glm.translate(new mat4(1), new vec3(0, 0, -5))); // Move back
            transformations.Add(glm.rotate(rotationAngle, new vec3(0, 1, 0))); // Rotate around Y-axis

            ModelMatrix = MathHelper.MultiplyMatrices(transformations);

            timer.Reset();
            timer.Start();
        }

        public void CleanUp()
        {
            sh.DestroyShader();
            
        }
    }
}