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
using System.Windows.Forms;

namespace Graphics
{
    class Renderer
    {
        Shader sh;
        
        uint xyzAxesBufferID;

        uint cubeBufferID; 
        uint pyramidBufferID; 
        uint clockBufferID;
        uint coneBaseBufferID;
        uint coneSideBufferID;

        //3D Drawing
        mat4 ModelMatrix;

        mat4 ModelMatrix2;

        mat4 ViewMatrix;
        mat4 ProjectionMatrix;
        
        int ShaderModelMatrixID;
        int ShaderViewMatrixID;
        int ShaderProjectionMatrixID;

        const int clockEdges = 50; 

        public float translationX=0, 
                     translationY=0, 
                     translationZ=0;

        public vec3 scale = new vec3(1,1,1); 

        Stopwatch timer = Stopwatch.StartNew();


        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            Gl.glClearColor(0, 0, 0.4f, 1);

            float[] xyzAxesVertices = {
		        //x
		        0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, //R
		        100.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, //R
		        //y
	            0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, //G
		        0.0f, 100.0f, 0.0f, 0.0f, 1.0f, 0.0f, //G
		        //z
	            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,  //B
		        0.0f, 0.0f, 100.0f, 0.0f, 0.0f, 1.0f,  //B
            };

            float[] cubeVertices =
            {
                //BOTTOM FACE
                0.0f, 0.0f,  0.0f, 1.0f, 0.0f, 0.0f, 
                10.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
                10.0f, 40.0f, 0.0f,1.0f, 0.0f, 0.0f,
                0.0f, 40.0f, 0.0f,1.0f, 0.0f, 0.0f,

                //FAR SIDE FACE
                0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 20.0f, 1.0f, 0.0f, 0.0f,
                10.0f, 0.0f, 20.0f,1.0f, 0.0f, 0.0f,
                10.0f, 0.0f, 0.0f,1.0f, 0.0f, 0.0f,

                //BACK FACE
                0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 20.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 40.0f, 20.0f,1.0f, 0.0f, 0.0f,
                0.0f, 40.0f, 0.0f,1.0f, 0.0f, 0.0f,

                //NEAR SIDE FACE 
                0.0f, 40.0f, 20.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 40.0f, 0.0f, 1.0f, 0.0f, 0.0f,
                10.0f, 40.0f, 0.0f,1.0f, 0.0f, 0.0f,
                10.0f, 40.0f, 20.0f,1.0f, 0.0f, 0.0f,

                //FRONT FACE
                10.0f, 40.0f, 0.0f, 1.0f, 0.0f, 0.0f,
                10.0f, 40.0f, 20.0f, 1.0f, 0.0f, 0.0f,
                10.0f, 0.0f, 20.0f,1.0f, 0.0f, 0.0f,
                10.0f, 0.0f, 0.0f,1.0f, 0.0f, 0.0f,
                
                //TOP FACE
                0.0f, 0.0f, 20.0f, 1.0f, 0.0f, 1.0f,
                10.0f, 0.0f, 20.0f, 1.0f, 0.0f, 1.0f,
                10.0f, 40.0f, 20.0f,1.0f, 0.0f, 1.0f,
                0.0f, 40.0f, 20.0f,1.0f, 0.0f, 1.0f,
            }; 

            float[] pyramidVertices =
            {
                //BASE
                // A
                0.0f, 0.0f, 20.0f,   0.0f, 1.0f, 0.0f,
                // B
                10.0f, 0.0f, 20.0f,  0.0f, 1.0f, 0.0f,
                // C
                10.0f, 20.0f, 20.0f, 0.0f, 1.0f, 0.0f,
                // D
                0.0f, 20.0f, 20.0f,  0.0f, 1.0f, 0.0f,

                //SIDES

                //(A,B)
                0.0f, 0.0f, 20.0f,   0.0f, 0.0f, 1.0f, // A
                10.0f, 0.0f, 20.0f,  0.0f, 0.0f, 1.0f, // B
                5.0f, 10.0f, 40.0f,  0.0f, 0.0f, 1.0f, // Apex

                //(B,C)
                10.0f, 0.0f, 20.0f,  0.0f, 0.0f, 1.0f, // B
                10.0f, 20.0f, 20.0f, 0.0f, 0.0f, 1.0f, // C
                5.0f, 10.0f, 40.0f,  0.0f, 0.0f, 1.0f, // Apex

                //(C,D)
                10.0f, 20.0f, 20.0f, 0.0f, 0.0f, 1.0f, // C
                0.0f, 20.0f, 20.0f,  0.0f, 0.0f, 1.0f, // D
                5.0f, 10.0f, 40.0f,  0.0f, 0.0f, 1.0f, // Apex

                //(D,A)
                0.0f, 20.0f, 20.0f,  0.0f, 0.0f, 1.0f, // D
                0.0f, 0.0f, 20.0f,   0.0f, 0.0f, 1.0f, // A
                5.0f, 10.0f, 40.0f,  0.0f, 0.0f, 1.0f  // Apex
            }; 

            float[] clockVertices = clockDraw( 10, centerY:  10, 100, 10, 0, 0, 0);

