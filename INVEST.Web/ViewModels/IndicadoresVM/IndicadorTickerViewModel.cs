namespace INVEST.Web.ViewModels.IndicadoresVM
{
    public class IndicadorTickerViewModel
    {
        public string Ticker { get; set; } = string.Empty;
        public string Setor { get; set; } = string.Empty;
        public int Nota { get; set; }

        // Em vez de decimal puro, agora cada indicador é um objeto avaliado
        public IndicadorAvaliadoVM DividendYield { get; set; } = new();
        public IndicadorAvaliadoVM MargemLiquida { get; set; } = new();
        public IndicadorAvaliadoVM Ebitda { get; set; } = new();
        public IndicadorAvaliadoVM Roe { get; set; } = new();

        // Atualize este método para apontar para as novas classes vibrantes
        public static string ObterClasseCor(int nivelQualidadeId)
        {
            return nivelQualidadeId switch
            {
                5 => "bg-excelente",
                4 => "bg-bom",
                3 => "bg-atencao",
                2 => "bg-alerta",
                1 => "bg-critico",
                _ => "bg-neutro"
            };
        }

        // ADICIONE este novo método para a coluna de Notas
        public static string ObterClasseCorNota(int nota)
        {
            return nota switch
            {
                10 => "nota-10",
                9 => "nota-9",
                8 => "nota-8",
                7 => "nota-7",
                >= 5 => "nota-7", // De 5 a 7 mantém uma cor neutra
                _ => "nota-ruim"  // Abaixo de 5, alerta vermelho
            };
        }
    }

    // Estrutura para carregar o dado e sua avaliação
    public class IndicadorAvaliadoVM
    {
        public decimal Valor { get; set; }

        // Recebe o NivelQualidadeId (1 a 5) do seu banco de dados
        public int NivelQualidadeId { get; set; }
    }
}