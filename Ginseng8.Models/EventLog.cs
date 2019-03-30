﻿using Ginseng.Models.Interfaces;
using Postulate.Base.Attributes;
using Postulate.SqlServer.IntKey;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;

namespace Ginseng.Models
{
	/// <summary>
	/// Note that IFeedItem is used so that incoming logged events work with Postulate Save.
	/// It's not that this is itself a feed item
	/// </summary>
	[Identity(nameof(Id), IdentityPosition.FirstColumn)]
	public class EventLog : IFeedItem
	{
		public int Id { get; set; }

		[References(typeof(Organization))]
		public int OrganizationId { get; set; }

		[References(typeof(WorkItem))]
		public int WorkItemId { get; set; }

		[MaxLength(50)]
		public string IconClass { get; set; }

		public string HtmlBody { get; set; }

		[MaxLength(50)]
		public string CreatedBy { get; set; }

		public DateTime DateCreated { get; set; }

		public static async Task Log(IDbConnection connection, IFeedItem item)
		{
			if (item.OrganizationId == 0)
			{
				await item.SetOrganizationIdAsync(connection);
			}

			await connection.SaveAsync(item);
		}

		public Task SetOrganizationIdAsync(IDbConnection connection)
		{
			// not needed functionally, although we must have a method for interface compliance
			throw new NotImplementedException();
		}
	}
}