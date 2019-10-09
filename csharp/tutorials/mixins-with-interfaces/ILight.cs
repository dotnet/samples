using System;
using System.Collections.Generic;
using System.Text;

namespace mixins_with_interfaces
{
    // <SnippetPowerStatus>
    public enum PowerStatus
    {
        NoPower,
        ACPower,
        FullBattery,
        MidBattery,
        LowBattery
    }
    // </SnippetPowerStatus>


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
