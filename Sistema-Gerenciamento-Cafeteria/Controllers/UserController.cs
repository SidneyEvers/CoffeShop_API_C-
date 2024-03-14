using Sistema_Gerenciamento_Cafeteria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sistema_Gerenciamento_Cafeteria.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        CafeEntities db = new CafeEntities();

        [HttpPost,Route("signup")]
        public HttpResponseMessage Signup([FromBody] USUARIOS Usuarios) 
        {
            try 
            {
                USUARIOS UserObj = db.USUARIOS
                .Where(u => u.email == Usuarios.email).FirstOrDefault();
                if (UserObj == null)
                {
                    Usuarios.role = "user";
                    Usuarios.status = "false";
                    db.USUARIOS.Add(Usuarios);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Feito com êxito!" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Email já existe" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost, Route("login")]
        public HttpResponseMessage Login([FromBody] USUARIOS usuario)
        {
            try
            {
                USUARIOS userObj = db.USUARIOS
                    .FirstOrDefault(u => u.email == usuario.email && u.password == usuario.password);

                if (userObj != null)
                {
                    if (userObj.status == "true")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { token = TokenManagement.GenerateToken(userObj.email, userObj.role) });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Aguardando aprovação do Administrador" });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Usuário ou senha incorretos" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost,Route("checkToken")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage checkToken()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "true" });
        }

        [HttpGet,Route("getAllUser")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllUser()
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaims tokenClaims = TokenManagement.ValidateToken(token);
                if(tokenClaims.role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                }
                var result = db.USUARIOS
                    .Select(u => new { u.id, u.nome, u.numeroContato, u.email, u.status, u.role })
                    .Where(x => (x.role == "user"))
                    .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

    }
}
