﻿using Ginseng.Models;
using Postulate.Base;

namespace Ginseng.Mvc.Queries
{
	public class Projects : Query<Project>
	{
		public Projects() : base("SELECT * FROM [dbo].[Project] WHERE [OrganizationId]=@orgId AND [IsActive]=@isActive ORDER BY [Priority], [Name]")
		{
		}

		public int OrgId { get; set; }
		public bool IsActive { get; set; }
	}
}