using Ginseng.Models.Conventions;
using Ginseng.Models.Interfaces;
using Postulate.Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Ginseng.Models
{
	/// <summary>
	/// Screenshot or other visual content or info that's part of a project
	/// </summary>
	public class ProjectImage : BaseTable, IBody
	{
		[References(typeof(Project))]
		public int ProjectId { get; set; }

		[MaxLength(255)]
		[Required]
		public string Title { get; set; }

		[MaxLength(255)]
		[Required]
		public string LargeImage { get; set; }

		[MaxLength(255)]
		[Required]
		public string SmallImage { get; set; }

		public string TextBody { get; set; }

		public string HtmlBody { get; set; }

		public int? Order { get; set; }
	}
}