            float[] coneBaseVertices = {
                //BASE CENTER
                5.0f, 30.0f, 20.0f,1.0f, 1.0f, 0.0f,
                // CIRCUMFERENCE
                10.0f, 30.0f, 20.0f,   1.0f, 1.0f, 0.0f,
                8.0f, 33.0f, 20.0f, 1.0f, 1.0f, 0.0f,
                5.0f, 35.0f, 20.0f,   1.0f, 1.0f, 0.0f,
                1.0f, 33.0f, 20.0f, 1.0f, 1.0f, 0.0f,
                0.0f, 30.0f, 20.0f,   1.0f, 1.0f, 0.0f,
                1.0f, 26.0f, 20.0f, 1.0f, 1.0f, 0.0f,
                5.0f, 25.0f, 20.0f,   1.0f, 1.0f, 0.0f,
                8.0f, 26.0f, 20.0f, 1.0f, 1.0f, 0.0f,
                10.0f, 30.0f, 20.0f,   1.0f, 1.0f, 0.0f
            };
            float[] coneSideVertices = {
                10.0f, 30.0f, 20.0f,  1.0f, 1.0f, 0.0f,
                 5.0f, 30.0f, 40.0f,  1.0f, 1.0f, 0.0f,

                 8.0f, 33.0f, 20.0f,  1.0f, 1.0f, 0.0f,
                 5.0f, 30.0f, 40.0f,  1.0f, 1.0f, 0.0f,

                 5.0f, 35.0f, 20.0f,  1.0f, 1.0f, 0.0f,
                 5.0f, 30.0f, 40.0f,  1.0f, 1.0f, 0.0f,

                 1.0f, 33.0f, 20.0f,  1.0f, 1.0f, 0.0f,
                 5.0f, 30.0f, 40.0f,  1.0f, 1.0f, 0.0f,

                 0.0f, 30.0f, 20.0f,  1.0f, 1.0f, 0.0f,
                 5.0f, 30.0f, 40.0f,  1.0f, 1.0f, 0.0f,

                 1.0f, 26.0f, 20.0f,  1.0f, 1.0f, 0.0f,
                 5.0f, 30.0f, 40.0f,  1.0f, 1.0f, 0.0f,

                 5.0f, 25.0f, 20.0f,  1.0f, 1.0f, 0.0f,
                 5.0f, 30.0f, 40.0f,  1.0f, 1.0f, 0.0f,

                 8.0f, 26.0f, 20.0f,  1.0f, 1.0f, 0.0f,
                 5.0f, 30.0f, 40.0f,  1.0f, 1.0f, 0.0f,

                10.0f, 30.0f, 20.0f,  1.0f, 1.0f, 0.0f,
                 5.0f, 30.0f, 40.0f,  1.0f, 1.0f, 0.0f,
            };


            xyzAxesBufferID = GPU.GenerateBuffer(xyzAxesVertices);

            cubeBufferID = GPU.GenerateBuffer(cubeVertices); 
            pyramidBufferID = GPU.GenerateBuffer(pyramidVertices); 
            clockBufferID = GPU.GenerateBuffer(clockVertices);
            coneBaseBufferID = GPU.GenerateBuffer(coneBaseVertices);
            coneSideBufferID = GPU.GenerateBuffer(coneSideVertices);

            // View matrix 
            ViewMatrix = glm.lookAt(
                        new vec3(50,  50, 50),
                        new vec3(0, 0, 0),
                        new vec3(0, 0,1)
                );
            // Model Matrix Initialization
            ModelMatrix = new mat4(1);

            ModelMatrix2 = new mat4(1);

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

            #region Cube
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, cubeBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_QUADS, 0, 6*4);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            #endregion

            #region Pyramid
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, pyramidBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            // Draw the base
            Gl.glDrawArrays(Gl.GL_QUADS, 0, 4);
            // Draw sides faces
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 4, 12);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            #endregion
            
            #region clock
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, clockBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix2.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            Gl.glDrawArrays(Gl.GL_POLYGON, 0, clockEdges);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            Gl.glDisableVertexAttribArray(2);
            #endregion

            #region Cone
            // Draw the base
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, coneBaseBufferID);
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLE_FAN, 0, 10);
            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            // Draw sides faces
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, coneSideBufferID);
            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLE_STRIP, 0, 18);
            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            #endregion
        }
        public float[] clockDraw(float centerX, float centerY, float centerZ, float rad, float R, float G, float B)
        {
            List<float> verticies = new List<float>();

            float step = (float)(2 * Math.PI) / clockEdges;

            float angle = 0.0f;
            while (angle < 2 * Math.PI)
            {
                float u = 0.5f + (0.5f * (float)Math.Cos(angle));
                float v = 0.5f - (0.5f * (float)Math.Sin(angle));
                float x = centerX + (float)(rad * Math.Cos(angle));
                float y = centerY + (float)(rad * Math.Sin(angle));
                verticies.AddRange(new float[] { x, y, centerZ, R, G, B, u, v });
                angle += step;
            }

            return verticies.ToArray();
        }
        public void Update()
        {
            timer.Stop();
            var deltaTime = timer.ElapsedMilliseconds/1000.0f;

            List<mat4> transformations = new List<mat4>();
            transformations.Add(glm.translate(new mat4(1), new vec3(translationX, translationY, translationZ)));
            transformations.Add(glm.scale(new mat4(1), scale));
            ModelMatrix =  MathHelper.MultiplyMatrices(transformations);

            vec3 clockCenter = new vec3(0, 30, 30);
            List<mat4> clockTransformations = new List<mat4>();
            clockTransformations.Add(glm.translate(new mat4(1), -1 * clockCenter));
            clockTransformations.Add(glm.rotate((float)(Math.PI / 2), new vec3(1, 0, 0)));
            clockTransformations.Add(glm.translate(new mat4(1), clockCenter));
            clockTransformations.Add(glm.scale(new mat4(1), new vec3(1.5f, 1.5f, 1.5f)));
            ModelMatrix2 =  MathHelper.MultiplyMatrices(clockTransformations);

            timer.Reset();
            timer.Start();
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
