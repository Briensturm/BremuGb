using OpenToolkit.Graphics.OpenGL;

namespace BremuGb.Frontend
{
    internal class Quad
    {
        private readonly float[] _vertices = { 1.0f,  1.0f, 0.0f,   1.0f, 0.0f,
                                               1.0f, -1.0f, 0.0f,   1.0f, 1.0f,
                                              -1.0f, -1.0f, 0.0f,   0.0f, 1.0f,
                                              -1.0f,  1.0f, 0.0f,   0.0f, 0.0f };

        private readonly uint[] _indices = { 0, 1, 3,
                                             1, 2, 3 };

        private readonly int _vertexArrayObject;

        private readonly int _vertexBufferObject;
        private readonly int _elementBufferObject;

        internal Quad()
        {
            _vertexArrayObject = GL.GenVertexArray();

            _vertexBufferObject = GL.GenBuffer();
            _elementBufferObject = GL.GenBuffer();


            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * _vertices.Length, _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(float) * _indices.Length, _indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        internal void Bind()
        {
            GL.BindVertexArray(_vertexArrayObject);
        }

        internal void Render()
        {
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        internal void Delete()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);

            GL.DeleteVertexArray(_vertexArrayObject);
        }
    }
}
