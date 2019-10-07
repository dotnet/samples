using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace virtual_interface_methods
{
    // <SnippetTimerLightFinal>
    public interface ITimerLight : ILight
    {
        public virtual async Task TurnOnFor(int duration)
        {
            Console.WriteLine("Using default interface implementation for ITimerLight.TurnOnFor");
            SwitchOn();
            await Task.Delay(duration);
            SwitchOff();
            Console.WriteLine("Completed ITimerLight.TurnOnFor sequence");
        }
    }
    // </SnippetTimerLightFinal>
}
