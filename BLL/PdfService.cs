using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;



namespace BLL
{
    public class PdfService
    {
        /// <summary>
        /// Extrae texto de un PDF almacenado como byte[]
        /// </summary>
        public static string ExtraerTextoDePdf(byte[] pdfBytes)
        {
            try
            {
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return string.Empty;
                }

                using (var document = PdfDocument.Open(pdfBytes))
                {
                    var textoCompleto = new StringBuilder();

                    foreach (var pagina in document.GetPages())
                    {
                        textoCompleto.AppendLine(pagina.Text);
                    }

                    return textoCompleto.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al extraer texto del PDF: {ex.Message}");
            }
        }
    }

}
