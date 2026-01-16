using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVEST.Application.Common.Errors
{
    public sealed record Error(ErrorType Type, string Message);
}