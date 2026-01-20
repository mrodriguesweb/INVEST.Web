using FluentAssertions;
using INVEST.Domain.Entities;

namespace INVEST.Tests.Domain
{
    public class AcaoTests
    {
        [Fact]
        public void ReplaceTickers_Should_Normalize_And_Deduplicate()
        {
            var acao = new Acao("Empresa X", 2020, false, setorId: 1);
            acao.ReplaceTickers(new[] { "vale3", " VALE3 ", "petr4" });

            acao.Tickers.Select(t => t.Name)
                .Should()
                .BeEquivalentTo(new[] { "VALE3", "PETR4" });
        }

        [Fact]
        public void ReplaceTickers_Should_Throw_When_NoTickers_Provided()
        {

            var acao = new Acao("Empresa X", 2020, false, setorId: 1);

            Action act = () => acao.ReplaceTickers(null);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("rawTickers");
        }

        [Fact]
        public void ReplaceTickers_Should_Throw_When_Tickers_Are_Empty()
        {

            var acao = new Acao("Empresa X", 2020, false, setorId: 1);

            Action act = () => acao.ReplaceTickers(["", ""]);

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EditarDados_Should_Not_Change_Name()
        {
            var acao = new Acao("Empresa X", 2020, false, setorId: 1);

            acao.EditarDados(anoEntrada: 2021, estatal: true, setorId: 2);

            acao.Name.Should().Be("Empresa X");
        }
    }
}
