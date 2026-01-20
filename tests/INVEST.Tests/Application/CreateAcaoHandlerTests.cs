using FluentAssertions;
using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Handlers;
using INVEST.Application.Acoes.Repository;
using INVEST.Application.Setores.Queries;
using NSubstitute;

namespace INVEST.Tests.Application
{
    public class CreateAcaoHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Fail_When_Setor_Invalid()
        {
            var repo = Substitute.For<IAcaoRepository>();
            var setores = Substitute.For<ISetorQuery>();

            setores.Exists(Arg.Any<int>()).Returns(false);

            var handler = new CreateAcaoHandler(repo, setores);

            var cmd = new CreateAcaoCommand(
                Name: "Empresa X",
                AnoEntrada: 2020,
                Estatal: false,
                SetorId: 999,
                Tickers: new List<string> { "VALE3" }
            );

            var result = await handler.Handle(cmd);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message.Contains("Setor inválido."));
            await repo.DidNotReceive().SaveChanges();
        }

        [Fact]
        public async Task Handle_Should_Fail_When_NoTickers_Provided()
        {
            var repo = Substitute.For<IAcaoRepository>();
            var setores = Substitute.For<ISetorQuery>();

            var handler = new CreateAcaoHandler(repo, setores);

            var cmd = new CreateAcaoCommand(
                Name: "Empresa X",
                AnoEntrada: 2020,
                Estatal: false,
                SetorId: 999,
                Tickers: null
            );

            var result = await handler.Handle(cmd);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message.Contains("Informe ao menos 1 ticker."));
            await repo.DidNotReceive().SaveChanges();
        }

        [Fact]
        public async Task Handle_Should_Fail_When_Tickers_Are_Empty()
        {
            var repo = Substitute.For<IAcaoRepository>();
            var setores = Substitute.For<ISetorQuery>();

            var handler = new CreateAcaoHandler(repo, setores);

            var cmd = new CreateAcaoCommand(
                Name: "Empresa X",
                AnoEntrada: 2020,
                Estatal: false,
                SetorId: 999,
                Tickers: []
            );

            var result = await handler.Handle(cmd);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message.Contains("Informe ao menos 1 ticker."));
            await repo.DidNotReceive().SaveChanges();
        }

    }
}