using Sistema_Gerenciamento_Cafeteria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sistema_Gerenciamento_Cafeteria.Controllers
{
    [RoutePrefix("api/bill")]
    public class FaturaController : ApiController
    {
        CafeEntities db = new CafeEntities();
        Response response = new Response();
        private string pdfPath = @"C:\Users\Sidney\source\repos\Sistema-Gerenciamento-Cafeteria";

        [HttpPost,Route("generateReport")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GenerateReport([FromBody] Faturas fatura)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaims tokenClaims = TokenManagement.ValidateToken(token);

                var ticks = DateTime.Now.Ticks;
                var guid = Guid.NewGuid().ToString();
                var uniqueId = ticks.ToString() + '-' + guid;
                fatura.uuid = uniqueId;
                db.Faturas.Add(fatura);
                db.SaveChanges();
                Get(fatura);
                return Request.CreateResponse(HttpStatusCode.OK, new { uuid = fatura.uuid});



            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        private void Get(Faturas fatura)
        {
            PdfDocument pdf = null;

            try
            {
                dynamic productDetails = JsonConvert.DeserializeObject(fatura.detalhesProduto);
                var todayDate = "Data: " + Convert.ToDateTime(DateTime.Today).ToString("mm/dd/yyy");
                PdfWriter writer = new PdfWriter(pdfPath + fatura.uuid + ".pdf");
                pdf = new PdfDocument(writer);
                Document document = new Document(pdf);


                //Header
                Paragraph header = new Paragraph("Sistema de gerenciamento de café")
                    .SetBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(25);
                document.Add(header);


                //New Line
                Paragraph newLine = new Paragraph(new Text("\n"));

                //Line Separator
                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                //Customer details
                Paragraph costumerDetails = new Paragraph("Nome " + fatura.nome + "\nEmail: " + fatura.email + "\nTelefone: " + fatura.numeroContato + "\nMétodo de pagamento: " + fatura.metodoPagamento);
                document.Add(costumerDetails);

                //Table
                Table table = new Table(5, false);
                table.SetWidth(new UnitValue(UnitValue.PERCENT, 100));

                //Header 
                Cell headerName = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Nome"));


                Cell headerCategory = new Cell(1, 1)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .Add(new Paragraph("Categoria"));

                Cell headerQuantity = new Cell(1, 1)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .Add(new Paragraph("Quantidade"));

                Cell headerPrice = new Cell(1, 1)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .Add(new Paragraph("Preço"));


                Cell headerSubTotal = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBold()
               .Add(new Paragraph("Total"));


                table.AddCell(headerName);
                table.AddCell(headerCategory);
                table.AddCell(headerQuantity);
                table.AddCell(headerPrice);
                table.AddCell(headerSubTotal);


                foreach (JObject product in productDetails)
                {
                    Cell nameCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["nome"].ToString()));

                    Cell categoryCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["categoria"].ToString()));

                    Cell quantityCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["quantidade"].ToString()));

                    Cell priceCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["preço"].ToString()));

                    Cell subTotalCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["total"].ToString()));

                    table.AddCell(nameCell);
                    table.AddCell(categoryCell);
                    table.AddCell(quantityCell);
                    table.AddCell(priceCell);
                    table.AddCell(subTotalCell);
                }
                document.Add(table);
                Paragraph last = new Paragraph("Total: " + fatura.valorTotal + "\nObrigado pela visita, volte sempre!");
                document.Add(last);
                document.Close();

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (pdf != null)
                {
                    pdf.Close();
                }
            }
        }

        [HttpPost,Route("getPdf")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetPdf([FromBody] Faturas fatura)
        {
            try
            {
                if(fatura != null)
                {
                    Get(fatura);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

                string filePath = pdfPath + fatura.uuid.ToString() + ".pdf";

                byte[] bytes = File.ReadAllBytes(filePath);

                response.Content = new ByteArrayContent(bytes);

                response.Content.Headers.ContentLength = bytes.LongLength;

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fatura.uuid.ToString() + ".pdf";

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fatura.uuid.ToString() + ".pdf"));
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }
    }
}
