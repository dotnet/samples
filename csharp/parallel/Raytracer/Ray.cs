namespace Raytracer
{
    struct Ray
    {
        public Vector Start;
        public Vector Dir;

        public Ray(Vector start, Vector dir) => (Start, Dir) = (start, dir);
    }
}
