using System;
using System.Threading.Tasks;

namespace virtual_interface_methods
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var light = new ExtraFancyLight();

            var indicator = light as ILight;
            Console.WriteLine(indicator.Power());

            await light.Blink(500, 5);

            await light.TurnOnFor(2000);

            if (light is IBlinkingLight blinker)
            {
                await blinker.Blink(500, 5);
                Console.WriteLine(blinker.Power());
            }
            else
                Console.WriteLine("light can't blink");
            if (light is ITimerLight timer)
            {
                await timer.TurnOnFor(1000);
                Console.WriteLine(timer.Power());
            }
            else
                Console.WriteLine("No timer functionality");
        }
    }
}
