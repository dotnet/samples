using System;
using System.Collections.Generic;
using System.Text;

namespace attributes
{
    // <Snippet1>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    class NewPropertyOrFieldAttribute : Attribute { }
    // </Snippet1>
}
