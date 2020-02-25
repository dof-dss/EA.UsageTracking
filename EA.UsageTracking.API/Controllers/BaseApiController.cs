using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EA.UsageTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
    }
}