﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Ginseng.Mvc.Helpers
{
	public static partial class HtmlHelpers
	{
		public const string AllApps = "(all apps)";

		public static IHtmlContent CurrentAppName(this IHtmlHelper<dynamic> html)
		{
			return new HtmlString(CurrentAppNameString(html));
		}

		public static string CurrentAppNameString(this IHtmlHelper<dynamic> html)
		{
			AppPageModel model = html.ViewContext.ViewData.Model as AppPageModel;
			return (model?.CurrentOrgUser?.CurrentApp != null) ? model.CurrentOrgUser.CurrentApp.Name : AllApps;
		}

		public static bool HasCurrentApp(this IHtmlHelper<dynamic> html)
		{
			AppPageModel model = html.ViewContext.ViewData.Model as AppPageModel;
			return model?.CurrentOrgUser?.CurrentAppId.HasValue ?? false;
		}

		public static string CurrentAppName<T>(this IHtmlHelper<T> html)
		{
			AppPageModel model = html.ViewContext.ViewData.Model as AppPageModel;
			return model?.CurrentOrgUser?.CurrentApp?.Name ?? "no app selected";
		}

		public static int CurrentAppId<T>(this IHtmlHelper<T> html)
		{
			AppPageModel model = html.ViewContext.ViewData.Model as AppPageModel;
			return model?.CurrentOrgUser?.CurrentAppId ?? 0;
		}

		public static int CurrentAppId(this IHtmlHelper<dynamic> html)
		{
			AppPageModel model = html.ViewContext.ViewData.Model as AppPageModel;
			return model?.CurrentOrgUser?.CurrentAppId ?? 0;
		}

		public static async Task<SelectList> AppFilterOptions(this IHtmlHelper<dynamic> html)
		{
			AppPageModel model = html.ViewContext.ViewData.Model as AppPageModel;
			if (model != null)
			{
				return await model.CurrentOrgAppSelectAsync();
			}

			return null;
		}
	}
}