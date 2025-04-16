using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gregatr.Domain.Services;

namespace Gregatr.Domain.Entities
{
    public interface ISource
    {
        //Parser Parser
        //{
        //    get;
        //    set;
        //}

        IEnumerable<object> Items
        {
            get;
            set;
        }
    }
}
