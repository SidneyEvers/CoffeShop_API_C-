using Sistema_Gerenciamento_Cafeteria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sistema_Gerenciamento_Cafeteria.Controllers
{
    [RoutePrefix("api/categoria")]
    public class CategoriaController : ApiController
    {
        CafeEntities db = new CafeEntities();
        Response response = new Response();

        [HttpPost,Route("addNewCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewCategory([FromBody] Categoria categoria)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaims tokenClaims = TokenManagement.ValidateToken(token);
                if(tokenClaims.role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                db.Categoria.Add(categoria);
                db.SaveChanges();
                response.Message = "Categoria adicionada com sucesso";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet,Route("getAllCategory")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage GetAllCategory()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, db.Categoria.ToList());
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}
