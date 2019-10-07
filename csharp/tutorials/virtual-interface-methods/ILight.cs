using System;
using System.Collections.Generic;
using System.Text;

namespace virtual_interface_methods
{
    public enum PowerStatus
    {
        NoPower,
        ACPower,
        FullBattery,
        MidBattery,
        LowBattery
    }


    // <SnippetILightInterface>
    public interface ILight
    {
        void SwitchOn();
        void SwitchOff();
        bool IsOn();
        public virtual PowerStatus Power() => PowerStatus.NoPower;
    }
    // </SnippetILightInterface>
}
