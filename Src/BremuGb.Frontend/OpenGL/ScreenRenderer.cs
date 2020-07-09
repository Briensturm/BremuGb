using OpenToolkit.Graphics.OpenGL;

namespace BremuGb.Frontend.OpenGL
{
    internal class ScreenRenderer
    {
        private Shader _shader;
        private Texture _texture;
        private Quad _quad;

        private bool _textureChanged;
        private bool _isClosed;

        internal void UpdateTexture(byte[] pixelData)
        {
            _texture.UpdateTextureData(pixelData, 160, 144, PixelFormat.Rgb);
            _textureChanged = true;
        }

        internal void Initialize()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            var data = new byte[160 * 144 * 3];

            _shader = new Shader(Resources.ShaderResource.VertexShader, Resources.ShaderResource.FragmentShader);
            _shader.UseShaderProgram();

            _texture = new Texture(data, 160, 144, PixelFormat.Rgb);
            _texture.Use();

            _quad = new Quad();
            _quad.Bind();

            //check for OpenGL errors
            OpenGlUtility.ThrowIfOpenGlError();
        }

        internal bool Render()
        {
            //only render if the texture has been changed since last successful render call
            if (!_textureChanged || _isClosed)
                return false;
            _textureChanged = false;

            _quad.Render();

            OpenGlUtility.ThrowIfOpenGlError();

            return true;
        }

        internal void Close()
        {
            _isClosed = true;

            _quad.Delete();
            _texture.Delete();
            _shader.Delete();            

            OpenGlUtility.ThrowIfOpenGlError();
        }
    }
}
