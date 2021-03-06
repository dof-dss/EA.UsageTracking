﻿using System.Diagnostics;
using EA.UsageTracking.Application.API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EA.UsageTracking.Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreTenant]
    public class MetaController : BaseApiController
    {
        [HttpGet("/info")]
        public ActionResult<string> Info()
        {
            var assembly = typeof(Startup).Assembly;

            var creationDate = System.IO.File.GetCreationTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            return Ok($"Version: {version}, Last Updated: {creationDate}");
        }
    }
}