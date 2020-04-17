namespace Raytracer
{
    readonly struct Ray
    {
        public readonly Vector Start;
        public readonly Vector Dir;

        public Ray(Vector start, Vector dir) => (Start, Dir) = (start, dir);
    }
}
