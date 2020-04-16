using Common.DTO;
using PerfilacionDeCalidad.Backend.Data;
using PerfilacionDeCalidad.Backend.Enum;
using PerfilacionDeCalidad.Backend.Helpers;
using PerfilacionDeCalidad.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Logic
{
    public class PomasLogic
    {
        private readonly DataContext _dataContext;

        public PomasLogic(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<TransportGuidesDTO> GetData()
        {
            var currentDate = DateTime.UtcNow.ToLocalTime();
            var StartDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 3, 0, 0);
            var EndDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day + 1, 2, 59, 0);
            if (currentDate < StartDate)
            {
                StartDate = StartDate.AddDays(-1);
                EndDate = EndDate.AddDays(-1);
            }
            var Pomas = (from TransportGuides in _dataContext.TransportGuides
                         join DetailTransportGuide in _dataContext.DetailTransportGuide on TransportGuides.ID equals DetailTransportGuide.TransportGuide.ID
                         join Pallets in _dataContext.Palets on DetailTransportGuide.ID equals Pallets.DetailTransportGuide.ID
                         where TransportGuides.FechaRegistro.ToLocalTime() >= StartDate && TransportGuides.FechaRegistro.ToLocalTime() <= EndDate
                         select new
                         {
                             IdTransportGuide = TransportGuides.ID,
                             IdPoma = TransportGuides.Poma.ID,
                             CodigoPalet = Pallets.Codigo,
                             Finca = TransportGuides.Finca.FincaName,
                             TerminalDestino = TransportGuides.Puerto.PuertoName,
                             Poma = TransportGuides.Poma.Numero,
                             EstadoPoma = EnumHelper.GetEnumDescription((EstadosPoma)TransportGuides.Estado),
                             FechaCreacion = TransportGuides.FechaRegistro.ToLocalTime(),
                             Fruta = DetailTransportGuide.Fruta.FrutaName,
                             Buque = DetailTransportGuide.Buque.BuqueName,
                             Llegada = TransportGuides.LlegadaTerminal,
                             Salida = TransportGuides.SalidaFinca,
                             Estimado = TransportGuides.Estimado,
                             LlegadaTerminal = TransportGuides.LlegadaTerminal,
                             Cajas = TransportGuides.Recibido,
                             Exportador = DetailTransportGuide.Exportador.ExportadorName,
                             Destino = DetailTransportGuide.Destino.DestinoName,
                             Carga = Pallets.Carga,
                             CodigoDeBarras = Pallets.CodigoPalet,
                             idPallet = Pallets.ID,
                             CajasPalet = Pallets.NumeroCajas,
                             Pallets.Perfilar
                         }).OrderBy(x => x.FechaCreacion).ToList();

            List<TransportGuidesDTO> List = Pomas.GroupBy(x => new { x.Finca, x.TerminalDestino, x.Poma, x.FechaCreacion })
                .Select(x => new TransportGuidesDTO
                {
                    IdTransportGuide = x.FirstOrDefault().IdTransportGuide,
                    IdPoma = x.FirstOrDefault().IdPoma,
                    Finca = x.FirstOrDefault().Finca,
                    TerminalDestino = x.FirstOrDefault().TerminalDestino,
                    Poma = x.FirstOrDefault().Poma,
                    FechaCreacion = x.FirstOrDefault().FechaCreacion.ToLocalTime(),
                    Llegada = x.FirstOrDefault().Llegada,
                    Salida = x.FirstOrDefault().Salida,
                    Estimado = x.FirstOrDefault().Estimado,
                    LlegadaTerminal = x.FirstOrDefault().LlegadaTerminal,
                    EstadoPoma = x.FirstOrDefault().EstadoPoma,
                    Frutas = x.GroupBy(g2 => g2.Fruta).Select(s2 => new TransportFruitsDTO
                    {
                        Fruta = s2.FirstOrDefault().Fruta,
                        Buque = s2.FirstOrDefault().Buque,
                        FechaCreacion = x.FirstOrDefault().FechaCreacion.ToLocalTime(),
                        Llegada = s2.FirstOrDefault().Llegada,
                        Salida = s2.FirstOrDefault().Salida,
                        Estimado = s2.FirstOrDefault().Estimado,
                        LlegadaTerminal = s2.FirstOrDefault().LlegadaTerminal,
                        Cajas = s2.FirstOrDefault().Cajas,
                        Exportador = s2.FirstOrDefault().Exportador,
                        Destino = s2.FirstOrDefault().Destino,
                        Pallets = s2.Select(s3 => new TransportPalletsDTO
                        {
                            IdPallet = s3.idPallet,
                            CodigoPalet = s3.CodigoPalet,
                            Carga = s3.Carga,
                            CodigoDeBarras = s3.CodigoDeBarras,
                            CajasPalet = s3.CajasPalet,
                            Perfilar = s3.Perfilar
                        }).ToList()
                    }).ToList()
                }).ToList();

            return List;
        }

        public List<TransportGuidesDTO> GetData(int tipoEsportar)
        {
            var Pomas = (from TransportGuides in _dataContext.TransportGuides
                         join DetailTransportGuide in _dataContext.DetailTransportGuide on TransportGuides.ID equals DetailTransportGuide.TransportGuide.ID
                         join Pallets in _dataContext.Palets on DetailTransportGuide.ID equals Pallets.DetailTransportGuide.ID
                         select new
                         {
                             IdTransportGuide = TransportGuides.ID,
                             IdPoma = TransportGuides.Poma.ID,
                             CodigoPalet = Pallets.Codigo,
                             Finca = TransportGuides.Finca.FincaName,
                             TerminalDestino = TransportGuides.Puerto.PuertoName,
                             Poma = TransportGuides.Poma.Numero,
                             EstadoPoma = EnumHelper.GetEnumDescription((EstadosPoma)TransportGuides.Estado),
                             FechaCreacion = TransportGuides.FechaRegistro.ToLocalTime(),
                             Fruta = DetailTransportGuide.Fruta.FrutaName,
                             Buque = DetailTransportGuide.Buque.BuqueName,
                             Llegada = TransportGuides.LlegadaTerminal,
                             Salida = TransportGuides.SalidaFinca,
                             Estimado = TransportGuides.Estimado,
                             LlegadaTerminal = TransportGuides.LlegadaTerminal,
                             Cajas = TransportGuides.Recibido,
                             Exportador = DetailTransportGuide.Exportador.ExportadorName,
                             Destino = DetailTransportGuide.Destino.DestinoName,
                             Carga = Pallets.Carga,
                             CodigoDeBarras = Pallets.CodigoPalet,
                             idPallet = Pallets.ID,
                             CajasPalet = Pallets.NumeroCajas,
                             Pallets.Perfilar
                         }).OrderBy(x => x.FechaCreacion).ToList();

            List<TransportGuidesDTO> List = Pomas.GroupBy(x => new { x.Finca, x.TerminalDestino, x.Poma, x.FechaCreacion })
                .Select(x => new TransportGuidesDTO
                {
                    IdTransportGuide = x.FirstOrDefault().IdTransportGuide,
                    IdPoma = x.FirstOrDefault().IdPoma,
                    Finca = x.FirstOrDefault().Finca,
                    TerminalDestino = x.FirstOrDefault().TerminalDestino,
                    Poma = x.FirstOrDefault().Poma,
                    FechaCreacion = x.FirstOrDefault().FechaCreacion.ToLocalTime(),
                    Llegada = x.FirstOrDefault().Llegada,
                    Salida = x.FirstOrDefault().Salida,
                    Estimado = x.FirstOrDefault().Estimado,
                    LlegadaTerminal = x.FirstOrDefault().LlegadaTerminal,
                    EstadoPoma = x.FirstOrDefault().EstadoPoma,
                    Frutas = x.GroupBy(g2 => g2.Fruta).Select(s2 => new TransportFruitsDTO
                    {
                        Fruta = s2.FirstOrDefault().Fruta,
                        Buque = s2.FirstOrDefault().Buque,
                        FechaCreacion = x.FirstOrDefault().FechaCreacion.ToLocalTime(),
                        Llegada = s2.FirstOrDefault().Llegada,
                        Salida = s2.FirstOrDefault().Salida,
                        Estimado = s2.FirstOrDefault().Estimado,
                        LlegadaTerminal = s2.FirstOrDefault().LlegadaTerminal,
                        Cajas = s2.FirstOrDefault().Cajas,
                        Exportador = s2.FirstOrDefault().Exportador,
                        Destino = s2.FirstOrDefault().Destino,
                        Pallets = s2.Select(s3 => new TransportPalletsDTO
                        {
                            IdPallet = s3.idPallet,
                            CodigoPalet = s3.CodigoPalet,
                            Carga = s3.Carga,
                            CodigoDeBarras = s3.CodigoDeBarras,
                            CajasPalet = s3.CajasPalet,
                            Perfilar = s3.Perfilar
                        }).ToList()
                    }).ToList()
                }).ToList();

            if(tipoEsportar == 1)
            {
                List = List.Where(x => x.EstadoPoma == EnumHelper.GetEnumDescription(EstadosPoma.Perfilado)).ToList();
            }
            if (tipoEsportar == 2)
            {
                List = List.Where(x => x.EstadoPoma == EnumHelper.GetEnumDescription(EstadosPoma.Chequeado)).ToList();
            }
            if (tipoEsportar == 3)
            {
                List = List.Where(x => x.EstadoPoma == EnumHelper.GetEnumDescription(EstadosPoma.NoChequeado)).ToList();
            }

            return List;
        }

        public List<TransportGuidesDTO> GetFilter(List<TransportGuidesDTO> List, Parameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.Finca))
            {
                List = List.Where(x => x.Finca.ToLower().Contains(parameters.Finca.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(parameters.Puerto))
            {
                List = List.Where(x => x.TerminalDestino.ToLower().Contains(parameters.Puerto.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(parameters.Buque))
            {
                List = List.Where(x => x.Frutas.Any(w => w.Buque.ToLower().Contains(parameters.Buque.ToLower()))).ToList();
            }

            if (!string.IsNullOrEmpty(parameters.Destino))
            {
                List = List.Where(x => x.Frutas.Any(w => w.Destino.ToLower().Contains(parameters.Destino.ToLower()))).ToList();
            }

            if (!string.IsNullOrEmpty(parameters.Exportador))
            {
                List = List.Where(x => x.Frutas.Any(w => w.Exportador.ToLower().Contains(parameters.Exportador.ToLower()))).ToList();
            }

            if (!string.IsNullOrEmpty(parameters.Fruta))
            {
                List = List.Where(x => x.Frutas.Any(w => w.Fruta.ToLower().Contains(parameters.Fruta.ToLower()))).ToList();
            }

            if (!string.IsNullOrEmpty(parameters.Poma))
            {
                List = List.Where(x => x.Poma.ToString().ToLower().Contains(parameters.Poma.ToLower())).ToList();
            }

            if (parameters.RangoFechas != null && parameters.RangoFechas[0] != null && parameters.RangoFechas[0] != null)
            {
                List = List.Where(x => x.FechaCreacion >= parameters.RangoFechas[0].ToLocalTime() && x.FechaCreacion <= parameters.RangoFechas[1].ToLocalTime()).ToList();
            }

            return List;
        }
    }
}
