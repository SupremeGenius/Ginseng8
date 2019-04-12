using Ginseng.Models;
using Ginseng.Mvc.Classes;
using Ginseng.Mvc.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ginseng.Mvc.Pages.Roadmap
{
	public class IndexModel : AppPageModel
	{
		public IndexModel(IConfiguration config) : base(config)
		{
		}

		public Application Application { get; set; }
		public IEnumerable<Milestone> Milestones { get; set; }
		public Dictionary<RoadmapCell, AppMilestone> AppMilestones { get; set; }

		public bool IsMilestoneOnRoadmap(int milestoneId)
		{
			var cell = new RoadmapCell(CurrentOrgUser.CurrentAppId ?? 0, milestoneId);
			return (AppMilestones.ContainsKey(cell) && AppMilestones[cell].OnRoadmap);
		}

		public IEnumerable<Milestone> ActiveMilestones()
		{
			return GetMilestones((cell) => AppMilestones.ContainsKey(cell) && AppMilestones[cell].OnRoadmap);
		}

		public IEnumerable<Milestone> InactiveMilestones()
		{
			return GetMilestones((cell) => !AppMilestones.ContainsKey(cell) || !AppMilestones[cell].OnRoadmap);
		}

		private IEnumerable<Milestone> GetMilestones(Func<RoadmapCell, bool> predicate)
		{
			return Milestones.Where(ms =>
			{
				var cell = new RoadmapCell(CurrentOrgUser.CurrentAppId ?? 0, ms.Id);
				return predicate.Invoke(cell);
			});
		}

		public async Task OnGetAsync()
		{
			using (var cn = Data.GetConnection())
			{
				Milestones = await new Milestones() { OrgId = OrgId }.ExecuteAsync(cn);

				var cells = await new AppMilestones() { OrgId = OrgId, AppId = CurrentOrgUser.CurrentAppId }.ExecuteAsync(cn);
				AppMilestones = cells.ToDictionary(row => new RoadmapCell(row.ApplicationId, row.MilestoneId));
			}
		}

		public async Task<IActionResult> OnPostToggleMilestoneAsync(int id, bool selected)
		{
			var appId = CurrentOrgUser.CurrentAppId.Value;
			var appMs = await Data.FindWhereAsync<AppMilestone>(new { ApplicationId = appId, MilestoneId = id })
				?? new AppMilestone() { ApplicationId = appId, MilestoneId = id };
			appMs.OnRoadmap = selected;
			await Data.TrySaveAsync(appMs);
			return RedirectToPage("/Roadmap/Index");
		}
	}
}