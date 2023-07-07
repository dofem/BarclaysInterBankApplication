using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Infastructure.HttpHelper.Contract
{
    public interface IHttpService
    {
        Task<ApiResponse<T>> MakeHttpRequestAsync<T>(Uri requestUri, string payload, string authToken, AuthType authType, CustomHttpMethod httpMethod);
    }
}
