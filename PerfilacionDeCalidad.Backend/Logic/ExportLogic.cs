using ClosedXML.Excel;
using Common.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PerfilacionDeCalidad.Backend.Data;
using PerfilacionDeCalidad.Backend.Enum;
using PerfilacionDeCalidad.Backend.Helpers;
using PerfilacionDeCalidad.Backend.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Logic
{
    public class ExportLogic
    {
        private readonly DataContext _dataContext;
        private readonly IHostingEnvironment _host;

        public ExportLogic(DataContext dataContext, IHostingEnvironment host)
        {
            _dataContext = dataContext;
            _host = host;
        }
        public MemoryStream ExportExcel(Parameters parameters)
        {
            string fileName = "ExportPallets.xlsx";
            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Merge Cells");

            ws = this.CreateHeader(ws);

            PomasLogic pomasLogic = new PomasLogic(this._dataContext);
            var result = pomasLogic.GetDataExcel(parameters.TipoExportar);
            result = pomasLogic.GetFilter(result, parameters);
            var PalletForFruits = result.Select(x => new { PalletsByFruits = x.Frutas.Select(x2 => x2.Pallets.Count).ToList() }).ToList();
            int index1 = 2;
            int index1ForFruits = 0;
            int index2 = 2;
            int index3 = 2;
            result.ForEach(x => {
                x.Frutas.ForEach(x2 => {
                    x2.Pallets.ForEach(x3 => {
                        ws.Cell("N" + index3.ToString()).Value = x3.Carga;
                        ws.Cell("O" + index3.ToString()).Value = "'" + x3.CodigoDeBarras;
                        ws.Cell("P" + index3.ToString()).Value = x3.CajasPalet;
                        ws.Cell("P" + index3.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("P" + index3.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell("Q" + index3.ToString()).Value = x3.Perfilar ? "SI" : "NO";
                        ws.Cell("Q" + index3.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("Q" + index3.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell("R" + index3.ToString()).Value = x3.UsuarioLectura;
                        ws.Cell("R" + index3.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("R" + index3.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell("S" + index3.ToString()).Value = string.IsNullOrEmpty(x3.UsuarioLectura) ? "" : x3.HoraLectura.ToString("yyyy-MM-dd HH:mm tt");
                        ws.Cell("S" + index3.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("S" + index3.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell("T" + index3.ToString()).Value = x3.Perfilar ? x3.UsuarioInspeccion : "N/A";
                        ws.Cell("T" + index3.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("T" + index3.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell("U" + index3.ToString()).Value = x3.Perfilar ? string.IsNullOrEmpty(x3.UsuarioInspeccion) ? "" : x3.HoraInspeccion.ToString("yyyy-MM-dd HH:mm tt") : "N/A";
                        ws.Cell("U" + index3.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("U" + index3.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        index3++;
                    });
                    ws.Cell("D" + index2.ToString()).Value = x2.Fruta;
                    ws.Cell("D" + index2.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("D" + index2.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range("D" + index2.ToString() + ":E" + (index2 + x2.Pallets.Count() - 1).ToString()).Column(1).Merge();
                    ws.Cell("E" + index2.ToString()).Value = x2.Buque;
                    ws.Cell("E" + index2.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("E" + index2.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range("E" + index2.ToString() + ":F" + (index2 + x2.Pallets.Count() - 1).ToString()).Column(1).Merge();
                    ws.Cell("F" + index2.ToString()).Value = "'" + x2.Llegada.ToString("HH:mm tt");
                    ws.Cell("F" + index2.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("F" + index2.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range("F" + index2.ToString() + ":G" + (index2 + x2.Pallets.Count() - 1).ToString()).Column(1).Merge();
                    ws.Cell("G" + index2.ToString()).Value = "'" + x2.Salida.ToString("HH:mm tt");
                    ws.Cell("G" + index2.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("G" + index2.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range("G" + index2.ToString() + ":H" + (index2 + x2.Pallets.Count() - 1).ToString()).Column(1).Merge();
                    ws.Cell("H" + index2.ToString()).Value = "'" + x2.Estimado.ToString("HH:mm tt");
                    ws.Cell("H" + index2.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("H" + index2.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range("H" + index2.ToString() + ":I" + (index2 + x2.Pallets.Count() - 1).ToString()).Column(1).Merge();
                    ws.Cell("I" + index2.ToString()).Value = "'" + (x2.LlegadaTerminal == null ? "" : ((DateTime)x2.LlegadaTerminal).ToString("HH:mm tt"));
                    ws.Cell("I" + index2.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("I" + index2.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range("I" + index2.ToString() + ":J" + (index2 + x2.Pallets.Count() - 1).ToString()).Column(1).Merge();
                    if (x2.LlegadaTerminal != null && x2.LlegadaTerminal > x2.Estimado)
                    {
                        ws.Range("I" + index2.ToString() + ":I" + (index2 + x2.Pallets.Count() - 1).ToString()).Style.Fill.BackgroundColor = XLColor.Red;
                    }
                    ws.Cell("J" + index2.ToString()).Value = !x2.Cajas ? "SI" : "NO";
                    ws.Cell("J" + index2.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("J" + index2.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range("J" + index2.ToString() + ":K" + (index2 + x2.Pallets.Count() - 1).ToString()).Column(1).Merge();
                    ws.Cell("K" + index2.ToString()).Value = x2.Cajas ? "SI" : "NO";
                    ws.Cell("K" + index2.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("K" + index2.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range("K" + index2.ToString() + ":L" + (index2 + x2.Pallets.Count() - 1).ToString()).Column(1).Merge();
                    ws.Cell("L" + index2.ToString()).Value = x2.Exportador;
                    ws.Cell("L" + index2.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("L" + index2.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range("l" + index2.ToString() + ":M" + (index2 + x2.Pallets.Count() - 1).ToString()).Column(1).Merge();
                    ws.Cell("M" + index2.ToString()).Value = x2.Destino;
                    ws.Cell("M" + index2.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("M" + index2.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Range("M" + index2.ToString() + ":N"+(index2 + x2.Pallets.Count() - 1).ToString()).Column(1).Merge();
                    index2 += x2.Pallets.Count;
                });
                ws.Cell("A" + index1.ToString()).Value = x.Finca;
                ws.Cell("A" + index1.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("A" + index1.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("A" + index1.ToString() + ":B" + (index1 + PalletForFruits[index1ForFruits].PalletsByFruits.Sum() - 1).ToString()).Column(1).Merge();
                ws.Cell("B" + index1.ToString()).Value = x.TerminalDestino;
                ws.Cell("B" + index1.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("B" + index1.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("B" + index1.ToString() + ":C" + (index1 + PalletForFruits[index1ForFruits].PalletsByFruits.Sum() - 1).ToString()).Column(1).Merge();
                ws.Cell("C" + index1.ToString()).Value = x.Poma;
                ws.Cell("C" + index1.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("C" + index1.ToString()).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Range("C" + index1.ToString() + ":D" + (index1 + PalletForFruits[index1ForFruits].PalletsByFruits.Sum() - 1).ToString()).Column(1).Merge();
                index1 += PalletForFruits[index1ForFruits].PalletsByFruits.Sum();
                index1ForFruits++;
            });

            var url = Environment.CurrentDirectory;
            workbook.SaveAs(fileName);

            string file = Path.Combine(url, fileName);
            var memory = new MemoryStream();

            using (var stream = new FileStream(file, FileMode.Open))
            {
                stream.CopyTo(memory);
            }

            memory.Position = 0;

            return memory;
        }

        public IXLWorksheet CreateHeader(IXLWorksheet ws)
        {
            ws.Range("A1:U1").Style.Fill.BackgroundColor = XLColor.DarkBlue;
            ws.Range("A1:U1").Rows().Style.Font.Bold = true;
            ws.Range("A1:U1").Rows().Style.Font.FontColor = XLColor.White;
            ws.Range("A1:U1").Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("A1:U1").Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Rows(1, 1).Height = 30;
            ws.Columns(1,1).Width = 12;
            ws.Columns(2,2).Width = 18;
            ws.Columns(3,3).Width = 10;
            ws.Columns(4,4).Width = 14;
            ws.Columns(5,5).Width = 14;
            ws.Columns(6,6).Width = 18;
            ws.Columns(7,7).Width = 16;
            ws.Columns(8,8).Width = 18;
            ws.Columns(9,9).Width = 18;
            ws.Columns(10,10).Width = 16;
            ws.Columns(11,11).Width = 16;
            ws.Columns(12,12).Width = 12;
            ws.Columns(13,13).Width = 12;
            ws.Columns(14,14).Width = 12;
            ws.Columns(15,15).Width = 22;
            ws.Columns(16,16).Width = 14;
            ws.Columns(17,17).Width = 12;
            ws.Columns(18,18).Width = 20;
            ws.Columns(19,19).Width = 20;
            ws.Columns(20,20).Width = 20;
            ws.Columns(21,21).Width = 20;
            ws.Cell("A1").Value = "Finca";
            ws.Cell("B1").Value = "Terminal Destino";
            ws.Cell("C1").Value = "Poma";
            ws.Cell("D1").Value = "Fruta";
            ws.Cell("E1").Value = "Buque";
            ws.Cell("F1").Value = "H. Llegada Camión";
            ws.Cell("G1").Value = "H. Salida Finca";
            ws.Cell("H1").Value = "H. Hora Estimada";
            ws.Cell("I1").Value = "H. Llegada Terminal";
            ws.Cell("J1").Value = "Transito (Cajas)";
            ws.Cell("K1").Value = "Recibidas (Cajas)";
            ws.Cell("L1").Value = "Exportador";
            ws.Cell("M1").Value = "Destino";
            ws.Cell("N1").Value = "Carga";
            ws.Cell("O1").Value = "Código de Barras";
            ws.Cell("P1").Value = "Cajas Pallet";
            ws.Cell("Q1").Value = "Perfilar";
            ws.Cell("R1").Value = "Usuario Chequeo";
            ws.Cell("S1").Value = "Hora Chequeo";
            ws.Cell("T1").Value = "Usuario Inspección";
            ws.Cell("U1").Value = "Hora Inspección";
            return ws;
        }
    }
}
