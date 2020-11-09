//-----> Contributed By- Abhishek Tiwari (849729)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClaimsMicroservice.Models;
using ClaimsMicroservice.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ClaimsController));

        private readonly IClaimRepository _claimRepository;
        public ClaimsController(IClaimRepository claimRepository)
        {
            _claimRepository = claimRepository;
        }





        /// <summary>
        /// To get the Claim Status
        /// </summary>
        /// <param name="policyID"></param>
        /// <param name="claimID"></param>
        /// <returns></returns>

        //https://localhost:44387/api/Claims/getClaimStatus?claimID=1&policyID=1
        [HttpGet]
        [Route("getClaimStatus")]
        public async Task<ActionResult<string>> GetClaimStatus([FromQuery] int claimID, [FromQuery] int policyID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _log4net.Info("GetClaimStatus Method Called");
                    return Ok(_claimRepository.GetClaimStatus(claimID, policyID));
                }
                else
                    _log4net.Info("Model is not valid in GetClaimStatus");
                return BadRequest();

            }
            catch (Exception e)
            {
                _log4net.Error("Exception in GetClaimStatus" + e.Message);
                return new NoContentResult();
            }
        }



        /// <summary>
        /// To Post the Claim and Get back the status
        /// </summary>
        /// <param name="policyID"></param>
        /// <param name="memberID"></param>
        /// <param name="benefitID"></param>
        /// <param name="hospitalID"></param>
        /// <param name="claimAmt"></param>
        /// <param name="benefit"></param>
        /// <returns></returns>

        //https://localhost:44387/api/Claims/submitClaim?policyID=1&memberID=1&benefitID=1&hospitalID=1&claimAmt=80000&benefit="MedicalCheckup"
        [HttpPost]
        [Route("submitClaim")]
        public async Task<ActionResult<string>> SubmitClaim([FromQuery] int policyID, [FromQuery] int memberID, [FromQuery] int benefitID, [FromQuery] int hospitalID, [FromQuery] double claimAmt, [FromQuery] string benefit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _log4net.Info("SubmitClaim Method Called");
                    return Ok(_claimRepository.SubmitClaim(policyID, memberID, benefitID, hospitalID, claimAmt, benefit).Result);
                }
                else
                {
                    _log4net.Info("Model is not valid in SubmitClaim");
                    return BadRequest();
                }

            }
            catch (Exception e)
            {
                _log4net.Error("Exception in SubmitClaim" + e.Message);
                return new NoContentResult();
            }
        }
    }
}
