using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferModels.Responses
{
    public record TryRefreshTokenResponse(bool IsSuccessful, string AccessToken);
}
