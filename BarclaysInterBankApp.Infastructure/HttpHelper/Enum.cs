using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Infastructure.HttpHelper
{
    public enum AuthType
    {
        Basic,
        Bearer,
        None
    }

    public enum CustomHttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE,
    }
}
