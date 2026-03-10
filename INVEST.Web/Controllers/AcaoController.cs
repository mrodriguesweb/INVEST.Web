using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Handlers;
using INVEST.Application.Acoes.Queries;
using INVEST.Application.Acoes.Services;
using INVEST.Application.Setores.Queries;
using INVEST.Web.Extensions;
using INVEST.Web.Mappers;
using INVEST.Web.ViewModels.AcoesVM;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace INVEST.Web.Controllers
{
    public class AcaoController(IAcaoQuery acaoQuery, ISetorQuery setorQuery, CreateAcaoHandler createAcaoHandler, EditAcaoHandler editAcaoHandler, DeleteAcaoHandler deleteAcaoHandler, IAcaoService acaoService, AtualizarCotacoesHandler atualizarCotacoesHandler) : Controller
    {

        public async Task<IActionResult> List()
        {

            var acoesList = await acaoService.GetList();

            List<AcaoViewModel> VM = acoesList.Select(a => new AcaoViewModel
            {
                Id = a.Id,
                Name = a.Name,
                SetorName = a.SetorName,
                LinkLogoEmpresa = a.LinkLogoEmpresa,
                Cotacoes = string.Join(" | ",
                    a.Tickers.Select(t =>
                        t.LastQuote.HasValue
                            ? $"{t.Name}: {t.LastQuote.Value.ToString("0.##", CultureInfo.InvariantCulture)}"
                            : $"{t.Name}: -"
                    )
                ),
                UltimaCotacaoCapturada = a.UltimaCotacaoCapturada
            })
            .ToList();

            return View(VM);
        }

        [HttpGet]
        public async Task<IActionResult> PrepareCreate()
        {

            var setoresList = await setorQuery.List();

            CreateAcaoViewModel vm = new()
            {
                opcoesSetores = setoresList.ToSelectList("Id", "Name")
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAcaoViewModel VM)
        {

            if (!ModelState.IsValid)
                return View("PrepareCreate", VM);

            var tickers = (VM.Tickers)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

            var cmd = new CreateAcaoCommand(
                Name: VM.Name,
                AnoEntrada: VM.AnoEntrada,
                Estatal: VM.Estatal,
                SetorId: VM.IdSetor,
                Tickers: tickers
            );

            var result = await createAcaoHandler.Handle(cmd);

            if (!result.Success)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Message);

                return View("PrepareCreate", VM);
            }

            TempData["SuccessMessage"] = "Ação criada com sucesso!";
            return RedirectToAction("List");

        }

        [HttpGet]
        public async Task<IActionResult> PrepareEdit(int id)
        {

            var setoresList = await setorQuery.List();

            var acaoDto = await acaoQuery.GetById(id);

            if (acaoDto == null)
            {
                return NotFound();
            }

            EditAcaoViewModel vm = AcaoMapper.MapToViewModel(acaoDto);

            vm.opcoesSetores = setoresList.ToSelectList("Id", "Name");

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditAcaoViewModel VM)
        {

            if (!ModelState.IsValid)
                return View("PrepareEdit", VM);

            var tickers = (VM.Tickers)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

            var cmd = new EditAcaoCommand(
                Id: VM.Id,
                AnoEntrada: VM.AnoEntrada,
                Estatal: VM.Estatal,
                SetorId: VM.IdSetor,
                Tickers: tickers
            );

            var result = await editAcaoHandler.Handle(cmd);

            if (!result.Success)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err);

                return View("PrepareEdit", VM);
            }

            TempData["SuccessMessage"] = "Ação editada com sucesso!";
            return RedirectToAction("List");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await deleteAcaoHandler.Handle(id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = string.Join(" ", result.Errors);
                return RedirectToAction("PrepareEdit", new { id });
            }

            TempData["SuccessMessage"] = "Ação excluída com sucesso!";
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> AtualizarCotacoes(int id)
        {
            await atualizarCotacoesHandler.HandleAsync(id);
            TempData["SendUpdateQuotes"] = "Cotações enviadas para atualização em segundo plano!";
            return RedirectToAction(nameof(List));
        }

    }
}