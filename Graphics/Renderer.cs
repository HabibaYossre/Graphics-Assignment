using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Tao.OpenGl;
using GlmNet;
using static System.Windows.Forms.AxHost;

namespace Graphics
{
    class Renderer
    {
        Shader sh;

        uint lidBufferID;
        uint mainStructureBufferID;
        uint xyzAxesBufferID;
        uint starsBufferID; // Buffer for stars (GL_POINTS)
        uint waveBufferID;  // Buffer for wavy surface (GL_TRIANGLE_STRIP)

        //3D Drawing
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

        vec3 lidCenter;
        Texture BoxTex;
        vec3 ObjectScale = new vec3(1, 1, 1);

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            
            //Light Blue:
            Gl.glClearColor(0.7f, 0.8f, 1.0f, 1.0f);
            //Gl.glClearColor(0.6f, 0.8f, 0.6f, 1.0f); green
            
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            BoxTex = new Texture(projectPath + "\\Textures\\im7.jpg", 1);

            // Define vertices for the lid structure (yellow)
            float[] lidStructureVertices = {
                // front
                0.0f, 20.0f, 20.0f, 1.0f, 1.0f, 0.0f, // Yellow
                -10.0f, 10.0f, 10.0f, 0.0f, 0.5f, 1.0f, // Yellow
                10.0f, 10.0f, 10.0f, 1.0f, 1.0f, 0.0f, // Yellow
                // back
                0.0f, -20.0f, 20.0f, 1.0f, 1.0f, 0.0f, // Yellow
                10.0f, -10.0f, 10.0f, 0.0f, 0.5f, 1.0f, // Yellow
                -10.0f, -10.0f, 10.0f, 1.0f, 1.0f, 0.0f, // Yellow
                // left
                -20.0f, 0.0f, 20.0f, 1.0f, 1.0f, 0.0f, // Yellow
                -10.0f, 10.0f, 10.0f, 0.0f, 0.5f, 1.0f, // Yellow
                -10.0f, -10.0f, 10.0f, 1.0f, 1.0f, 0.0f, // Yellow
                // right
                20.0f, 0.0f, 20.0f, 1.0f, 1.0f, 0.0f, // Yellow
                10.0f, 10.0f, 10.0f, 0.0f, 0.5f, 1.0f, // Yellow
                10.0f, -10.0f, 10.0f, 1.0f, 1.0f, 0.0f // Yellow
            };

            lidCenter = new vec3(0.0f, 0.0f, 0.0f);

            // Define vertices for the main structure (blue with texture coordinates)
            float[] mainStructureVertices = {
                // Top Face (Positive Z)
                -10.0f, 10.0f, 10.0f, 0.0f, 0.5f, 1.0f, 0, 0, // Light blue
                10.0f, 10.0f, 10.0f, 0.0f, 0.5f, 1.0f, 1, 0,
                10.0f, -10.0f, 10.0f, 0.0f, 0.5f, 1.0f, 1, 1,
                -10.0f, -10.0f, 10.0f, 0.0f, 0.5f, 1.0f, 0, 1,

                // Bottom Face (Negative Z)
                -10.0f, 10.0f, -10.0f, 0.0f, 0.3f, 0.8f, 0, 0, // Slightly darker blue
                10.0f, 10.0f, -10.0f, 0.0f, 0.3f, 0.8f, 1, 0,
                10.0f, -10.0f, -10.0f, 0.0f, 0.3f, 0.8f, 1, 1,
                -10.0f, -10.0f, -10.0f, 0.0f, 0.3f, 0.8f, 0, 1,

                // Left Face (Negative X)
                -10.0f, 10.0f, 10.0f, 0.0f, 0.4f, 0.9f, 0, 0, // Medium blue
                -10.0f, 10.0f, -10.0f, 0.0f, 0.4f, 0.9f, 1, 0,
                -10.0f, -10.0f, -10.0f, 0.0f, 0.4f, 0.9f, 1, 1,
                -10.0f, -10.0f, 10.0f, 0.0f, 0.4f, 0.9f, 0, 1,

                // Right Face (Positive X)
                10.0f, 10.0f, 10.0f, 0.0f, 0.4f, 0.9f, 0, 0, // Medium blue
                10.0f, 10.0f, -10.0f, 0.0f, 0.4f, 0.9f, 1, 0,
                10.0f, -10.0f, -10.0f, 0.0f, 0.4f, 0.9f, 1, 1,
                10.0f, -10.0f, 10.0f, 0.0f, 0.4f, 0.9f, 0, 1,

                // Front Face (Positive Y)
                -10.0f, 10.0f, 10.0f, 0.0f, 0.5f, 1.0f, 0, 0, // Light blue
                -10.0f, 10.0f, -10.0f, 0.0f, 0.5f, 1.0f, 1, 0,
                10.0f, 10.0f, -10.0f, 0.0f, 0.5f, 1.0f, 1, 1,
                10.0f, 10.0f, 10.0f, 0.0f, 0.5f, 1.0f, 0, 1,

                // Back Face (Negative Y)
                -10.0f, -10.0f, 10.0f, 0.0f, 0.3f, 0.8f, 0, 0, // Slightly darker blue
                -10.0f, -10.0f, -10.0f, 0.0f, 0.3f, 0.8f, 1, 0,
                10.0f, -10.0f, -10.0f, 0.0f, 0.3f, 0.8f, 1, 1,
                10.0f, -10.0f, 10.0f, 0.0f, 0.3f, 0.8f, 0, 1
            };

