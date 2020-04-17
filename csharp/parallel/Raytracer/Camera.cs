namespace Raytracer
{
    class Camera
    {
        public Vector Pos;
        public Vector Forward;
        public Vector Up;
        public Vector Right;

        public Camera(Vector pos, Vector forward, Vector up, Vector right) =>
            (Pos, Forward, Up, Right) = (pos, forward, up, right);

        public static Camera Create(Vector pos, Vector lookAt)
        {
            var forward = Vector.Norm(Vector.Minus(lookAt, pos));
            var down = new Vector(0, -1, 0);
            var right = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, down)));
            var up = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, right)));

            return new Camera(pos, forward, up, right);
        }
    }
}
