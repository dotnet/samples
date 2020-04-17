namespace Raytracer
{
    readonly struct Light
    {
        public readonly Vector Pos;
        public readonly Color Color;

        public Light(Vector pos, Color color) => (Pos, Color) = (pos, color);
    }
}
