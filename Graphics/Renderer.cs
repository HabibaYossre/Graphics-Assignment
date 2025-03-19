using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

//include GLM library
using GlmNet;

using System.IO;
using System.Diagnostics;

namespace Graphics
{
    class Renderer
    {
        Shader sh;

        uint lidBufferID;
        uint mainStructureBufferID;
        uint xyzAxesBufferID;

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

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            Gl.glClearColor(0, 0, 0.4f, 1);
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            // Renamed to lidStructureVertices and updated colors to yellow
            float[] lidStructureVertices = { 
		        // front
		        0.0f, 20.0f, 20.0f, 1.0f, 1.0f, 0.0f, // Yellow
                -10.0f, 10.0f, 10.0f, 1.0f, 1.0f, 0.0f, // Yellow
                10.0f, 10.0f, 10.0f, 1.0f, 1.0f, 0.0f, // Yellow
                // back
                0.0f, -20.0f, 20.0f, 1.0f, 1.0f, 0.0f, // Yellow
                10.0f, -10.0f, 10.0f, 1.0f, 1.0f, 0.0f, // Yellow
                -10.0f, -10.0f, 10.0f, 1.0f, 1.0f, 0.0f, // Yellow
                // left
                -20.0f, 0.0f, 20.0f, 1.0f, 1.0f, 0.0f, // Yellow
                -10.0f, 10.0f, 10.0f, 1.0f, 1.0f, 0.0f, // Yellow
                -10.0f, -10.0f, 10.0f, 1.0f, 1.0f, 0.0f, // Yellow
                // right
                20.0f, 0.0f, 20.0f, 1.0f, 1.0f, 0.0f, // Yellow
                10.0f, 10.0f, 10.0f, 1.0f, 1.0f, 0.0f, // Yellow
                10.0f, -10.0f, 10.0f, 1.0f, 1.0f, 0.0f // Yellow
            };

            lidCenter = new vec3(0.0f, 0.0f, 0.0f);

            // Renamed to mainStructureVertices and updated colors to blue
            float[] mainStructureVertices =
            {
                // Top Face (Positive Z)
                //0.0f, 0.5f, 1.0f
                -10.0f, 10.0f, 10.0f, 1.0f, 1.0f, 0.0f,  // Light blue
                10.0f, 10.0f, 10.0f, 0.0f, 0.5f, 1.0f,
                10.0f, -10.0f, 10.0f, 1.0f, 1.0f, 0.0f,
                -10.0f, -10.0f, 10.0f, 0.0f, 0.5f, 1.0f,

                // Bottom Face (Negative Z)
                -10.0f, 10.0f, -10.0f, 0.0f, 0.3f, 0.8f,  // Slightly darker blue
                10.0f, 10.0f, -10.0f, 0.0f, 0.3f, 0.8f,
                10.0f, -10.0f, -10.0f, 0.0f, 0.3f, 0.8f,
                -10.0f, -10.0f, -10.0f, 0.0f, 0.3f, 0.8f,

                // Left Face (Negative X)
                -10.0f, 10.0f, 10.0f, 0.0f, 0.4f, 0.9f,  // Medium blue
                -10.0f, 10.0f, -10.0f, 0.0f, 0.4f, 0.9f,
                -10.0f, -10.0f, -10.0f, 0.0f, 0.4f, 0.9f,
                -10.0f, -10.0f, 10.0f, 0.0f, 0.4f, 0.9f,

                // Right Face (Positive X)
                10.0f, 10.0f, 10.0f, 0.0f, 0.4f, 0.9f,  // Medium blue
                10.0f, 10.0f, -10.0f, 0.0f, 0.4f, 0.9f,
                10.0f, -10.0f, -10.0f, 0.0f, 0.4f, 0.9f,
                10.0f, -10.0f, 10.0f, 0.0f, 0.4f, 0.9f,

                // Front Face (Positive Y)
                -10.0f, 10.0f, 10.0f, 0.0f, 0.5f, 1.0f,  // Light blue
                -10.0f, 10.0f, -10.0f, 0.0f, 0.5f, 1.0f,
                10.0f, 10.0f, -10.0f, 0.0f, 0.5f, 1.0f,
                10.0f, 10.0f, 10.0f, 0.0f, 0.5f, 1.0f,

                // Back Face (Negative Y)
                -10.0f, -10.0f, 10.0f, 0.0f, 0.3f, 0.8f,  // Slightly darker blue
                -10.0f, -10.0f, -10.0f, 0.0f, 0.3f, 0.8f,
                10.0f, -10.0f, -10.0f, 0.0f, 0.3f, 0.8f,
                10.0f, -10.0f, 10.0f, 0.0f, 0.3f, 0.8f
            };

            float[] xyzAxesVertices = {
		        //x
		        -100.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, //R
		        100.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, //R

		        //y
	            0.0f, -100.0f, 0.0f, 0.0f, 1.0f, 0.0f, //G
		        0.0f, 100.0f, 0.0f, 0.0f, 1.0f, 0.0f, //G
		        //diag1
		        -100.0f, 100.0f, 0.0f, 1.0f, 1.0f, 0.0f, //R
		        100.0f, -100.0f, 0.0f, 1.0f, 1.0f, 0.0f, //R
                //diag1
		        0.0f, -100.0f, 100.0f, 0.0f, 1.0f, 1.0f, //R
		        0.0f, 100.0f, -100.0f, 0.0f, 1.0f, 1.0f, //R
                //diag3
		        -100.0f, 0.0f, 100.0f, 0.0f, 1.0f, 1.0f, //R
		        100.0f, 0.0f, -100.0f, 0.0f, 1.0f, 1.0f, //R
            };

            lidBufferID = GPU.GenerateBuffer(lidStructureVertices);
            mainStructureBufferID = GPU.GenerateBuffer(mainStructureVertices);
            xyzAxesBufferID = GPU.GenerateBuffer(xyzAxesVertices);

            // View matrix 
            ViewMatrix = glm.lookAt(
                        new vec3(50, 50, 50), // Camera is at (0,5,5), in World Space
                        new vec3(0, 0, 0), // and looks at the origin
                        new vec3(0, 0, 1)  // Head is up (set to 0,-1,0 to look upside-down)
                );
            // Model Matrix Initialization
            ModelMatrix = new mat4(1);

            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            ProjectionMatrix = glm.perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);

            // Our MVP matrix which is a multiplication of our 3 matrices 
            sh.UseShader();

            //Get a handle for our "MVP" uniform (the holder we created in the vertex shader)
            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");

            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            timer.Start();
        }

        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);
            #region XYZ axis
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, xyzAxesBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, new mat4(1).to_array()); // Identity

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_LINES, 0, 10);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            #endregion

            #region Main Structure
            Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, mainStructureBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_QUADS, 0, 24);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion

            #region Lid Structure
            Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);

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
        }

        public void Update()
        {
            timer.Stop();
            var deltaTime = timer.ElapsedMilliseconds / 1000.0f;
            rotationAngle += deltaTime * rotationSpeed;

            List<mat4> transformations = new List<mat4>();
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