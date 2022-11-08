using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreTokens.Tokens;

internal abstract class SimpleToken
{
    public abstract string GetName();
    public abstract IEnumerable<string> GetValue();
}
