﻿using Ginseng.Mvc.Interfaces;
using Postulate.Base;
using Postulate.Base.Attributes;
using Postulate.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Ginseng.Mvc.Queries
{
	public class OpenWorkItemsResult : IWorkItemNumber
	{
		public int Id { get; set; }
		public int Number { get; set; }
		public string Title { get; set; }
		public int? Priority { get; set; }
		public string HtmlBody { get; set; }
		public int? BusinessUserId { get; set; }
		public int? DeveloperUserId { get; set; }
		public int ApplicationId { get; set; }
		public string ApplicationName { get; set; }
		public bool HasImpediment { get; set; }
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
		public int? DataModelId { get; set; }
		public int MilestoneId { get; set; }
		public string MilestoneName { get; set; }
		public DateTime? MilestoneDate { get; set; }
		public DateTime SortMilestoneDate { get; set; }
		public int? MilestoneDaysAway { get; set; }
		public int? CloseReasonId { get; set; }
		public string CloseReasonName { get; set; }
		public int ActivityId { get; set; }
		public string ActivityName { get; set; }
		public int? ActivityOrder { get; set; }
		public string BusinessUserName { get; set; }
		public string DeveloperUserName { get; set; }
		public int? AssignedUserId { get; set; }
		public string AssignedUserName { get; set; }
		public string WorkItemSize { get; set; }
		public int? SizeId { get; set; }
		public int? DevEstimateHours { get; set; }
		public int? SizeEstimateHours { get; set; }
		public int EstimateHours { get; set; }
		public string WorkItemUserIdColumn { get; set; }
		public decimal ColorGradientPosition { get; set; }
		public string HandOffUserName { get; set; }
		public bool? IsForward { get; set; }
		public string FromActivityName { get; set; }
		public DateTime? HandOffDate { get; set; }

		public string ActivityStatus()
		{
			string assignedTo = (AssignedUserId.HasValue) ? AssignedUserName : "paused";
			return $"{ActivityName ?? "(not started)"} - {assignedTo}";
		}

		public bool IsPaused()
		{
			return (ActivityId != 0 && !AssignedUserId.HasValue);
		}

		public bool IsStopped()
		{
			return (MilestoneId != 0 && ActivityId == 0);
		}
	}

	public class OpenWorkItems : Query<OpenWorkItemsResult>, ITestableQuery
	{
		const string AssignedUserExpression = "(CASE [act].[ResponsibilityId] WHEN 1 THEN [wi].[BusinessUserId] WHEN 2 THEN [wi].[DeveloperUserId] END)";

		public OpenWorkItems() : base(
			$@"SELECT
                [wi].[Id],
                [wi].[Number],
				[pri].[Value] AS [Priority],
                [wi].[Title],
				[wi].[HtmlBody],
                [wi].[BusinessUserId],
                [wi].[DeveloperUserId],
                [wi].[ApplicationId], [app].[Name] AS [ApplicationName],
				[wi].[HasImpediment],
                COALESCE([wi].[ProjectId], 0) AS [ProjectId], COALESCE([p].[Name], '(no project)') AS [ProjectName],
                COALESCE([wi].[MilestoneId], 0) AS [MilestoneId], COALESCE([ms].[Name], '(no milestone)') AS [MilestoneName], [ms].[Date] AS [MilestoneDate], COALESCE([ms].[Date], '12/31/9999') AS [SortMilestoneDate], DATEDIFF(d, getdate(), [ms].[Date]) AS [MilestoneDaysAway],
                [wi].[CloseReasonId], [cr].[Name] AS [CloseReasonName],
                COALESCE([wi].[ActivityId], 0) AS [ActivityId],
                [act].[Name] AS [ActivityName],
				[act].[Order] AS [ActivityOrder],
                COALESCE([biz_ou].[DisplayName], [ousr].[UserName]) AS [BusinessUserName],
                COALESCE([dev_ou].[DisplayName], [dusr].[UserName]) AS [DeveloperUserName],
                CASE [act].[ResponsibilityId]
                    WHEN 1 THEN COALESCE([biz_ou].[DisplayName], [ousr].[UserName])
                    WHEN 2 THEN COALESCE([dev_ou].[DisplayName], [dusr].[UserName])
                END [AssignedUserName],
				{AssignedUserExpression} AS [AssignedUserId],
				[p].[DataModelId],
                [sz].[Name] AS [WorkItemSize],
                [wi].[SizeId],
                [wid].[EstimateHours] AS [DevEstimateHours],
                [sz].[EstimateHours] AS [SizeEstimateHours],
				COALESCE([wid].[EstimateHours], [sz].[EstimateHours], 0) AS [EstimateHours],
				[r].[WorkItemUserIdColumn],
				COALESCE([gp].[ColorGradientPosition], 0) AS [ColorGradientPosition],
				COALESCE([ho_ou].[DisplayName], [housr].[UserName]) AS [HandOffUserName],
				[ho].[IsForward],
				[from_act].[Name] AS [FromActivityName],
				[ho].[HtmlBody] AS [HandOffBody],
				[ho].[DateCreated] AS [HandOffDate]
            FROM
                [dbo].[WorkItem] [wi]
                INNER JOIN [dbo].[Application] [app] ON [wi].[ApplicationId]=[app].[Id]
				LEFT JOIN [dbo].[WorkItemPriority] [pri] ON [wi].[Id]=[pri].[WorkItemId]
                LEFT JOIN [dbo].[Project] [p] ON [wi].[ProjectId]=[p].[Id]
                LEFT JOIN [dbo].[Activity] [act] ON [wi].[ActivityId]=[act].[Id]
                LEFT JOIN [app].[Responsibility] [r] ON [act].[ResponsibilityId]=[r].[Id]
                LEFT JOIN [dbo].[Milestone] [ms] ON [wi].[MilestoneId]=[ms].[Id]
                LEFT JOIN [app].[CloseReason] [cr] ON [wi].[CloseReasonId]=[cr].[Id]
                LEFT JOIN [dbo].[WorkItemDevelopment] [wid] ON [wi].[Id]=[wid].[WorkItemId]
				LEFT JOIN [dbo].[HandOff] [ho] ON [wi].[LastHandOffId]=[ho].[Id]
                LEFT JOIN [dbo].[OrganizationUser] [biz_ou] ON
                    [wi].[OrganizationId]=[biz_ou].[OrganizationId] AND
                    [wi].[BusinessUserId]=[biz_ou].[UserId]
                LEFT JOIN [dbo].[AspNetUsers] [ousr] ON [wi].[BusinessUserId]=[ousr].[UserId]
                LEFT JOIN [dbo].[OrganizationUser] [dev_ou] ON
                    [wi].[OrganizationId]=[dev_ou].[OrganizationId] AND
                    [wi].[DeveloperUserId]=[dev_ou].[UserId]
				LEFT JOIN [dbo].[AspNetUsers] [housr] ON [ho].[FromUserId]=[housr].[UserId]
				LEFT JOIN [dbo].[OrganizationUser] [ho_ou] ON
					[wi].[OrganizationId]=[ho_ou].[OrganizationId] AND
					[ho].[FromUserId]=[ho_ou].[UserId]
				LEFT JOIN [dbo].[Activity] [from_act] ON [ho].[FromActivityId]=[from_act].[Id]
                LEFT JOIN [dbo].[AspNetUsers] [dusr] ON [wi].[DeveloperUserId]=[dusr].[UserId]
                LEFT JOIN [dbo].[WorkItemSize] [sz] ON [wi].[SizeId]=[sz].[Id]
				LEFT JOIN [dbo].[FnColorGradientPositions](@orgId) [gp] ON
					COALESCE([wid].[EstimateHours], [sz].[EstimateHours], 0) >= [gp].[MinHours] AND
					COALESCE([wid].[EstimateHours], [sz].[EstimateHours], 0) < [gp].[MaxHours]
            WHERE
                [wi].[OrganizationId]=@orgId {{andWhere}}
            ORDER BY
                COALESCE([pri].[Value], 100000),
                [wi].[Number]")
		{
		}

		public int OrgId { get; set; }

		[Case(true, "[wi].[CloseReasonId] IS NULL")]
		[Case(false, "[wi].[CloseReasonId] IS NOT NULL")]
		public bool? IsOpen { get; set; } = true;

		[Where("[wi].[Number]=@number")]
		public int? Number { get; set; }

		[Where(AssignedUserExpression + "=@assignedUserId")]
		public int? AssignedUserId { get; set; }

		[Where("[wi].[ApplicationId]=@appId")]
		public int? AppId { get; set; }

		[Case(true, "[wi].[MilestoneId] IS NOT NULL")]
		public bool? HasMilestone { get; set; }

		[Where("[wi].[ProjectId]=@projectId")]
		public int? ProjectId { get; set; }

		[Where("EXISTS(SELECT 1 FROM [dbo].[WorkItemLabel] WHERE [WorkItemId]=[wi].[Id] AND [LabelId]=@labelId)")]
		public int? LabelId { get; set; }

		[Where("[wi].[MilestoneId]=@milestoneId")]
		public int? MilestoneId { get; set; }

		[Where("[wi].[SizeId]=@sizeId")]
		public int? SizeId { get; set; }

		[Where("[wi].[ActivityId]=@activityId")]
		public int? ActivityId { get; set; }

		[Where("[wi].[ActivityId] IN @activityIds")]
		public int[] ActivityIds { get; set; }		

		[Case(false, AssignedUserExpression + " IS NULL")]
		[Case(true, AssignedUserExpression + " IS NOT NULL")]
		public bool? HasAssignedUserId { get; set; }
	
		[Case(true, "[wi].[ActivityId] IS NOT NULL AND (" + AssignedUserExpression + ") IS NULL")]
		public bool? IsPaused { get; set; }		

		[Case(true, "[wi].[MilestoneId] IS NOT NULL AND [wi].[ActivityId] IS NULL")]
		public bool? IsStopped { get; set; }		

		[Case(true, "")]
		public bool? IsHung { get; set; }

		[Phrase("wi].[Title", "wi].[TextBody")]
		public string TitleAndBodySearch { get; set; }

		[Where("[ms].[Date]<getdate()")]
		public bool? IsPastDue { get; set; }

		public static IEnumerable<ITestableQuery> GetTestCases()
		{
			// ideally here, you return instances of your query witch parameter set to get coverage of all the generated SQL combinations

			yield return new OpenWorkItems() { OrgId = 0 };
			yield return new OpenWorkItems() { IsOpen = true };
			yield return new OpenWorkItems() { Number = 0 };
			yield return new OpenWorkItems() { AssignedUserId = 0 };
			yield return new OpenWorkItems() { AppId = 0 };
			yield return new OpenWorkItems() { HasMilestone = true };
			yield return new OpenWorkItems() { HasMilestone = false };
			yield return new OpenWorkItems() { ProjectId = 0 };
			yield return new OpenWorkItems() { LabelId = 1 };
			yield return new OpenWorkItems() { MilestoneId = 1 };
			yield return new OpenWorkItems() { ActivityId = 1 };
			yield return new OpenWorkItems() { ActivityIds = new int[] { 1, 2, 3, } };
			yield return new OpenWorkItems() { HasAssignedUserId = true };
			yield return new OpenWorkItems() { HasAssignedUserId = false };
			yield return new OpenWorkItems() { IsPaused = true };
			yield return new OpenWorkItems() { IsStopped = true };
			yield return new OpenWorkItems() { TitleAndBodySearch = "whatever this that" };
			yield return new OpenWorkItems() { IsPastDue = true };
		}

		public IEnumerable<dynamic> TestExecute(IDbConnection connection)
		{
			return TestExecuteHelper(connection);
		}
	}
}