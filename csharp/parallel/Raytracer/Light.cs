namespace Raytracer
{
    class Light
    {
        public Vector Pos;
        public Color Color;

        public Light(Vector pos, Color color) => (Pos, Color) = (pos, color);
    }
}
