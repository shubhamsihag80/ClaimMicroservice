//-----> Contributed By- Abhishek Tiwari (849729)

using ClaimsMicroservice.Models;
using log4net.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClaimsMicroservice.Repository
{
    public class ClaimRepository : IClaimRepository
    {
        
        public string GetClaimStatus(int claimID, int policyID)
        {
            try
            {
                string filterClaim = (from p in ClaimData.claims
                                      where (p.ClaimID == claimID && p.PolicyID == policyID)
                                      select p.ClaimStatus).FirstOrDefault();
                return filterClaim;
            }
            catch(Exception e)
            {
                return e.Message;
            }
            
        }

        public async Task<string> SubmitClaim(int policyID, int memberID, int benefitID, int hospitalID, double claimAmt, string benefit)
        {
            string status = "";
            double claimAmount = ClaimAmountFetch(policyID, memberID, benefitID);
            string benefitFetched = BenefitFetch(policyID, memberID);
            if(benefitFetched.Equals("Invalid Data")||claimAmount == -1)
            {
                return status = "Invalid Details Provided";
            }
            else if ((claimAmount >= claimAmt) && (benefitFetched.Equals(benefit)) && IsHospital(policyID, hospitalID).Result)
            {
                status = "Pending Action";
                Claim claims = new Claim()
                {
                    ClaimID = ClaimData.claims.Count() + 1,
                    ClaimStatus = status,
                    PolicyID = policyID,
                    AmountClaimed = claimAmt,
                    BenefitsAvailed = benefit,
                    HospitalID = hospitalID,
                    Remarks = "Verified",
                    Settled = "False"
                };
                ClaimData.claims.Add(claims);
            }
            else
            {
                status = "Claim Rejected";
            }
            return status;
        }
        public async Task<Boolean> IsHospital(int policyID, int hospitalID)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    client.BaseAddress = new Uri("http://40.76.162.124/api/");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response3 = new HttpResponseMessage();
                    response3 = client.GetAsync("Policy/GetChainOfProviders/" + policyID).Result;
                    List<ProviderPolicy> providers = new List<ProviderPolicy>();
                    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                    string apiResponse = await response3.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<ProviderPolicy>>(apiResponse);
                    Boolean flag = false;
                    foreach (var row in data)
                    {
                        if (row.HospitalID == hospitalID)
                        {
                            flag = true;
                        }
                    }
                    return flag;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public double ClaimAmountFetch(int policyID, int memberID, int benefitID)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    client.BaseAddress = new Uri("http://40.76.162.124/api/");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response1 = new HttpResponseMessage();
                    response1 = client.GetAsync("Policy/GetEligibleClaimAmount?PolicyID=" + policyID + "&MemberID=" + memberID + "&BenefitID=" + benefitID).Result;
                    double claimAmount = Convert.ToDouble(response1.Content.ReadAsStringAsync().Result);
                    return claimAmount;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
            
        }
        public string BenefitFetch(int policyID, int memberID)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    client.BaseAddress = new Uri("http://40.76.162.124/api/");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response2 = new HttpResponseMessage();
                    response2 = client.GetAsync("Policy/GetEligibleBenefits?PolicyID=" + policyID + "&MemberID=" + memberID).Result;
                    string benefit = response2.Content.ReadAsStringAsync().Result;
                    return benefit;
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}
