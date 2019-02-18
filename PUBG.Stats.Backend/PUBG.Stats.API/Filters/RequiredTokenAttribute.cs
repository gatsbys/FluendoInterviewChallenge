using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PUBG.Stats.API.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequiredTokenAttribute : Attribute
    {
    }

}
