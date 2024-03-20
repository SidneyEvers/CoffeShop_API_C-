using Sistema_Gerenciamento_Cafeteria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sistema_Gerenciamento_Cafeteria.Controllers
{
    [RoutePrefix("api/Produto")]
    public class ProdutoController : ApiController
    {
        CafeEntities db = new CafeEntities();
        Response response = new Response();

        [HttpPost,Route("addNewProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewProduct([FromBody] Produtos produto)
        {
            
            
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaims tokenClaims = TokenManagement.ValidateToken(token);

                if (tokenClaims.role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                produto.status = "true";
                db.Produtos.Add(produto);
                db.SaveChanges();
                response.Message = "Produto adicionado com sucesso";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            
        }

        [HttpGet,Route("getAllProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllProduct()
        {
            try
            {
                var result = from Produto in db.Produtos
                             join Categoria in db.Categoria
                             on Produto.categoriaId equals Categoria.id
                             select new
                             {
                                 Produto.id,
                                 Produto.nome,
                                 Produto.descricao,
                                 Produto.preco,
                                 Produto.status,
                                 categoriaId = Categoria.id,
                                 categoriaNome = Categoria.name
                             };
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet,Route("getProductByCategory/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetProductByCategory(int id) 
        {
            try
            {
                var result = db.Produtos
                    .Where(x => x.categoriaId == id && x.status == "true")
                    .Select(x => new { x.id, x.nome }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet,Route("getProductById/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetProductById(int id)
        {
            try
            {
                Produtos productObj = db.Produtos.Find(id);
                return Request.CreateResponse(HttpStatusCode.OK, productObj);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            
        }

        [HttpPost,Route("updateProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpDateProdut([FromBody] Produtos produto)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaims tokenClaims = TokenManagement.ValidateToken(token);

                if (tokenClaims.role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                Produtos produtoObj = db.Produtos.Find(produto.id);
                if(produtoObj == null)
                {
                    response.Message = "Produto não existe";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                produtoObj.nome = produto.nome;
                produtoObj.categoriaId = produto.categoriaId;
                produtoObj.descricao = produto.descricao;
                produtoObj.preco = produto.preco;
                db.Entry(produtoObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.Message = "Produto alterado com sucesso";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex) 
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost,Route("deleteProduct/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteProduct(int id)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaims tokenClaims = TokenManagement.ValidateToken(token);

                if (tokenClaims.role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                Produtos produtoObj = db.Produtos.Find(id);
                if(produtoObj == null)
                {
                    response.Message = "Produto não encontrado";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                db.Produtos.Remove(produtoObj);
                db.SaveChanges();
                response.Message = "Produto deletado com sucesso";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost,Route("updateProductStatus")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateProductStatus([FromBody] Produtos produto)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaims tokenClaims = TokenManagement.ValidateToken(token);

                if (tokenClaims.role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                Produtos produtoObj = db.Produtos.Find(produto.id);
                if(produtoObj == null)
                {
                    response.Message = "Produto não encontrado";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                produtoObj.status = produto.status;
                db.Entry(produtoObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.Message = "Stauts do produto alterado com sucesso";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}
