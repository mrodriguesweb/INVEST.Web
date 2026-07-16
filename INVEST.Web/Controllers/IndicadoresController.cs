using INVEST.Application.Indicadores.Repository;
using INVEST.Application.TipoIndicador.Repository; // Adicionado
using INVEST.Application.TipoIndicador.DTOs; // Adicionado para o DTO de faixas
using INVEST.Web.ViewModels.IndicadoresVM;
using Microsoft.AspNetCore.Mvc;
using INVEST.Domain.Enums;

namespace INVEST.Web.Controllers
{
    public class IndicadoresController : Controller
    {
        private readonly IIndicadorRepository _indicadorRepository;
        private readonly ITipoIndicadorRepository _tipoIndicadorRepository;

        // Injetamos os dois repositórios
        public IndicadoresController(IIndicadorRepository indicadorRepository, ITipoIndicadorRepository tipoIndicadorRepository)
        {
            _indicadorRepository = indicadorRepository;
            _tipoIndicadorRepository = tipoIndicadorRepository;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            // 1. Busca os valores extraídos pelo seu Worker (O "Radar")
            var indicadores = await _indicadorRepository.ListIndicadoresWithTickers(ct);

            // 2. Busca os parâmetros de qualidade dinâmicos que você configurou no banco
            var parametrosQualidade = await _tipoIndicadorRepository.ListWithQualityParameters(ct);

            // 3. Mapeamento desatando o nó e avaliando a qualidade de cada um
            var viewModel = indicadores.Select(dto =>
            {
                // 1. Calculamos o nível de qualidade isoladamente primeiro
                var nivelMargem = AvaliarQualidade(dto.HistoricoIndicadores.MargemLiquida, parametrosQualidade, (byte)TipoIndicadorEnum.MargemLiquida);
                var nivelEbitda = AvaliarQualidade(dto.HistoricoIndicadores.Ebitda, parametrosQualidade, (byte)TipoIndicadorEnum.Ebitda);
                var nivelRoe = AvaliarQualidade(dto.HistoricoIndicadores.Roe, parametrosQualidade, (byte)TipoIndicadorEnum.Roe);

                // 2. Montamos a ViewModel injetando os valores já calculados
                return new IndicadorTickerViewModel
                {
                    Ticker = dto.Ticker,
                    Setor = dto.Setor,

                    MargemLiquida = new IndicadorAvaliadoVM
                    {
                        Valor = dto.HistoricoIndicadores.MargemLiquida,
                        NivelQualidadeId = nivelMargem
                    },
                    Ebitda = new IndicadorAvaliadoVM
                    {
                        Valor = dto.HistoricoIndicadores.Ebitda,
                        NivelQualidadeId = nivelEbitda
                    },
                    Roe = new IndicadorAvaliadoVM
                    {
                        Valor = dto.HistoricoIndicadores.Roe,
                        NivelQualidadeId = nivelRoe
                    },

                    // 3. O motor de notas recebe apenas os IDs calculados
                    Nota = CalcularNotaFinal(nivelMargem, nivelEbitda, nivelRoe)
                };
            }).ToList();

            return View(viewModel);
        }

        // Método auxiliar privado: O motor de regras que cruza o valor da Vale3 com as faixas do banco
        private int AvaliarQualidade(decimal valor, List<TipoIndicadorListItemDto> parametros, int idTipoIndicador)
        {
            // Encontra a configuração deste indicador específico
            var config = parametros.FirstOrDefault(p => p.Id == idTipoIndicador);

            // Se não houver configuração cadastrada no banco, retorna 0 (ficará sem cor na tela)
            if (config == null || !config.Qualidades.Any())
                return 0;

            // Procura a faixa exata onde o valor numérico se encaixa
            var faixaEncontrada = config.Qualidades
                .FirstOrDefault(q => valor >= q.ValorMinimo && valor <= q.ValorMaximo);

            // Retorna o NivelQualidadeId (1 a 5) ou 0 caso o valor fuja de todas as faixas
            return faixaEncontrada?.IdNivelQualidade ?? 0;
        }

        // Usamos "params int[]" para que você possa passar os 10 ou 15 indicadores dinamicamente
        private int CalcularNotaFinal(params int[] niveisQualidade)
        {
            // Filtra apenas os indicadores que realmente foram avaliados (Nível > 0)
            var niveisValidos = niveisQualidade.Where(n => n > 0).ToList();

            // Se a empresa não tem NENHUM indicador avaliado, retorna 0 (ou outro valor padrão)
            if (!niveisValidos.Any())
                return 0;

            decimal somaPontos = 0m;

            foreach (var nivel in niveisValidos)
            {
                somaPontos += nivel switch
                {
                    5 => 10m, // Excelente
                    4 => 9m,  // Bom
                    3 => 7m,  // Atenção
                    2 => 3m,  // Alerta
                    1 => 0m,  // Crítico
                    _ => 0m
                };
            }

            // Calcula a média
            decimal media = somaPontos / niveisValidos.Count;

            // Arredonda para o inteiro mais próximo (ex: 9.6 vira 10, 8.4 vira 8)
            return (int)Math.Round(media, MidpointRounding.AwayFromZero);
        }

    }
}