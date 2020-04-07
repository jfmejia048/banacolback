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
            var Pomas = (from Poma in _dataContext.Pomas
                         join Finca in _dataContext.Fincas on Poma.ID equals Finca.Pomas.ID
                         join Fruta in _dataContext.Frutas on Poma.ID equals Fruta.Poma.ID
                         join Palets in _dataContext.Palets on Fruta.ID equals Palets.Fruta.ID
                         join Puerto in _dataContext.Puertos on Palets.Puerto.ID equals Puerto.ID
                         join Buque in _dataContext.Buques on Palets.Buque.ID equals Buque.ID
                         join Exportador in _dataContext.Exportadores on Palets.Exportador.ID equals Exportador.ID
                         join Destino in _dataContext.Destinos on Palets.Destino.ID equals Destino.ID
                         where Poma.FechaRegistro.ToLocalTime() >= StartDate && Poma.FechaRegistro.ToLocalTime() <= EndDate
                         select new
                         {
                             IdPoma = Poma.ID,
                             CodigoPalet = Palets.Codigo,
                             Finca = Finca.FincaName,
                             TerminalDestino = Puerto.PuertoName,
                             Poma = Poma.Numero,
                             EstadoPoma = EnumHelper.GetEnumDescription((EstadosPoma)Poma.Estado),
                             FechaCreacion = Poma.FechaRegistro.ToLocalTime(),
                             Fruta,
                             Buque = Buque.BuqueName,
                             Llegada = Palets.LlegadaTerminal,
                             Salida = Palets.SalidaFinca,
                             Palets.Estimado,
                             Palets.LlegadaTerminal,
                             Cajas = Poma.Recibido,
                             Exportador = Exportador.ExportadorName,
                             Destino = Destino.DestinoName,
                             Palets.Carga,
                             CodigoDeBarras = Palets.CodigoPalet,
                             idPallet = Palets.ID,
                             CajasPalet = Palets.NumeroCajas,
                             Palets.Perfilar
                         }).OrderBy(x => x.FechaCreacion).ToList();

            List<TransportGuidesDTO> List = Pomas.GroupBy(x => new { x.Finca, x.TerminalDestino, x.Poma, x.FechaCreacion })
                .Select(x => new TransportGuidesDTO
                {
                    IdPoma = x.FirstOrDefault().IdPoma,
                    Finca = x.FirstOrDefault().Finca,
                    TerminalDestino = x.FirstOrDefault().TerminalDestino,
                    Poma = x.FirstOrDefault().Poma,
                    FechaCreacion = x.FirstOrDefault().FechaCreacion.ToLocalTime(),
                    EstadoPoma = x.FirstOrDefault().EstadoPoma,
                    Frutas = x.GroupBy(g2 => g2.Fruta.Codigo).Select(s2 => new TransportFruitsDTO
                    {
                        Fruta = s2.FirstOrDefault().Fruta.FrutaName,
                        Buque = s2.FirstOrDefault().Buque,
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
            var Pomas = (from Poma in _dataContext.Pomas
                         join Finca in _dataContext.Fincas on Poma.ID equals Finca.Pomas.ID
                         join Fruta in _dataContext.Frutas on Poma.ID equals Fruta.Poma.ID
                         join Palets in _dataContext.Palets on Fruta.ID equals Palets.Fruta.ID
                         join Puerto in _dataContext.Puertos on Palets.Puerto.ID equals Puerto.ID
                         join Buque in _dataContext.Buques on Palets.Buque.ID equals Buque.ID
                         join Exportador in _dataContext.Exportadores on Palets.Exportador.ID equals Exportador.ID
                         join Destino in _dataContext.Destinos on Palets.Destino.ID equals Destino.ID
                         select new
                         {
                             IdPoma = Poma.ID,
                             CodigoPalet = Palets.Codigo,
                             Finca = Finca.FincaName,
                             TerminalDestino = Puerto.PuertoName,
                             Poma = Poma.Numero,
                             EstadoPoma = EnumHelper.GetEnumDescription((EstadosPoma)Poma.Estado),
                             FechaCreacion = Poma.FechaRegistro.ToLocalTime(),
                             Fruta = Fruta.FrutaName,
                             Buque = Buque.BuqueName,
                             Llegada = Palets.LlegadaTerminal,
                             Salida = Palets.SalidaFinca,
                             Estimado = Palets.Estimado,
                             LlegadaTerminal = Palets.LlegadaTerminal,
                             Cajas = Poma.Recibido,
                             Exportador = Exportador.ExportadorName,
                             Destino = Destino.DestinoName,
                             Carga = Palets.Carga,
                             CodigoDeBarras = Palets.CodigoPalet,
                             idPallet = Palets.ID,
                             CajasPalet = Palets.NumeroCajas,
                             Palets.Perfilar
                         }).OrderBy(x => x.FechaCreacion).ToList();

            List<TransportGuidesDTO> List = Pomas.GroupBy(x => new { x.Finca, x.TerminalDestino, x.Poma, x.FechaCreacion })
                .Select(x => new TransportGuidesDTO
                {
                    IdPoma = x.FirstOrDefault().IdPoma,
                    Finca = x.FirstOrDefault().Finca,
                    TerminalDestino = x.FirstOrDefault().TerminalDestino,
                    Poma = x.FirstOrDefault().Poma,
                    FechaCreacion = x.FirstOrDefault().FechaCreacion.ToLocalTime(),
                    EstadoPoma = x.FirstOrDefault().EstadoPoma,
                    Frutas = x.GroupBy(g2 => g2.Fruta).Select(s2 => new TransportFruitsDTO
                    {
                        Fruta = s2.FirstOrDefault().Fruta,
                        Buque = s2.FirstOrDefault().Buque,
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
