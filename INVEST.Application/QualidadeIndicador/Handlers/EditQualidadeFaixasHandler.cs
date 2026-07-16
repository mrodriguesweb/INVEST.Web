using INVEST.Application.QualidadeIndicador.DTOs;
using INVEST.Application.QualidadeIndicador.Repository;
using INVEST.Application.TipoIndicador.Queries;

namespace INVEST.Application.QualidadeIndicador.Handlers
{
    public sealed class EditQualidadeFaixasHandler(ITipoIndicadorQuery tipoIndicadorQuery, IQualidadeIndicadorRepository qualidadeIndicadorRepository)
    {

        public async Task<EditFaixasQualidadeResult> Handle(EditFaixasQualidadeCommand cmd)
        {

            var errors = new List<string>();

            if (cmd.Id <= 0) errors.Add("TipoIndicadorId inválido.");

            var rawFaixas = cmd.Faixas ?? new List<QualidadeIndicadorEditCommand>();

            if (rawFaixas.Count == 0) errors.Add("Informe ao menos 1 faixa válida.");

            if (errors.Count > 0)
                return EditFaixasQualidadeResult.Fail(errors.ToArray());

            if (!await tipoIndicadorQuery.Exists(cmd.Id))
                return EditFaixasQualidadeResult.Fail("Tipo de Indicador inválido.");

            // 1. Validação de Unicidade: Impede dois "Excelentes" ou dois "Críticos"
            var niveisDuplicados = rawFaixas
                .GroupBy(f => f.Id) // O Id aqui representa o NivelQualidadeId que veio da Controller
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .Any();

            if (niveisDuplicados)
                errors.Add("Não é permitido cadastrar mais de uma faixa para o mesmo nível de qualidade.");

            // 2. Validação de Sobreposição (Overlap) e Inconsistência
            // Ordenamos pelas faixas mínimas para garantir que uma não invada a outra
            var faixasOrdenadas = rawFaixas.OrderBy(f => f.ValorMinimo).ToList();

            for (int i = 0; i < faixasOrdenadas.Count; i++)
            {
                var faixa = faixasOrdenadas[i];

                if (faixa.ValorMinimo < 0)
                {
                    errors.Add($"Valor mínimo da faixa {faixa.Id} não pode ser negativo.");
                    continue;
                }

                if (faixa.ValorMinimo >= faixa.ValorMaximo)
                {
                    errors.Add($"A faixa associada ao nível {faixa.Id} tem o valor mínimo maior ou igual ao máximo.");
                    continue;
                }

                // Verifica se o máximo da faixa atual invade o mínimo da PRÓXIMA faixa
                if (i < faixasOrdenadas.Count - 1)
                {
                    var proximaFaixa = faixasOrdenadas[i + 1];
                    if (faixa.ValorMaximo > proximaFaixa.ValorMinimo)
                    {
                        errors.Add($"Existe uma sobreposição de valores entre as faixas.");
                    }
                }
            }

            if (errors.Count > 0)
                return EditFaixasQualidadeResult.Fail(errors.ToArray());

            // 3. Montagem das Entidades de Domínio
            var indicadoresParaSalvar = faixasOrdenadas.Select(faixa =>
                new INVEST.Domain.Entities.QualidadeIndicador(
                    cmd.Id,
                    faixa.Id, // NivelQualidadeId
                    faixa.ValorMinimo,
                    faixa.ValorMaximo
                )).ToList();

            // 4. Execução do padrão "Clear and Replace"
            // Removemos as faixas antigas antes de inserir as novas
            await qualidadeIndicadorRepository.DeleteByTipoIndicadorIdAsync(cmd.Id);

            await qualidadeIndicadorRepository.AddRange(indicadoresParaSalvar);
            await qualidadeIndicadorRepository.SaveChanges();

            return EditFaixasQualidadeResult.Ok(cmd.Id);

        }
    }
}