using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Description;

using System.Diagnostics;
using GuildfordBoroughCouncil.Api.Status.Models;
using GuildfordBoroughCouncil.Api.Status.BartecAuth;
using GuildfordBoroughCouncil.Api.Status.WasteCollectorR4;

namespace GuildfordBoroughCouncil.Api.Status.Controllers
{
    [RoutePrefix("waste-collector/v4")]
    public class WasteCollectorR4Controller : ApiController
    {
        private decimal Uprn
        {
            get
            {
                return 10007100404;
            }
        }

        private decimal Usrn
        {
            get
            {
                return 16000942;
            }
        }

        private string Token
        {
            get
            {
                var AuthService = new AuthenticationAPIWebServiceSoapClient("AuthenticationAPIWebServiceSoap");
                var Auth = AuthService.Authenticate(Properties.Settings.Default.WasteCollectorUsername, Properties.Settings.Default.WasteCollectorPassword);

                if (Auth.Errors.Error != null)
                {
                    throw new Exception("Unable to authenticate with WasteCollector" + ((String.IsNullOrWhiteSpace(Auth.Errors.Error.Message)) ? "." : " - " + Auth.Errors.Error.Message));
                }
                else
                {
                    var AuthToken = Auth.Token;
                    return AuthToken.TokenString;
                }
            }
        }

        private WasteCollectorAPIWebServiceSoapClient Service
        {
            get
            {
                return new WasteCollectorAPIWebServiceSoapClient("WasteCollectorAPIWebServiceSoapR4");
            }
        }

        [HttpGet]
        [Route]
        [ResponseType(typeof(pingdom_http_custom_check))]
        public async Task<IHttpActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var Check = new pingdom_http_custom_check();

                var auth = Authenticate().ExecuteAsync(cancellationToken).Result;
                auth.EnsureSuccessStatusCode();
                Check.response_time = (await auth.Content.ReadAsAsync<pingdom_http_custom_check>()).response_time;

                var pr = (await PremisesRounds()).ExecuteAsync(cancellationToken).Result;
                pr.EnsureSuccessStatusCode();
                Check.response_time += (await pr.Content.ReadAsAsync<pingdom_http_custom_check>()).response_time;

                var pfc = (await PremisesFutureCollections()).ExecuteAsync(cancellationToken).Result;
                pfc.EnsureSuccessStatusCode();
                Check.response_time += (await pfc.Content.ReadAsAsync<pingdom_http_custom_check>()).response_time;

                var pa = (await PremisesAttributes()).ExecuteAsync(cancellationToken).Result;
                pa.EnsureSuccessStatusCode();
                Check.response_time += (await pa.Content.ReadAsAsync<pingdom_http_custom_check>()).response_time;

                var pe = (await PremisesEvents()).ExecuteAsync(cancellationToken).Result;
                pe.EnsureSuccessStatusCode();
                Check.response_time += (await pe.Content.ReadAsAsync<pingdom_http_custom_check>()).response_time;

                var sr = (await ServiceRequests()).ExecuteAsync(cancellationToken).Result;
                sr.EnsureSuccessStatusCode();
                Check.response_time += (await sr.Content.ReadAsAsync<pingdom_http_custom_check>()).response_time;

                var se = (await StreetEvents()).ExecuteAsync(cancellationToken).Result;
                se.EnsureSuccessStatusCode();
                Check.response_time += (await se.Content.ReadAsAsync<pingdom_http_custom_check>()).response_time;

                Check.response_time = Check.response_time / 7;

