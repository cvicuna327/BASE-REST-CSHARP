using AFEX.plantilla.api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using AFEX.plantilla.api.ESB_AFEX_LDAP;

namespace AFEX.plantilla.api.Controllers
{
    /// <summary>
    /// login controller class for authenticate users
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("ping")]
        public IHttpActionResult Ping()
        {
            //return Content(HttpStatusCode., String.Empty);
            return Ok();
        }

        [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Authenticate(LoginRequest login)
        {
            try
            {
                if (login == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                var _LDAP = new ESB_AFEX_LDAP.ESB_AV_ServicioAuthSoapClient();
                var _RESP = _LDAP.AutenticarLDAP(login.Username, login.Password, AplicacionAuth.MoneyGram);

                bool isCredentialValid = (_RESP.Count() > 0);
                if (isCredentialValid)
                {
                    var token = TokenGenerator.GenerateTokenJwt(login.Username);

                    return Ok($"Bearer {token}");
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
