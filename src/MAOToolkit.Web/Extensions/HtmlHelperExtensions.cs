using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace MAOToolkit.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Insert Meta Keywords
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public static IHtmlContent MetaKeywords(this IHtmlHelper htmlHelper, string? keywords)
        {
            if (String.IsNullOrEmpty(keywords))
                return HtmlString.Empty;
            return new HtmlString(
                String.Format("<meta name=\"keywords\" content=\"{0}\" />",
                              htmlHelper.Encode(keywords)));
        }

        /// <summary>
        /// Insert Meta Description
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static IHtmlContent MetaDescription(this IHtmlHelper htmlHelper, string? description)
        {
            if (String.IsNullOrEmpty(description))
                return HtmlString.Empty;
            return new HtmlString(
                String.Format("<meta name=\"description\" content=\"{0}\" />",
                              htmlHelper.Encode(description)));
        }

        /// <summary>
        /// Insert Canonical meta tag.
        /// </summary>
        /// <param rel="htmlHelper"></param>
        /// <param rel="canonical"></param>
        /// <returns></returns>
        public static IHtmlContent LinkCanonical(this IHtmlHelper htmlHelper, string? canonical)
        {
            if (String.IsNullOrEmpty(canonical))
                return HtmlString.Empty;
            return new HtmlString(
                String.Format("<link rel=\"canonical\" href=\"{0}\" />",
                              htmlHelper.Encode(canonical)));
        }

        /// <summary>
        /// Insert Description for property from ModelMetadata.
        /// </summary>
        public static IHtmlContent DescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var modelExplorer = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>().CreateModelExpression(htmlHelper.ViewData, expression);
            string? description = modelExplorer.Metadata.Description;

            return new HtmlString(description);
        }

        /// <summary>
        /// Insert question mark with description for property from ModelMetadata.
        /// </summary>
        public static IHtmlContent HintFor<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var modelExplorer = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>().CreateModelExpression(htmlHelper.ViewData, expression);
            string? description = modelExplorer.Metadata.Description;

            if (!String.IsNullOrEmpty(description))
            {
                return new HtmlString(String.Format("<span class=\"bi bi-question-circle-fill text-muted\" style=\"cursor:pointer\" data-bs-toggle=\"tooltip\" title=\"{0}\" tabindex=\"0\"></span>", description));
            }

            return HtmlString.Empty;
        }

        /// <summary>
        /// Insert Prompt for property from DisplayAttribute.
        /// </summary>
        public static IHtmlContent PromptFor<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            if (expression.Body is not MemberExpression memberExpression)
                throw new InvalidOperationException("Expression must be a member expression");

            var displayAttr = typeof(TModel).GetProperty(memberExpression.Member.Name)?.GetCustomAttribute<DisplayAttribute>();
            if (displayAttr is not null)
            {
                return new HtmlString(htmlHelper.Encode(displayAttr.GetPrompt()));
            }

            return HtmlString.Empty;
        }

        public static IEnumerable<SelectListItem> GetEnumSelectList<TEnum>(this IHtmlHelper html, string selectedValue) where TEnum : struct
        {
            var selectList = html.GetEnumSelectList<TEnum>();

            foreach (var item in selectList.Where(x => x.Value == selectedValue))
            {
                item.Selected = true;
            }

            return selectList;
        }
    }
}