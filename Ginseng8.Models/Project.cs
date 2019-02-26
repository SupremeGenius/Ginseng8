﻿using Ginseng.Models.Conventions;
using Ginseng.Models.Interfaces;
using Postulate.Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Ginseng.Models
{
	/// <summary>
	/// A collection of tasks centered around a single goal, feature, or some other unifying idea
	/// </summary>
	public class Project : BaseTable, IBody
	{
		[References(typeof(Organization))]
		[PrimaryKey]
		public int OrganizationId { get; set; }

		[PrimaryKey]
		[MaxLength(50)]
		public string Name { get; set; }

		public string TextBody { get; set; }

		public string HtmlBody { get; set; }

		public bool IsActive { get; set; } = true;
	}
}