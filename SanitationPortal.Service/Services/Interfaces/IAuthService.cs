using SanitationPortal.Models.Requests;
using SanitationPortal.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanitationPortal.Service.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Response<bool>> Register(UserRegisterRequest request);
    }
}
