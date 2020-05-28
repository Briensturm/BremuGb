using System;

using OpenToolkit.Graphics.OpenGL;

namespace BremuGb.UI
{
    internal static class OpenGlUtility
    {
        internal static void ThrowIfOpenGlError()
        {
            ErrorCode error = GL.GetError();
            if (error != 0)
                throw new Exception($"OpenGL error occured: {error}");
        }
    }
}
