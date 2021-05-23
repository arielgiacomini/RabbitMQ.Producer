using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ.Producer.Core.Query
{
    public static class LeituraPlanilhaExcelQuery
    {
        public static IList<ImportacaoRepescagemAtivaViewModel> GetLinesExcel(string pathFileExcel)
        {
            IList<ImportacaoRepescagemAtivaViewModel> nomeAndIdade = new List<ImportacaoRepescagemAtivaViewModel>();

            var xls = new XLWorkbook(pathFileExcel);

            var planilhas = xls.Worksheets.ToList();

            foreach (var planilha in planilhas)
            {
                var totalLinhas = planilha.Rows().Count();

                var dataVigencia = TranslateStringForDate(planilha.Name).DataFimVigencia.AddDays(10);

                for (int i = 2; i <= totalLinhas; i++)
                {
                    nomeAndIdade.Add(new ImportacaoRepescagemAtivaViewModel
                    {
                        Id = i,
                        CpfDeRepescagem = planilha.Cell($"A{i}").Value.ToString(),
                        DataFimVigencia = dataVigencia
                    });
                }
            }

            return nomeAndIdade;
        }

        private static ImportacaoRepescagemAtivaViewModel TranslateStringForDate(string nameAbaPlanilha)
        {
            ImportacaoRepescagemAtivaViewModel importacaoRepescagemAtivaViewModel = new ImportacaoRepescagemAtivaViewModel();

            var day = nameAbaPlanilha.Substring(0, 2);
            var month = nameAbaPlanilha.Substring(3, 2);

            bool inicioZeroDay = false;
            bool inicioZeroMonth = false;

            if (day.StartsWith("0"))
            {
                inicioZeroDay = true;
            }

            if (inicioZeroDay)
            {
                day = day.Remove(0, 1);
            }

            if (month.StartsWith("0"))
            {
                inicioZeroMonth = true;
            }

            if (inicioZeroMonth)
            {
                month = month.Remove(0, 1);
            }

            importacaoRepescagemAtivaViewModel.DataFimVigencia = new DateTime(year: DateTime.Now.Year, month: Convert.ToInt32(month), day: Convert.ToInt32(day));

            return importacaoRepescagemAtivaViewModel;
        }

        public class ImportacaoRepescagemAtivaViewModel
        {
            public int Id { get; set; }
            public string CpfDeRepescagem { get; set; }
            public DateTime DataFimVigencia { get; set; }
        }
    }
}