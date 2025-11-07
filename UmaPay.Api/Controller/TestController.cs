using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UmaPay.Api.Controller
{
    using Azure;
    using Domain;
    using Interface.Service;
    using System.Security.Cryptography;
    using System.Text;
    using UmaPay.Api.Attributes;

    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        #region Properties

        private readonly IConfiguration _configuration;

        #endregion  Properties

        #region Constructor

        public TestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion Constructor

        #region Api Methods

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok("Test Sucess lst");
        }

        [HttpGet("generate")]
        public ActionResult<string> GenerateStoken(string transactionId, string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var appCode = _configuration["Nuvei:ApplicationCode"];
                var appKey = _configuration["Nuvei:ApplicationKey"];

                if (string.IsNullOrEmpty(appCode) || string.IsNullOrEmpty(appKey))
                {
                    return StatusCode(500, new { error = "Server configuration error" });
                }

                var stoken = GenerateStokenInternal(transactionId, appCode, userId, appKey);
                return Ok(stoken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while generating the stoken" });
            }
        }

        #endregion Api Methods

        #region Helper

        private string GenerateStokenInternal(string transactionId, string appCode, string userId, string appKey)
        {
            var forMd5 = $"{transactionId}_{appCode}_{userId}_{appKey}";

            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(forMd5);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        #endregion Helper


    }
}