using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using System.Windows.Forms;
namespace Graphics
{
    class Shader
    {
        int ProgramID;
        public int ID
        {
            get
            {
                return ProgramID;
            }
        }
        public Shader(string vertex_shader_file_path, string fragment_shader_file_path)
        {
            ProgramID = LoadShaders(vertex_shader_file_path, fragment_shader_file_path);
        }
        int LoadShaders(string vs_path, string fs_path)
        {
            int vertexShaderID = Gl.glCreateShader(Gl.GL_VERTEX_SHADER);
            int fragmentShaderID = Gl.glCreateShader(Gl.GL_FRAGMENT_SHADER);
            string vertexShaderCode = "";
            string fragmentShaderCode = "";

            //Compiling vertex shader
            try
            {
                StreamReader sr = new StreamReader(vs_path);
                vertexShaderCode = sr.ReadToEnd();
                sr.Close();
            }
            catch
            {
                return 0;
            }
            int result = Gl.GL_FALSE;
            int InfoLogLength;
            Gl.glShaderSource(vertexShaderID, 1, new[] { vertexShaderCode }, new[] { vertexShaderCode.Length });
            Gl.glCompileShader(vertexShaderID);

            int[] parameters = new int[] { result };
            Gl.glGetShaderiv((uint)vertexShaderID, Gl.GL_COMPILE_STATUS, parameters);
            result = parameters[0];
            
            Gl.glGetShaderiv((uint)vertexShaderID, Gl.GL_INFO_LOG_LENGTH, parameters);
            int bufSize = parameters[0];
            if (bufSize > 0)
            {
                StringBuilder il = new StringBuilder(bufSize);
                Gl.glGetShaderInfoLog(vertexShaderID, bufSize, IntPtr.Zero, il);
                string log = il.ToString();
                MessageBox.Show("Error in vertex shader: \r\n" + log);
            }

            //Compiling fragment shader
            try
            {
                StreamReader sr = new StreamReader(fs_path);
                fragmentShaderCode = sr.ReadToEnd();
                sr.Close();
            }
            catch
            {
                return 0;
            }
            result = Gl.GL_FALSE;
            Gl.glShaderSource(fragmentShaderID, 1, new[] { fragmentShaderCode }, new[] { fragmentShaderCode.Length });
            Gl.glCompileShader(fragmentShaderID);

            parameters = new int[] { result };
            Gl.glGetShaderiv((uint)fragmentShaderID, Gl.GL_COMPILE_STATUS, parameters);
            result = parameters[0];

            Gl.glGetShaderiv((uint)fragmentShaderID, Gl.GL_INFO_LOG_LENGTH, parameters);
            bufSize = parameters[0];
            if (bufSize > 0)
            {
                StringBuilder il = new StringBuilder(bufSize);
                Gl.glGetShaderInfoLog(fragmentShaderID, bufSize, IntPtr.Zero, il);
                string log = il.ToString();
                MessageBox.Show("Error in fragment shader: \r\n"+log);
            }

            //Linking the program
            int ProgramID = Gl.glCreateProgram();
            Gl.glAttachShader(ProgramID, vertexShaderID);
            Gl.glAttachShader(ProgramID, fragmentShaderID);
            Gl.glLinkProgram(ProgramID);


            Gl.glGetProgramiv(ProgramID, Gl.GL_LINK_STATUS, parameters);
            result = parameters[0];
            Gl.glGetProgramiv(ProgramID, Gl.GL_INFO_LOG_LENGTH, parameters);
            bufSize = parameters[0];
            if(bufSize > 0)
            {
                StringBuilder il = new StringBuilder(bufSize);
                Gl.glGetProgramInfoLog(ProgramID, bufSize, IntPtr.Zero, il);
                string log = il.ToString();
                MessageBox.Show("Error in Linked Program: \r\n" + log);
            }

            Gl.glDeleteShader(vertexShaderID);
            Gl.glDeleteShader(fragmentShaderID);
            return ProgramID;
        }
        public void UseShader()
        {
            Gl.glUseProgram(ProgramID);
        }
        public void DestroyShader()
        {
            Gl.glDeleteProgram(ProgramID);
        }
    }
}
