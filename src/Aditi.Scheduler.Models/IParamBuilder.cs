using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aditi.Scheduler.Models
{
    public interface IParamBuilder
    {
        Dictionary<string, object> Build();
    }
}
