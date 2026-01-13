using Microsoft.AspNetCore.Mvc.Rendering;

namespace INVEST.Web.Extensions
{
    public static class SelectListExtensions
    {
        /// <summary>
        /// Converte uma lista genérica em uma lista de SelectListItem.
        /// </summary>
        /// <typeparam name="T">Tipo da lista</typeparam>
        /// <param name="source">Lista de origem</param>
        /// <param name="valueProperty">Nome da propriedade que será usada como Value</param>
        /// <param name="textProperty">Nome da propriedade que será usada como Text</param>
        /// <returns>Lista de SelectListItem</returns>
        public static List<SelectListItem> ToSelectList<T>(
            this List<T> source,
            string valueProperty,
            string textProperty)
        {
            var props = typeof(T).GetProperties();

            var valueProp = props.FirstOrDefault(p => p.Name == valueProperty);
            var textProp = props.FirstOrDefault(p => p.Name == textProperty);

            if (valueProp == null || textProp == null)
                throw new ArgumentException("Propriedades informadas não existem no tipo.");

            return source.Select(item => new SelectListItem
            {
                Value = valueProp.GetValue(item)?.ToString(),
                Text = textProp.GetValue(item)?.ToString()
            }).ToList();
        }
    }

}