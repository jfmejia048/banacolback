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
            var EndDate = StartDate.AddDays(1);
            EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 2, 59, 0);
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
                             IdPoma = TransportGuides.ID,
                             Finca = TransportGuides.Finca.FincaName,
                             TerminalDestino = TransportGuides.Destino.DestinoName,
                             Poma = TransportGuides.Numero,
                             Placa = TransportGuides.Vehiculo.Placa,
                             EstadoPoma = EnumHelper.GetEnumDescription((EstadosPoma)TransportGuides.Estado),
                             FechaCreacion = TransportGuides.FechaRegistro.ToLocalTime(),
                             Fruta = DetailTransportGuide.Fruta.FrutaName,
                             Buque = TransportGuides.Buque.BuqueName,
                             Llegada = TransportGuides.LlegadaCamion.ToLocalTime(),
                             Salida = TransportGuides.SalidaFinca.ToLocalTime(),
                             Estimado = TransportGuides.Estimado.ToLocalTime(),
                             LlegadaTerminal = TransportGuides.LlegadaTerminal,
                             Cajas = TransportGuides.Recibido,
                             Exportador = TransportGuides.Exportador.ExportadorName,
                             PuertoDestino = TransportGuides.Puerto.PuertoName,
                             Carga = Pallets.Carga,
                             CodigoDeBarras = Pallets.CodigoPalet,
                             idPallet = Pallets.ID,
                             CajasPalet = Pallets.NumeroCajas,
                             Pallets.Perfilar,
                             Tipo = Pallets.Tipo
                         }).OrderBy(x => x.FechaCreacion).ToList();

            List<TransportGuidesDTO> List = Pomas.GroupBy(x => new { x.Finca, x.TerminalDestino, x.Poma, x.FechaCreacion })
                .Select(x => new TransportGuidesDTO
                {
                    IdPoma = x.FirstOrDefault().IdPoma,
                    Finca = x.FirstOrDefault().Finca,
                    PuertoDestino = x.FirstOrDefault().PuertoDestino,
                    Poma = x.FirstOrDefault().Poma,
                    Placa = x.FirstOrDefault().Placa,
                    FechaCreacion = x.FirstOrDefault().FechaCreacion,
                    Llegada = x.FirstOrDefault().Llegada,
                    Salida = x.FirstOrDefault().Salida,
                    Estimado = x.FirstOrDefault().Estimado,
                    LlegadaTerminal = x.FirstOrDefault().LlegadaTerminal == null ? null : (DateTime?)x.FirstOrDefault().LlegadaTerminal.Value.ToLocalTime(),
                    EstadoPoma = x.FirstOrDefault().EstadoPoma,
                    TerminalDestino = x.FirstOrDefault().TerminalDestino,
                    Buque = x.FirstOrDefault().Buque,
                    Exportador = x.FirstOrDefault().Exportador,
                    Frutas = x.GroupBy(g2 => g2.idPallet).Select(s2 => new TransportFruitsDTO
                    {
                        IdPallet = s2.FirstOrDefault().idPallet,
                        CodigoDeBarras = s2.FirstOrDefault().CodigoDeBarras,
                        Fruta = s2.FirstOrDefault().Fruta,
                        CajasPalet = s2.FirstOrDefault().CajasPalet,
                        Carga = s2.FirstOrDefault().Carga,
                        Tipo = s2.FirstOrDefault().Tipo,
                        Perfilar = s2.FirstOrDefault().Perfilar,
                        Cajas = s2.FirstOrDefault().Cajas
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
                             IdPoma = TransportGuides.ID,
                             Finca = TransportGuides.Finca.FincaName,
                             TerminalDestino = TransportGuides.Destino.DestinoName,
                             Poma = TransportGuides.Numero,
                             Placa = TransportGuides.Vehiculo.Placa,
                             EstadoPoma = EnumHelper.GetEnumDescription((EstadosPoma)TransportGuides.Estado),
                             FechaCreacion = TransportGuides.FechaRegistro.ToLocalTime(),
                             Fruta = DetailTransportGuide.Fruta.FrutaName,
                             Buque = TransportGuides.Buque.BuqueName,
                             Llegada = TransportGuides.LlegadaCamion.ToLocalTime(),
                             Salida = TransportGuides.SalidaFinca.ToLocalTime(),
                             Estimado = TransportGuides.Estimado.ToLocalTime(),
                             LlegadaTerminal = TransportGuides.LlegadaTerminal,
                             Cajas = TransportGuides.Recibido,
                             Exportador = TransportGuides.Exportador.ExportadorName,
                             PuertoDestino = TransportGuides.Puerto.PuertoName,
                             Carga = Pallets.Carga,
                             CodigoDeBarras = Pallets.CodigoPalet,
                             idPallet = Pallets.ID,
                             CajasPalet = Pallets.NumeroCajas,
                             Pallets.Perfilar,
                             Tipo = Pallets.Tipo
                         }).OrderBy(x => x.FechaCreacion).ToList();

            List<TransportGuidesDTO> List = Pomas.GroupBy(x => new { x.Finca, x.TerminalDestino, x.Poma, x.FechaCreacion })
                .Select(x => new TransportGuidesDTO
                {
                    IdPoma = x.FirstOrDefault().IdPoma,
                    Finca = x.FirstOrDefault().Finca,
                    PuertoDestino = x.FirstOrDefault().PuertoDestino,
                    Poma = x.FirstOrDefault().Poma,
                    Placa = x.FirstOrDefault().Placa,
                    FechaCreacion = x.FirstOrDefault().FechaCreacion,
                    Llegada = x.FirstOrDefault().Llegada,
                    Salida = x.FirstOrDefault().Salida,
                    Estimado = x.FirstOrDefault().Estimado,
                    LlegadaTerminal = x.FirstOrDefault().LlegadaTerminal == null ? null : (DateTime?)x.FirstOrDefault().LlegadaTerminal.Value.ToLocalTime(),
                    EstadoPoma = x.FirstOrDefault().EstadoPoma,
                    TerminalDestino = x.FirstOrDefault().TerminalDestino,
                    Buque = x.FirstOrDefault().Buque,
                    Exportador = x.FirstOrDefault().Exportador,
                    Frutas = x.GroupBy(g2 => g2.idPallet).Select(s2 => new TransportFruitsDTO
                    {
                        IdPallet = s2.FirstOrDefault().idPallet,
                        CodigoDeBarras = s2.FirstOrDefault().CodigoDeBarras,
                        Fruta = s2.FirstOrDefault().Fruta,
                        CajasPalet = s2.FirstOrDefault().CajasPalet,
                        Carga = s2.FirstOrDefault().Carga,
                        Tipo = s2.FirstOrDefault().Tipo,
                        Perfilar = s2.FirstOrDefault().Perfilar,
                        Cajas = s2.FirstOrDefault().Cajas
                    }).ToList()
                }).ToList();

            if (tipoEsportar == 1)
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

        public List<TransportGuidesExcelDTO> GetDataExcel(int tipoEsportar)
        {
            var Pomas = (from TransportGuides in _dataContext.TransportGuides
                         join DetailTransportGuide in _dataContext.DetailTransportGuide on TransportGuides.ID equals DetailTransportGuide.TransportGuide.ID
                         join Pallets in _dataContext.Palets on DetailTransportGuide.ID equals Pallets.DetailTransportGuide.ID
                         select new
                         {
                             IdPoma = TransportGuides.ID,
                             Finca = TransportGuides.Finca.FincaName,
                             TerminalDestino = TransportGuides.Puerto.PuertoName,
                             Poma = TransportGuides.Numero,
                             EstadoPoma = EnumHelper.GetEnumDescription((EstadosPoma)TransportGuides.Estado),
                             FechaCreacion = TransportGuides.FechaRegistro.ToLocalTime(),
                             Fruta = DetailTransportGuide.Fruta.FrutaName,
                             Buque = TransportGuides.Buque.BuqueName,
                             Llegada = TransportGuides.LlegadaCamion.ToLocalTime(),
                             Salida = TransportGuides.SalidaFinca.ToLocalTime(),
                             Estimado = TransportGuides.Estimado.ToLocalTime(),
                             LlegadaTerminal = TransportGuides.LlegadaTerminal,
                             Cajas = TransportGuides.Recibido,
                             Exportador = TransportGuides.Exportador.ExportadorName,
                             Destino = TransportGuides.Destino.DestinoName,
                             Carga = Pallets.Carga,
                             CodigoDeBarras = Pallets.CodigoPalet,
                             idPallet = Pallets.ID,
                             CajasPalet = Pallets.NumeroCajas,
                             Pallets.Perfilar,
                             Pallets.UsuarioLectura,
                             LecturaPalet = Pallets.LecturaPalet.ToLocalTime(),
                             Pallets.UsuarioInspeccion,
                             InspeccionPalet = Pallets.InspeccionPalet.ToLocalTime()
                         }).OrderBy(x => x.FechaCreacion).ToList();

            List<TransportGuidesExcelDTO> List = Pomas.GroupBy(x => new { x.Finca, x.TerminalDestino, x.Poma, x.FechaCreacion })
                .Select(x => new TransportGuidesExcelDTO
                {
                    IdPoma = x.FirstOrDefault().IdPoma,
                    Finca = x.FirstOrDefault().Finca,
                    TerminalDestino = x.FirstOrDefault().TerminalDestino,
                    Poma = x.FirstOrDefault().Poma,
                    FechaCreacion = x.FirstOrDefault().FechaCreacion,
                    Llegada = x.FirstOrDefault().Llegada,
                    Salida = x.FirstOrDefault().Salida,
                    Estimado = x.FirstOrDefault().Estimado,
                    LlegadaTerminal = x.FirstOrDefault().LlegadaTerminal == null ? null : (DateTime?)x.FirstOrDefault().LlegadaTerminal.Value.ToLocalTime(),
                    EstadoPoma = x.FirstOrDefault().EstadoPoma,
                    Frutas = x.GroupBy(g2 => g2.Fruta).Select(s2 => new TransportFruitsExcelDTO
                    {
                        Fruta = s2.FirstOrDefault().Fruta,
                        Buque = x.FirstOrDefault().Buque,
                        FechaCreacion = x.FirstOrDefault().FechaCreacion,
                        Llegada = s2.FirstOrDefault().Llegada,
                        Salida = s2.FirstOrDefault().Salida,
                        Estimado = s2.FirstOrDefault().Estimado,
                        LlegadaTerminal = s2.FirstOrDefault().LlegadaTerminal == null ? null : (DateTime?)s2.FirstOrDefault().LlegadaTerminal.Value.ToLocalTime(),
                        Cajas = s2.FirstOrDefault().Cajas,
                        Exportador = x.FirstOrDefault().Exportador,
                        Destino = x.FirstOrDefault().Destino,
                        Pallets = s2.Select(s3 => new TransportPalletsDTO
                        {
                            IdPallet = s3.idPallet,
                            Carga = s3.Carga,
                            CodigoDeBarras = s3.CodigoDeBarras,
                            CajasPalet = s3.CajasPalet,
                            Perfilar = s3.Perfilar,
                            UsuarioLectura = s3.UsuarioLectura,
                            HoraLectura = s3.LecturaPalet,
                            UsuarioInspeccion = s3.UsuarioInspeccion,
                            HoraInspeccion = s3.InspeccionPalet,
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
                List = List.Where(x => x.PuertoDestino.ToLower().Contains(parameters.Puerto.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(parameters.Buque))
            {
                List = List.Where(x => x.Buque.ToLower().Contains(parameters.Buque.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(parameters.Destino))
            {
                List = List.Where(x => x.TerminalDestino.ToLower().Contains(parameters.Destino.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(parameters.Exportador))
            {
                List = List.Where(x => x.Exportador.ToLower().Contains(parameters.Exportador.ToLower())).ToList();
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

        public List<TransportGuidesExcelDTO> GetFilter(List<TransportGuidesExcelDTO> List, Parameters parameters)
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
                List = List.Where(x => x.TerminalDestino.ToLower().Contains(parameters.Destino.ToLower())).ToList();
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
