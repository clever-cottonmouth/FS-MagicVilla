using MagicVilla_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_Web.Services.IServices
{
    public interface IApiMessageRequestBuilder
    {
        HttpRequestMessage Build(ApiRequest apiRequest);
    }
}