            // Define vertices for stars (GL_POINTS)
            float[] starsVertices = GenerateStars(500); // Generate 50 stars

            // Define vertices for a wavy surface (GL_TRIANGLE_STRIP)
            float[] waveVertices = GenerateWaveSurface(20.0f, 5.0f, 20); // Width = 20.0, Amplitude = 5.0, Segments = 20

            // Define vertices for XYZ axes
            float[] xyzAxesVertices = {
                //x
                -100.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, //R
                100.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, //R

                //y
                0.0f, -100.0f, 0.0f, 0.0f, 1.0f, 0.0f, //G
                0.0f, 100.0f, 0.0f, 0.0f, 1.0f, 0.0f, //G

                //z
                0.0f, 0.0f, -100.0f, 0.0f, 0.0f, 1.0f, //B
                0.0f, 0.0f, 100.0f, 0.0f, 0.0f, 1.0f  //B
            };

            // Generate buffers for all vertex data
            lidBufferID = GPU.GenerateBuffer(lidStructureVertices);
            mainStructureBufferID = GPU.GenerateBuffer(mainStructureVertices);
            starsBufferID = GPU.GenerateBuffer(starsVertices);
            waveBufferID = GPU.GenerateBuffer(waveVertices);
            xyzAxesBufferID = GPU.GenerateBuffer(xyzAxesVertices);

            // View matrix
            ViewMatrix = glm.lookAt(
                new vec3(50, 50, 50), // Camera position
                new vec3(0, 0, 0),    // Look at origin
                new vec3(0, 0, 1)     // Up vector
            );

            // Model Matrix Initialization
            ModelMatrix = new mat4(1);

            // Projection Matrix
            ProjectionMatrix = glm.perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);

            // Use shader and set up uniform locations
            sh.UseShader();
            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");

            // Pass matrices to the shader
            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

            timer.Start();
        }

        // Helper function to generate star vertices
        private float[] GenerateStars(int count)
        {
            Random random = new Random();
            List<float> stars = new List<float>();

            for (int i = 0; i < count; i++)
            {
                float x = (float)(random.NextDouble() * 40 - 20); // Random X between -20 and 20
                float y = (float)(random.NextDouble() * 40 - 20); // Random Y between -20 and 20
                float z = (float)(random.NextDouble() * 40 - 20); // Random Z between -20 and 20
                stars.Add(x);
                stars.Add(y);
                stars.Add(z);
                stars.Add(1.0f); // R
                stars.Add(1.0f); // G
                stars.Add(1.0f); // B
            }

            return stars.ToArray();
        }

        // Helper function to generate wave surface vertices
        private float[] GenerateWaveSurface(float width, float amplitude, int segments)
        {
            List<float> wave = new List<float>();
            float step = width / segments;

            for (int i = 0; i <= segments; i++)
            {
                float x = -width / 2 + i * step;
                float z = (float)(amplitude * Math.Sin(x)); // Sine wave
                wave.Add(x);
                wave.Add(-10.0f); // Y position (below the box)
                wave.Add(z);
                wave.Add(0.0f); // R
                wave.Add(1.0f); // G
                wave.Add(0.0f); // B
            }

            return wave.ToArray();
        }

        public void ScalePressfunction(char c)
        {
            

            if (c == 'a') //scale down
            {
                ObjectScale.y = Math.Min(ObjectScale.y * 1.5f, 3f);
                ObjectScale.x = Math.Min(ObjectScale.x * 1.5f, 3f);
            }

            if (c == 's') //scale up
            {
                ObjectScale.x = Math.Max(ObjectScale.x / 1.5f, 1f);
                ObjectScale.y = Math.Max(ObjectScale.y / 1.5f, 1f);
            }

        }



        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            #region XYZ axis
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, xyzAxesBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, new mat4(1).to_array()); // Identity
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_LINES, 0, 6);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region Main Structure
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, mainStructureBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0); // Position
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)0);

            Gl.glEnableVertexAttribArray(1); // Color
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glEnableVertexAttribArray(2); // Texture coordinates
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            // Bind texture
            Gl.glActiveTexture(Gl.GL_TEXTURE0);
            BoxTex.Bind();

            // Set texture sampler uniform
            int textureSamplerLocation = Gl.glGetUniformLocation(sh.ID, "textureSampler");
            Gl.glUniform1i(textureSamplerLocation, 0); // Use texture unit 0

            Gl.glDrawArrays(Gl.GL_QUADS, 0, 24);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            Gl.glDisableVertexAttribArray(2);
            #endregion

            #region Lid Structure
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, lidBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 12);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region Stars (GL_POINTS)
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, starsBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POINTS, 0, 50); // Draw 50 stars

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region Wavy Surface (GL_TRIANGLE_STRIP)
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, waveBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_TRIANGLE_STRIP, 0, 21); // Draw wavy surface (20 segments + 1)

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
            transformations.Add(glm.scale(new mat4(1), ObjectScale));
            transformations.Add(glm.translate(new mat4(1), -1 * lidCenter));
            transformations.Add(glm.rotate(rotationAngle, new vec3(0, 0, 1)));
            transformations.Add(glm.translate(new mat4(1), lidCenter));
            transformations.Add(glm.translate(new mat4(1), new vec3(translationX, translationY, translationZ)));

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