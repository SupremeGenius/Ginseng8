﻿using Dapper;
using Ginseng.Models;
using Ginseng.Mvc.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Ginseng.Mvc.Pages.Work
{
	[Authorize]
	public class FeedModel : AppPageModel
	{
		public FeedModel(IConfiguration config) : base(config)
		{
		}

		public IEnumerable<Event> Events { get; set; }
		public Dictionary<int, EventSubscription> Subscriptions { get; set; }
		public IEnumerable<EventLogsResult> EventLogs { get; set; }

		public async Task OnGetAsync()
		{
			using (var cn = Data.GetConnection())
			{
				Events = await new Events().ExecuteAsync(cn);

				await CreateDefaultEventSubscriptionsAsync(cn);

				var subs = await new MyEventSubscriptions() { OrgId = OrgId, UserId = UserId, AppId = CurrentOrgUser.CurrentAppId ?? 0 }.ExecuteAsync(cn);
				Subscriptions = subs.ToDictionary(row => row.EventId);
				var eventIds = subs.Where(s => s.Visible).Select(es => es.EventId).ToArray();
				EventLogs = await new EventLogs() { EventIds = eventIds, OrgId = OrgId, AppId = CurrentOrgUser.CurrentAppId ?? 0 }.ExecuteAsync(cn);
			}
		}

		private async Task CreateDefaultEventSubscriptionsAsync(SqlConnection cn)
		{
			if (CurrentOrgUser.CurrentAppId.HasValue)
			{
				await cn.ExecuteAsync(
					@"INSERT INTO [dbo].[EventSubscription] (
						[EventId], [OrganizationId], [ApplicationId], [UserId], [Visible], [SendEmail], [SendText], [InApp],
						[CreatedBy], [DateCreated]
					) SELECT
						[e].[Id], @orgId, @appId, @userId, 1, 0, 0, 0, @createdBy, @dateCreated
					FROM 
						[app].[Event] [e]
					WHERE
						NOT EXISTS(
							SELECT 1 FROM [dbo].[EventSubscription] 
							WHERE 
								[EventId]=[e].[Id] AND
								[OrganizationId]=@orgId AND
								[ApplicationId]=@appId AND
								[UserId]=@userId
						)", new { OrgId, appId = CurrentOrgUser.CurrentAppId, UserId, CreatedBy = User.Identity.Name, dateCreated = CurrentUser.LocalTime });
			}
		}

		public async Task<IActionResult> OnGetDisableNotificationsAsync()
		{
			using (var cn = Data.GetConnection())
			{
				await cn.ExecuteAsync(
					@"UPDATE [es] SET [SendEmail]=0, [SendText]=0, [InApp]=0, [DateModified]=@localTime, [ModifiedBy]=@userName
					FROM [dbo].[EventSubscription] [es]
					WHERE 
						[es].[UserId]=@userId AND 
						[es].[OrganizationId]=@orgId AND
						[es].[ApplicationId]=@appId", 
					new { UserId, OrgId, localTime = CurrentUser.LocalTime, userName = User.Identity.Name, AppId = CurrentOrgUser.CurrentAppId });
			}
			return RedirectToPage("/Dashboard/Feed");
		}
	}
}