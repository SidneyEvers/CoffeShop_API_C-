using Sistema_Gerenciamento_Cafeteria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Sistema_Gerenciamento_Cafeteria.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        CafeEntities db = new CafeEntities();
        Response response = new Response();


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

        [HttpPost,Route("updateUserStatus")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage UpdateUserStatus(USUARIOS user)
        {
            try 
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaims tokenClaims = TokenManagement.ValidateToken(token);

                if(tokenClaims.role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                USUARIOS userObj = db.USUARIOS.Find(user.id);
                if(userObj == null)
                {
                    response.Message = "Usuario não encontrado";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                userObj.status = user.status;
                db.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.Message = "Usuario alterado com sucesso.";
                return Request.CreateResponse(HttpStatusCode.OK, response);


            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }


        [HttpPost,Route("changePassword")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage changePassword(ChangePassword changePassword)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaims tokenClaims = TokenManagement.ValidateToken(token);

                USUARIOS userObj = db.USUARIOS
                    .Where(x => (x.email == tokenClaims.email && x.password == changePassword.OldPassword)).FirstOrDefault();
                if(userObj != null)
                {
                    userObj.password = changePassword.NewPassword;
                    db.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    response.Message = "Senha atualizada com sucesso";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.Message = "Senha antiga incorreta";
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, response);

                }

            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        private string CreateEmailBody(string email, string password)
        {
            try
            {
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("/Template/forgot-password.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{email}", email);
                body = body.Replace("{password}", password);
                body = body.Replace("{frontendURL}", "http://localhost:4200/");
                return body;

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        [HttpPost,Route("forgotPassword")]
        public async Task<HttpResponseMessage> ForgotPassword([FromBody] USUARIOS user)
        {
            USUARIOS userObj = db.USUARIOS
                .Where(x => x.email == user.email).FirstOrDefault();
            response.Message = "Senha enviado para seu email.";
            if(userObj == null) 
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            var message = new MailMessage();
            message.To.Add(new MailAddress(user.email));
            message.Subject = "Senha do gerenciamento de café";
            message.Body = CreateEmailBody(user.email, userObj.password);
            message.IsBodyHtml = true;
            using(var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
                await Task.FromResult(0);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

    }
}
