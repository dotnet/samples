using System.Collections.Generic;

namespace Raytracer
{
    class Scene
    {
        public SceneObject[] Things;
        public Light[] Lights;
        public Camera Camera;

        public Scene(SceneObject[] things, Light[] lights, Camera camera) =>
            (Things, Lights, Camera) = (things, lights, camera);

        public IEnumerable<ISect> Intersect(Ray r)
        {
            foreach (SceneObject obj in Things)
            {
                yield return obj.Intersect(r);
            }
        }
    }
}
