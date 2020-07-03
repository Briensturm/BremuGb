using System;

using OpenToolkit.Graphics.OpenGL;

namespace BremuGb.Frontend
{
    internal class Shader
    {
        private readonly int _shaderProgramHandle;

        internal Shader(string vertexShaderSource, string fragmentShaderSource)
        {
            //create and compile vertex and fragment shader
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);            

            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            CompileShader(vertexShader);
            CompileShader(fragmentShader);

            //create and link the shader program
            _shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(_shaderProgramHandle, vertexShader);
            GL.AttachShader(_shaderProgramHandle, fragmentShader);

            LinkShaderProgram();

            //cleanup
            GL.DetachShader(_shaderProgramHandle, vertexShader);
            GL.DetachShader(_shaderProgramHandle, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);            
        }

        internal void UseShaderProgram()
        {
            GL.UseProgram(_shaderProgramHandle);
        }

        private void CompileShader(int shaderHandle)
        {
            GL.CompileShader(shaderHandle);

            //check for compilation errors
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out var code);

            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shaderHandle);
                throw new Exception($"Error during shader compilation: {infoLog}");
            }
        }

        private void LinkShaderProgram()
        {
            GL.LinkProgram(_shaderProgramHandle);

            //check for linker errors
            GL.GetProgram(_shaderProgramHandle, GetProgramParameterName.LinkStatus, out var code);

            if (code != (int)All.True)
            {
                var infoLog = GL.GetProgramInfoLog(_shaderProgramHandle);
                throw new Exception($"Error during shader program linking: {infoLog}");
            }
        }
    }
}
