using OpenToolkit.Graphics.OpenGL;

namespace BremuGb.UI
{
    internal class Texture
    {
        private readonly int _textureHandle;

        internal Texture(byte[] pixelData, int width, int height, PixelFormat pixelFormat)
        {
            _textureHandle = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0,
                                    pixelFormat, PixelType.UnsignedByte, pixelData);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        //caller must bind texture first
        internal void UpdateTextureData(byte[] pixelData, int width, int height, PixelFormat pixelFormat)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height,
                                pixelFormat, PixelType.UnsignedByte, pixelData);
        }

        internal void Use()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
        }
    }
}
