using invesT.Enums;
using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Handlers;
using INVEST.Application.QualidadeIndicador.DTOs;
using INVEST.Application.QualidadeIndicador.Handlers;
using INVEST.Application.QualidadeIndicador.Repository;
using INVEST.Application.TipoIndicador.Repository;
using INVEST.Domain.Entities;
using INVEST.Web.Extensions;
using INVEST.Web.ViewModels.IndicadoresVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace INVEST.Web.Controllers
{
    public class TipoIndicadorController : Controller
    {
        private readonly ITipoIndicadorRepository _repository;
        private readonly IQualidadeIndicadorRepository _qualityRepository;
        private readonly EditQualidadeFaixasHandler _editQualidadeFaixasHandler;

        public TipoIndicadorController(ITipoIndicadorRepository repository, IQualidadeIndicadorRepository qualityRepository, EditQualidadeFaixasHandler editQualidadeFaixasHandler)
        {
            _repository = repository;
            _qualityRepository = qualityRepository;
            _editQualidadeFaixasHandler = editQualidadeFaixasHandler;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var dtos = await _repository.ListWithQualityParameters(ct);

            var model = dtos.Select(dto => new TipoIndicadorViewModel
            {
                Id = dto.Id,
                Name = dto.Name,

                // Mapeamento elegante usando Switch Expression do C#
                TipoDescricao = (TypeIndicadorValue)dto.Type switch
                {
                    TypeIndicadorValue.DECIMAL => "Decimal",
                    TypeIndicadorValue.SHORT => "Inteiro",
                    TypeIndicadorValue.BOOL => "Booleano (Sim/Não)",
                    _ => "Desconhecido" // Fallback de segurança
                },

                Qualidades = dto.Qualidades.Select(q => new QualidadeIndicadorViewModel
                {
                    ValorMinimo = q.ValorMinimo,
                    ValorMaximo = q.ValorMaximo
                }).ToList()
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var niveisSeed = await _qualityRepository.List();

            var tipoIndicador = await _repository.GetByIdWithQualityParameters(id);

            if (tipoIndicador == null)
            {
                return NotFound();
            }

            var model = new TipoIndicadorEditViewModel
            {
                Id = id,
                Name = tipoIndicador.Name,
                NiveisDisponiveis = niveisSeed.ToSelectList("Id", "Name"),
                Qualidades = tipoIndicador.Qualidades.Select(q => new QualidadeIndicadorEditViewModel
                {
                    ValorMinimo = q.ValorMinimo,
                    ValorMaximo = q.ValorMaximo,
                    NivelQualidadeId = niveisSeed.FirstOrDefault(n => n.Id == q.IdNivelQualidade)?.Id ?? 0
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TipoIndicadorEditViewModel VM)
        {
            if (!ModelState.IsValid)
            {
                var niveisSeed = await _qualityRepository.List();

                VM.NiveisDisponiveis = niveisSeed.ToSelectList("Id", "Name");
                return View(VM);
            }

            var cmd = new EditFaixasQualidadeCommand(
                Id: VM.Id,
                Faixas: VM.Qualidades.Select(q => new QualidadeIndicadorEditCommand
                (
                    Id: q.NivelQualidadeId,
                    ValorMinimo: q.ValorMinimo,
                    ValorMaximo: q.ValorMaximo
                )).ToList()
            );

            var result = await _editQualidadeFaixasHandler.Handle(cmd);

            if (!result.Success)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err);

                return View("Edit", VM);
            }

            TempData["MensagemSucesso"] = "Faixas atualizadas com sucesso!";

            return RedirectToAction(nameof(Index));
        }

    }
}