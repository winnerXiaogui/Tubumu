using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tubumu.Modules.Framework.Infrastructure.FastReflectionLib
{
    public interface IFastReflectionFactory<TKey, TValue>
    {
        TValue Create(TKey key);
    }
}