                return Ok(Check);
            }
            catch(HttpRequestException ex)
            {
                if (ex.Message.Contains(" 504 "))
                    return StatusCode(HttpStatusCode.GatewayTimeout);
                else
                    return StatusCode(HttpStatusCode.BadGateway);
            }
        }

        [HttpGet]
        [Route("authenticate")]
        [ResponseType(typeof(pingdom_http_custom_check))]
        public IHttpActionResult Authenticate()
        {
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var token = Token;

                stopWatch.Stop();

                return Ok(new pingdom_http_custom_check { response_time = stopWatch.Elapsed.TotalMilliseconds, status = "OK" });
            }
            catch (System.TimeoutException)
            {
                return StatusCode(HttpStatusCode.GatewayTimeout);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.BadGateway);
            }
        }

        [HttpGet]
        [Route("premises-rounds")]
        [ResponseType(typeof(pingdom_http_custom_check))]
        public async Task<IHttpActionResult> PremisesRounds()
        {
            try
            {
                var token = Token;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var Data = await Service.Premises_Rounds_GetAsync(token, Uprn);

                if (Data.Premises_Rounds_GetResult.Errors.Error.Result != "0")
                {
                    throw new Exception(Data.Premises_Rounds_GetResult.Errors.Error.Message);
                }

                stopWatch.Stop();

                return Ok(new pingdom_http_custom_check { response_time = stopWatch.Elapsed.TotalMilliseconds, status = "OK" });
            }
            catch (System.TimeoutException)
            {
                return StatusCode(HttpStatusCode.GatewayTimeout);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.BadGateway);
            }
        }

        [HttpGet]
        [Route("premises-future-collections")]
        [ResponseType(typeof(pingdom_http_custom_check))]
        public async Task<IHttpActionResult> PremisesFutureCollections()
        {
            try
            {
                var token = Token;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var Data = await Service.Premises_FutureCollections_GetAsync(token, Uprn);

                if (Data.Premises_FutureCollections_GetResult.Errors.Error.Result != "0")
                {
                    throw new Exception(Data.Premises_FutureCollections_GetResult.Errors.Error.Message);
                }

                stopWatch.Stop();

                return Ok(new pingdom_http_custom_check { response_time = stopWatch.Elapsed.TotalMilliseconds, status = "OK" });
            }
            catch (System.TimeoutException)
            {
                return StatusCode(HttpStatusCode.GatewayTimeout);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.BadGateway);
            }
        }

        [HttpGet]
        [Route("premises-attributes")]
        [ResponseType(typeof(pingdom_http_custom_check))]
        public async Task<IHttpActionResult> PremisesAttributes()
        {
            try
            {
                var token = Token;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var Data = await Service.Premises_Attributes_GetAsync(token, Uprn);

                if (Data.Premises_Attributes_GetResult.Errors.Error.Result != "0")
                {
                    throw new Exception(Data.Premises_Attributes_GetResult.Errors.Error.Message);
                }

                stopWatch.Stop();

                return Ok(new pingdom_http_custom_check { response_time = stopWatch.Elapsed.TotalMilliseconds, status = "OK" });
            }
            catch (System.TimeoutException)
            {
                return StatusCode(HttpStatusCode.GatewayTimeout);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.BadGateway);
            }
        }

        [HttpGet]
        [Route("premises-events")]
        [ResponseType(typeof(pingdom_http_custom_check))]
        public async Task<IHttpActionResult> PremisesEvents()
        {
            try
            {
                var token = Token;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var Data = await Service.Premises_Events_GetAsync(token, Uprn, null, DateTime.Today.AddDays(-21), null, null, null, null, new ctMapBox(), new ctMapBox(), null, null, null, null, null, null, null, false);

                if (Data.Premises_Events_GetResult.Errors.Error.Result != "0")
                {
                    throw new Exception(Data.Premises_Events_GetResult.Errors.Error.Message);
                }

                stopWatch.Stop();

                return Ok(new pingdom_http_custom_check { response_time = stopWatch.Elapsed.TotalMilliseconds, status = "OK" });
            }
            catch (System.TimeoutException)
            {
                return StatusCode(HttpStatusCode.GatewayTimeout);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.BadGateway);
            }
        }

        [HttpGet]
        [Route("service-requests")]
        [ResponseType(typeof(pingdom_http_custom_check))]
        public async Task<IHttpActionResult> ServiceRequests()
        {
            try
            {
                var token = Token;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var Data = await Service.ServiceRequests_GetAsync(token, null, Uprn, new ctMapBox(), new ctMapBox(), null, null, null, null, null, null);

                if (Data.ServiceRequests_GetResult.Errors.Error.Result != "0")
                {
                    throw new Exception(Data.ServiceRequests_GetResult.Errors.Error.Message);
                }

                stopWatch.Stop();

                return Ok(new pingdom_http_custom_check { response_time = stopWatch.Elapsed.TotalMilliseconds, status = "OK" });
            }
            catch (System.TimeoutException)
            {
                return StatusCode(HttpStatusCode.GatewayTimeout);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.BadGateway);
            }
        }

        [HttpGet]
        [Route("street-events")]
        [ResponseType(typeof(pingdom_http_custom_check))]
        public async Task<IHttpActionResult> StreetEvents()
        {
            try
            {
                var token = Token;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var Data = await Service.Streets_Events_GetAsync(token, Usrn, DateTime.Today.AddDays(-21), null, new ctMapBox(), null, null, null, true);

                if (Data.Streets_Events_GetResult.Errors.Error.Result != "0")
                {
                    throw new Exception(Data.Streets_Events_GetResult.Errors.Error.Message);
                }

                stopWatch.Stop();

                return Ok(new pingdom_http_custom_check { response_time = stopWatch.Elapsed.TotalMilliseconds, status = "OK" });
            }
            catch (System.TimeoutException)
            {
                return StatusCode(HttpStatusCode.GatewayTimeout);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.BadGateway);
            }
        }
    }
}