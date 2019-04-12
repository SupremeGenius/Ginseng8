using Ginseng.Models.Conventions;
using Postulate.Base.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ginseng.Models
{
	/// <summary>
	/// Controls visibility of milestone on Roadmap page
	/// </summary>
	public class AppMilestone : BaseTable
	{
		[References(typeof(Application))]
		[PrimaryKey]
		public int ApplicationId { get; set; }

		[References(typeof(Milestone))]
		[PrimaryKey]
		public int MilestoneId { get; set; }

		[DefaultExpression("1")]
		public bool OnRoadmap { get; set; } = true;

		[NotMapped]
		public string ApplicationName { get; set; }
	}
}